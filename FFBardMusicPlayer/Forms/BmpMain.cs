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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static FFBardMusicPlayer.Controls.BmpPlayer;
using System.Security.Principal;
using FFBardMusicPlayer.FFXIV;

namespace FFBardMusicPlayer.Forms
{
    public partial class BmpMain : Form
    {
        private readonly BmpProcessSelect processSelector = new BmpProcessSelect();
        private bool keyboardWarning;
        private readonly DialogResult updateResult;
        private readonly string updateTitle = string.Empty;
        private readonly string updateText = string.Empty;
        private bool proceedPlaylistMidi;
        private bool tempPlaying;

        public bool DonationStatus
        {
            set
            {
                if (value)
                {
                    BackColor             = Color.PaleGoldenrod;
                    BottomTable.BackColor = Color.DarkKhaki;
                    Explorer.BackColor    = Color.DarkKhaki;
                    // Donation button is the "About" button...
                }
            }
        }

        public BmpMain()
        {
            InitializeComponent();

            UpdatePerformance();

            var update = new BmpUpdate();
            if (!Program.ProgramOptions.DisableUpdate)
            {
                updateResult = update.ShowDialog();
                if (updateResult == DialogResult.Yes)
                {
                    updateTitle  = update.Version.UpdateTitle;
                    updateText   = update.Version.UpdateText;
                    updateResult = DialogResult.Yes;
                }

                if (updateResult == DialogResult.Ignore)
                {
                    var log =
                        " This is a preview of a future version of BMP! Please be kind and report any bugs or unexpected behaviors to discord channel.";
                    ChatLogAll.AppendRtf(BmpChatParser.FormatRtf(log, Color.LightYellow, true));
                }

                if (!string.IsNullOrEmpty(update.Version?.UpdateLog))
                {
                    var log = $"= BMP Update =\n {update.Version.UpdateLog} \n";
                    ChatLogAll.AppendRtf(BmpChatParser.FormatRtf(log, Color.LightGreen, true));
                }
            }

            Text = update.Version?.ToString();

            // Clear local orchestra
            InfoTabs.TabPages.Remove(localOrchestraTab);

            LocalOrchestra.OnMemoryCheck += delegate(object o, bool status)
            {
                if (status)
                {
                    FFXIV.Memory.StopThread();
                }
                else
                {
                    FFXIV.Memory.StartThread();
                }
            };

            FFXIV.FindProcessRequest += delegate { this.Invoke(t => t.FindProcess()); };

            FFXIV.FindProcessError += delegate(object o, BmpHook.ProcessError error)
            {
                this.Invoke(t => t.ErrorProcess(error));
            };

            FFXIV.Hotkeys.OnFileLoad += delegate
            {
                this.Invoke(t => t.Hotkeys_OnFileLoad(FFXIV.Hotkeys));
            };
            FFXIV.Hook.OnKeyPressed += Hook_OnKeyPressed;
            FFXIV.Memory.OnProcessReady += delegate(object o, Process proc)
            {
                Log($"[{proc.Id}] Process scanned and ready.");
                if (Sharlayan.Reader.CanGetActors())
                {
                    if (!Sharlayan.Reader.CanGetCharacterId())
                    {
                        Log(
                            "[MEMORY] Cannot get Character ID.\n Key bindings won't be loaded, load it manually by selecting an ID in the bottom right.");
                    }

                    if (!Sharlayan.Reader.CanGetChatInput())
                    {
                        Log("[MEMORY] Cannot get chat input status.\n Automatic pausing when chatting won't work.");
                    }

                    if (!Sharlayan.Reader.CanGetPerformance())
                    {
                        Log(
                            "[MEMORY] Cannot get performance status.\n Performance detection will not work. Force it to work by ticking Settings > Force playback.");
                    }
                }
                else
                {
                    var signatures = Sharlayan.Signatures.Resolve().ToList();
                    var sigCount = signatures.Count;
                    foreach (var sig in signatures)
                    {
                        if (Sharlayan.Scanner.Instance.Locations.ContainsKey(sig.Key))
                        {
                            sigCount--;
                        }
                        else
                        {
                            Console.WriteLine($"Could not find signature {sig.Key}");
                        }
                    }

                    if (sigCount == signatures.Count)
                    {
                        Log(
                            $"[MEMORY] Cannot read memory ({sigCount}/{signatures.Count}). Functionality will be severely limited.");
                        this.Invoke(t => t.ErrorProcess(BmpHook.ProcessError.ProcessNonAccessible));
                    }
                    else
                    {
                        Log("[MEMORY] Cannot read actors. Local performance will be broken.");
                    }
                }
            };
            FFXIV.Memory.OnProcessLost += delegate { Log("Attached process exited."); };
            FFXIV.Memory.OnChatReceived += delegate(object o, ChatLogItem item)
            {
                this.Invoke(t => t.Memory_OnChatReceived(item));
            };
            FFXIV.Memory.OnPerformanceChanged += delegate(object o, List<uint> ids)
            {
                this.Invoke(t => t.LocalOrchestraUpdate((o as FFXIVMemory).GetActorItems(ids)));
            };
            FFXIV.Memory.OnPerformanceReadyChanged += delegate(object o, bool performance)
            {
                this.Invoke(t => t.Memory_OnPerformanceReadyChanged(performance));
            };
            FFXIV.Memory.OnCurrentPlayerJobChange += delegate(object o, CurrentPlayerResult res)
            {
                this.Invoke(t => t.Memory_OnCurrentPlayerJobChange(res));
            };
            FFXIV.Memory.OnCurrentPlayerLogin += delegate(object o, CurrentPlayerResult res)
            {
                var world = string.Empty;
                if (Sharlayan.Reader.CanGetWorld())
                {
                    world = Sharlayan.Reader.GetWorld();
                }

                Log(string.IsNullOrEmpty(world)
                    ? $"Character [{res.CurrentPlayer.Name}] logged in."
                    : $"Character [{res.CurrentPlayer.Name}] logged in at [{world}].");

                if (!Program.ProgramOptions.DisableUpdate)
                {
                    var don = new BmpDonationChecker(res.CurrentPlayer.Name, world);
                    don.OnDonatorResponse += delegate(object obj, BmpDonationChecker.DonatorResponse donres)
                    {
                        if (donres.Donator)
                        {
                            if (!string.IsNullOrEmpty(donres.DonationMessage))
                            {
                                Log(donres.DonationMessage);
                            }
                        }

                        this.Invoke(t => t.DonationStatus = donres.Donator);
                    };
                }

                this.Invoke(t => t.UpdatePerformance());
            };
            FFXIV.Memory.OnCurrentPlayerLogout += delegate(object o, CurrentPlayerResult res)
            {
                var format = $"Character [{res.CurrentPlayer.Name}] logged out.";
                Log(format);
            };
            FFXIV.Memory.OnPartyChanged += delegate
            {
                this.Invoke(t => t.LocalOrchestraUpdate());
            };

            Player.OnStatusChange += delegate
            {
                this.Invoke(t => t.UpdatePerformance());
            };

            Player.OnSongSkip += OnSongSkip;

            Player.OnMidiProgressChange += OnPlayProgressChange;

            Player.OnMidiStatusChange += OnPlayStatusChange;
            Player.OnMidiStatusEnded  += OnPlayStatusEnded;

            Player.OnMidiNote  += OnMidiVoice;
            Player.OffMidiNote += OffMidiVoice;
            Player.ProgChangeMidi += ChangeMidiVoice;

            Player.Player.OpenInputDevice(Settings.GetMidiInput().Name);

            Settings.OnMidiInputChange += delegate(object o, MidiInput input)
            {
                Player.Player.CloseInputDevice();
                if (input.Id != -1)
                {
                    Player.Player.OpenInputDevice(input.Name);
                    Log($"Switched to {input.Name} ({input.Id})");
                }
            };
            Settings.OnKeyboardTest += delegate
            {
                foreach (var keybind in FFXIV.Hotkeys.GetPerformanceKeybinds())
                {
                    FFXIV.Hook.SendSyncKeybind(keybind);
                    Thread.Sleep(100);
                }
            };

            Settings.OnForcedOpen += delegate(object o, bool open)
            {
                this.Invoke(t =>
                {
                    if (open)
                    {
                        Log("Forced playback was enabled. You will not be able to use keybinds, such as spacebar.");
                        WarningLog("Forced playback enabled.");
                    }

                    t.UpdatePerformance();
                });
            };

            Explorer.OnBrowserVisibleChange += delegate(object o, bool visible)
            {
                MainTable.SuspendLayout();
                MainTable.RowStyles[MainTable.GetRow(ChatPlaylistTable)].Height = visible ? 0 : 100;
                MainTable.RowStyles[MainTable.GetRow(ChatPlaylistTable)].SizeType =
                    visible ? SizeType.Absolute : SizeType.Percent;
                //ChatPlaylistTable.Invoke(t => t.Visible = !visible);

                MainTable.RowStyles[MainTable.GetRow(Explorer)].Height = visible ? 100 : 30;
                MainTable.RowStyles[MainTable.GetRow(Explorer)].SizeType =
                    visible ? SizeType.Percent : SizeType.Absolute;
                MainTable.ResumeLayout(true);
            };
            Explorer.OnBrowserSelect += Browser_OnMidiSelect;

            Playlist.OnMidiSelect               += Playlist_OnMidiSelect;
            Playlist.OnPlaylistRequestAdd       += Playlist_OnPlaylistRequestAdd;
            Playlist.OnPlaylistManualRequestAdd += Playlist_OnPlaylistManualRequestAdd;

            ResizeBegin += (s, e) => { LocalOrchestra.SuspendLayout(); };
            ResizeEnd   += (s, e) => { LocalOrchestra.ResumeLayout(true); };

            if (Properties.Settings.Default.SaveLog)
            {
                var target = new FileTarget("chatlog")
                {
                    FileName          = "logs/ff14log.txt",
                    Layout            = @"${date:format=yyyy-MM-dd HH\:mm\:ss} ${message}",
                    ArchiveDateFormat = "${shortdate}",
                    ArchiveEvery      = FileArchivePeriod.Day,
                    ArchiveFileName   = "logs/ff14log-${shortdate}.txt",
                    Encoding          = Encoding.UTF8,
                    KeepFileOpen      = true
                };

                var config = new NLog.Config.LoggingConfiguration();
                config.AddRule(LogLevel.Info, LogLevel.Info, target);
                LogManager.Configuration = config;
            }

            var upath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming).FilePath;
            //Console.WriteLine(string.Format(".config: [{0}]", upath));

