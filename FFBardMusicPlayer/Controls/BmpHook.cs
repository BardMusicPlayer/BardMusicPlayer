﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Sharlayan;
using Sharlayan.Models.ReadResults;
using Sharlayan.Core;
using Sharlayan.Core.Enums;
using Timer = System.Timers.Timer;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpHook : UserControl {

		// Keep each BMP+FFXIV instance hidden in proc list
		private Mutex procMutex = null;

		public FFXIVHook hook = new FFXIVHook();
		public FFXIVMemory memory = new FFXIVMemory();

		public FFXIVKeybindDat hotkeys = new FFXIVKeybindDat();
		public FFXIVHotbarDat hotbar = new FFXIVHotbarDat();
		public FFXIVAddonDat addon = new FFXIVAddonDat();

		public event EventHandler<bool> forceModeChanged;
		public event EventHandler perfSettingsChanged;
		public event EventHandler findProcessRequest;

        private Timer errorMessageTimer = null;

		public enum ProcessError {
			ProcessFailed,
			ProcessNonAccessible,
		}
		public event EventHandler<ProcessError> findProcessError;

		private string CurrentCharId {
			get {
				if(CharIdSelector.SelectedValue != null)
					return CharIdSelector.SelectedText;
				return string.Empty;
			}
			set {
				CharIdSelector.Invoke(t => t.SelectedIndex = CharIdSelector.FindStringExact(value));
			}
		}

		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		public BmpHook() {
			InitializeComponent();

			UpdateCharIds();

			memory.OnProcessLost += Memory_OnProcessLost;

			memory.OnProcessReady += Memory_OnProcessReady;
			memory.OnCharacterIdChanged += Memory_OnCharacterIdChanged;

			memory.OnChatReceived += Memory_OnChatReceived;

		}

		private void Memory_OnProcessLost(object sender, EventArgs e) {
			if(procMutex != null) {
				try {
					this.Invoke(new System.Action(() => procMutex.ReleaseMutex()));
				} catch(Exception ex) {
					Log(ex.Message);
				}
			}
			memory.UnsetProcess();
			this.ShutdownMemory();
			hook.Unhook();
			this.Invoke(t => t.SetHookStatus());
		}

		private void Log(string text) {
			string str = string.Format("[BMP] {0}", text);
			Console.WriteLine(str);
			logger.Debug(str);
		}

		private void Memory_OnProcessReady(object sender, Process proc) {
			string str = string.Format("Ready: {0} ({1})", proc.ProcessName, proc.Id);
			this.Invoke(t => t.SetHookStatus(str));
		}

		private void Memory_OnCharacterIdChanged(object sender, string id) {
			hook.ClearLastPerformanceKeybinds();
			hotkeys.LoadKeybindDat(id);
			hotbar.LoadHotbarDat(id);
			addon.LoadAddonDat(id);

			Properties.Settings.Default.LastCharId = id;
			Properties.Settings.Default.Save();

			CurrentCharId = id;
		}

		private void Memory_OnChatReceived(object sender, ChatLogItem arg) {

			string format = BmpChatParser.Fixup(arg);
			if(!string.IsNullOrEmpty(format)) {
				logger.Info(format);
				Console.WriteLine(format);
			}
		}

        // Memory funcs
        public bool IsPlayerJobReady()
        {
            if (Properties.Settings.Default.ForcedOpen)
            {
                return true;
            }

            if (Reader.CanGetPlayerInfo())
            {
                return IsPlayerJobReady(Reader.GetCurrentPlayer());
            }

            return false;
        }

        public bool IsPlayerJobReady(CurrentPlayerResult currentPlayer)
        {
            return (currentPlayer.CurrentPlayer.Job == Sharlayan.Core.Enums.Actor.Job.BRD);
        }

        public bool IsPerformanceReady() {
			// Force keyboard up
			if(Properties.Settings.Default.ForcedOpen) {
				return true;
			}
			if(Reader.CanGetPerformance()) {
				PerformanceResult res = Reader.GetPerformance();
				return res.IsUp();
			}
			return false;
		}

		public bool GetPerformanceInstrument(string ins, out Performance.Instrument ins2) {

			if(!string.IsNullOrEmpty(ins)) {
				if(!Enum.TryParse(ins, out ins2)) {
					if(int.TryParse(ins, out int intInst)) {
						ins2 = (Performance.Instrument) intInst;
						return true;
					}
				} else {
					return true;
				}
			}
			ins2 = Performance.Instrument.Piano;
			return false;
		}

		public bool GetHotkeyForInstrument(Performance.Instrument ins, out FFXIVKeybindDat.Keybind keybind) {
			
			string keyMap = hotbar.GetInstrumentKeyMap(ins);
			if(!string.IsNullOrEmpty(keyMap)) {
				keybind = hotkeys[keyMap];
				return true;
			}
			keybind = new FFXIVKeybindDat.Keybind();
			return false;
		}
		public bool GetHotkeyForHotbarSlot(int hnum, int snum, int jnum, out FFXIVKeybindDat.Keybind keybind) {

			string keyMap = hotbar.GetHotbarSlotKeyMap(hnum, snum, jnum);
			if(!string.IsNullOrEmpty(keyMap)) {
				keybind = hotkeys[keyMap];
				return true;
			}
			keybind = new FFXIVKeybindDat.Keybind();
			return false;
		}

		public void UnequipPerformance() {
			if(IsPerformanceReady() && !memory.ChatInputOpen) {
				if(hotkeys["ESC"] is FFXIVKeybindDat.Keybind keybind) {
					hook.SendSyncKeybind(keybind);
				}
			}
		}

		public void PlayPerformanceNote(string noteKey) {
			if(IsPerformanceReady()) {
				if(hotkeys.GetKeybindFromNoteKey(noteKey) is FFXIVKeybindDat.Keybind keybind) {
					hook.SendAsyncKeybind(keybind);
				}
			}
		}
		public void SendChatString(string text) {
			if(true) {
				Stopwatch watch = new Stopwatch();
				FFXIVKeybindDat.Keybind chatKeybind = hotkeys["CMD_CHAT"];

				hook.FocusWindow();
				// Now that our window is focused, we may use SendInput as much as we want

				List<FFXIVHook.KEYBDINPUT> keyInputs = new List<FFXIVHook.KEYBDINPUT>();
				if(IsPerformanceReady()) {
					// First reset the keyboard then focus chat input
					keyInputs.Clear();
					foreach(FFXIVKeybindDat.Keybind keybind in hotkeys.GetPerformanceKeybinds()) {
						keyInputs.Add(new FFXIVHook.KEYBDINPUT {
							wVk = (ushort) keybind.GetKey(),
							dwFlags = 0x0002,
						});
					}
					hook.SendKeyInput(keyInputs);
				}
				if(Reader.CanGetChatInput() && !memory.ChatInputOpen) {
					while(!memory.ChatInputOpen) {
						if(chatKeybind is FFXIVKeybindDat.Keybind) {
							hook.SendSyncKeybind(chatKeybind);
							System.Threading.Thread.Sleep(100);
						}
					}
				}
				if(Reader.CanGetChatInput() && !string.IsNullOrEmpty(memory.ChatInputString)) {
					hook.SendSyncKey(Keys.A | Keys.Control);
					watch.Start();
					while(!string.IsNullOrEmpty(memory.ChatInputString)) {
						hook.SendSyncKey(Keys.Back);
						if(watch.ElapsedMilliseconds > 500) {
							break;
						}
						System.Threading.Thread.Sleep(1);
					}
					watch.Stop();
				}
				hook.SendString(text);

				bool entered = false;
				if(Reader.CanGetChatInput()) {
					watch.Start();

					while(!memory.ChatInputString.Equals(text)) {
						// ...
						if(watch.ElapsedMilliseconds > 100) {
							break;
						}
						System.Threading.Thread.Sleep(1);
					}
					entered = memory.ChatInputString.Equals(text);
				}
				hook.SendSyncKey(Keys.Enter);
			}
		}

		public void SetHookStatus(string status = null) {
			if(string.IsNullOrEmpty(status)) {
				status = "Hook process";
			}
			HookButton.Text = status;
		}

        public void SetErrorStatus(string status)
        {
            // remove it, since we'll be updating the text again
            if (errorMessageTimer != null)
            {
                errorMessageTimer.Stop();
                errorMessageTimer.Dispose();
                errorMessageTimer = null;
            }

            // set the label text
            HookGlobalMessageLabel.Text = status;

            // dispatch hiding the system error
            errorMessageTimer = new Timer
            {
                Interval = 10 * 1000, // 10 seconds
                Enabled = true
            };
            errorMessageTimer.Elapsed += delegate (object o, System.Timers.ElapsedEventArgs e)
            {
                this.Invoke(t => { HookGlobalMessageLabel.Text = ""; });
                errorMessageTimer.Stop();
                errorMessageTimer.Dispose();
                errorMessageTimer = null;
            };
        }

		public void SetProcess(Process proc) {
			try {
				var a = proc.HasExited;
			} catch (Win32Exception ex) {
				Log(string.Format(ex.Message));
				findProcessError.Invoke(this, ProcessError.ProcessNonAccessible);
				return;
			}
			if(hook.Hook(proc)) {
				Log(string.Format("Process hooking succeeded."));

				string str = string.Format("Hooked: {0} ({1})", proc.ProcessName, proc.Id);
				this.Invoke(t => t.SetHookStatus(str));

				procMutex = new Mutex(true, string.Format("bard-music-player-{0}", proc.Id));
				if(procMutex.WaitOne(TimeSpan.Zero, true)) {
					SetupMemory(proc);
				}

			} else {
				Log(string.Format("Process hooking failed."));
				SetHookStatus("F: Hook process...");
				findProcessError.Invoke(this, ProcessError.ProcessFailed);
			}
		}

		public void SetupMemory(Process proc) {


			if(memory.IsAttached()) {
				memory.UnsetProcess();
			}
			if(proc.ProcessName == "ffxiv_dx11") {
				Log(string.Format("FFXIV memory parsing..."));

				// memory setprocess
				memory.SetProcess(proc);
				if(Program.programOptions.DisableMemory) {
					memory.Refresh();
				} else {
					memory.StartThread();
				}
			}
		}

		public void ShutdownMemory() {
			memory.StopThread();
			while(memory.IsThreadAlive()) {
				// ...
			}
		}

		public void UpdateCharIds() {
			CharIdSelector.Items.Clear();
			foreach(string id in FFXIVDatFile.GetIdList()) {
				ToolStripMenuItem item = new ToolStripMenuItem(id);
				if(id.Equals(Properties.Settings.Default.LastCharId)) {
					item.Checked = true;
				}
				CharIdSelector.Items.Add(item);
			}
		}

		private void HookButton_Click(object sender, EventArgs e) {
			if(memory.IsAttached()) {
				memory.UnsetProcess();
				this.ShutdownMemory();
				hook.Unhook();
				this.SetHookStatus();
			} else {
				findProcessRequest?.Invoke(this, EventArgs.Empty);
			}
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
		}

		private void CharIdSelector_SelectedIndexChanged(object sender, EventArgs e) {
			string id = (sender as ComboBox).Text as string;

			Log(string.Format("Forced FFXIV character ID: [{0}].", id));

			hook.ClearLastPerformanceKeybinds();
			hotkeys.LoadKeybindDat(id);
			hotbar.LoadHotbarDat(id);

			perfSettingsChanged?.Invoke(this, EventArgs.Empty);

		}

		private void ForceModeToggle_CheckedChanged(object sender, EventArgs e) {

			bool value = (sender as CheckBox).Checked;
			Properties.Settings.Default.ForcedOpen = value;
			forceModeChanged?.Invoke(sender, value);
		}
	}
}
