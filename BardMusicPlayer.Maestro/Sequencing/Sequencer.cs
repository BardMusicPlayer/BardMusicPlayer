/*
 * Copyright(c) 2022 Parulina, trotlinebeercan, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Timer = System.Timers.Timer;

using Sanford.Multimedia.Midi;
using System.Text.RegularExpressions;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Maestro.Utils;
using BardMusicPlayer.Maestro.Sequencing.Internal;
using BardMusicPlayer.Transmogrify.Song;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Maestro.Sequencing
{
    public class Sequencer : Sequencer_Internal
    {
        InputDevice midiInput = null;

        private Dictionary<Track, Instrument> preferredInstruments = new Dictionary<Track, Instrument>();
        private Dictionary<Track, int> preferredOctaveShift = new Dictionary<Track, int>();

        public EventHandler OnLoad;
        public EventHandler<ChannelMessageEventArgs> OnNote;
        public EventHandler<ChannelMessageEventArgs> OffNote;
        public EventHandler<ChannelMessageEventArgs> ProgChange;
        public EventHandler<ChannelMessageEventArgs> ChannelAfterTouch;

        public EventHandler<MetaMessageEventArgs> OnLyric;
        public EventHandler<int> OnTempoChange;
        public EventHandler<string> OnTrackNameChange;

        private Timer secondTimer = new Timer(200);
        public EventHandler<int> OnTick;

        public Dictionary<Track, int> notesPlayedCount = new Dictionary<Track, int>();


        private string loadedError = string.Empty;
        public string LoadedError
        {
            get { return loadedError; }
        }

        public enum FILETYPES
        {
            None = 0,
            BmpSong = 1
        };

        FILETYPES loadedFileType = FILETYPES.None;
        public FILETYPES LoadedFileType
        {
            get
            {
                return loadedFileType;
            }
        }

        string loadedFilename = string.Empty;
        public string LoadedFilename
        {
            get
            {
                return loadedFilename;
            }
        }

        BmpSong loadedBmpSong = null;
        public BmpSong LoadedBmpSong
        {
            get
            {
                return loadedBmpSong;
            }
            set
            {
                loadedBmpSong = value;
            }
        }

        public bool Loaded
        {
            get
            {
                return (Sequence != null);
            }
        }

        int midiTempo = 120;

        public int CurrentTick
        {
            get { return this.Position; }
        }
        public int MaxTick
        {
            get { return this.Length; }
        }

        public string CurrentTime
        {
            get
            {
                float ms = GetTimeFromTick(CurrentTick);
                TimeSpan t = TimeSpan.FromMilliseconds(ms);
                return string.Format("{0:D2}:{1:D2}", (int)t.TotalMinutes, t.Seconds);
                //return string.Format("{0}", CurrentTick);
            }
        }

        public TimeSpan CurrentTimeAsTimeSpan
        {
            get
            {
                float ms = GetTimeFromTick(CurrentTick);
                TimeSpan t = TimeSpan.FromMilliseconds(ms);
                return t;
            }
        }

        public string MaxTime
        {
            get
            {
                float ms = GetTimeFromTick(MaxTick - 1);
                TimeSpan t = TimeSpan.FromMilliseconds(ms);
                return string.Format("{0:D2}:{1:D2}", (int)t.TotalMinutes, t.Seconds);
                //return string.Format("{0}", MaxTick);
            }
        }

        public TimeSpan MaxTimeAsTimeSpan
        {
            get
            {
                float ms = GetTimeFromTick(MaxTick - 1);
                TimeSpan t = TimeSpan.FromMilliseconds(ms);
                return t;
            }
        }

        int loadedTrack = 0;
        private int intendedTrack = 0;
        public int CurrentTrack
        {
            get
            {
                return loadedTrack;
            }
        }
        public int MaxTrack
        {
            get
            {
                if (Sequence.Count <= 0)
                {
                    return 0;
                }
                return this.Sequence.Count - 1;
            }
        }

        public Track LoadedTrack
        {
            get
            {
                if (loadedTrack >= Sequence.Count || loadedTrack < 0)
                {
                    return null;
                }
                if (BmpPigeonhole.Instance.PlayAllTracks) return Sequence[0];
                else return Sequence[loadedTrack];
            }
        }

        int lyricCount = 0;
        public int LyricNum
        {
            get
            {
                return lyricCount;
            }
        }

        public Sequencer() : base()
        {
            Sequence = new Sequence();

            this.ChannelMessagePlayed += OnChannelMessagePlayed;
            this.MetaMessagePlayed += OnMetaMessagePlayed;

            secondTimer.Elapsed += OnSecondTimer;
        }

        public int GetTrackNum(Track track)
        {
            for (int i = 0; i < Sequence.Count; i++)
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
            OnTick?.Invoke(this, this.Position);
        }

        public void Seek(double ms)
        {
            int ticks = (int)(Sequence.Division * ((midiTempo / 60000f) * ms));
            if ((this.Position + ticks) < this.MaxTick && (this.Position + ticks) >= 0)
            {
                this.Position = ticks;
            }
        }

        public void Seek(int ticks)
        {
            this.Position = ticks;
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

        public float GetTimeFromTick(int tick)
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
                Console.WriteLine("[Sequencer] No Midi input");
                return;
            }
            MidiInCaps cap = InputDevice.GetDeviceCapabilities(device);
            try
            {
                midiInput = new InputDevice(device);
                midiInput.StartRecording();
                midiInput.ChannelMessageReceived += OnSimpleChannelMessagePlayed;

                Console.WriteLine(string.Format("{0} opened.", cap.name));
            }
            catch (InputDeviceException)
            {
                Console.WriteLine(string.Format("Couldn't open input {0}.", device));
            }
        }

        public void OpenInputDevice(string device)
        {
            for (int i = 0; i < InputDevice.DeviceCount; i++)
            {
                MidiInCaps cap = InputDevice.GetDeviceCapabilities(i);
                if (cap.name == device)
                {
                    try
                    {
                        midiInput = new InputDevice(i);
                        midiInput.StartRecording();
                        midiInput.ChannelMessageReceived += OnSimpleChannelMessagePlayed;

                        Console.WriteLine(string.Format("{0} opened.", cap.name));
                    }
                    catch (InputDeviceException)
                    {
                        Console.WriteLine(string.Format("Couldn't open input {0}.", device));
                    }
                }
            }
        }

        public void CloseInputDevice()
        {
            if (midiInput != null)
            {
                if (!midiInput.IsDisposed)
                {
                    midiInput.StopRecording();
                    midiInput.Close();
                }
            }
        }

        private void OnSimpleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder(e.Message);
            int note = builder.Data1;
            int vel = builder.Data2;
            ChannelCommand cmd = e.Message.Command;
            if ((cmd == ChannelCommand.NoteOff) || (cmd == ChannelCommand.NoteOn && vel == 0))
            {
                OffNote?.Invoke(this, e);
            }
            if ((cmd == ChannelCommand.NoteOn) && vel > 0)
            {
                OnNote?.Invoke(this, e);
            }
            if (cmd == ChannelCommand.ProgramChange)
            {
                string instName = Instrument.ParseByProgramChange(e.Message.Data1);
                if (!string.IsNullOrEmpty(instName))
                    ProgChange?.Invoke(this, e);
            }
            if (cmd == ChannelCommand.ChannelPressure)
            {
                ChannelAfterTouch?.Invoke(this, e);
            }
        }

        private void OnChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            OnSimpleChannelMessagePlayed(sender, e);
        }
        private void OnMetaMessagePlayed(object sender, MetaMessageEventArgs e)
        {
            if (e.Message.MetaType == MetaType.Tempo)
            {
                TempoChangeBuilder builder = new TempoChangeBuilder(e.Message);
                midiTempo = (60000000 / builder.Tempo);
                OnTempoChange?.Invoke(this, midiTempo);
            }
            if (e.Message.MetaType == MetaType.Lyric)
            {
                OnLyric?.Invoke(this, e);
            }
            if (e.Message.MetaType == MetaType.TrackName)
            {
                MetaTextBuilder builder = new MetaTextBuilder(e.Message);
                ParseTrackName(e.MidiTrack, builder.Text);
                if (e.MidiTrack == LoadedTrack)
                    OnTrackNameChange?.Invoke(this, builder.Text);
            }
            if (e.Message.MetaType == MetaType.InstrumentName)
            {
                MetaTextBuilder builder = new MetaTextBuilder(e.Message);
                OnTrackNameChange?.Invoke(this, builder.Text);
                Console.WriteLine("Instrument name: " + builder.Text);
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
                Regex rex = new Regex(@"^([A-Za-z]+)([-+]\d)?");
                if (rex.Match(trackName) is Match match)
                {
                    string instrument = match.Groups[1].Value;
                    string octaveshift = match.Groups[2].Value;

                    bool foundInstrument = false;

                    if (!string.IsNullOrEmpty(instrument))
                    {
                        if (Instrument.TryParse(instrument, out Instrument tempInst))
                        {
                            preferredInstruments[track] = tempInst;
                            foundInstrument = true;
                        }
                    }
                    if (foundInstrument)
                    {
                        if (!string.IsNullOrEmpty(octaveshift))
                        {
                            if (int.TryParse(octaveshift, out int os))
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
            if (tracknumber >= preferredInstruments.Count)
                return Instrument.None;

            return preferredInstruments.ElementAt(tracknumber).Value;
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

            loadedFileType = FILETYPES.BmpSong;
            loadedBmpSong = bmpSong;
            Sequence = new Sequence(bmpSong.GetSequencerMidi());
            load(Sequence, trackNum);
        }

        public void load(Sequence sequence, int trackNum = 1)
        {
            OnTrackNameChange?.Invoke(this, string.Empty);
            OnTempoChange?.Invoke(this, 0);

            loadedError = string.Empty;
            if (trackNum >= Sequence.Count)
            {
                trackNum = Sequence.Count - 1;
            }
            intendedTrack = trackNum;

            preferredInstruments.Clear();
            preferredOctaveShift.Clear();

            // Collect statistics
            notesPlayedCount.Clear();
            foreach (Track track in Sequence)
            {
                notesPlayedCount[track] = 0;
                foreach (MidiEvent ev in track.Iterator())
                {
                    if (ev.MidiMessage is ChannelMessage chanMsg)
                    {
                        if (chanMsg.Command == ChannelCommand.NoteOn)
                        {
                            if (chanMsg.Data2 > 0)
                            {
                                notesPlayedCount[track]++;
                            }
                        }
                    }
                }
            }

            // Count notes and select fìrst that actually has stuff
            if (trackNum == 1)
            {
                while (trackNum < Sequence.Count)
                {
                    int tnotes = 0;

                    foreach (MidiEvent ev in Sequence[trackNum].Iterator())
                    {
                        if (intendedTrack == 1)
                        {
                            if (ev.MidiMessage is ChannelMessage chanMsg)
                            {
                                if (chanMsg.Command == ChannelCommand.NoteOn)
                                {
                                    tnotes++;
                                }
                            }
                            if (ev.MidiMessage is MetaMessage metaMsg)
                            {
                                if (metaMsg.MetaType == MetaType.Lyric)
                                {
                                    tnotes++;
                                }
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
                    Console.WriteLine("No playable track...");
                    trackNum = intendedTrack;
                }
            }

            // Show initial tempo
            foreach (MidiEvent ev in Sequence[0].Iterator())
            {
                if (ev.AbsoluteTicks == 0)
                {
                    if (ev.MidiMessage is MetaMessage metaMsg)
                    {
                        if (metaMsg.MetaType == MetaType.Tempo)
                        {
                            OnMetaMessagePlayed(this, new MetaMessageEventArgs(Sequence[0], metaMsg));
                        }
                    }
                }
            }

            // Parse track names and octave shifts
            foreach (Track track in Sequence)
            {
                foreach (MidiEvent ev in track.Iterator())
                {
                    if (ev.MidiMessage is MetaMessage metaMsg)
                    {
                        if (metaMsg.MetaType == MetaType.TrackName)
                        {
                            MetaTextBuilder builder = new MetaTextBuilder(metaMsg);
                            this.ParseTrackName(track, builder.Text);
                        }
                    }
                }
            }

            loadedTrack = trackNum;
            lyricCount = 0;
            // Search beginning for text stuff
            foreach (MidiEvent ev in LoadedTrack.Iterator())
            {
                if (ev.MidiMessage is MetaMessage msg)
                {
                    if (msg.MetaType == MetaType.TrackName)
                    {
                        OnMetaMessagePlayed(this, new MetaMessageEventArgs(LoadedTrack, msg));
                    }
                    if (msg.MetaType == MetaType.Lyric)
                    {
                        lyricCount++;
                    }
                }
                if (ev.MidiMessage is ChannelMessage chanMsg)
                {
                    if (chanMsg.Command == ChannelCommand.ProgramChange)
                    {
                        OnSimpleChannelMessagePlayed(this, new ChannelMessageEventArgs(Sequence[0], chanMsg));
                    }
                }
            }

            OnLoad?.Invoke(this, EventArgs.Empty);
            Console.WriteLine("Loaded Midi [" + loadedFilename + "] t" + trackNum);
        }
    }
}
