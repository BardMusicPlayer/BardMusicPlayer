/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BardMusicPlayer.DalamudBridge;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.FFXIV;
using BardMusicPlayer.Maestro.Utils;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Transmogrify;
using BardMusicPlayer.Transmogrify.Song.Config;
using Sanford.Multimedia.Midi;
using Sequencer = BardMusicPlayer.Maestro.Sequencing.Sequencer;

namespace BardMusicPlayer.Maestro.Performance;

public class Performer
{
    private FFXIVHook _hook = new();
    private Timer _startDelayTimer { get; set; } = new();
    private bool _holdNotes { get; set; } = true;
    private bool _forcePlayback { get; set; }
    private Sequencer _sequencer { get; set; }
    private Sequencer mainSequencer { get; set; }
    private int _trackNumber { get; set; } = 1;
    private long _lastNoteTimestamp;
    private bool _livePlayDelay { get; set; }
    public int SingerTrackNr { get; set; }
    public Instrument ChosenInstrument { get; set; } = Instrument.Piano;
    public int OctaveShift { get; set; }
    public int TrackNumber { get => _trackNumber;
        set {
            if (value == _trackNumber)
                return;

            if (_sequencer?.LoadedTrack == null)
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
            var tOctaveShift = mainSequencer.GetTrackPreferredOctaveShift(_sequencer.Sequence[_trackNumber]);
            if (tOctaveShift != OctaveShift)
            {
                OctaveShift = tOctaveShift;
                BmpMaestro.Instance.PublishEvent(new OctaveShiftChangedEvent(game, OctaveShift, HostProcess));
            }
        }
    }

    public bool PerformerEnabled { get; set; } = true;
    public bool UsesDalamud => BmpPigeonhole.Instance.UsePluginForInstrumentOpen && GameExtensions.IsConnected(PId);

    public bool HostProcess { get; set; }
    public int PId;
    public Game game;
    public string PlayerName => game.PlayerName ?? "Unknown";

    public string HomeWorld => game.HomeWorld ?? "Unknown";

    public string SongName 
    { 
        get
        {
            if (_sequencer.LoadedBmpSong == null) //no song, no title
                return "";
                
            if (_sequencer.LoadedBmpSong.DisplayedTitle == null) //no displayed title, pretend the normal title
                return _sequencer.LoadedBmpSong.Title;

            return _sequencer.LoadedBmpSong.DisplayedTitle.Length < 2 ?                   //title with 1 char makes no sense for me
                _sequencer.LoadedBmpSong.Title : _sequencer.LoadedBmpSong.DisplayedTitle; //finally, display the title
        } 
    }
    public string TrackInstrument
    {
        get
        {
            if (_sequencer == null || _sequencer.LoadedBmpSong == null)
                return "Unknown";
            if (TrackNumber == 0)
                return "None";
            if (this._trackNumber >= _sequencer.Sequence.Count)
                return "None";
            if (_sequencer.LoadedBmpSong.TrackContainers[TrackNumber - 1].ConfigContainers.Count == 0)
                return "None";
            try
            {
                Transmogrify.Song.Config.ClassicProcessorConfig classicConfig = (Transmogrify.Song.Config.ClassicProcessorConfig)_sequencer.LoadedBmpSong.TrackContainers[TrackNumber - 1].ConfigContainers[0].ProcessorConfig; // track -1 cuz track 0 isn't in this container

                return classicConfig.Instrument.Name;
            }
            catch (BmpTransmogrifyException e)
            {

                return "Unknown";
            }
        }
    }