            Settings.RefreshMidiInput();

            Log("Bard Music Player initialized.");
        }

        public void LogMidi(string format)
        {
            ChatLogAll.AppendRtf(BmpChatParser.FormatRtf($"[MIDI] {format}", Color.LightPink));
        }

        public void Log(string format) { ChatLogAll.AppendRtf(BmpChatParser.FormatRtf($"[SYSTEM] {format}")); }

        public void WarningLog(string format) { FFXIV.SetErrorStatus($"Warning: {format}"); }

        public void ErrorLog(string format) { FFXIV.SetErrorStatus($"[ERROR] {format}"); }

        public void FindProcess()
        {
            processSelector.ShowDialog(this);
            if (processSelector.DialogResult == DialogResult.Yes)
            {
                var proc = processSelector.SelectedProcess;
                if (proc != null)
                {
                    FFXIV.SetProcess(proc);

                    if (processSelector.UseLocalOrchestra)
                    {
                        InfoTabs.TabPages.Remove(localOrchestraTab);
                        InfoTabs.TabPages.Insert(2, localOrchestraTab);
                        Player.Status = PlayerStatus.Conducting;
                    }
                    else
                    {
                        Player.Status = PlayerStatus.Performing;
                        InfoTabs.TabPages.Remove(localOrchestraTab);
                    }

                    LocalOrchestra.OrchestraEnabled = processSelector.UseLocalOrchestra;
                    if (LocalOrchestra.OrchestraEnabled)
                    {
                        LocalOrchestra.PopulateLocalProcesses(processSelector.MultiboxProcesses);
                        LocalOrchestra.Sequencer = Player.Player;
                        InfoTabs.SelectTab(2);
                    }
                }
            }
        }

