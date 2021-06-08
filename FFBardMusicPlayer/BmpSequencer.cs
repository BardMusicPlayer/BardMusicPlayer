using System;
using System.Collections.Generic;
using System.IO;
using Timer = System.Timers.Timer;
using Sanford.Multimedia.Midi;
using System.Text.RegularExpressions;
using FFBardMusicCommon;

/*
public class TickList : Dictionary<int, List<MetaMidiEvent>> {

}
*/

namespace FFBardMusicPlayer
{
    public class BmpSequencer : BmpCustomSequencer
    {
        private InputDevice midiInput;
        private readonly Dictionary<Track, Instrument> preferredInstruments = new Dictionary<Track, Instrument>();
        private readonly Dictionary<Track, int> preferredOctaveShift = new Dictionary<Track, int>();
        public EventHandler OnLoad;
        public EventHandler<ChannelMessageEventArgs> OnNote;
        public EventHandler<ChannelMessageEventArgs> OffNote;
        public EventHandler<string> OnLyric;
        public EventHandler<int> OnTempoChange;
        public EventHandler<string> OnTrackNameChange;
        private readonly Timer secondTimer = new Timer(200);
        public EventHandler<int> OnTick;
        public Dictionary<Track, int> NotesPlayedCount = new Dictionary<Track, int>();

        public string LoadedError { get; private set; } = string.Empty;

