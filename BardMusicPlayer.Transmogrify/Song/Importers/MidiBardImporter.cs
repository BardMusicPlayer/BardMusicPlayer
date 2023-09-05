/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song.Manipulation;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;
using System.Text;

namespace BardMusicPlayer.Transmogrify.Song.Importers;

public static class MidiBardImporter
{
    public class MidiFileConfig
    {
        public List<TrackConfig> Tracks = new List<TrackConfig>();
        public int ToneMode = 0;
        public bool AdaptNotes = true;
        public float Speed = 1;
    }

    public class TrackConfig
    {
        public int Index = 0;
        public bool Enabled = true;
        public string Name ="";
        public int Transpose = 0;
        public int Instrument = 0;
        public List<long> AssignedCids = new List<long>();
    }

    public class MidiTrack
    {
        public int Index { get; set; }
        public int TrackNumber { get; set; }
        public int trackInstrument { get; set; }
        public int Transpose { get; set; }
        public int ToneMode { get; set; }
        public TrackChunk trackChunk { get; set; }
    }

    /// <summary>
    /// Config lesen
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static MidiFile OpenMidiFile(string filename)
    {
        List<MidiTrack> tracks = new List<MidiTrack>();
        MemoryStream memoryStream = new MemoryStream();
        FileStream fileStream = File.Open(Path.ChangeExtension(filename, "json"), FileMode.Open);
        fileStream.CopyTo(memoryStream);
        fileStream.Close();

        var data = memoryStream.ToArray();
        MidiFileConfig pdatalist = JsonConvert.DeserializeObject<MidiFileConfig>(new UTF8Encoding(true).GetString(data));

        //Read the midi
        MidiFile midifile = MidiFile.Read(filename);

        //create the dict for the cids to tracks
        Dictionary<int, int> cids = new Dictionary<int, int>();
        int idx = 0;
        int cid_count = 1;
        foreach (TrackChunk chunk in midifile.GetTrackChunks())
        {
            if (chunk.GetNotes().Count < 1)
                continue;

            int cid = (int)pdatalist.Tracks[idx].AssignedCids[0];
            if (cids.ContainsKey(cid))
                cid = cids[cid];
            else
            {
                cids.Add(cid, cid_count);
                cid = cid_count;
                cid_count++;
            }

            MidiTrack midiTrack = new MidiTrack();
            midiTrack.Index           = pdatalist.Tracks[idx].Index;
            midiTrack.TrackNumber     = cid;
            midiTrack.trackInstrument = pdatalist.Tracks[idx].Instrument - 1;
            midiTrack.Transpose       = pdatalist.Tracks[idx].Transpose / 12;
            midiTrack.ToneMode        = pdatalist.ToneMode;
            midiTrack.trackChunk      = chunk;

            tracks.Add(midiTrack);
            idx++;
        }
        pdatalist = null;
        return Convert(midifile, tracks);
    }

    public static void PrepareGuitarTrack(TrackChunk tc, int mode, int prognumber)
    {
        if (mode == 3)
        {
            TrackManipulations.ClearProgChanges(tc);
        }
    }