    public Sequencer Sequencer
    {
        get => _sequencer;
        set
        {
            if (value != null)
            {
                if (value.LoadedFileType == Sequencer.FILETYPES.None && !HostProcess)
                    return;
                    
                //Close the input else it will hang
                _sequencer?.CloseInputDevice();

                _startDelayTimer.Enabled = false;
                mainSequencer            = value;

                _sequencer = new Sequencer();
                if (value.LoadedFileType == Sequencer.FILETYPES.BmpSong)
                {
                    _sequencer.Sequence = mainSequencer.Sequence;
                    OctaveShift         = 0;
                }

                if (HostProcess)
                {
                    if (BmpPigeonhole.Instance.MidiInputDev != -1)
                        _sequencer.OpenInputDevice(BmpPigeonhole.Instance.MidiInputDev);
                }

                _sequencer.OnNote            += InternalNote;
                _sequencer.OffNote           += InternalNote;
                _sequencer.ProgChange        += InternalProg;
                _sequencer.OnLyric           += InternalLyrics;
                _sequencer.ChannelAfterTouch += InternalAT;

                _holdNotes         = BmpPigeonhole.Instance.HoldNotes;
                _lastNoteTimestamp = 0;
                _livePlayDelay     = BmpPigeonhole.Instance.LiveMidiPlayDelay;

                if (HostProcess && BmpPigeonhole.Instance.ForcePlayback)
                    _forcePlayback = true;

                // set the initial octave shift here, if we have a track to play
                if (_trackNumber < _sequencer.Sequence.Count)
                {
                    OctaveShift = mainSequencer.GetTrackPreferredOctaveShift(_sequencer.Sequence[_trackNumber]);
                    BmpMaestro.Instance.PublishEvent(new OctaveShiftChangedEvent(game, OctaveShift, HostProcess));
                }
                Update(value);
            }
        }
    }

    #region public
    public Performer(Game arg)
    {
        ChosenInstrument = ChosenInstrument;

        if (arg != null)
        {
            _hook.Hook(arg.Process, false);
            PId                      =  arg.Pid;
            game                     =  arg;
            _startDelayTimer.Elapsed += startDelayTimer_Elapsed;
        }
    }

    public void ProcessOnNote(NoteEvent note)
    {
        if (!_forcePlayback)
        {
            if (!PerformerEnabled)
                return;

            if (game.InstrumentHeld.Equals(Instrument.None))
                return;
        }

        if (note.note is < 0 or > 36)
            return;

        if (game.NoteKeys[(NoteKey)note.note] is var keybind)
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
            if (!PerformerEnabled)
                return;

            if (game.InstrumentHeld.Equals(Instrument.None))
                return;
        }

        if (note.note is < 0 or > 36)
            return;

