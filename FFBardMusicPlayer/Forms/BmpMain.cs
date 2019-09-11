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
using static FFBardMusicPlayer.Controls.BmpPlayer;

namespace FFBardMusicPlayer.Forms {
	public partial class BmpMain : Form {

		private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private bool keyboardWarning = false;

		private DialogResult updateResult;
		private string updateTitle = string.Empty;
		private string updateText = string.Empty;

		private bool proceedPlaylistMidi = false;
		NoteChordSimulation<BmpPlayer.NoteEvent> chordNotes;

		// TODO remove forced mode checkbox?

		public BmpMain() {
			InitializeComponent();
			SetupCommands();

			this.UpdatePerformance();

			BmpUpdate update = new BmpUpdate();

			updateResult = update.ShowDialog();
			if(updateResult == DialogResult.Yes) {
				updateTitle = update.version.updateTitle;
				updateText = update.version.updateText;
				updateResult = DialogResult.Yes;
			}

			this.Text = update.version.ToString();

			FFXIV.hotkeys.OnFileLoad += delegate (Object o, EventArgs empty) {
				this.Invoke(t => t.Hotkeys_OnFileLoad(FFXIV.hotkeys));
			};
			FFXIV.hook.OnKeyPressed += Hook_OnKeyPressed;
			FFXIV.memory.OnProcessReady += delegate (object o, Process proc) {
				this.Log(string.Format("[{0}] Process scanned and ready.", proc.Id));
			};
			FFXIV.memory.OnProcessLost += delegate (object o, EventArgs arg) {
				this.Log("Attached process exited.");
			};
			FFXIV.memory.OnChatReceived += delegate (object o, ChatLogItem item) {
				this.Invoke(t => t.Memory_OnChatReceived(item));
			};
			FFXIV.memory.OnPerformanceReadyChanged += delegate (object o, bool performance) {
				this.Invoke(t => t.Memory_OnPerformanceReadyChanged(performance));
			};
			FFXIV.memory.OnCurrentPlayerJobChange += delegate (object o, CurrentPlayerResult res) {
				this.Invoke(t => t.Memory_OnCurrentPlayerJobChange(res));
			};
			FFXIV.memory.OnCurrentPlayerLogin += delegate (object o, CurrentPlayerResult res) {
				string format = string.Format("Character [{0}] logged in.", res.CurrentPlayer.Name);
				this.Log(format);

				this.Invoke(t => t.UpdatePerformance());
			};
			FFXIV.memory.OnCurrentPlayerLogout += delegate (object o, CurrentPlayerResult res) {
				string format = string.Format("Character [{0}] logged out.", res.CurrentPlayer.Name);
				this.Log(format);
			};

			Player.Status = Player.Status;
			Player.OnStatusChange += delegate (object o, PlayerStatus status) {
				this.Invoke(t => t.UpdatePerformance());
			};

			Player.OnSongSkip += OnSongSkip;
			Player.OnMidiLyric += OnMidiLyric;

			Player.OnMidiStatusChange += OnPlayStatusChange;
			Player.OnMidiStatusEnded += OnPlayStatusEnded;

			Player.OnMidiNote += OnMidiVoice;
			Player.OffMidiNote += OffMidiVoice;

			Settings.OnMidiInputChange += delegate (object o, MidiInput input) {
				Player.Player.OpenInputDevice(input.id);
				Console.WriteLine("Switched to {0} (input {1})", input.name, input.id);
			};
			Settings.OnKeyboardTest += delegate (object o, EventArgs arg) {
				foreach(FFXIVKeybindDat.Keybind keybind in FFXIV.hotkeys.GetPerformanceKeybinds()) {
					FFXIV.hook.SendSyncKeybind(keybind);
					Thread.Sleep(100);
				}
			};

			chordNotes = new NoteChordSimulation<BmpPlayer.NoteEvent>();
			chordNotes.NoteEvent += OnMidiVoice;

			Explorer.OnBrowserVisibleChange += delegate (object o, bool visible) {
				MainTable.RowStyles[MainTable.GetRow(ChatPlaylistTable)].Height = visible ? 0 : 100;
				MainTable.RowStyles[MainTable.GetRow(ChatPlaylistTable)].SizeType = visible ? SizeType.Absolute : SizeType.Percent;
				//ChatPlaylistTable.Invoke(t => t.Visible = !visible);

				MainTable.RowStyles[MainTable.GetRow(Explorer)].Height = visible ? 100 : 30;
				MainTable.RowStyles[MainTable.GetRow(Explorer)].SizeType = visible ? SizeType.Percent : SizeType.Absolute;
			};
			Explorer.OnBrowserSelect += Browser_OnMidiSelect;

			Playlist.OnMidiSelect += Playlist_OnMidiSelect;
			Playlist.OnPlaylistRequestAdd += Playlist_OnPlaylistRequestAdd;

			if(Properties.Settings.Default.SaveLog) {
				FileTarget target = new NLog.Targets.FileTarget("chatlog") {
					FileName = "logs/ff14log.txt",
					Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss} ${message}",
					ArchiveDateFormat = "${shortdate}",
					ArchiveEvery = FileArchivePeriod.Day,
					ArchiveFileName = "logs/ff14log-${shortdate}.txt",
					Encoding = Encoding.UTF8,
				};

				var config = new NLog.Config.LoggingConfiguration();
				config.AddRule(LogLevel.Info, LogLevel.Info, target);
				NLog.LogManager.Configuration = config;
			}