    public static MidiFile Convert(MidiFile midiFile, List<MidiTrack> tracks)
    {
        MidiFile exportMidi = new MidiFile();
        exportMidi.ReplaceTempoMap(midiFile.GetTempoMap());

        List<TrackChunk> chunks = new List<TrackChunk>();
        List<int> trackNums = new List<int>();

        //tracks suchen
        foreach (var d in tracks)
        {
            if (trackNums.Contains(d.TrackNumber))
                continue;

            trackNums.Add(d.TrackNumber);
        }

        //und verarbeiten
        foreach (int TrackNum in trackNums)
        {
            var d = tracks.FindAll(n => n.TrackNumber == TrackNum);
            if (d.Count < 1)
                continue;
            //The fast way
            else if (d.Count == 1)
            {
                int chanNum = d.First().Index; // TrackNum - 1;

                PrepareGuitarTrack(d.First().trackChunk, d.First().ToneMode, Instrument.Parse(d.First().trackInstrument + 1).MidiProgramChangeCode);
                TrackManipulations.SetTrackName(d.First().trackChunk, Instrument.Parse(d.First().trackInstrument + 1).Name);
                TrackManipulations.SetInstrument(d.First().trackChunk, Instrument.Parse(d.First().trackInstrument + 1).MidiProgramChangeCode);
                if (d.First().Transpose > 0)
                    d.First().trackChunk.ProcessNotes(n => n.NoteNumber += (SevenBitNumber)(d.First().Transpose * 12));
                if (d.First().Transpose < 0)
                    d.First().trackChunk.ProcessNotes(n => n.NoteNumber -= (SevenBitNumber)(-(d.First().Transpose * 12)));

                TrackManipulations.SetChanNumber(d.First().trackChunk, chanNum);
                exportMidi.Chunks.Add(d.First().trackChunk);
            }
            else if (d.Count > 1)
            {
                int chanNum = d.First().Index; // TrackNum - 1;

                List<KeyValuePair<long, KeyValuePair<int, TimedEvent>>> tis = new List<KeyValuePair<long, KeyValuePair<int, TimedEvent>>>();
                foreach (var subChunk in d)
                {
                    if (d.First().Transpose > 0)
                        d.First().trackChunk.ProcessNotes(n => n.NoteNumber += (SevenBitNumber)(d.First().Transpose * 12));
                    if (d.First().Transpose < 0)
                        d.First().trackChunk.ProcessNotes(n => n.NoteNumber -= (SevenBitNumber)(-(d.First().Transpose * 12)));

                    foreach (TimedEvent t in subChunk.trackChunk.GetTimedEvents())
                    {
                        if (t.Event.EventType == MidiEventType.NoteOn ||
                            t.Event.EventType == MidiEventType.NoteOff)
                            tis.Add(new KeyValuePair<long, KeyValuePair<int, TimedEvent>>(t.Time, new KeyValuePair<int, TimedEvent>(Instrument.Parse(subChunk.trackInstrument + 1).MidiProgramChangeCode, t)));
                    }
                }

                TrackChunk newTC = new TrackChunk(new SequenceTrackNameEvent("None"));
                int instr = -1;
                using (var events = newTC.ManageTimedEvents())
                {
                    foreach (var t in tis.OrderBy(n => n.Key))
                    {
                        long time = t.Key;
                        TimedEvent ev = t.Value.Value;

                        if (instr != t.Value.Key)
                        {
                            if (instr == -1)
                            {
                                var fev = events.Objects.Where(fe => fe.Event.EventType == MidiEventType.SequenceTrackName).FirstOrDefault();
                                if (fev != null)
                                    (fev.Event as SequenceTrackNameEvent).Text = Instrument.ParseByProgramChange(t.Value.Key).Name;
                                var pe = new ProgramChangeEvent((SevenBitNumber)t.Value.Key);
                                pe.Channel = (FourBitNumber)chanNum;
                                events.Objects.Add(new TimedEvent(pe, 0));
                            }

                            instr = t.Value.Key;
                            var x = ev.Event as NoteOnEvent;
                            if (x != null)
                            {
                                ProgramChangeEvent pc = new ProgramChangeEvent((SevenBitNumber)instr);
                                pc.Channel = x.Channel;
                                if (ev.TimeAs<MetricTimeSpan>(midiFile.GetTempoMap()).TotalMilliseconds > 30)
                                {
                                    var newTime = ev.TimeAs<MetricTimeSpan>(midiFile.GetTempoMap()).Subtract(new MetricTimeSpan(0, 0, 0, 30), TimeSpanMode.TimeTime);
                                    events.Objects.Add(new TimedEvent(pc, TimeConverter.ConvertFrom(newTime, midiFile.GetTempoMap())));
                                }
                                else
                                    events.Objects.Add(new TimedEvent(pc, ev.Time));
                            }
                        }
                        events.Objects.Add(ev);
                    }
                }
                TrackManipulations.SetChanNumber(newTC, chanNum);
                exportMidi.Chunks.Add(newTC);
                tis.Clear();
            }
        }
        return exportMidi;
    }
}