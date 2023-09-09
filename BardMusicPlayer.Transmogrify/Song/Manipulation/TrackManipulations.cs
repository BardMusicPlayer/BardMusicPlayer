/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Text;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;

namespace BardMusicPlayer.Transmogrify.Song.Manipulation;

public static class TrackManipulations
{
    #region Get/Set Channel

    /// <summary>
    /// Get channel number by first note on from a <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="track"></param>
    /// <returns>Channelnumber as <see cref="int"/></returns>
    public static int GetChannelNumber(TrackChunk track)
    {
        var ev = track.Events.OfType<NoteOnEvent>().FirstOrDefault();
        if (ev != null)
            return ev.Channel;
        return -1;
    }

    /// <summary>
    /// Sets the channel number for a <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="track"></param>
    /// <param name="channelNumber"></param>
    /// <returns></returns>
    public static void SetChanNumber(TrackChunk track, int channelNumber)
    {
        if (channelNumber < 0)
            return;
        channelNumber = (channelNumber & 0x0F);

        using (var notesManager = track.ManageNotes())
        {
            Parallel.ForEach(notesManager.Objects, note =>
            {
                note.Channel = (FourBitNumber)channelNumber;
            });
            notesManager.SaveChanges();
        }

        using (var manager = track.ManageTimedEvents())
        {
            Parallel.ForEach(manager.Objects, midiEvent =>
            {
                if (midiEvent.Event is ProgramChangeEvent pe)
                    pe.Channel = (FourBitNumber)channelNumber;
                if (midiEvent.Event is ControlChangeEvent ce)
                    ce.Channel = (FourBitNumber)channelNumber;
                if (midiEvent.Event is PitchBendEvent pbe)
                    pbe.Channel = (FourBitNumber)channelNumber;
            });
            manager.SaveChanges();
        }
    }

    #endregion

    #region Get/Set Instrument-ProgramChangeEvent
    /// <summary>
    /// Get the program number of the <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="track"></param>
    /// <returns>The <see cref="int"/> representation of the instrument</returns>
    public static int GetInstrument(TrackChunk track)
    {
        var ev = track.Events.Where(e => e.EventType == MidiEventType.ProgramChange).FirstOrDefault();
        if (ev != null)
            return (ev as ProgramChangeEvent).ProgramNumber;
        return 1; //return a "None" instrument cuz we don't have all midi instrument in XIV
    }

    /// <summary>
    /// Create or overwrite the first progchange in <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="track"></param>
    /// <param name="instrument"></param>
    public static void SetInstrument(TrackChunk track, int instrument)
    {
        int channel = GetChannelNumber(track);
        if (channel == -1)
            return;

        using (var events = track.ManageTimedEvents())
        {
            var ev = events.Objects.Where(e => e.Event.EventType == MidiEventType.ProgramChange).FirstOrDefault();
            if (ev != null)
            {
                var prog = ev.Event as ProgramChangeEvent;
                prog.ProgramNumber = (SevenBitNumber)instrument;
            }
            else
            {
                var pe = new ProgramChangeEvent((SevenBitNumber)instrument);
                pe.Channel = (FourBitNumber)channel;
                events.Objects.Add(new TimedEvent(pe, 0));
            }
            events.SaveChanges();
        }
    }
    #endregion

    #region Get/Set TrackName
    /// <summary>
    /// Get the name of the <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="track">TrackChunk</param>
    /// <returns>The track-name as <see cref="string"/></returns>
    public static string GetTrackName(TrackChunk track)
    {
        var trackName = track.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;
        if (trackName != null)
            return trackName;
        return "No Name";
    }

    /// <summary>
    /// Sets the <see cref="TrackChunk"/> name
    /// </summary>
    /// <param name="track"></param>
    /// <param name="TrackName"></param>
    public static void SetTrackName(TrackChunk track, string TrackName)
    {
        using (var events = track.ManageTimedEvents())
        {
            var fev = events.Objects.Where(e => e.Event.EventType == MidiEventType.SequenceTrackName).FirstOrDefault();
            if (fev != null)
            {
                (fev.Event as SequenceTrackNameEvent).Text = TrackName;
                events.SaveChanges();
            }
            else
            {
                SequenceTrackNameEvent name = new SequenceTrackNameEvent(TrackName);
                track.Events.Insert(0, name);
            }

        }
    }
    #endregion

    #region Misc
    /// <summary>
    /// Remove all prog changes from <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="track"></param>
    /// <returns></returns>
    public static void ClearProgChanges(TrackChunk track)
    {
        using (var manager = track.ManageTimedEvents())
        {
            manager.Objects.RemoveAll(e => e.Event.EventType == MidiEventType.ProgramChange);
            manager.Objects.RemoveAll(e => e.Event.EventType == MidiEventType.ProgramName);
            manager.SaveChanges();
        }
    }

    /// <summary>
    /// DrumMapper Helper
    /// </summary>
    public class DrumMaps
    {
        public int MidiNote { get; set; } = 0;
        public string Instrument { get; set; } = "None";
        public int GameNote { get; set; } = 0;
    }

    /// <summary>
    /// Maps a midi drum track to specific notes and corrosponding separate <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="track"></param>
    /// <param name="fileName"></param>
    /// <returns>The Dictionary(InstrumentName, TrackChunk) or in case of an error: (ErrorMsg, Null)</returns>
    public static Dictionary<string, TrackChunk> DrumMapping(TrackChunk track, string fileName)
    {
        MemoryStream memoryStream = new MemoryStream();
        FileStream fileStream = File.Open(fileName, FileMode.Open);
        fileStream.CopyTo(memoryStream);
        fileStream.Close();

        Dictionary<string, TrackChunk> drumTracks = new Dictionary<string, TrackChunk>();
        List<DrumMaps> drumlist = null;
        var data = memoryStream.ToArray();
        try
        {
            drumlist = JsonConvert.DeserializeObject<List<DrumMaps>>(new UTF8Encoding(true).GetString(data));
        }
        catch
        {
            drumTracks.Add("Malformed drum map!", null);
            return drumTracks;
        }
        memoryStream.Close();
        memoryStream.Dispose();

        if (drumlist == null)
        {
            drumTracks.Add("Drum map is empty!", null);
            return drumTracks;
        }

        //And do it
        foreach (Note note in track.GetNotes())
        {
            var drum = drumlist.Where(dm => dm.MidiNote == note.NoteNumber).FirstOrDefault();
            if (drum == null)
                continue;

            var ret = drumTracks.Where(item => item.Key == drum.Instrument).FirstOrDefault();
            if (ret.Key == null)
            {
                drumTracks[drum.Instrument] = new TrackChunk(new SequenceTrackNameEvent(drum.Instrument));
                using (var notesManager = drumTracks[drum.Instrument].ManageNotes())
                {
                    TimedObjectsCollection<Note> notes = notesManager.Objects;
                    note.NoteNumber = (SevenBitNumber)drum.GameNote;
                    notes.Add(note);
                }
            }
            else
            {
                using (var notesManager = drumTracks[drum.Instrument].ManageNotes())
                {
                    TimedObjectsCollection<Note> notes = notesManager.Objects;
                    note.NoteNumber = (SevenBitNumber)drum.GameNote;
                    notes.Add(note);
                }
            }
        }
        drumlist.Clear();
        return drumTracks;
    }
    #endregion

}