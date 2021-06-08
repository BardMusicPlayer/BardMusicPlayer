using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using FFBardMusicCommon;
using FFBardMusicPlayer.FFXIV;
using Sharlayan;
using Sharlayan.Core;
using Sharlayan.Core.Enums;
using Timer = System.Timers.Timer;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpHook : UserControl
    {
        // Keep each BMP+FFXIV instance hidden in proc list
        private Mutex procMutex;
        public FFXIVHook Hook = new FFXIVHook();
        public FFXIVMemory Memory = new FFXIVMemory();
        public FFXIVKeybindDat Hotkeys = new FFXIVKeybindDat();
        public FFXIVHotbarDat Hotbar = new FFXIVHotbarDat();
        public FFXIVAddonDat Addon = new FFXIVAddonDat();

        public event EventHandler<bool> ForceModeChanged;

        public event EventHandler PerfSettingsChanged;

        public event EventHandler FindProcessRequest;

        private Timer errorMessageTimer;

        public enum ProcessError
        {
            ProcessFailed,
            ProcessNonAccessible
        }

        public event EventHandler<ProcessError> FindProcessError;

        private string CurrentCharId
        {
            get => CharIdSelector.SelectedValue != null ? CharIdSelector.SelectedText : string.Empty;
            set => CharIdSelector.Invoke(t => t.SelectedIndex = CharIdSelector.FindStringExact(value));
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public BmpHook()
        {
            InitializeComponent();

            UpdateCharIds();

            Memory.OnProcessLost += Memory_OnProcessLost;

            Memory.OnProcessReady       += Memory_OnProcessReady;
            Memory.OnCharacterIdChanged += Memory_OnCharacterIdChanged;

            Memory.OnChatReceived += Memory_OnChatReceived;
        }

        private void Memory_OnProcessLost(object sender, EventArgs e)
        {
            if (procMutex != null)
            {
                try
                {
                    Invoke(new System.Action(() => procMutex.ReleaseMutex()));
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
            }

            Memory.UnsetProcess();
            ShutdownMemory();
            Hook.Unhook();
            this.Invoke(t => t.SetHookStatus());
        }

        private void Log(string text)
        {
            var str = $"[BMP] {text}";
            Console.WriteLine(str);
            Logger.Debug(str);
        }

        private void Memory_OnProcessReady(object sender, Process proc)
        {
            var str = $"Ready: {proc.ProcessName} ({proc.Id})";
            this.Invoke(t => t.SetHookStatus(str));
        }

        private void Memory_OnCharacterIdChanged(object sender, string id)
        {
            Hook.ClearLastPerformanceKeybinds();
            Hotkeys.LoadKeybindDat(id);
            Hotbar.LoadHotbarDat(id);
            Addon.LoadAddonDat(id);

            Properties.Settings.Default.LastCharId = id;
            Properties.Settings.Default.Save();

            CurrentCharId = id;
        }

        private void Memory_OnChatReceived(object sender, ChatLogItem arg)
        {
            var format = BmpChatParser.Fixup(arg);
            if (!string.IsNullOrEmpty(format))
            {
                Logger.Info(format);
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
                var res = Reader.GetCurrentPlayer();
                return res.CurrentPlayer.Job == Actor.Job.BRD;
            }

            return false;
        }

        public bool IsPerformanceReady()
        {
            // Force keyboard up
            if (Properties.Settings.Default.ForcedOpen)
            {
                return true;
            }

            if (Reader.CanGetPerformance())
            {
                var res = Reader.GetPerformance();
                return res.IsUp();
            }

            return false;
        }

        public bool GetPerformanceInstrument(string ins, out Instrument ins2)
        {
            if (!string.IsNullOrEmpty(ins))
            {
                if (!Enum.TryParse(ins, out ins2))
                {
                    if (int.TryParse(ins, out var intInst))
                    {
                        ins2 = (Instrument) intInst;
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            ins2 = Instrument.Piano;
            return false;
        }

        public bool GetHotkeyForInstrument(Instrument ins, out FFXIVKeybindDat.Keybind keybind)
        {
            var keyMap = Hotbar.GetInstrumentKeyMap(ins);
            if (!string.IsNullOrEmpty(keyMap))
            {
                keybind = Hotkeys[keyMap];
                return true;
            }

            keybind = new FFXIVKeybindDat.Keybind();
            return false;
        }

        public bool GetHotkeyForHotbarSlot(int hnum, int snum, int jnum, out FFXIVKeybindDat.Keybind keybind)
        {
            var keyMap = Hotbar.GetHotbarSlotKeyMap(hnum, snum, jnum);
            if (!string.IsNullOrEmpty(keyMap))
            {
                keybind = Hotkeys[keyMap];
                return true;
            }

            keybind = new FFXIVKeybindDat.Keybind();
            return false;
        }

        public void UnequipPerformance()
        {
            if (IsPerformanceReady() && !Memory.ChatInputOpen)
            {
                if (Hotkeys["ESC"] is FFXIVKeybindDat.Keybind keybind)
                {
                    Hook.SendSyncKeybind(keybind);
                }
            }
        }

        public void PlayPerformanceNote(string noteKey)
        {
            if (IsPerformanceReady())
            {
                if (Hotkeys.GetKeybindFromNoteKey(noteKey) is FFXIVKeybindDat.Keybind keybind)
                {
                    Hook.SendAsyncKeybind(keybind);
                }
            }
        }

        public void SendChatString(string text)
        {
            var watch = new Stopwatch();
            var chatKeybind = Hotkeys["CMD_CHAT"];

            Hook.FocusWindow();
            // Now that our window is focused, we may use SendInput as much as we want

            var keyInputs = new List<FFXIVHook.Keybdinput>();
            if (IsPerformanceReady())
            {
                // First reset the keyboard then focus chat input
                keyInputs.Clear();
                foreach (var keybind in Hotkeys.GetPerformanceKeybinds())
                {
                    keyInputs.Add(new FFXIVHook.Keybdinput
                    {
                        wVk     = (ushort) keybind.GetKey(),
                        dwFlags = 0x0002
                    });
                }

                Hook.SendKeyInput(keyInputs);
            }

            if (Reader.CanGetChatInput() && !Memory.ChatInputOpen)
            {
                while (!Memory.ChatInputOpen)
                {
                    if (chatKeybind != null)
                    {
                        Hook.SendSyncKeybind(chatKeybind);
                        Thread.Sleep(100);
                    }
                }
            }

            if (Reader.CanGetChatInput() && !string.IsNullOrEmpty(Memory.ChatInputString))
            {
                Hook.SendSyncKey(Keys.A | Keys.Control);
                watch.Start();
                while (!string.IsNullOrEmpty(Memory.ChatInputString))
                {
                    Hook.SendSyncKey(Keys.Back);
                    if (watch.ElapsedMilliseconds > 500)
                    {
                        break;
                    }

                    Thread.Sleep(1);
                }

                watch.Stop();
            }

            Hook.SendString(text);

            var entered = false;
            if (Reader.CanGetChatInput())
            {
                watch.Start();

                while (!Memory.ChatInputString.Equals(text))
                {
                    // ...
                    if (watch.ElapsedMilliseconds > 100)
                    {
                        break;
                    }

                    Thread.Sleep(1);
                }

                entered = Memory.ChatInputString.Equals(text);
            }

            Hook.SendSyncKey(Keys.Enter);
        }

        public void SetHookStatus(string status = null)
        {
            if (string.IsNullOrEmpty(status))
            {
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
                Enabled  = true
            };
            errorMessageTimer.Elapsed += delegate
            {
                this.Invoke(t => { HookGlobalMessageLabel.Text = ""; });
                errorMessageTimer.Stop();
                errorMessageTimer.Dispose();
                errorMessageTimer = null;
            };
        }

        public void SetProcess(Process proc)
        {
            try
            {
                var a = proc.HasExited;
            }
            catch (Win32Exception ex)
            {
                Log(string.Format(ex.Message));
                FindProcessError?.Invoke(this, ProcessError.ProcessNonAccessible);
                return;
            }

            if (Hook.Hook(proc))
            {
                Log("Process hooking succeeded.");

                var str = $"Hooked: {proc.ProcessName} ({proc.Id})";
                this.Invoke(t => t.SetHookStatus(str));

                procMutex = new Mutex(true, $"bard-music-player-{proc.Id}");
                if (procMutex.WaitOne(TimeSpan.Zero, true))
                {
                    SetupMemory(proc);
                }
            }
            else
            {
                Log("Process hooking failed.");
                SetHookStatus("F: Hook process...");
                FindProcessError?.Invoke(this, ProcessError.ProcessFailed);
            }
        }

        public void SetupMemory(Process proc)
        {
            if (Memory.IsAttached())
            {
                Memory.UnsetProcess();
            }

            if (proc.ProcessName == "ffxiv_dx11")
            {
                Log("FFXIV memory parsing...");

                // memory setprocess
                Memory.SetProcess(proc);
                if (Program.ProgramOptions.DisableMemory)
                {
                    Memory.Refresh();
                }
                else
                {
                    Memory.StartThread();
                }
            }
        }

        public void ShutdownMemory()
        {
            Memory.StopThread();
            while (Memory.IsThreadAlive())
            {
                // ...
            }
        }

        public void UpdateCharIds()
        {
            CharIdSelector.Items.Clear();
            foreach (var id in FFXIVDatFile.GetIdList())
            {
                var item = new ToolStripMenuItem(id);
                if (id.Equals(Properties.Settings.Default.LastCharId))
                {
                    item.Checked = true;
                }

                CharIdSelector.Items.Add(item);
            }
        }

        private void HookButton_Click(object sender, EventArgs e)
        {
            if (Memory.IsAttached())
            {
                Memory.UnsetProcess();
                ShutdownMemory();
                Hook.Unhook();
                SetHookStatus();
            }
            else
            {
                FindProcessRequest?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CharIdSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = (sender as ComboBox).Text;

            Log($"Forced FFXIV character ID: [{id}].");

            Hook.ClearLastPerformanceKeybinds();
            Hotkeys.LoadKeybindDat(id);
            Hotbar.LoadHotbarDat(id);

            PerfSettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ForceModeToggle_CheckedChanged(object sender, EventArgs e)
        {
            var value = ((CheckBox) sender).Checked;
            Properties.Settings.Default.ForcedOpen = value;
            ForceModeChanged?.Invoke(sender, value);
        }
    }
}