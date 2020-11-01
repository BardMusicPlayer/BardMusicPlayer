using FFBardMusicCommon;
using FFBardMusicPlayer.Controls;
using NLog;
using NLog.Targets;
using Sharlayan.Core;
using Sharlayan.Models.ReadResults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;

using static FFBardMusicPlayer.Controls.BmpPlayer;
using System.Security.Principal;

namespace FFBardMusicPlayer.Forms {
	public partial class BmpMain : Form {

		BmpProcessSelect processSelector = new BmpProcessSelect();
		private bool keyboardWarning = false;

		private DialogResult updateResult;
		private string updateTitle = string.Empty;
		private string updateText = string.Empty;

		private bool proceedPlaylistMidi = false;
		bool tempPlaying = false;

		public bool DonationStatus {
			set {
				if(value) {
					this.BackColor = Color.PaleGoldenrod;
					BottomTable.BackColor = Color.DarkKhaki;
					Explorer.BackColor = Color.DarkKhaki;
					// Donation button is the "About" button...
				}
			}
		}


		public BmpMain() {
			InitializeComponent();

			this.UpdatePerformance();

			BmpUpdate update = new BmpUpdate();
			if(!Program.programOptions.DisableUpdate) {
				updateResult = update.ShowDialog();
				if(updateResult == DialogResult.Yes) {
					updateTitle = update.version.updateTitle;
					updateText = update.version.updateText;
					updateResult = DialogResult.Yes;
				}
				if(updateResult == DialogResult.Ignore) {
					string log = " This is a preview of a future version of BMP! Please be kind and report any bugs or unexpected behaviors to discord channel.";
					ChatLogAll.AppendRtf(BmpChatParser.FormatRtf(log, Color.LightYellow, true));
				}
				if(!string.IsNullOrEmpty(update.version.updateLog)) {
					string log = string.Format("= BMP Update =\n {0} \n", update.version.updateLog);
					ChatLogAll.AppendRtf(BmpChatParser.FormatRtf(log, Color.LightGreen, true));
				}
			}
			this.Text = update.version.ToString();

			// Clear local orchestra
			InfoTabs.TabPages.Remove(localOrchestraTab);

			LocalOrchestra.onMemoryCheck += delegate (Object o, bool status) {
				if(status) {
					this.FFXIV.memory.StopThread();
				} else {
					this.FFXIV.memory.StartThread();
				}
			};

			FFXIV.findProcessRequest += delegate (Object o, EventArgs empty) {
				this.Invoke(t => t.FindProcess());
			};

			FFXIV.findProcessError += delegate (Object o, BmpHook.ProcessError error) {
				this.Invoke(t => t.ErrorProcess(error));
			};

			FFXIV.hotkeys.OnFileLoad += delegate (Object o, EventArgs empty) {
				this.Invoke(t => t.Hotkeys_OnFileLoad(FFXIV.hotkeys));
			};
			FFXIV.hook.OnKeyPressed += Hook_OnKeyPressed;
			FFXIV.memory.OnProcessReady += delegate (object o, Process proc) {
				this.Log(string.Format("[{0}] Process scanned and ready.", proc.Id));
				if(Sharlayan.Reader.CanGetActors()) {
					if(!Sharlayan.Reader.CanGetCharacterId()) {
						this.Log("[MEMORY] Cannot get Character ID.\n Key bindings won't be loaded, load it manually by selecting an ID in the bottom right.");
					}
					if(!Sharlayan.Reader.CanGetChatInput()) {
						this.Log("[MEMORY] Cannot get chat input status.\n Automatic pausing when chatting won't work.");
					}
					if(!Sharlayan.Reader.CanGetPerformance()) {
						this.Log("[MEMORY] Cannot get performance status.\n Performance detection will not work. Force it to work by ticking Settings > Force playback.");
					}
				} else {
					List<Sharlayan.Models.Signature> signatures = Sharlayan.Signatures.Resolve().ToList();
					int sigCount = signatures.Count;
					foreach(Sharlayan.Models.Signature sig in signatures) {
						if(Sharlayan.Scanner.Instance.Locations.ContainsKey(sig.Key)) {
							sigCount--;
						} else {
							Console.WriteLine(string.Format("Could not find signature {0}", sig.Key));
						}
					}
					if(sigCount == signatures.Count) {
						this.Log(string.Format("[MEMORY] Cannot read memory ({0}/{1}). Functionality will be severely limited.", sigCount, signatures.Count));
						this.Invoke(t => t.ErrorProcess(BmpHook.ProcessError.ProcessNonAccessible));
					} else {
						this.Log("[MEMORY] Cannot read actors. Local performance will be broken.");
					}
				}
			};
			FFXIV.memory.OnProcessLost += delegate (object o, EventArgs arg) {
				this.Log("Attached process exited.");
			};
			FFXIV.memory.OnChatReceived += delegate (object o, ChatLogItem item) {
				this.Invoke(t => t.Memory_OnChatReceived(item));
			};
			FFXIV.memory.OnPerformanceChanged += delegate (object o, List<uint> ids) {
				this.Invoke(t => t.LocalOrchestraUpdate((o as FFXIVMemory).GetActorItems(ids)));
			};
			FFXIV.memory.OnPerformanceReadyChanged += delegate (object o, bool performance) {
				this.Invoke(t => t.Memory_OnPerformanceReadyChanged(performance));
			};
			FFXIV.memory.OnCurrentPlayerJobChange += delegate (object o, CurrentPlayerResult res) {
				this.Invoke(t => t.Memory_OnCurrentPlayerJobChange(res));
			};
			FFXIV.memory.OnCurrentPlayerLogin += delegate (object o, CurrentPlayerResult res) {
				string world = string.Empty;
				if(Sharlayan.Reader.CanGetWorld()) {
					world = Sharlayan.Reader.GetWorld();
				}
				if(string.IsNullOrEmpty(world)) {
					this.Log(string.Format("Character [{0}] logged in.", res.CurrentPlayer.Name));
				} else {
					this.Log(string.Format("Character [{0}] logged in at [{1}].", res.CurrentPlayer.Name, world));
				}

				if(!Program.programOptions.DisableUpdate) {
					BmpDonationChecker don = new BmpDonationChecker(res.CurrentPlayer.Name, world);
					don.OnDonatorResponse += delegate (Object obj, BmpDonationChecker.DonatorResponse donres) {
						if(donres.donator) {
							if(!string.IsNullOrEmpty(donres.donationMessage)) {
								this.Log(donres.donationMessage);
							}
						}
						this.Invoke(t => t.DonationStatus = donres.donator);
					};
				}

				this.Invoke(t => t.UpdatePerformance());
			};
			FFXIV.memory.OnCurrentPlayerLogout += delegate (object o, CurrentPlayerResult res) {
				string format = string.Format("Character [{0}] logged out.", res.CurrentPlayer.Name);
				this.Log(format);
			};
			FFXIV.memory.OnPartyChanged += delegate (object o, PartyResult res) {
				this.Invoke(t => t.LocalOrchestraUpdate());
			};

			Player.OnStatusChange += delegate (object o, PlayerStatus status) {
				this.Invoke(t => t.UpdatePerformance());
			};

			Player.OnSongSkip += OnSongSkip;

			Player.OnMidiProgressChange += OnPlayProgressChange;

			Player.OnMidiStatusChange += OnPlayStatusChange;
			Player.OnMidiStatusEnded += OnPlayStatusEnded;

			Player.OnMidiNote += OnMidiVoice;
			Player.OffMidiNote += OffMidiVoice;

			Player.Player.OpenInputDevice(Settings.GetMidiInput().name);

			Settings.OnMidiInputChange += delegate (object o, MidiInput input) {
				Player.Player.CloseInputDevice();
				if(input.id != -1) {
					Player.Player.OpenInputDevice(input.name);
					Log(string.Format("Switched to {0} ({1})", input.name, input.id));
				}
			};
			Settings.OnKeyboardTest += delegate (object o, EventArgs arg) {
				
				foreach(FFXIVKeybindDat.Keybind keybind in FFXIV.hotkeys.GetPerformanceKeybinds()) {
					FFXIV.hook.SendSyncKeybind(keybind);
					Thread.Sleep(100);
				}
			};

            Settings.OnForcedOpen += delegate (object o, bool open)
            {
                this.Invoke(t => {
                    if (open)
                    {
                        Log(string.Format("Forced playback was enabled. You will not be able to use keybinds, such as spacebar."));
                        WarningLog("Forced playback enabled.");
                    }

                    t.UpdatePerformance();
               });
            };

			Explorer.OnBrowserVisibleChange += delegate (object o, bool visible) {
				MainTable.SuspendLayout();
				MainTable.RowStyles[MainTable.GetRow(ChatPlaylistTable)].Height = visible ? 0 : 100;
				MainTable.RowStyles[MainTable.GetRow(ChatPlaylistTable)].SizeType = visible ? SizeType.Absolute : SizeType.Percent;
				//ChatPlaylistTable.Invoke(t => t.Visible = !visible);

				MainTable.RowStyles[MainTable.GetRow(Explorer)].Height = visible ? 100 : 30;
				MainTable.RowStyles[MainTable.GetRow(Explorer)].SizeType = visible ? SizeType.Percent : SizeType.Absolute;
				MainTable.ResumeLayout(true);
			};
			Explorer.OnBrowserSelect += Browser_OnMidiSelect;

			Playlist.OnMidiSelect += Playlist_OnMidiSelect;
			Playlist.OnPlaylistRequestAdd += Playlist_OnPlaylistRequestAdd;
            Playlist.OnPlaylistManualRequestAdd += Playlist_OnPlaylistManualRequestAdd;

			this.ResizeBegin += (s, e) => {
				LocalOrchestra.SuspendLayout();
			};
			this.ResizeEnd += (s, e) => {
				LocalOrchestra.ResumeLayout(true);
			};

			if(Properties.Settings.Default.SaveLog) {
				FileTarget target = new NLog.Targets.FileTarget("chatlog") {
					FileName = "logs/ff14log.txt",
					Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss} ${message}",
					ArchiveDateFormat = "${shortdate}",
					ArchiveEvery = FileArchivePeriod.Day,
					ArchiveFileName = "logs/ff14log-${shortdate}.txt",
					Encoding = Encoding.UTF8,
					KeepFileOpen = true,
				};

				var config = new NLog.Config.LoggingConfiguration();
				config.AddRule(LogLevel.Info, LogLevel.Info, target);
				NLog.LogManager.Configuration = config;
			}