			string upath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming).FilePath;
			Console.WriteLine(string.Format(".config: [{0}]", upath));

			Log("Bard Music Player initialized.");
		}
		public void LogMidi(string format) {
			ChatLogAll.AppendRtf(BmpChatParser.FormatRtf("[MIDI] " + format, "\\red255\\green180\\blue255"));
		}
		public void Log(string format) {
			ChatLogAll.AppendRtf(BmpChatParser.FormatRtf("[SYSTEM] " + format));
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			Properties.Settings.Default.Upgrade();

			this.Location = Properties.Settings.Default.Location;

			if(Properties.Settings.Default.SigIgnore) {
				this.Log("Using local signature cache.");
			}
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);

			FFXIV.FindProcess();

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

			if(this.updateResult == DialogResult.Yes) {
				this.Invoke(new Action(() => {
					MessageBox.Show(this, updateText, updateTitle);
				}));
			}
		}

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);

			FFXIV.ShutdownMemory();

			Player.Player.CloseInputDevice();
			Player.Player.Pause();

			FFXIV.hook.ClearLastPerformanceKeybinds();

			Properties.Settings.Default.Location = this.Location;
			Properties.Settings.Default.Save();

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

			Func<bool> cmdFunc = chatListener.FindChatCommand(item);
			if(cmdFunc != null) {
				ChatLogCmd.AppendRtf(rtf);
				if(cmdFunc()) {
					// successful command?
				}
			}
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
						if(Player.Status == PlayerStatus.PerformerSolo) {
							if(Player.Player.IsPlaying) {
								Player.Player.Pause();
								FFXIV.hook.ClearLastPerformanceKeybinds();
							}
						}
						if(Player.Status == PlayerStatus.PerformerMulti) {
							// Don't do anything, keep playing
						}
					}
				}
			}
			this.UpdatePerformance();
		}

		private void Memory_OnCurrentPlayerJobChange(CurrentPlayerResult res) {
			this.Invoke(t => t.UpdatePerformance());
		}

		private void UpdatePerformance() {
			Playlist.Visible = (Player.Status == PlayerStatus.PerformerSolo);
			Orchestra.Visible = (Player.Status == PlayerStatus.Conducting);
			Player.Interactable = FFXIV.IsPerformanceReady();
			Player.Keyboard.OverrideText = FFXIV.IsPerformanceReady() ? string.Empty : "Open Bard Performance mode to play.";
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

			Explorer.Invoke(t => t.SetTrackName(entry.FilePath.FilePath));
			Explorer.Invoke(t => t.SetTrackNums(Player.Player.CurrentTrack, Player.Player.MaxTrack));
			Explorer.SongBrowserVisible = false;
		}
		private void Playlist_OnMidiSelect(object o, BmpMidiEntry entry) {
			if(Explorer.SelectFile(entry.FilePath.FilePath)) {
				Explorer.Invoke(t => t.SelectTrack(entry.Track.Track));
				Explorer.EnterFile();
			}
			Playlist.Select(entry.FilePath.FilePath);
			if(proceedPlaylistMidi) {
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
		///


		private void NextSong() {
			if(Playlist.AdvanceNext(out string filename, out int track)) {
				Playlist.PlaySelectedMidi();
			} else {
				// If failed playlist when you wanted to, just stop
				if(proceedPlaylistMidi) {
					Player.Player.Stop();
				}
			}
		}

		private void OnSongSkip(Object o, EventArgs a) {
			proceedPlaylistMidi = true;
			NextSong();
		}


		private void OnMidiLyric(Object o, string lyric) {
			if(Properties.Settings.Default.PlayLyrics) {
				FFXIV.SendChatString(lyric);
			}
		}

		private void OnPlayStatusChange(Object o, bool playing) {
			if(!playing) {
				FFXIV.hook.ClearLastPerformanceKeybinds();
				chordNotes.Clear();
			} else {
				if(Properties.Settings.Default.OpenFFXIV) {
					FFXIV.hook.FocusWindow();
				}
			}
		}

		private void OnPlayStatusEnded(object o, EventArgs e) {
			if(!Player.Loop) {
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


		private bool SoloPlay {
			get {
				return (Player.Status == PlayerStatus.PerformerSolo);
			}
		}

		private bool WantsSlow {
			get {
				return Properties.Settings.Default.SlowPlay;
			}
		}
		private bool WantsHold {
			get {
				return Properties.Settings.Default.HoldNotes;
			}
		}
		// OnMidiVoice + OffMidiVoice is called with correct octave shift
		private void OnMidiVoice(Object o, NoteEvent onNote) {

			if(Player.Status == PlayerStatus.Conducting) {
				return;
			}

			if(!FFXIV.IsPerformanceReady()) {
				return;
			}

			if(!FFXIV.memory.ChatInputOpen) {
				if(WantsSlow) {
					if(FFXIV.hotkeys.GetKeybindFromNoteByte(onNote.note) is FFXIVKeybindDat.Keybind keybind) {
						int delay = Decimal.ToInt32(Properties.Settings.Default.PlayHold);
						// Slow play

						Player.Player.InternalClock.Stop();
						FFXIV.hook.SendSyncKey(keybind.GetKey(), true, true, false);

						//Bmp.Player.InternalClock.Sleep(delay);
						Thread.Sleep(delay);

						FFXIV.hook.SendSyncKey(keybind.GetKey(), true, false, true);
						Player.Player.InternalClock.Continue();

						return;
					}
				}
				if(Properties.Settings.Default.AutoArpeggiate) {
					if(chordNotes.OnKey(onNote)) {
						// Chord detected and queued
						Console.WriteLine("Delay " + onNote + " by 100ms");
					}
				}
				if(!chordNotes.HasTimer(onNote)) {
					if(FFXIV.hotkeys.GetKeybindFromNoteByte(onNote.note) is FFXIVKeybindDat.Keybind keybind) {
						if(WantsHold) {
							FFXIV.hook.SendKeybindDown(keybind);
						} else {
							FFXIV.hook.SendAsyncKeybind(keybind);
						}
					}
				}
			}
		}

		private void OffMidiVoice(Object o, NoteEvent offNote) {

			if(Player.Status == PlayerStatus.Conducting) {
				return;
			}

			if(!FFXIV.IsPerformanceReady()) {
				return;
			}

			if(WantsSlow) {
				return;
			}

			if(!FFXIV.memory.ChatInputOpen) {
				if(WantsHold) {
					if(FFXIV.hotkeys.GetKeybindFromNoteByte(offNote.note) is FFXIVKeybindDat.Keybind keybind) {
						FFXIV.hook.SendKeybindUp(keybind);
					}
					chordNotes.OffKey(offNote);
				}
			}
		}

		private void AboutLabel_Click(object sender, EventArgs e) {
			BmpAbout about = new BmpAbout();
			about.ShowDialog(this);
		}
	}
}
