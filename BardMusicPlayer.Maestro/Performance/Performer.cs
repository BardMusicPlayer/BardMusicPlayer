/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.DalamudBridge;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.FFXIV;
using BardMusicPlayer.Maestro.Utils;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages;
using Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.EventArgs;
using Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.Message_Builders;
using Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Sequencing;
using Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Sequencing.Track_Classes;
using MessageType = BardMusicPlayer.DalamudBridge.Helper.Dalamud.MessageType;
using Sequencer = BardMusicPlayer.Maestro.Sequencing.Sequencer;

namespace BardMusicPlayer.Maestro.Performance
{
    public class Performer
    {
        private FFXIVHook _hook = new FFXIVHook();
        private System.Timers.Timer _startDelayTimer { get; set; } = new System.Timers.Timer();
        private bool _holdNotes { get; set; } = true;
        private bool _forcePlayback { get; set; } = false;
        private Sequencer _sequencer { get; set; } = null;
        private Sequencer mainSequencer { get; set; } = null;
        private int _trackNumber { get; set; } = 1;
        private long _lastNoteTimestamp = 0;
        private bool _livePlayDelay { get; set; } = false;
        public int SingerTrackNr { get; set; } = 0;
        public Instrument ChosenInstrument { get; set; } = Instrument.Piano;
        public int OctaveShift { get; set; } = 0;
        public int TrackNumber { get { return _trackNumber; }
            set {
                if (value == _trackNumber)
                    return;

                if ((_sequencer == null) || (_sequencer.LoadedTrack == null))
                {
                    BmpMaestro.Instance.PublishEvent(new TrackNumberChangedEvent(game, _trackNumber, HostProcess));
                    return;
                }

                if (value > mainSequencer.MaxTrack)
                {
                    BmpMaestro.Instance.PublishEvent(new TrackNumberChangedEvent(game, _trackNumber, HostProcess));
                    return;
                }

                _trackNumber = value;
                BmpMaestro.Instance.PublishEvent(new TrackNumberChangedEvent(game, _trackNumber, HostProcess));
                var tOctaveShift = mainSequencer.GetTrackPreferredOctaveShift(_sequencer.Sequence[this._trackNumber]);
                if (tOctaveShift != OctaveShift)
                {
                    OctaveShift = tOctaveShift;
                    BmpMaestro.Instance.PublishEvent(new OctaveShiftChangedEvent(game, OctaveShift, HostProcess));
                }
            }
        }

        public bool PerformerEnabled { get; set; } = true;
        public bool UsesDalamud {  get { return BmpPigeonhole.Instance.UsePluginForInstrumentOpen && GameExtensions.IsConnected(PId); } }
        public bool HostProcess { get; set; } = false;
        public int PId = 0;
        public Game game;
        public string PlayerName { get { return game.PlayerName ?? "Unknown"; } }
        public string HomeWorld { get { return game.HomeWorld ?? "Unknown"; } }
        public string SongName 
        { 
            get
            {
                if (_sequencer.LoadedBmpSong == null) //no song, no title
                    return "";
                
                if (_sequencer.LoadedBmpSong.DisplayedTitle == null) //no displayed title, pretent the normal title
                    return _sequencer.LoadedBmpSong.Title;

                if (_sequencer.LoadedBmpSong.DisplayedTitle.Length < 2) //title with 1 char makes no sence for me
                    return _sequencer.LoadedBmpSong.Title;

                return _sequencer.LoadedBmpSong.DisplayedTitle; //finally, display the title
            } 
        }
        public string TrackInstrument 
        { 
            get {
                    if (_sequencer == null || _sequencer.LoadedBmpSong == null)
                        return "Unknown";
                    if (TrackNumber == 0)
                        return "None";
                    if (this._trackNumber >= _sequencer.Sequence.Count)
                        return "None";

                    Transmogrify.Song.Config.ClassicProcessorConfig classicConfig = (Transmogrify.Song.Config.ClassicProcessorConfig)_sequencer.LoadedBmpSong.TrackContainers[TrackNumber - 1].ConfigContainers[0].ProcessorConfig; // track -1 cuz track 0 isn't in this container
                    return classicConfig.Instrument.Name;
                }
        }