        public void ErrorProcess(BmpHook.ProcessError error)
        {
            if (error == BmpHook.ProcessError.ProcessFailed)
            {
                Log("Process hooking failed.");
            }
            else if (error == BmpHook.ProcessError.ProcessNonAccessible)
            {
                var admin =
                    new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
                if (!admin)
                {
                    MessageBox.Show(this, "Process memory cannot be read.\nPlease start BMP in administrator mode.",
                        "Process not accessible");
                }
                else
                {
                    Log(
                        "Process hooking failed due to lack of privilege. Please make sure the game is not running in administrator mode.");
                }
            }
        }

        public bool IsOnScreen(Form form)
        {
            // Create rectangle
            var formRectangle = new Rectangle(form.Left, form.Top, form.Width, form.Height);

            // Test
            return Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(formRectangle));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Properties.Settings.Default.Upgrade();

            Location = Properties.Settings.Default.Location;
            Size     = Properties.Settings.Default.Size;

            if (!IsOnScreen(this))
            {
                Location = new Point(100, 100);
            }

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Maximized;
            }

            if (Properties.Settings.Default.SigIgnore)
            {
                Log("Using local signature cache.");
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            FindProcess();

            var ll = Properties.Settings.Default.LastLoaded;
            if (!string.IsNullOrEmpty(ll))
            {
                if (Explorer.SelectFile(ll))
                {
                    Playlist.Select(ll);
                    Explorer.EnterFile();
                }
            }
            else
            {
                if (Playlist.HasMidi())
                {
                    Playlist.PlaySelectedMidi();
                }
            }

            if (!string.IsNullOrEmpty(Program.ProgramOptions.LoadMidiFile))
            {
                Explorer.SelectFile(Program.ProgramOptions.LoadMidiFile);
                Explorer.EnterFile();
                Playlist.Deselect();
            }

            if (updateResult == DialogResult.Yes)
            {
                Invoke(new Action(() => { MessageBox.Show(this, updateText, updateTitle); }));
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (!IsDisposed)
            {
                Properties.Settings.Default.Location = Location;
                Properties.Settings.Default.Size     = Size;
                Properties.Settings.Default.Save();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            FFXIV.ShutdownMemory();

            Player.Player.CloseInputDevice();
            Player.Player.Pause();

            FFXIV.Hook.ClearLastPerformanceKeybinds();

            base.OnClosing(e);
        }

        private void Hotkeys_OnFileLoad(FFXIVKeybindDat hotkeys)
        {
            Player.Keyboard.UpdateNoteKeys(hotkeys);

            if (!hotkeys.ExtendedKeyboardBound && !keyboardWarning)
            {
                keyboardWarning = true;

                var keybindWarning = new BmpKeybindWarning();
                keybindWarning.ShowDialog(this);

                //Log(string.Format("Your performance keybinds aren't set up correctly, songs will be played incomplete."));
            }
        }

        private void Memory_OnChatReceived(ChatLogItem item)
        {
            var rtf = BmpChatParser.FormatChat(item);
            ChatLogAll.AppendRtf(rtf);
        }

        private void Memory_OnPerformanceReadyChanged(bool performance)
        {
            if (performance)
            {
                if (Properties.Settings.Default.OpenBMP)
                {
                    BringFront();
                }
            }
            else
            {
                if (!Properties.Settings.Default.ForcedOpen)
                {
                    // If playing alone, stop playing
                    if (Properties.Settings.Default.UnequipPause)
                    {
                        if (Player.Status == PlayerStatus.Performing)
                        {
                            if (Player.Player.IsPlaying)
                            {
                                Player.Player.Pause();
                                FFXIV.Hook.ClearLastPerformanceKeybinds();
                            }
                        }
                    }
                }
            }

            UpdatePerformance();
        }

        private void Memory_OnCurrentPlayerJobChange(CurrentPlayerResult res)
        {
            this.Invoke(t => t.UpdatePerformance());
        }

        private void LocalOrchestraUpdate()
        {
            var actorIds = new List<ActorItem>();
            var performerNames = LocalOrchestra.GetPerformerNames();
            if (Sharlayan.Reader.CanGetActors())
            {
                foreach (var actor in Sharlayan.Reader.GetActors().CurrentPCs.Values)
                {
                    if (performerNames.Contains(actor.Name))
                    {
                        actorIds.Add(actor);
                    }
                }
            }

            LocalOrchestraUpdate(actorIds);
        }

        private void LocalOrchestraUpdate(List<ActorItem> actors) { LocalOrchestra.UpdateMemory(); }

        private void UpdatePerformance()
        {
            if (Player.Status == PlayerStatus.Conducting)
            {
                Player.Interactable          = true;
                Player.Keyboard.OverrideText = "Conducting in progress.";
                Player.Keyboard.Enabled      = false;
            }
            else if (!Program.ProgramOptions.DisableMemory)
            {
                Player.Interactable = FFXIV.IsPerformanceReady();
                Player.Keyboard.OverrideText =
                    FFXIV.IsPerformanceReady() ? string.Empty : "Open Bard Performance mode to play.";
                Player.Keyboard.Enabled = true;
            }
        }

        private void BringFront()
        {
            TopMost = true;
            Activate();
            TopMost = false;
        }

        // Use invoke on gui changing properties
        private void Browser_OnMidiSelect(object o, BmpMidiEntry entry)
        {
            var error = false;
            var diff = entry.FilePath.FilePath != Player.Player.LoadedFilename;
            try
            {
                Player.LoadFile(entry.FilePath.FilePath, entry.Track.Track);
                Player.Player.Stop();
            }
            catch (Exception e)
            {
                LogMidi($"[{entry.FilePath.FilePath}] cannot be loaded:");
                LogMidi(e.Message);
                Console.WriteLine(e.StackTrace);
                error = true;
            }

            if (!error)
            {
                if (diff && Properties.Settings.Default.Verbose)
                {
                    LogMidi($"[{entry.FilePath.FilePath}] loaded.");
                }

                Properties.Settings.Default.LastLoaded = entry.FilePath.FilePath;
                Properties.Settings.Default.Save();
            }

            Playlist.Deselect();

            SuspendLayout();
            Explorer.SetTrackName(entry.FilePath.FilePath);
            Explorer.SetTrackNums(Player.Player.CurrentTrack, Player.Player.MaxTrack);
            ResumeLayout(true);

            Explorer.SongBrowserVisible = false;

            Statistics.SetBpmCount(Player.Tempo);
            Statistics.SetTotalTrackCount(Player.Player.MaxTrack);
            Statistics.SetTotalNoteCount(Player.TotalNoteCount);
            Statistics.SetTrackNoteCount(Player.CurrentNoteCount);

            if (LocalOrchestra.OrchestraEnabled)
            {
                LocalOrchestra.Sequencer = Player.Player;
            }
        }

        private void Playlist_OnMidiSelect(object o, BmpMidiEntry entry)
        {
            if (Explorer.SelectFile(entry.FilePath.FilePath))
            {
                Explorer.Invoke(t => t.SelectTrack(entry.Track.Track));
                Explorer.EnterFile();
            }

            Playlist.Select(entry.FilePath.FilePath);
            if (proceedPlaylistMidi && Playlist.AutoPlay)
            {
                Player.Player.Play();
                proceedPlaylistMidi = false;
            }
        }

        private void Playlist_OnPlaylistRequestAdd(object o, EventArgs arg)
        {
            // Add from Bmp object
            var filename = Player.Player.LoadedFilename;
            if (!string.IsNullOrEmpty(filename))
            {
                var track = Player.Player.CurrentTrack;

                Playlist.AddPlaylistEntry(filename, track);
            }
        }

        private void Playlist_OnPlaylistManualRequestAdd(object o, BmpPlaylist.BmpPlaylistRequestAddEvent args)
        {
            if (!string.IsNullOrEmpty(args.FilePath))
            {
                // ensure the midi is in our directory before we accept it and add it to the playlist
                if (Explorer.SelectFile(args.FilePath))
                {
                    Playlist.AddPlaylistEntry(args.FilePath, args.Track, args.DropIndex);
                }
            }
        }

        private void NextSong()
        {
            if (Playlist.AdvanceNext(out var filename, out var track))
            {
                Playlist.PlaySelectedMidi();
            }
            else
            {
                // If failed playlist when you wanted to, just stop
                if (proceedPlaylistMidi)
                {
                    Player.Player.Stop();
                }
            }
        }

        private void OnSongSkip(object o, EventArgs a)
        {
            if (LocalOrchestra.OrchestraEnabled)
            {
                LocalOrchestra.PerformerStop();
            }
            else
            {
                Player.Player.Stop();
            }

            proceedPlaylistMidi = true;
            NextSong();
        }

        private void OnPlayProgressChange(object o, int progress)
        {
            if (LocalOrchestra.OrchestraEnabled)
            {
                LocalOrchestra.PerformerProgress(progress);
            }
        }

        private void OnPlayStatusChange(object o, bool playing)
        {
            if (!playing)
            {
                if (tempPlaying)
                {
                    ChatLogAll.AppendRtf(BmpChatParser.FormatRtf("Playback paused."));
                    tempPlaying = false;
                }

                if (LocalOrchestra.OrchestraEnabled)
                {
                    LocalOrchestra.PerformerPlay(false);
                }

                FFXIV.Hook.ClearLastPerformanceKeybinds();
            }
            else
            {
                if (!tempPlaying)
                {
                    ChatLogAll.AppendRtf(BmpChatParser.FormatRtf("Playback resumed."));
                    tempPlaying = true;
                }

                if (LocalOrchestra.OrchestraEnabled)
                {
                    LocalOrchestra.PerformerPlay(true);
                }

                Statistics.Restart();
                if (Properties.Settings.Default.OpenFFXIV)
                {
                    FFXIV.Hook.FocusWindow();
                }
            }
        }

        private void OnPlayStatusEnded(object o, EventArgs e)
        {
            if (LocalOrchestra.OrchestraEnabled)
            {
                LocalOrchestra.PerformerStop();
            }

            if (Player.Loop)
            {
                Player.Player.Play();
                if (LocalOrchestra.OrchestraEnabled)
                {
                    LocalOrchestra.PerformerPlay(true);
                }
            }
            else
            {
                proceedPlaylistMidi = true;
                NextSong();
            }
        }

        private void Hook_OnKeyPressed(object o, Keys key)
        {
            if (Properties.Settings.Default.ForcedOpen)
            {
                return;
            }

            if (FFXIV.IsPerformanceReady() && !FFXIV.Memory.ChatInputOpen)
            {
                switch (key)
                {
                    case Keys.F10:
                    {
                        foreach (var keybind in FFXIV.Hotkeys.GetPerformanceKeybinds())
                        {
                            FFXIV.Hook.SendAsyncKey(keybind.GetKey());
                            Thread.Sleep(100);
                        }

                        break;
                    }
                    case Keys.Space when Player.Player.IsPlaying:
                        Player.Player.Pause();
                        break;
                    case Keys.Space:
                        Player.Player.Play();
                        break;
                }

                if (Player.Player.IsPlaying)
                {
                    switch (key)
                    {
                        case Keys.Right: Player.Player.Seek( 1000); break;
                        case Keys.Left:  Player.Player.Seek(-1000); break;
                        case Keys.Up:    Player.Player.Seek( 10000); break;
                        case Keys.Down:  Player.Player.Seek(-10000); break;
                    }
                }
            }
        }

        private bool WantsHold => Properties.Settings.Default.HoldNotes;

        // OnMidiVoice + OffMidiVoice is called with correct octave shift
        private void OnMidiVoice(object o, NoteEvent onNote)
        {
            Statistics.AddNoteCount();

            if (Properties.Settings.Default.Verbose)
            {
                var thiskeybind = FFXIV.Hotkeys.GetKeybindFromNoteByte(onNote.Note);
                if (thiskeybind == null)
                {
                    var ns = FFXIVKeybindDat.RawNoteByteToPianoKey(onNote.Note);
                    if (!string.IsNullOrEmpty(ns))
                    {
                        var str = $"Note {ns} is out of range, it will not be played.";
                        LogMidi(str);
                    }
                }
            }

            if (LocalOrchestra.OrchestraEnabled)
                return;

            if (Player.Status == PlayerStatus.Conducting)
                return;

            if (!FFXIV.IsPerformanceReady())
                return;
            
            // If from midi file
            if (onNote.Track != Player.Player.LoadedTrack)
                return;

            if (Sharlayan.Reader.CanGetChatInput() && FFXIV.Memory.ChatInputOpen)
                return;
            

            if (FFXIV.Hotkeys.GetKeybindFromNoteByte(onNote.Note) is FFXIVKeybindDat.Keybind keybind)
            {
                if (WantsHold)
                {
                    FFXIV.Hook.SendKeybindDown(keybind);
                }
                else
                {
                    FFXIV.Hook.SendAsyncKeybind(keybind);
                }
            }
        }

        private void OffMidiVoice(object o, NoteEvent offNote)
        {
            if (LocalOrchestra.OrchestraEnabled)
            {
                return;
            }

            if (Player.Status == PlayerStatus.Conducting)
            {
                return;
            }

            if (!FFXIV.IsPerformanceReady())
            {
                return;
            }

            if (offNote.Track != null)
            {
                if (offNote.Track != Player.Player.LoadedTrack)
                {
                    return;
                }
            }

            if (!FFXIV.Memory.ChatInputOpen)
            {
                if (WantsHold)
                {
                    if (FFXIV.Hotkeys.GetKeybindFromNoteByte(offNote.Note) is FFXIVKeybindDat.Keybind keybind)
                    {
                        FFXIV.Hook.SendKeybindUp(keybind);
                    }
                }
            }
        }

		private void ChangeMidiVoice(Object o, ProgChangeEvent progdata)
		{
			if (LocalOrchestra.OrchestraEnabled)
				return;

			if (Player.Status == PlayerStatus.Conducting)
				return;

			if (!FFXIV.IsPerformanceReady())
				return;

			if (progdata.track != null)
				if (progdata.track != Player.Player.LoadedTrack)
					return;
			
			if (!FFXIV.Memory.ChatInputOpen)
            {
                int tone = -1;
                switch (progdata.voice)
                {
                    case 29: // overdriven guitar
                        tone = 0;
                        break;
                    case 27: // clean guitar
                        tone = 1;
                        break;
                    case 28: // muted guitar
                        tone = 2;
                        break;
                    case 30: // power chords
                        tone = 3;
                        break;
                    case 31: // special guitar
                        tone = 4;
                        break;
                }

				if (tone > -1 && tone < 5 && FFXIV.Hotkeys.GetKeybindFromToneKey(tone) is FFXIVKeybindDat.Keybind keybind)
				{
					FFXIV.Hook.SendSyncKeybind(keybind);
				}
            }
        }

        private void DonationButton_Click(object sender, EventArgs e)
        {
            var about = new BmpAbout();
            about.ShowDialog(this);
        }
    }
}