        public string LoadedFilename { get; private set; } = string.Empty;

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
                return $"{(int) t.TotalMinutes:D2}:{t.Seconds:D2}";
                //return string.Format("{0}", CurrentTick);
            }
        }

        public string MaxTime
        {
            get
            {
                var ms = GetTimeFromTick(MaxTick - 1);
                var t = TimeSpan.FromMilliseconds(ms);
                return $"{(int) t.TotalMinutes:D2}:{t.Seconds:D2}";
                //return string.Format("{0}", MaxTick);
            }
        }

        private int intendedTrack;

        public int CurrentTrack { get; private set; }

        public int MaxTrack => Sequence.Count <= 0 ? 0 : Sequence.Count - 1;

        public Track LoadedTrack
        {
            get
            {
                if (CurrentTrack >= Sequence.Count || CurrentTrack < 0)
                    return null;

                return Properties.Settings.Default.PlayAllTracks ? Sequence[0] : Sequence[CurrentTrack];
            }
        }

        public int LyricNum { get; private set; }

        public BmpSequencer()
        {
            Sequence = new Sequence();

            ChannelMessagePlayed += OnChannelMessagePlayed;
            MetaMessagePlayed    += OnMetaMessagePlayed;

            secondTimer.Elapsed += OnSecondTimer;
        }

        public BmpSequencer(string filename, int trackNum = 0)
        {
            Sequence = new Sequence();

            ChannelMessagePlayed += OnChannelMessagePlayed;
            MetaMessagePlayed    += OnMetaMessagePlayed;

            secondTimer.Elapsed += OnSecondTimer;

            Load(filename);
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

        private void OnSecondTimer(object sender, EventArgs e) { OnTick?.Invoke(this, Position); }

        public void Seek(double ms)
        {
            var ticks = (int) (Sequence.Division * (midiTempo / 60000f * ms));
            if (Position + ticks < MaxTick && Position + ticks >= 0)
            {
                Position += ticks;
            }
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

            /*float ms = 0f;
            int mul = midiTempo;

            List<Track> trackEnumsToRemove = new List<Track>();
            Dictionary<Track, IEnumerator<MidiEvent>> trackEnums = new Dictionary<Track, IEnumerator<MidiEvent>>();
            foreach(Track t in Sequence) {
                var en = t.Iterator().GetEnumerator();
                if(en.MoveNext()) {
                    trackEnums.Add(t, en);
                }
            }

            for(int i = 0; i < tick; i++) {
                foreach(KeyValuePair<Track, IEnumerator<MidiEvent>> tem in trackEnums) {
                    if(tem.Value == null) {
                        continue;
                    }
                    Track tek = tem.Key;
                    IEnumerator<MidiEvent> ten = tem.Value;
                    MidiEvent ev = ten.Current;
                    bool end = false;
                    while(ten.Current.AbsoluteTicks < i) {
                        if(!ten.MoveNext()) {
                            end = true;
                            break;
                        }
                    }
                    if(ten.Current != ev) {
                        // New event, apply
                        if(ev.MidiMessage is MetaMessage) {
                            MetaMessage msg = ev.MidiMessage as MetaMessage;
                            if(msg.MetaType == MetaType.Tempo) {
                                TempoChangeBuilder builder = new TempoChangeBuilder(msg);
                                mul = (int) ((60000000 / builder.Tempo) * Speed);
                            }
                        }
                    }
                    if(end) {
                        trackEnumsToRemove.Add(tek);
                    }
                }
                if(trackEnumsToRemove.Count > 0) {
                    foreach(Track t in trackEnumsToRemove) {
                        trackEnums.Remove(t);
                    }
                    trackEnumsToRemove.Clear();
                }
                ms += 1 * (60000f / (float) (mul * Sequence.Division));
            }
            return ms;*/
        }

        private void Chaser_Chased(object sender, ChasedEventArgs e) { throw new NotImplementedException(); }

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

                        Console.WriteLine($"{cap.name} opened.");
                    }
                    catch (InputDeviceException e)
                    {
                        Console.WriteLine($"Couldn't open input {device}.");
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

        public string ProgramToInstrumentName(int prog)
        {
            switch (prog)
            {
                case 0:
                {
                    return "Piano";
                }
                case 46:
                {
                    return "Harp";
                }
                case 24:
                {
                    return "Lute";
                }
                case 68:
                {
                    return "Oboe";
                }
                case 71:
                {
                    return "Clarinet";
                }
                case 75:
                {
                    return "Panpipes";
                }
                case 72:
                {
                    return "Fife";
                }
                case 47:
                {
                    return "Timpani";
                }
                // Drums
                //case 59: { }
                //case 35: { }
            }

            return string.Empty;
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

            if (cmd == ChannelCommand.NoteOn && vel > 0)
            {
                OnNote?.Invoke(this, e);
            }

            if (cmd == ChannelCommand.ProgramChange)
            {
                var instName = ProgramToInstrumentName(e.Message.Data1);
                if (!string.IsNullOrEmpty(instName))
                {
                    Console.WriteLine($"Program change to voice/instrument: {instName} {e.Message.Data2}");
                }
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
                var builder = new TempoChangeBuilder(e.Message);
                midiTempo = 60000000 / builder.Tempo;
                OnTempoChange?.Invoke(this, midiTempo);
            }

            if (e.Message.MetaType == MetaType.Lyric)
            {
                var builder = new MetaTextBuilder(e.Message);
                if (e.MidiTrack == LoadedTrack)
                {
                    OnLyric?.Invoke(this, builder.Text);
                }
            }

            if (e.Message.MetaType == MetaType.TrackName)
            {
                var builder = new MetaTextBuilder(e.Message);
                ParseTrackName(e.MidiTrack, builder.Text);
                if (e.MidiTrack == LoadedTrack)
                {
                    OnTrackNameChange?.Invoke(this, builder.Text);
                }
            }

            if (e.Message.MetaType == MetaType.InstrumentName)
            {
                var builder = new MetaTextBuilder(e.Message);
                OnTrackNameChange?.Invoke(this, builder.Text);
                Console.WriteLine($"Instrument name: {builder.Text}");
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
                if (rex.Match(trackName) is Match match)
                {
                    var instrument = match.Groups[1].Value;
                    var octaveshift = match.Groups[2].Value;

                    var foundInstrument = false;

                    if (!string.IsNullOrEmpty(instrument))
                    {
                        if (Enum.TryParse(instrument, out Instrument tempInst))
                        {
                            preferredInstruments[track] = tempInst;
                            foundInstrument             = true;
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

        public void Load(string file, int trackNum = 1)
        {
            OnTrackNameChange?.Invoke(this, string.Empty);
            OnTempoChange?.Invoke(this, 0);

            if (!File.Exists(file))
            {
                throw new FileNotFoundException("Midi file does not exist.");
            }

            LoadedError = string.Empty;
            try
            {
                Sequence = new Sequence(DryWetUtil.ScrubFile(file));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw e;
            }

            if (trackNum >= Sequence.Count)
            {
                trackNum = Sequence.Count - 1;
            }

            LoadedFilename = file;
            intendedTrack  = trackNum;

            preferredInstruments.Clear();
            preferredOctaveShift.Clear();

            // Collect statistics
            NotesPlayedCount.Clear();
            foreach (var track in Sequence)
            {
                NotesPlayedCount[track] = 0;
                foreach (var ev in track.Iterator())
                {
                    if (ev.MidiMessage is ChannelMessage chanMsg)
                    {
                        if (chanMsg.Command == ChannelCommand.NoteOn)
                        {
                            if (chanMsg.Data2 > 0)
                            {
                                NotesPlayedCount[track]++;
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
                    var tnotes = 0;
                    if (intendedTrack == 1)
                    {
                        foreach (var ev in Sequence[trackNum].Iterator())
                        {
                            switch (ev.MidiMessage)
                            {
                                case ChannelMessage chanMsg when chanMsg.Command == ChannelCommand.NoteOn:
                                    tnotes++;
                                    break;
                                
                                case MetaMessage metaMsg when metaMsg.MetaType == MetaType.Lyric:
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
                    Console.WriteLine("No playable track...");
                    trackNum = intendedTrack;
                }
            }

            // Show initial tempo
            foreach (var ev in Sequence[0].Iterator())
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
            foreach (var track in Sequence)
            {
                foreach (var ev in track.Iterator())
                {
                    if (ev.MidiMessage is MetaMessage metaMsg)
                    {
                        if (metaMsg.MetaType == MetaType.TrackName)
                        {
                            var builder = new MetaTextBuilder(metaMsg);
                            ParseTrackName(track, builder.Text);
                        }
                    }
                }
            }

            CurrentTrack = trackNum;
            LyricNum  = 0;
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
                                LyricNum++;
                                break;
                        }

                        break;
                    }
                    case ChannelMessage chanMsg:
                    {
                        if (chanMsg.Command == ChannelCommand.ProgramChange)
                        {
                            OnSimpleChannelMessagePlayed(this, new ChannelMessageEventArgs(Sequence[0], chanMsg));
                        }

                        break;
                    }
                }
            }

            OnLoad?.Invoke(this, EventArgs.Empty);
            Console.WriteLine($"Loaded Midi [{file}] t{trackNum}");
        }
    }
}