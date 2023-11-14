/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian.Structs;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Transmogrify.Processor.Utilities;

internal static partial class Extensions
{
    /// <summary>
    /// Creates a NoteDictionary from the <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="originalChunk"></param>
    /// <param name="tempoMap"></param>
    /// <param name="firstNoteus"></param>
    /// <param name="noteVelocity"></param>
    /// <returns></returns>
    internal static Task<Dictionary<int, Dictionary<long, Note>>> GetNoteDictionary(TrackChunk originalChunk, TempoMap tempoMap, long firstNoteus, int noteVelocity)
    {
        tempoMap = tempoMap.Clone();
        var notesDictionary = new Dictionary<int, Dictionary<long, Note>>();

        for (var i = 0; i < 128; i++)
            notesDictionary.Add(i, new Dictionary<long, Note>());

        foreach (var note in originalChunk.GetNotes())
        {
            long noteOnMS;
            long noteOffMS;

            try
            {
                noteOnMS = 5000000 + (note.GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds) - firstNoteus;
                noteOffMS = 5000000 + (note.GetTimedNoteOffEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds) - firstNoteus;
            }
            catch (Exception)
            { continue; }

            int noteNumber = note.NoteNumber;

            var newNote = new Note((SevenBitNumber)noteNumber,
                time: noteOnMS / 1000,
                length: (noteOffMS / 1000) - (noteOnMS / 1000)
            )
            {
                Channel     = (FourBitNumber)0,
                Velocity    = (SevenBitNumber)noteVelocity,
                OffVelocity = (SevenBitNumber)noteVelocity
            };

            if (notesDictionary[noteNumber].TryGetValue(noteOnMS, out var value))
            {
                if (value.Length < note.Length)
                    notesDictionary[noteNumber][noteOnMS] = newNote;
            }
            else
                notesDictionary[noteNumber].Add(noteOnMS, newNote);
        }

        return Task.FromResult(notesDictionary);
    }

    /// <summary>
    /// Add the program change events to <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="originalChunk"></param>
    /// <param name="tempoMap"></param>
    /// <param name="firstNote"></param>
    /// <returns><see cref="Task{TResult}"/> is <see cref="TrackChunk"/></returns>
    internal static Task<TrackChunk> AddProgramChangeEvents(TrackChunk originalChunk, TempoMap tempoMap, long firstNote)
    {
        var newChunk = new TrackChunk();
        var programChangeEvents = originalChunk.ManageTimedEvents().Objects
            .Where(e => e.Event.EventType == MidiEventType.ProgramChange)
            .OrderBy(e => e.Time);

        foreach (var timedEvent in programChangeEvents)
        {
            if (timedEvent.Event is not ProgramChangeEvent programChangeEvent)
                continue;

            // Skip all except guitar | implement if we need this again
            if (programChangeEvent.ProgramNumber < 27 || programChangeEvent.ProgramNumber > 31)
                continue;

            var channel = programChangeEvent.Channel;
            using var manager = new TimedObjectsManager(newChunk.Events, ObjectType.TimedEvent | ObjectType.Note);

            // Calculate the new time for the program change event
            var newTime = Math.Max(5000, 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote);

            // Add the program change event to the new chunk
            manager.Objects.Add(new TimedEvent(new ProgramChangeEvent(programChangeEvent.ProgramNumber), newTime));
        }

        return Task.FromResult(newChunk);
    }

    /// <summary>
    /// Add the lyric events to <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="originalChunk"></param>
    /// <param name="tempoMap"></param>
    /// <param name="firstNote"></param>
    /// <returns><see cref="Task{TResult}"/> is <see cref="TrackChunk"/></returns>
    internal static Task<TrackChunk> AddLyricsEvents(TrackChunk originalChunk, TempoMap tempoMap, long firstNote)
    {
        var newChunk = new TrackChunk();
        var events = originalChunk.ManageTimedEvents().Objects.Where(e => e.Event.EventType == MidiEventType.Text);
        foreach (var timedEvent in events)
        {
            if (timedEvent.Event is not LyricEvent lyricsEvent)
                continue;

            using var manager = new TimedObjectsManager(newChunk.Events, ObjectType.TimedEvent | ObjectType.Note);
            var timedEvents = manager.Objects;
            if ((5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote) < 5000)
                timedEvents.Add(new TimedEvent(new LyricEvent(lyricsEvent.Text), 5000));
            else
                timedEvents.Add(new TimedEvent(new LyricEvent(lyricsEvent.Text), 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote));
        }
        return Task.FromResult(newChunk);
    }

    /// <summary>
    /// Realigns the track events in <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="originalChunk"></param>
    /// <param name="delta"></param>
    /// <returns><see cref="Task{TResult}"/> is <see cref="TrackChunk"/></returns>
    internal static Task<TrackChunk> RealignTrackEvents(TrackChunk originalChunk, long delta)
    {
        var offset = Instrument.Parse(originalChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text).SampleOffset; //get the offset
        using (var manager = originalChunk.ManageTimedEvents())
        {
            foreach (var _event in manager.Objects)
            {
                var noteEvent = _event.Event as NoteEvent;
                var programChangeEvent = _event.Event as ProgramChangeEvent;
                var lyricsEvent = _event.Event as LyricEvent;
                var newStart = _event.Time + offset - delta;

                //Note alignment
                if (noteEvent != null)
                    _event.Time = newStart;

                //lyrics
                if (lyricsEvent != null)
                {
                    if (newStart <= -1)
                        manager.Objects.Remove(_event);
                    else
                        _event.Time = newStart;
                }

                //Prog alignment
                if (programChangeEvent != null)
                {
                    if (newStart <= -1)
                        manager.Objects.Remove(_event);
                    else
                        _event.Time = newStart;

                    //if theres a new offset, use this one
                    if ((programChangeEvent.ProgramNumber >= 27) && (programChangeEvent.ProgramNumber <= 31))
                        offset = Instrument.ParseByProgramChange(programChangeEvent.ProgramNumber).SampleOffset;
                }
            }


            /*foreach (TimedEvent _event in manager.Events)
            {
                var programChangeEvent = _event.Event as ChannelAftertouchEvent;
                if (programChangeEvent == null)
                    continue;

                long newStart = _event.Time - delta;
                if (newStart <= -1)
                    manager.Events.Remove(_event);
                else
                    _event.Time = newStart;
            }*/

        }
        return Task.FromResult(originalChunk);
    }
}