        if (game.NoteKeys[(NoteKey)note.note] is var keybind)
        {

            var diff = Stopwatch.GetTimestamp() / 10000 - _lastNoteTimestamp;
            if (diff < 15)
            {
                var sleepDuration = (int)(15 - diff);
                Task.Delay(sleepDuration).Wait();
            }

            if (game.ChatStatus && !_forcePlayback)
                return;

            if (_holdNotes)
                _hook.SendKeybindDown(keybind);
            else
                _hook.SendAsyncKeybind(keybind);

            _lastNoteTimestamp = Stopwatch.GetTimestamp() / 10000;
        }
    }

    public void ProcessOffNote(NoteEvent note)
    {
        if (!PerformerEnabled)
            return;

        if (note.note is < 0 or > 36)
            return;

        if (game.NoteKeys[(NoteKey)note.note] is var keybind)
        {
            if (game.ChatStatus && !_forcePlayback)
                return;

            if (_holdNotes)
                _hook.SendKeybindUp(keybind);
        }
    }

    public void SetProgress(int progress)
    {
        if (_sequencer is not null)
        {
            _sequencer.Position = progress;
        }
    }

    public void Play(bool play, int delay = 0)
    {
        if (_sequencer is not null)
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
                    _startDelayTimer.Enabled  = true;
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

        if (_sequencer is not null)
        {
            _sequencer.Stop();
            _hook.ClearLastPerformanceKeybinds();
        }
    }

    public void Update(Sequencer bmpSeq)
    {
        var tn = _trackNumber;

        var seq = bmpSeq?.Sequence;
        if (seq is null)
        {
            return;
        }

        if (tn >= 0 && tn < seq.Count && seq[tn] is { } track)
        {
            // OctaveNum now holds the track octave and the selected octave together
            Console.WriteLine(@"Track #{0}/{1} setOctave: {2} prefOctave: {3}", tn, bmpSeq.MaxTrack, OctaveShift, bmpSeq.GetTrackPreferredOctaveShift(track));
            var notes = (from ev in track.Iterator() where ev.MidiMessage.MessageType == MessageType.Channel select ev.MidiMessage as ChannelMessage into msg where msg.Command == ChannelCommand.NoteOn let note = msg.Data1 let vel = msg.Data2 where vel > 0 select NoteHelper.ApplyOctaveShift(note, OctaveShift)).ToList();
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
            DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = DalamudBridge.Helper.Dalamud.MessageType.Instrument, game = game, IntData = Instrument.Parse(TrackInstrument).Index });
        else
        {
            var key = game.InstrumentKeys[Instrument.Parse(TrackInstrument)];
            if (key != Keys.None)
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

            _hook.ClearLastPerformanceKeybinds();

            if (UsesDalamud)
                DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = DalamudBridge.Helper.Dalamud.MessageType.Instrument, game = game, IntData = 0 });
            else
                _hook.SendSyncKeybind(game.NavigationMenuKeys[NavigationMenuKey.ESC]);
            await Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).ConfigureAwait(false);
        }

        if (UsesDalamud)
            DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = DalamudBridge.Helper.Dalamud.MessageType.Instrument, game = game, IntData = Instrument.Parse(TrackInstrument).Index });
        else
        {
            var key = game.InstrumentKeys[Instrument.Parse(TrackInstrument)];
            if (key != Keys.None)
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
            DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = DalamudBridge.Helper.Dalamud.MessageType.Instrument, game = game, IntData = 0 });
        else
            _hook.SendSyncKeybind(game.NavigationMenuKeys[NavigationMenuKey.ESC]);
    }

    /// <summary>
    /// do the ready check
    /// </summary>
    public void DoReadyCheck()
    {
        if (!_forcePlayback)
        {
            if (!PerformerEnabled)
                return;

            if (game.InstrumentHeld.Equals(Instrument.None))
                return;
        }

        if (UsesDalamud)
        {
            game.StartEnsemble();
            return;
        }

        var task = Task.Run(() =>
        {
            _hook.SendSyncKeybind(game.NavigationMenuKeys[NavigationMenuKey.VIRTUAL_PAD_SELECT]);
            Task.Delay(100).Wait();
            _hook.SendSyncKeybind(game.NavigationMenuKeys[NavigationMenuKey.LEFT]);
            Task.Delay(100).Wait();
            _hook.SendSyncKeybind(game.NavigationMenuKeys[NavigationMenuKey.OK]);
            Task.Delay(400).Wait();
            _hook.SendSyncKeybind(game.NavigationMenuKeys[NavigationMenuKey.OK]);
        });
    }

    /// <summary>
    /// Accept the ready check
    /// </summary>
    public void EnsembleAccept()
    {
        if (!_forcePlayback)
        {
            if (!PerformerEnabled)
                return;

            if (game.InstrumentHeld.Equals(Instrument.None))
                return;
        }

        if (UsesDalamud)
        {
            DalamudBridge.DalamudBridge.Instance.ActionToQueue(new DalamudBridgeCommandStruct { messageType = DalamudBridge.Helper.Dalamud.MessageType.AcceptReply, game = game, BoolData = true});
            return;
        }

        _hook.SendSyncKeybind(game.NavigationMenuKeys[NavigationMenuKey.OK]);
        Task.Delay(200);
        _hook.SendSyncKeybind(game.NavigationMenuKeys[NavigationMenuKey.OK]);
    }

    /// <summary>
    /// Close the input device
    /// </summary>
    public void Close()
    {
        if (_sequencer is not null)
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
            _hook.SendSyncKeybind(Keys.Enter);
            Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).Wait();
        }
        _hook.CopyToClipboard(text);
        Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).Wait();
        _hook.SendSyncKeybind(Keys.Enter);
    }

    public void SendText(string text)
    {
        if (!game.ChatStatus)
        {
            _hook.SendSyncKeybind(Keys.Enter);
            Task.Delay(BmpPigeonhole.Instance.EnsembleReadyDelay).Wait();
        }
        _hook.SendString(text);
        Task.Delay(text.Length * 8 + 20).Wait();
        _hook.SendSyncKeybind(Keys.Enter);
    }

    public void SendText(ChatMessageChannelType type, string text)
    {
        game.SendText(type, text);
    }

    public void TapKey(string modifier, string character)
    {
        try
        {
            var key = KeyTranslation.ASCIIToGame[character];

            if (modifier.ToLower().Contains("shift"))
                key = (int)Keys.Shift + key;
            else if (modifier.ToLower().Contains("ctrl"))
                key = (int)Keys.Control + key;
            else if (modifier.ToLower().Contains("alt"))
                key = (int)Keys.Alt + key;
            _hook.SendSyncKeybind(key);
        }
        catch
        {
            // ignored
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
        if (_sequencer?.Sequence == null)
            return false;

        // don't open instrument if we're not on a valid track
        return _trackNumber != 0 && _trackNumber < _sequencer.Sequence.Count;
    }

    private void startDelayTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (_sequencer is not null)
        {
            _sequencer.Play();
            _startDelayTimer.Enabled = false;
        }
    }

    private void InternalNote(object o, ChannelMessageEventArgs args)
    {
        var builder = new ChannelMessageBuilder(args.Message);

        var noteEvent = new NoteEvent
        {
            note     = builder.Data1,
            origNote = builder.Data1,
            trackNum = _sequencer.GetTrackNum(args.MidiTrack),
            track    = args.MidiTrack
        };

        if (_sequencer.GetTrackNum(noteEvent.track) == _trackNumber || !_sequencer.IsPlaying)
        {
            noteEvent.note = NoteHelper.ApplyOctaveShift(noteEvent.note, OctaveShift);

            var cmd = args.Message.Command;
            var vel = builder.Data2;
            if (cmd == ChannelCommand.NoteOff || cmd == ChannelCommand.NoteOn && vel == 0)
            {
                ProcessOffNote(noteEvent);
            }
            if (cmd == ChannelCommand.NoteOn && vel > 0)
            {
                if (_livePlayDelay)
                    ProcessOnNoteLive(noteEvent);
                else
                    ProcessOnNote(noteEvent);
            }
        }
    }

    private void InternalProg(object sender, ChannelMessageEventArgs args)
    {
        if (!_forcePlayback)
        {
            if (!PerformerEnabled)
                return;

            if (game.InstrumentHeld.Equals(Instrument.None))
                return;
        }

        var programEvent = new ProgChangeEvent
        {
            track    = args.MidiTrack,
            trackNum = _sequencer.GetTrackNum(args.MidiTrack),
            voice    = args.Message.Data1
        };
        if (programEvent.voice is < 27 or > 31)
            return;

        if (_sequencer.GetTrackNum(programEvent.track) == _trackNumber)
        {
            if (game.ChatStatus && !_forcePlayback)
                return;

            var tone = programEvent.voice switch
            {
                29 => // overdriven guitar
                    0,
                27 => // clean guitar
                    1,
                28 => // muted guitar
                    2,
                30 => // power chords
                    3,
                31 => // special guitar
                    4,
                _ => -1
            };

            if (tone is > -1 and < 5 && game.InstrumentToneMenuKeys[(InstrumentToneMenuKey)tone] is var keybind)
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

    private void InternalLyrics(object sender, MetaMessageEventArgs e)
    {
        if (SingerTrackNr <= 0) //0 mean no singer
            return;

        if (!UsesDalamud)
            return;

        var builder = new MetaTextBuilder(e.Message);
        var text = builder.Text;
        var t = mainSequencer.MaxTrack;
        if (_sequencer.GetTrackNum(e.MidiTrack) == SingerTrackNr+ Sequencer.LyricStartTrack-1)
            game.SendText(ChatMessageChannelType.Say, text);
    }
    #endregion
}