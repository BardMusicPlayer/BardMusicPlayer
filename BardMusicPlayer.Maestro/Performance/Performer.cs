/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.FFXIV;
using BardMusicPlayer.Maestro.Sequencing;
using BardMusicPlayer.Maestro.Utils;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public bool IsSinger { get; set; } = false;
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
        public EventHandler onUpdate;
        public bool HostProcess { get; set; } = false;
        public int PId = 0;
        public Game game;
        public string PlayerName { get { return game.PlayerName ?? "Unknown"; } }
        public string HomeWorld { get { return game.HomeWorld ?? "Unknown"; } }
        public string SongName { get { 
            return _sequencer.LoadedBmpSong == null ? "" : 
                  (_sequencer.LoadedBmpSong.DisplayedTitle.Length < 2 ? _sequencer.LoadedBmpSong.Title : _sequencer.LoadedBmpSong.DisplayedTitle); 
            } }
        public string TrackInstrument 
        { 
            get {
                    if (_sequencer == null || _sequencer.LoadedBmpSong == null)
                        return "Unknown";
                    if (TrackNumber == 0)
                        return "None";
                    if (this._trackNumber >= _sequencer.Sequence.Count)
                        return "None";

                    var t = _sequencer.LoadedBmpSong.TrackContainers[TrackNumber - 1].SourceTrackChunk.Events.OfType<Melanchall.DryWetMidi.Core.SequenceTrackNameEvent>().FirstOrDefault()?.Text;
                    if (t != null)
                    {
                        if (t.StartsWith("Lyric:"))
                            return t;
                    }
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

            Sanford.Multimedia.Midi.Sequence seq = bmpSeq.Sequence;
            if (!(seq is Sanford.Multimedia.Midi.Sequence))
            {
                return;
            }

            if ((tn >= 0 && tn < seq.Count) && seq[tn] is Sanford.Multimedia.Midi.Track track)
            {
                // OctaveNum now holds the track octave and the selected octave together
                Console.WriteLine(String.Format("Track #{0}/{1} setOctave: {2} prefOctave: {3}", tn, bmpSeq.MaxTrack, OctaveShift, bmpSeq.GetTrackPreferredOctaveShift(track)));
                List<int> notes = new List<int>();
                foreach (Sanford.Multimedia.Midi.MidiEvent ev in track.Iterator())
                {
                    if (ev.MidiMessage.MessageType == Sanford.Multimedia.Midi.MessageType.Channel)
                    {
                        Sanford.Multimedia.Midi.ChannelMessage msg = (ev.MidiMessage as Sanford.Multimedia.Midi.ChannelMessage);
                        if (msg.Command == Sanford.Multimedia.Midi.ChannelCommand.NoteOn)
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

        public void OpenInstrument()
        {
            if (IsSinger)
                return;

            if (!game.InstrumentHeld.Equals(Instrument.None))
                return;

            if (!trackAndChannelOk())
                return;

            _hook.SendSyncKeybind(game.InstrumentKeys[Instrument.Parse(TrackInstrument)]);
        }

        public async Task<int> ReplaceInstrument()
        {
            if (IsSinger)
                return 0;

            if (!trackAndChannelOk())
                return 0;

            if (!game.InstrumentHeld.Equals(Instrument.None))
            {
                if (game.InstrumentHeld.Equals(Instrument.Parse(TrackInstrument)))
                    return 0;
                else
                {
                    _hook.ClearLastPerformanceKeybinds();
                    _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.ESC]);
                    await Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).ConfigureAwait(false);
                }
            }

            _hook.SendSyncKeybind(game.InstrumentKeys[Instrument.Parse(TrackInstrument)]);
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
            _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.ESC]);
        }

        /// <summary>
        /// Accept the ready check
        /// </summary>
        public void EnsembleAccept()
        {
            if (IsSinger)
                return;

            if (!_forcePlayback)
            {
                if (!this.PerformerEnabled)
                    return;

                if (game.InstrumentHeld.Equals(Instrument.None))
                    return;
            }

            _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.OK]);
            Task.Delay(200);
            _hook.SendSyncKeybind(game.NavigationMenuKeys[Quotidian.Enums.NavigationMenuKey.OK]);
        }

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

        private void InternalNote(Object o, Sanford.Multimedia.Midi.ChannelMessageEventArgs args)
        {
            if (IsSinger)
                return;

            Sanford.Multimedia.Midi.ChannelMessageBuilder builder = new Sanford.Multimedia.Midi.ChannelMessageBuilder(args.Message);

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

                Sanford.Multimedia.Midi.ChannelCommand cmd = args.Message.Command;
                int vel = builder.Data2;
                if ((cmd == Sanford.Multimedia.Midi.ChannelCommand.NoteOff) || (cmd == Sanford.Multimedia.Midi.ChannelCommand.NoteOn && vel == 0))
                {
                    this.ProcessOffNote(noteEvent);
                }
                if ((cmd == Sanford.Multimedia.Midi.ChannelCommand.NoteOn) && vel > 0)
                {
                    if (_livePlayDelay)
                        this.ProcessOnNoteLive(noteEvent);
                    else
                        this.ProcessOnNote(noteEvent);
                }
            }
        }

        private void InternalProg(object sender, Sanford.Multimedia.Midi.ChannelMessageEventArgs args)
        {
            if (IsSinger)
                return;

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

        private void InternalAT(object sender, Sanford.Multimedia.Midi.ChannelMessageEventArgs args)
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

        private void IntenalLyrics(object sender, Sanford.Multimedia.Midi.MetaMessageEventArgs e)
        {
            if (!IsSinger)
                return;

            Sanford.Multimedia.Midi.MetaTextBuilder builder = new Sanford.Multimedia.Midi.MetaTextBuilder(e.Message);
            string text = builder.Text;
            if (_sequencer.GetTrackNum(e.MidiTrack) == _trackNumber)
                SendText(text);
        }
#endregion
    }
}