        public Sequencer Sequencer
        {
            get{ return _sequencer; }
            set
            {
                if (value != null)
                {
                    if ((value.LoadedFileType == Sequencer.FILETYPES.None) && !HostProcess)
                        return;
                    
                    //Close the input else it will hang
                    if (_sequencer is Sequencer)
                        _sequencer.CloseInputDevice();

                    this._startDelayTimer.Enabled = false;
                    this.mainSequencer = value;

                    this._sequencer = new Sequencer();
                    if (value.LoadedFileType == Sequencer.FILETYPES.BmpSong)
                    {
                        this._sequencer.Sequence = mainSequencer.Sequence;
                        this.OctaveShift = 0;
                    }

                    if (HostProcess)
                    {
                        if (BmpPigeonhole.Instance.MidiInputDev != -1)
                            _sequencer.OpenInputDevice(BmpPigeonhole.Instance.MidiInputDev);
                    }

                    this._sequencer.OnNote += InternalNote;
                    this._sequencer.OffNote += InternalNote;
                    this._sequencer.ProgChange += InternalProg;
                    this._sequencer.OnLyric += IntenalLyrics;
                    this._sequencer.ChannelAfterTouch += InternalAT;

                    _holdNotes = BmpPigeonhole.Instance.HoldNotes;
                    _lastNoteTimestamp = 0;
                    _livePlayDelay = BmpPigeonhole.Instance.LiveMidiPlayDelay;

                    if (HostProcess && BmpPigeonhole.Instance.ForcePlayback)
                        _forcePlayback = true;

                    // set the initial octave shift here, if we have a track to play
                    if (this._trackNumber < _sequencer.Sequence.Count)
                    {
                        this.OctaveShift = mainSequencer.GetTrackPreferredOctaveShift(_sequencer.Sequence[this._trackNumber]);
                        BmpMaestro.Instance.PublishEvent(new OctaveShiftChangedEvent(game, OctaveShift, HostProcess));
                    }
                    this.Update(value);
                }
            }
        }

#region public
        public Performer(Game arg)
        {
            this.ChosenInstrument = this.ChosenInstrument;

            if (arg != null)
            {
                _hook.Hook(arg.Process, false);
                PId = arg.Pid;
                game = arg;
                _startDelayTimer.Elapsed += startDelayTimer_Elapsed;
            }
        }

        public void ProcessOnNote(NoteEvent note)
        {
            if (!_forcePlayback)
            {
                if (!this.PerformerEnabled)
                    return;

                if (game.InstrumentHeld.Equals(Instrument.None))
                    return;
            }

            if (note.note < 0 || note.note > 36)
                return;

            if (game.NoteKeys[(Quotidian.Enums.NoteKey)note.note] is Quotidian.Enums.Keys keybind)
            {
                if (game.ChatStatus && !_forcePlayback)
                    return;

                if (_holdNotes)
                    _hook.SendKeybindDown(keybind);
                else
                    _hook.SendAsyncKeybind(keybind);
            }
        }

        public void ProcessOnNoteLive(NoteEvent note)
        {
            if (!_forcePlayback)
            {
                if (!this.PerformerEnabled)
                    return;

                if (game.InstrumentHeld.Equals(Instrument.None))
                    return;
            }

            if (note.note < 0 || note.note > 36)
                return;

            if (game.NoteKeys[(Quotidian.Enums.NoteKey)note.note] is Quotidian.Enums.Keys keybind)
            {

                long diff = System.Diagnostics.Stopwatch.GetTimestamp() / 10000 - _lastNoteTimestamp;
                if (diff < 15)
                {
                    int sleepDuration = (int)(15 - diff);
                    Task.Delay(sleepDuration).Wait();
                }

                if (game.ChatStatus && !_forcePlayback)
                    return;

                if (_holdNotes)
                    _hook.SendKeybindDown(keybind);
                else
                    _hook.SendAsyncKeybind(keybind);

                _lastNoteTimestamp = System.Diagnostics.Stopwatch.GetTimestamp() / 10000;
            }
        }

