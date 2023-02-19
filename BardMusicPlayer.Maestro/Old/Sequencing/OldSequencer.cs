/*
 * Copyright(c) 2022 Parulina, trotlinebeercan, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.DeviceClasses.InputDeviceClass;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages.EventArg;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages.MessageBuilders;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Processing;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Sequencing;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Sequencing.TrackClasses;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Maestro.Old.Sequencing;

public class OldSequencer : Sequencer_Internal
{
    private InputDevice midiInput;

    private Dictionary<Track, Instrument> preferredInstruments = new();
    private Dictionary<Track, int> preferredOctaveShift = new();

    public EventHandler OnLoad;
    public EventHandler<ChannelMessageEventArgs> OnNote;
    public EventHandler<ChannelMessageEventArgs> OffNote;
    public EventHandler<ChannelMessageEventArgs> ProgChange;
    public EventHandler<ChannelMessageEventArgs> ChannelAfterTouch;

    public EventHandler<MetaMessageEventArgs> OnLyric;
    public EventHandler<int> OnTempoChange;
    public EventHandler<string> OnTrackNameChange;

    private Timer secondTimer = new(200);
    public EventHandler<int> OnTick;

    public Dictionary<Track, int> notesPlayedCount = new();

    public string LoadedError { get; private set; } = string.Empty;

    public enum FILETYPES
    {
        None = 0,
        BmpSong = 1
    }

    public FILETYPES LoadedFileType { get; private set; } = FILETYPES.None;

    public static string LoadedFilename => string.Empty;

    public BmpSong LoadedBmpSong { get; set; }

    public bool Loaded => Sequence != null;

    private int midiTempo = 120;

    public int CurrentTick => Position;

    public int MaxTick => Length;

    public string CurrentTime
    {
        get
        {
            var ms = GetTimeFromTick(CurrentTick);
            var t = TimeSpan.FromMilliseconds(ms);
            return $"{(int)t.TotalMinutes:D2}:{t.Seconds:D2}";
        }
    }

    public TimeSpan CurrentTimeAsTimeSpan
    {
        get
        {
            var ms = GetTimeFromTick(CurrentTick);
            var t = TimeSpan.FromMilliseconds(ms);
            return t;
        }
    }

    public string MaxTime
    {
        get
        {
            var ms = GetTimeFromTick(MaxTick - 1);
            var t = TimeSpan.FromMilliseconds(ms);
            return $"{(int)t.TotalMinutes:D2}:{t.Seconds:D2}";
        }
    }

    public TimeSpan MaxTimeAsTimeSpan
    {
        get
        {
            var ms = GetTimeFromTick(MaxTick - 1);
            var t = TimeSpan.FromMilliseconds(ms);
            return t;
        }
    }

    private int loadedTrack = 0;
    private int intendedTrack = 0;
    public int CurrentTrack => loadedTrack;

    int _lyricStartTrackIndex = 0;
    public int LyricStartTrack => _lyricStartTrackIndex;

    private int _maxTracks = 0;
    public int MaxTrack
    {
        get
        {
            if (_maxTracks < 0)
            {
                return 0;
            }
            return _maxTracks;
        }
    }

    public int MaxAllTrack
    {
        get
        {
            if (Sequence.Count <= 0)
            {
                return 0;
            }
            return Sequence.Count - 1;
        }
    }

    public Track LoadedTrack
    {
        get
        {
            if (CurrentTrack >= Sequence.Count || CurrentTrack < 0)
            {
                return null;
            }

            return BmpPigeonhole.Instance.PlayAllTracks ? Sequence[0] : Sequence[CurrentTrack];
        }
    }

    public int lyricCount = 0;
    public int LyricNum => lyricCount;

    public OldSequencer()
    {
        Sequence = new Sequence();

        ChannelMessagePlayed += OnChannelMessagePlayed;
        MetaMessagePlayed += OnMetaMessagePlayed;

        secondTimer.Elapsed += OnSecondTimer;
    }

    public int GetTrackNum(Track track)
    {
        for (var i = 0; i < Sequence.Count; i++)
        {
            if (Sequence[i] == track)
            {
                return i;
            }
        }
        return -1;
    }

    private void OnSecondTimer(object sender, EventArgs e)
    {
        OnTick?.Invoke(this, Position);
    }

    public void Seek(double ms)
    {
        var ticks = (int)(Sequence.Division * (midiTempo / 60000f * ms));
        if (Position + ticks < MaxTick && Position + ticks >= 0)
        {
            Position = ticks;
        }
    }

    public void Seek(int ticks)
    {
        Position = ticks;
    }

    public new void Play()
    {
        secondTimer.Start();
        OnSecondTimer(this, EventArgs.Empty);
        base.Play();
    }
    public new void Pause()
    {
        secondTimer.Stop();
        base.Pause();
    }

    public static float GetTimeFromTick(int tick)
    {
        if (tick <= 0)
        {
            return 0f;
        }
        return tick; // midi ppq and tempo  tick = 1ms now.
    }

    private void Chaser_Chased(object sender, ChasedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void OpenInputDevice(int device)
    {
        if (device == -1)
        {
            Console.WriteLine(@"[Sequencer] No Midi input");
            return;
        }
        var cap = InputDevice.GetDeviceCapabilities(device);
        try
        {
            midiInput = new InputDevice(device);
            midiInput.StartRecording();
            midiInput.ChannelMessageReceived += OnSimpleChannelMessagePlayed;

            Console.WriteLine(@"{0} opened.", cap.name);
        }
        catch (InputDeviceException)
        {
            Console.WriteLine(@"Couldn't open input {0}.", device);
        }
    }

    public void OpenInputDevice(string device)
    {
        for (var i = 0; i < InputDevice.DeviceCount; i++)
        {
            var cap = InputDevice.GetDeviceCapabilities(i);
            if (cap.name == device)
            {
                try
                {
                    midiInput = new InputDevice(i);
                    midiInput.StartRecording();
                    midiInput.ChannelMessageReceived += OnSimpleChannelMessagePlayed;

                    Console.WriteLine(@"{0} opened.", cap.name);
                }
                catch (InputDeviceException)
                {
                    Console.WriteLine(@"Couldn't open input {0}.", device);
                }
            }
        }
    }

    public void CloseInputDevice()
    {
        if (midiInput is { IsDisposed: false })
        {
            midiInput.StopRecording();
            midiInput.Close();
        }
    }

    private void OnSimpleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
    {
        var builder = new ChannelMessageBuilder(e.Message);
        var note = builder.Data1;
        var vel = builder.Data2;
        var cmd = e.Message.Command;
        if (cmd == ChannelCommand.NoteOff || cmd == ChannelCommand.NoteOn && vel == 0)
        {
            OffNote?.Invoke(this, e);
        }
        switch (cmd)
        {
            case ChannelCommand.NoteOn when vel > 0:
                OnNote?.Invoke(this, e);
                break;
            case ChannelCommand.ProgramChange:
                {
                    string instName = Instrument.ParseByProgramChange(e.Message.Data1);
                    if (!string.IsNullOrEmpty(instName))
                        ProgChange?.Invoke(this, e);
                    break;
                }
            case ChannelCommand.ChannelPressure:
                ChannelAfterTouch?.Invoke(this, e);
                break;
        }
    }

    private void OnChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
    {
        OnSimpleChannelMessagePlayed(sender, e);
    }
    private void OnMetaMessagePlayed(object sender, MetaMessageEventArgs e)
    {
        switch (e.Message.MetaType)
        {
            case MetaType.Tempo:
                {
                    var builder = new TempoChangeBuilder(e.Message);
                    midiTempo = 60000000 / builder.Tempo;
                    OnTempoChange?.Invoke(this, midiTempo);
                    break;
                }
            case MetaType.Lyric:
                OnLyric?.Invoke(this, e);
                break;
            case MetaType.TrackName:
                {
                    var builder = new MetaTextBuilder(e.Message);
                    ParseTrackName(e.MidiTrack, builder.Text);
                    if (e.MidiTrack == LoadedTrack)
                        OnTrackNameChange?.Invoke(this, builder.Text);
                    break;
                }
            case MetaType.InstrumentName:
                {
                    var builder = new MetaTextBuilder(e.Message);
                    OnTrackNameChange?.Invoke(this, builder.Text);
                    Console.WriteLine(@"Instrument name: " + builder.Text);
                    break;
                }
        }
    }

    public void ParseTrackName(Track track, string trackName)
    {
        if (track == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(trackName))
        {
            preferredInstruments[track] = Instrument.Piano;
            preferredOctaveShift[track] = 0;
        }
        else
        {
            var rex = new Regex(@"^([A-Za-z]+)([-+]\d)?");
            if (rex.Match(trackName) is { } match)
            {
                var instrument = match.Groups[1].Value;
                var octaveshift = match.Groups[2].Value;

                var foundInstrument = false;

                if (!string.IsNullOrEmpty(instrument))
                {
                    if (Instrument.TryParse(instrument, out var tempInst))
                    {
                        preferredInstruments[track] = tempInst;
                        foundInstrument = true;
                    }
                }
                if (foundInstrument)
                {
                    if (!string.IsNullOrEmpty(octaveshift))
                    {
                        if (int.TryParse(octaveshift, out var os))
                        {
                            if (Math.Abs(os) <= 4)
                            {
                                preferredOctaveShift[track] = os;
                            }
                        }
                    }
                }
            }
        }
    }

    public Instrument GetTrackPreferredInstrument(int tracknumber)
    {
        return tracknumber >= preferredInstruments.Count ? Instrument.None : preferredInstruments.ElementAt(tracknumber).Value;
    }

    public Instrument GetTrackPreferredInstrument(Track track)
    {
        if (track != null)
        {
            if (preferredInstruments.ContainsKey(track))
            {
                return preferredInstruments[track];
            }
        }
        return Instrument.Piano;
    }

    public int GetTrackPreferredOctaveShift(Track track)
    {
        if (track != null)
        {
            if (preferredOctaveShift.ContainsKey(track))
            {
                return preferredOctaveShift[track];
            }
        }
        return 0;
    }

    public void Load(BmpSong bmpSong, int trackNum = 1)
    {
        if (bmpSong == null)
            return;

        LoadedFileType = FILETYPES.BmpSong;
        LoadedBmpSong = bmpSong;

        using var midiStream = new MemoryStream();
        bmpSong.GetProcessedMidiFile().Result.Write(midiStream, MidiFileFormat.MultiTrack,
                   new WritingSettings { TextEncoding = Encoding.ASCII });
        midiStream.Flush();
        midiStream.Position = 0;

        Sequence = new Sequence(midiStream);
        load(Sequence, trackNum);
    }

    public void load(Sequence sequence, int trackNum = 1)
    {
        OnTrackNameChange?.Invoke(this, string.Empty);
        OnTempoChange?.Invoke(this, 0);

        LoadedError = string.Empty;
        if (trackNum >= Sequence.Count)
        {
            trackNum = Sequence.Count - 1;
        }
        intendedTrack = trackNum;

        preferredInstruments.Clear();
        preferredOctaveShift.Clear();

        // Collect statistics
        notesPlayedCount.Clear();
        foreach (var track in Sequence)
        {
            notesPlayedCount[track] = 0;
            foreach (var ev in track.Iterator())
            {
                if (ev.MidiMessage is ChannelMessage { Command: ChannelCommand.NoteOn, Data2: > 0 })
                {
                    notesPlayedCount[track]++;
                }
            }
        }

        // Count notes and select fìrst that actually has stuff
        if (trackNum == 1)
        {
            while (trackNum < Sequence.Count)
            {
                var tnotes = 0;

                foreach (var ev in Sequence[trackNum].Iterator())
                {
                    if (intendedTrack == 1)
                    {
                        switch (ev.MidiMessage)
                        {
                            case ChannelMessage { Command: ChannelCommand.NoteOn }:
                            case MetaMessage { MetaType: MetaType.Lyric }:
                                tnotes++;
                                break;
                        }
                    }
                }

                if (tnotes == 0)
                {
                    trackNum++;
                }
                else
                {
                    break;
                }
            }
            if (trackNum == Sequence.Count)
            {
                Console.WriteLine(@"No playable track...");
                trackNum = intendedTrack;
            }
        }

        // Show initial tempo
        foreach (var ev in Sequence[0].Iterator())
        {
            if (ev.AbsoluteTicks == 0)
            {
                if (ev.MidiMessage is MetaMessage { MetaType: MetaType.Tempo } metaMsg)
                {
                    OnMetaMessagePlayed(this, new MetaMessageEventArgs(Sequence[0], metaMsg));
                }
            }
        }

        // Parse track names and octave shifts
        _maxTracks = -1;
        _lyricStartTrackIndex = -1;
        foreach (var track in Sequence)
        {
            foreach (var ev in track.Iterator())
            {
                if (ev.MidiMessage is MetaMessage { MetaType: MetaType.TrackName } metaMsg)
                {
                    var builder = new MetaTextBuilder(metaMsg);
                    if (builder.Text.ToLower().Contains("lyrics:") && _lyricStartTrackIndex == -1)
                        _lyricStartTrackIndex = _maxTracks + 1;
                    else
                    {
                        ParseTrackName(track, builder.Text);
                        _maxTracks++;
                    }
                }
            }
        }

        loadedTrack = trackNum;
        // Search beginning for text stuff
        foreach (var ev in LoadedTrack.Iterator())
        {
            switch (ev.MidiMessage)
            {
                case MetaMessage msg:
                    {
                        switch (msg.MetaType)
                        {
                            case MetaType.TrackName:
                                OnMetaMessagePlayed(this, new MetaMessageEventArgs(LoadedTrack, msg));
                                break;
                            case MetaType.Lyric:
                                lyricCount++;
                                break;
                        }

                        break;
                    }
                case ChannelMessage { Command: ChannelCommand.ProgramChange } chanMsg:
                    OnSimpleChannelMessagePlayed(this, new ChannelMessageEventArgs(Sequence[0], chanMsg));
                    break;
            }
        }

        OnLoad?.Invoke(this, EventArgs.Empty);
    }
}