			string upath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming).FilePath;
			//Console.WriteLine(string.Format(".config: [{0}]", upath));

			Settings.RefreshMidiInput();

			Log("Bard Music Player initialized.");
		}

        public void LogMidi(string format)
        {
            ChatLogAll.AppendRtf(BmpChatParser.FormatRtf("[MIDI] " + format, Color.LightPink));
        }

        public void Log(string format)
        {
            ChatLogAll.AppendRtf(BmpChatParser.FormatRtf("[SYSTEM] " + format));
        }

        public void WarningLog(string format)
        {
            FFXIV.SetErrorStatus("Warning: " + format);
        }

        public void ErrorLog(string format)
        {
            FFXIV.SetErrorStatus("[ERROR] " + format);
        }

        public void FindProcess() {
			processSelector.ShowDialog(this);
			if(processSelector.DialogResult == DialogResult.Yes) {
				Process proc = processSelector.selectedProcess;
				if(proc != null) {
					FFXIV.SetProcess(proc);

					if(processSelector.useLocalOrchestra) {
						InfoTabs.TabPages.Remove(localOrchestraTab);
						InfoTabs.TabPages.Insert(2, localOrchestraTab);
						Player.Status = PlayerStatus.Conducting;
					} else {
						Player.Status = PlayerStatus.Performing;
						InfoTabs.TabPages.Remove(localOrchestraTab);
					}
					LocalOrchestra.OrchestraEnabled = processSelector.useLocalOrchestra;
					if(LocalOrchestra.OrchestraEnabled) {
						LocalOrchestra.PopulateLocalProcesses(processSelector.multiboxProcesses);
						LocalOrchestra.Sequencer = Player.Player;
						InfoTabs.SelectTab(2);
					}
				}
			}
		}

		public void ErrorProcess(BmpHook.ProcessError error) {
			if(error == BmpHook.ProcessError.ProcessFailed) {
				Log("Process hooking failed.");
			}
			else if(error == BmpHook.ProcessError.ProcessNonAccessible) {
				bool admin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
				if(!admin) {
					MessageBox.Show(this, "Process memory cannot be read.\nPlease start BMP in administrator mode.", "Process not accessible");
				} else {
					Log("Process hooking failed due to lack of privilege. Please make sure the game is not running in administrator mode.");
				}
			}
		}

		public bool IsOnScreen(Form form) {
			// Create rectangle
			Rectangle formRectangle = new Rectangle(form.Left, form.Top, form.Width, form.Height);

			// Test
			return Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(formRectangle));
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			//Properties.Settings.Default.Upgrade();

			this.Location = Properties.Settings.Default.Location;
			this.Size = Properties.Settings.Default.Size;

			if(!this.IsOnScreen(this)) {
				this.Location = new Point(100, 100);
			}

			if(this.WindowState == FormWindowState.Minimized) {
				this.WindowState = FormWindowState.Maximized;
			}

			if(Properties.Settings.Default.SigIgnore) {
				this.Log("Using local signature cache.");
			}
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);

			this.FindProcess();


			string ll = Properties.Settings.Default.LastLoaded;
			if(!string.IsNullOrEmpty(ll)) {
				if(Explorer.SelectFile(ll)) {
					Playlist.Select(ll);
					Explorer.EnterFile();
				}
			} else {
				if(Playlist.HasMidi()) {
					Playlist.PlaySelectedMidi();
				}
			}
			if(!string.IsNullOrEmpty(Program.programOptions.LoadMidiFile)) {
				Explorer.SelectFile(Program.programOptions.LoadMidiFile);
				Explorer.EnterFile();
				Playlist.Deselect();
			}

			if(this.updateResult == DialogResult.Yes) {
				this.Invoke(new Action(() => {
					MessageBox.Show(this, updateText, updateTitle);
				}));
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e) {
			base.OnFormClosing(e);

			if (!this.IsDisposed) {
				Properties.Settings.Default.Location = this.Location;
				Properties.Settings.Default.Size = this.Size;
				Properties.Settings.Default.Save();
			}
		}

		protected override void OnClosing(CancelEventArgs e) {

			FFXIV.ShutdownMemory();

			Player.Player.CloseInputDevice();
			Player.Player.Pause();

			FFXIV.hook.ClearLastPerformanceKeybinds();

			base.OnClosing(e);
		}

		private void Hotkeys_OnFileLoad(FFXIVKeybindDat hotkeys) {
			Player.Keyboard.UpdateNoteKeys(hotkeys);

			if(!hotkeys.ExtendedKeyboardBound && !keyboardWarning) {
				keyboardWarning = true;

				BmpKeybindWarning keybindWarning = new BmpKeybindWarning();
				keybindWarning.ShowDialog(this);

				//Log(string.Format("Your performance keybinds aren't set up correctly, songs will be played incomplete."));
			}
		}

		private void Memory_OnChatReceived(ChatLogItem item) {
			string rtf = BmpChatParser.FormatChat(item);
			ChatLogAll.AppendRtf(rtf);
		}

		private void Memory_OnPerformanceReadyChanged(bool performance) {
			if(performance) {
				if(Properties.Settings.Default.OpenBMP) {
					this.BringFront();
				}
			} else {
				if(!Properties.Settings.Default.ForcedOpen) {
					// If playing alone, stop playing
					if(Properties.Settings.Default.UnequipPause) {
						if(Player.Status == PlayerStatus.Performing) {
							if(Player.Player.IsPlaying) {
								Player.Player.Pause();
								FFXIV.hook.ClearLastPerformanceKeybinds();
							}
						}
					}
				}
			}
			this.UpdatePerformance();
		}

		private void Memory_OnCurrentPlayerJobChange(CurrentPlayerResult res) {
			this.Invoke(t => t.UpdatePerformance());
		}

		private void LocalOrchestraUpdate() {
			List<ActorItem> actorIds = new List<ActorItem>();
			List<string> performerNames = LocalOrchestra.GetPerformerNames();
			if(Sharlayan.Reader.CanGetActors()) {
				foreach(ActorItem actor in Sharlayan.Reader.GetActors().CurrentPCs.Values) {
					if(performerNames.Contains(actor.Name)) {
						actorIds.Add(actor);
					}
				}
			}
			this.LocalOrchestraUpdate(actorIds);
		}

		private void LocalOrchestraUpdate(List<ActorItem> actors) {
			LocalOrchestra.UpdateMemory();
		}

		private void UpdatePerformance() {
			if(Player.Status == PlayerStatus.Conducting) {
				Player.Interactable = true;
				Player.Keyboard.OverrideText = "Conducting in progress.";
				Player.Keyboard.Enabled = false;
			} else if(!Program.programOptions.DisableMemory) {
				Player.Interactable = FFXIV.IsPerformanceReady();
				Player.Keyboard.OverrideText = FFXIV.IsPerformanceReady() ? string.Empty : "Open Bard Performance mode to play.";
				Player.Keyboard.Enabled = true;
			}
		}

		private void BringFront() {
			this.TopMost = true;
			this.Activate();
			this.TopMost = false;
		}

		// Use invoke on gui changing properties
		private void Browser_OnMidiSelect(object o, BmpMidiEntry entry) {
			bool error = false;
			bool diff = (entry.FilePath.FilePath != Player.Player.LoadedFilename);
			try {
				Player.LoadFile(entry.FilePath.FilePath, entry.Track.Track);
				Player.Player.Stop();
			} catch (Exception e) {
				this.LogMidi(string.Format("[{0}] cannot be loaded:", entry.FilePath.FilePath));
				this.LogMidi(e.Message);
				Console.WriteLine(e.StackTrace);
				error = true;
			}
			if(!error) {
				if(diff && Properties.Settings.Default.Verbose) {
					this.LogMidi(string.Format("[{0}] loaded.", entry.FilePath.FilePath));
				}
				Properties.Settings.Default.LastLoaded = entry.FilePath.FilePath;
				Properties.Settings.Default.Save();
			}
			Playlist.Deselect();

			this.SuspendLayout();
			Explorer.SetTrackName(entry.FilePath.FilePath);
			Explorer.SetTrackNums(Player.Player.CurrentTrack, Player.Player.MaxTrack);
			this.ResumeLayout(true);

			Explorer.SongBrowserVisible = false;

			Statistics.SetBpmCount(Player.Tempo);
			Statistics.SetTotalTrackCount(Player.Player.MaxTrack);
			Statistics.SetTotalNoteCount(Player.TotalNoteCount);
			Statistics.SetTrackNoteCount(Player.CurrentNoteCount);

			if(LocalOrchestra.OrchestraEnabled) {
				LocalOrchestra.Sequencer = Player.Player;
			}
		}
		private void Playlist_OnMidiSelect(object o, BmpMidiEntry entry) {
			if(Explorer.SelectFile(entry.FilePath.FilePath)) {
				Explorer.Invoke(t => t.SelectTrack(entry.Track.Track));
				Explorer.EnterFile();
			}
			Playlist.Select(entry.FilePath.FilePath);
			if(proceedPlaylistMidi && Playlist.AutoPlay) {
				Player.Player.Play();
				proceedPlaylistMidi = false;
			}
		}
		private void Playlist_OnPlaylistRequestAdd(object o, EventArgs arg) {
			// Add from Bmp object
			string filename = Player.Player.LoadedFilename;
			if(!string.IsNullOrEmpty(filename)) {
				int track = Player.Player.CurrentTrack;

				Playlist.AddPlaylistEntry(filename, track);
			}
		}
        private void Playlist_OnPlaylistManualRequestAdd(object o, BmpPlaylist.BmpPlaylistRequestAddEvent args)
        {
            if (!string.IsNullOrEmpty(args.filePath))
            {
                // ensure the midi is in our directory before we accept it and add it to the playlist
                if (Explorer.SelectFile(args.filePath))
                {
                    Playlist.AddPlaylistEntry(args.filePath, args.track, args.dropIndex);
                }
            }
        }
		private void NextSong() {
			if(Playlist.AdvanceNext(out string filename, out int track)) {
				Playlist.PlaySelectedMidi();
			} else {
				// If failed playlist when you wanted to, just stop
				if (proceedPlaylistMidi)
                {
					Player.Player.Stop();
				}
			}
		}

		private void OnSongSkip(Object o, EventArgs a) {
			proceedPlaylistMidi = true;
			NextSong();
		}

		private void OnPlayProgressChange(Object o, int progress) {
			if(LocalOrchestra.OrchestraEnabled) {
				LocalOrchestra.PerformerProgress(progress);
			}
		}
		private void OnPlayStatusChange(Object o, bool playing) {
			if(!playing) {
				if(tempPlaying) {
					ChatLogAll.AppendRtf(BmpChatParser.FormatRtf("Playback paused."));
					tempPlaying = false;
				}
				if(LocalOrchestra.OrchestraEnabled) {
					LocalOrchestra.PerformerPlay(false);
				}
				FFXIV.hook.ClearLastPerformanceKeybinds();
			} else {
				if(!tempPlaying) {
					ChatLogAll.AppendRtf(BmpChatParser.FormatRtf("Playback resumed."));
					tempPlaying = true;
				}
				if(LocalOrchestra.OrchestraEnabled) {
					LocalOrchestra.PerformerPlay(true);
				}
				Statistics.Restart();
				if(Properties.Settings.Default.OpenFFXIV) {
					FFXIV.hook.FocusWindow();
				}
			}
		}

		private void OnPlayStatusEnded(object o, EventArgs e) {
            if (LocalOrchestra.OrchestraEnabled)
            {
                LocalOrchestra.PerformerStop();
            }
            if (Player.Loop)
            {
                Player.Player.Play();
                if (LocalOrchestra.OrchestraEnabled) LocalOrchestra.PerformerPlay(true);
            }
            else
            {
				proceedPlaylistMidi = true;
				this.NextSong();
			}
		}

		private void Hook_OnKeyPressed(Object o, Keys key) {
			if(Properties.Settings.Default.ForcedOpen) {
				return;
			}
			if(FFXIV.IsPerformanceReady() && !FFXIV.memory.ChatInputOpen) {

				if(key == Keys.F10) {
					foreach(FFXIVKeybindDat.Keybind keybind in FFXIV.hotkeys.GetPerformanceKeybinds()) {
						FFXIV.hook.SendAsyncKey(keybind.GetKey());
						System.Threading.Thread.Sleep(100);
					}
				}
				if(key == Keys.Space) {
					if(Player.Player.IsPlaying) {
						Player.Player.Pause();
					} else {
						Player.Player.Play();
					}
				}
				if(key == Keys.Right) {
					if(Player.Player.IsPlaying) {
						Player.Player.Seek(1000);
					}
				}
				if(key == Keys.Left) {
					if(Player.Player.IsPlaying) {
						Player.Player.Seek(-1000);
					}
				}
				if(key == Keys.Up) {
					if(Player.Player.IsPlaying) {
						Player.Player.Seek(10000);
					}
				}
				if(key == Keys.Down) {
					if(Player.Player.IsPlaying) {
						Player.Player.Seek(-10000);
					}
				}
			}
		}

		private bool WantsHold {
			get {
				return Properties.Settings.Default.HoldNotes;
			}
		}
		// OnMidiVoice + OffMidiVoice is called with correct octave shift
		private void OnMidiVoice(Object o, NoteEvent onNote) {

			Statistics.AddNoteCount();

			if(Properties.Settings.Default.Verbose) {
				FFXIVKeybindDat.Keybind thiskeybind = FFXIV.hotkeys.GetKeybindFromNoteByte(onNote.note);
				if(thiskeybind == null) {
					string ns = FFXIVKeybindDat.RawNoteByteToPianoKey(onNote.note);
					if(!string.IsNullOrEmpty(ns)) {
						string str = string.Format("Note {0} is out of range, it will not be played.", ns);
						this.LogMidi(str);
					}
				}
			}
			
			if(LocalOrchestra.OrchestraEnabled) {
				return;
			}

			if(Player.Status == PlayerStatus.Conducting) {
				return;
			}
			if(!FFXIV.IsPerformanceReady()) {
				return;
			}
			if(onNote.track != null) {
				// If from midi file
				if(onNote.track != Player.Player.LoadedTrack) {
					return;
				}
			}
			if(Sharlayan.Reader.CanGetChatInput()) {
				if(FFXIV.memory.ChatInputOpen) {
					return;
				}
			}

			if (FFXIV.hotkeys.GetKeybindFromNoteByte(onNote.note) is FFXIVKeybindDat.Keybind keybind)
			{
				if (WantsHold)
				{
					FFXIV.hook.SendKeybindDown(keybind);
				}
				else
				{
					FFXIV.hook.SendAsyncKeybind(keybind);
				}
			}
		}

		private void OffMidiVoice(Object o, NoteEvent offNote) {


			if(LocalOrchestra.OrchestraEnabled) {
				return;
			}
			
			if(Player.Status == PlayerStatus.Conducting) {
				return;
			}

			if(!FFXIV.IsPerformanceReady()) {
				return;
			}

			if(offNote.track != null) {
				if(offNote.track != Player.Player.LoadedTrack) {
					return;
				}
			}

			if(!FFXIV.memory.ChatInputOpen) {
				if(WantsHold) {
					if(FFXIV.hotkeys.GetKeybindFromNoteByte(offNote.note) is FFXIVKeybindDat.Keybind keybind) {
						FFXIV.hook.SendKeybindUp(keybind);
					}
				}
			}
		}

		private void DonationButton_Click(object sender, EventArgs e) {
			BmpAbout about = new BmpAbout();
			about.ShowDialog(this);
		}
	}
}