        public void ProcessOffNote(NoteEvent note)
        {
            if (!this.PerformerEnabled)
                return;

            if (note.note < 0 || note.note > 36)
                return;

            if (game.NoteKeys[(Quotidian.Enums.NoteKey)note.note] is Quotidian.Enums.Keys keybind)
            {
                if (game.ChatStatus && !_forcePlayback)
                    return;

                if (_holdNotes)
                    _hook.SendKeybindUp(keybind);
            }
        }

        public void SetProgress(int progress)
        {
            if (_sequencer is Sequencer)
            {
                _sequencer.Position = progress;
            }
        }

        public void Play(bool play, int delay = 0)
        {
            if (_sequencer is Sequencer)
            {
                if (play)
                {
                    if (_sequencer.IsPlaying)
                        return;

                    if (delay == 0)
                        _sequencer.Play();
                    else
                    {
                        if (_startDelayTimer.Enabled)
                            return;
                        _startDelayTimer.Interval = delay;
                        _startDelayTimer.Enabled = true;
                    }
                }
                else
                    _sequencer.Pause();
            }
        }

        public void Stop()
        {
            if (_startDelayTimer.Enabled)
                _startDelayTimer.Enabled = false;

            if (_sequencer is Sequencer)
            {
                _sequencer.Stop();
                _hook.ClearLastPerformanceKeybinds();
            }
        }

        public void Update(Sequencer bmpSeq)
        {
            int tn = _trackNumber;

            if (!(bmpSeq is Sequencer))
            {
                return;
            }

            Sequence seq = bmpSeq.Sequence;
            if (!(seq is Sequence))
            {
                return;
            }

            if ((tn >= 0 && tn < seq.Count) && seq[tn] is Track track)
            {
                // OctaveNum now holds the track octave and the selected octave together
                Console.WriteLine(String.Format("Track #{0}/{1} setOctave: {2} prefOctave: {3}", tn, bmpSeq.MaxTrack, OctaveShift, bmpSeq.GetTrackPreferredOctaveShift(track)));
                List<int> notes = new List<int>();
                foreach (MidiEvent ev in track.Iterator())
                {
                    if (ev.MidiMessage.MessageType == Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.MessageType.Channel)
                    {
                        ChannelMessage msg = (ev.MidiMessage as ChannelMessage);
                        if (msg.Command == ChannelCommand.NoteOn)
                        {
                            int note = msg.Data1;
                            int vel = msg.Data2;
                            if (vel > 0)
                            {
                                notes.Add(NoteHelper.ApplyOctaveShift(note, this.OctaveShift));
                            }
                        }
                    }
                }
                ChosenInstrument = bmpSeq.GetTrackPreferredInstrument(track);
            }
        }

        /// <summary>
        /// Open an instrument
        /// </summary>
        public void OpenInstrument()
        {
            if (!game.InstrumentHeld.Equals(Instrument.None))
                return;

            if (!trackAndChannelOk())
                return;

            if (UsesDalamud)
                DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = MessageType.Instrument, game = game, IntData = Instrument.Parse(TrackInstrument).Index });
            else
            {
                var key = game.InstrumentKeys[Instrument.Parse(TrackInstrument)];
                if (key != Quotidian.Enums.Keys.None)
                    _hook.SendSyncKeybind(key);
            }
        }

        /// <summary>
        /// Replace the instrument
        /// </summary>
        /// <returns></returns>
        public async Task<int> ReplaceInstrument()
        {
            if (!trackAndChannelOk())
                return 0;

            if (!game.InstrumentHeld.Equals(Instrument.None))
            {
                if (game.InstrumentHeld.Equals(Instrument.Parse(TrackInstrument)))
                    return 0;
                else
                {
                    _hook.ClearLastPerformanceKeybinds();

                    if (UsesDalamud)
                        DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = MessageType.Instrument, game = game, IntData = 0 });
                    else
                        _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.ESC]);
                    await Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).ConfigureAwait(false);
                }
            }

            if (UsesDalamud)
                DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = MessageType.Instrument, game = game, IntData = Instrument.Parse(TrackInstrument).Index });
            else
            {
                var key = game.InstrumentKeys[Instrument.Parse(TrackInstrument)];
                if (key != Quotidian.Enums.Keys.None)
                    _hook.SendSyncKeybind(key);
            }

            return 0;
        }

        /// <summary>
        /// Close the instrument
        /// </summary>
        public void CloseInstrument()
        {
            if (game.InstrumentHeld.Equals(Instrument.None))
                return;

            _hook.ClearLastPerformanceKeybinds();

            if (UsesDalamud)
                DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = MessageType.Instrument, game = game, IntData = 0 });
            else
                _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.ESC]);
        }

        /// <summary>
        /// do the ready check
        /// </summary>
        public void DoReadyCheck()
        {
            if (!_forcePlayback)
            {
                if (!this.PerformerEnabled)
                    return;

                if (game.InstrumentHeld.Equals(Instrument.None))
                    return;
            }

            if (UsesDalamud)
            {
                GameExtensions.StartEnsemble(game);
                return;
            }

            Task task = Task.Run(() =>
            {
                _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.VIRTUAL_PAD_SELECT]);
                Task.Delay(100).Wait();
                _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.LEFT]);
                Task.Delay(100).Wait();
                _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.OK]);
                Task.Delay(400).Wait();
                _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.OK]);
            });
        }

        /// <summary>
        /// Accept the ready check
        /// </summary>
        public void EnsembleAccept()
        {
            if (!_forcePlayback)
            {
                if (!this.PerformerEnabled)
                    return;

                if (game.InstrumentHeld.Equals(Instrument.None))
                    return;
            }

            if (UsesDalamud)
            {
                DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = MessageType.AcceptReply, game = game, BoolData = true});
                return;
            }

            _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.OK]);
            Task.Delay(200);
            _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.OK]);
        }

        /// <summary>
        /// Close the input device
        /// </summary>
        public void Close()
        {
            if (_sequencer is Sequencer)
            {
                _sequencer.CloseInputDevice();
                _sequencer.Dispose();
            }
            _hook.ClearLastPerformanceKeybinds();
        }

        /// <summary>
        /// Send a text in game; During playback set this into a task
        /// </summary>
        /// <param name="text"></param>
        public void SendTextCopyPasta(string text)
        {
            if (!game.ChatStatus)
            {
                _hook.SendSyncKeybind(Quotidian.Enums.Keys.Enter);
                Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).Wait();
            }
            _hook.CopyToClipboard(text);
            Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).Wait();
            _hook.SendSyncKeybind(Quotidian.Enums.Keys.Enter);
        }

        public void SendText(string text)
        {
            if (!game.ChatStatus)
            {
                _hook.SendSyncKeybind(Quotidian.Enums.Keys.Enter);
                Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).Wait();
            }
            _hook.SendString(text);
            Task.Delay((text.Length * 8) + 20).Wait();
            _hook.SendSyncKeybind(Quotidian.Enums.Keys.Enter);
        }

        public void SendText(ChatMessageChannelType type, string text)
        {
            GameExtensions.SendText(game, type, text);
        }

        public void TapKey(string modifier, string character)
        {
            try
            {
                Quotidian.Enums.Keys key = Quotidian.Enums.KeyTranslation.ASCIIToGame[character];

                if (modifier.ToLower().Contains("shift"))
                    key = (int)Quotidian.Enums.Keys.Shift + key;
                else if (modifier.ToLower().Contains("ctrl"))
                    key = (int)Quotidian.Enums.Keys.Control + key;
                else if (modifier.ToLower().Contains("alt"))
                    key = (int)Quotidian.Enums.Keys.Alt + key;
                _hook.SendSyncKeybind(key);
            }
            catch
            {

            }
        }
        #endregion

        #region private
        /// <summary>
        /// Checks if we are ont the right track and channel
        /// </summary>
        /// <returns></returns>
        private bool trackAndChannelOk()
        {
            // don't open instrument if we don't have anything loaded
            if (_sequencer == null || _sequencer.Sequence == null)
                return false;

            // don't open instrument if we're not on a valid track
            if (_trackNumber == 0 || _trackNumber >= _sequencer.Sequence.Count)
                return false;
            return true;
        }

        private void startDelayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_sequencer is Sequencer)
            {
                _sequencer.Play();
                _startDelayTimer.Enabled = false;
            }
        }

        private void InternalNote(Object o, ChannelMessageEventArgs args)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder(args.Message);

            NoteEvent noteEvent = new NoteEvent
            {
                note = builder.Data1,
                origNote = builder.Data1,
                trackNum = _sequencer.GetTrackNum(args.MidiTrack),
                track = args.MidiTrack,
            };

            if ((_sequencer.GetTrackNum(noteEvent.track) == this._trackNumber) || !_sequencer.IsPlaying)
            {
                noteEvent.note = NoteHelper.ApplyOctaveShift(noteEvent.note, this.OctaveShift);

                ChannelCommand cmd = args.Message.Command;
                int vel = builder.Data2;
                if ((cmd == ChannelCommand.NoteOff) || (cmd == ChannelCommand.NoteOn && vel == 0))
                {
                    this.ProcessOffNote(noteEvent);
                }
                if ((cmd == ChannelCommand.NoteOn) && vel > 0)
                {
                    if (_livePlayDelay)
                        this.ProcessOnNoteLive(noteEvent);
                    else
                        this.ProcessOnNote(noteEvent);
                }
            }
        }

        private void InternalProg(object sender, ChannelMessageEventArgs args)
        {
            if (!_forcePlayback)
            {
                if (!this.PerformerEnabled)
                    return;

                if (game.InstrumentHeld.Equals(Instrument.None))
                    return;
            }

            var programEvent = new ProgChangeEvent
            {
                track = args.MidiTrack,
                trackNum = _sequencer.GetTrackNum(args.MidiTrack),
                voice = args.Message.Data1,
            };
            if (programEvent.voice < 27 || programEvent.voice > 31)
                return;

            if (_sequencer.GetTrackNum(programEvent.track) == this._trackNumber)
            {
                if (game.ChatStatus && !_forcePlayback)
                    return;

                int tone = -1;
                switch (programEvent.voice)
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

                if (tone > -1 && tone < 5 && game.InstrumentToneMenuKeys[(Quotidian.Enums.InstrumentToneMenuKey)tone] is Quotidian.Enums.Keys keybind)
                    _hook.SendSyncKey(keybind);
            }
        }

        private void InternalAT(object sender, ChannelMessageEventArgs args)
        {
            /*var builder = new Sanford.Multimedia.Midi.ChannelMessageBuilder(args.Message);
            var atevent = new ChannelAfterTouchEvent
            {
                track = args.MidiTrack,
                trackNum = _sequencer.GetTrackNum(args.MidiTrack),
                command = args.Message.Data1,
            };

            if (_sequencer.GetTrackNum(atevent.track) != this._trackNumber)
                return;

            switch (atevent.command)
            {
                case 0:
                    OpenInstrument();
                    break;
                case 1:
                    CloseInstrument();
                    break;
            }*/

        }

        private void IntenalLyrics(object sender, MetaMessageEventArgs e)
        {
            if (SingerTrackNr <= 0) //0 mean no singer
                return;

            if (!UsesDalamud)
                return;

            MetaTextBuilder builder = new MetaTextBuilder(e.Message);
            string text = builder.Text;
            var t = mainSequencer.MaxTrack;
            if (_sequencer.GetTrackNum(e.MidiTrack) == SingerTrackNr+ mainSequencer.LyricStartTrack-1)
                GameExtensions.SendText(game, ChatMessageChannelType.Say, text);
        }
#endregion
    }
}

