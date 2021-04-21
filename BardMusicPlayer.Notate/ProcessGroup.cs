/*
 * MogLib/Notate/ProcessGroup.cs
 *
 * Copyright (C) 2021  MoogleTroupe
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Common;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using static BardMusicPlayer.Notate.Objects.NotateConfig;
using static BardMusicPlayer.Notate.Objects.NotateConfig.NotateGroup;

namespace BardMusicPlayer.Notate
{
    internal abstract class ProcessGroup<TC, T> where TC: ProcessGroup<TC, T> where T : NotateGroup
    {
        private protected readonly T notateGroup;
        private protected readonly TempoMap tempoMap;
        private protected ConcurrentDictionary<int, TrackChunk> trackChunks;

        private protected ProcessGroup(T notateGroup, TempoMap tempoMap, ConcurrentDictionary<int, TrackChunk> trackChunks)
        {
            this.notateGroup = notateGroup; 
            this.tempoMap = tempoMap.Clone();
            this.trackChunks = trackChunks.Where(track => notateGroup.tracks.ContainsKey(track.Key))
                .ToConcurrentDictionary(track => track.Key, track => (TrackChunk) track.Value.Clone());
        }

        private protected TrackChunk ProcessModifiers(ref long discardedNotes)
        {
            ConcurrentDictionary<long, long> discardedNotesFromForEach = new();
            Parallel.ForEach(notateGroup.tracks, (track , _, iteration)=>
            {
                long thisDiscardedNotes = 0;
                trackChunks[track.Key] = ProcessModifierList(track.Value.preMergeModifiers, trackChunks[track.Key], tempoMap.Clone(), ref thisDiscardedNotes);
                discardedNotesFromForEach.TryAdd(iteration, thisDiscardedNotes);
            });
            discardedNotes += discardedNotesFromForEach.Sum(x => x.Value);
            return ProcessModifierList(notateGroup.postMergeModifiers, trackChunks.Values.Merge(), tempoMap.Clone(), ref discardedNotes);
        }

        private static TrackChunk ProcessModifierList(IEnumerable<Modifier> modifiers, TrackChunk track, TempoMap tempoMap, ref long discardedNotes)
        {
            foreach (var modifier in modifiers)
            {
                switch (modifier.configuration)
                {
                    // Todo

                    default:
                        continue;
                }
            }
            return track;
        }

        internal static TrackChunk OffSet50Ms(TrackChunk track)
        {
            var processedChunk = new TrackChunk();

            var events = track.GetNotes().ToArray();
            
            long lastStartTime = 0;
            long lastStopTime = 0;
            long lastDur = 0;
            int lastNoteNum = 0;
            long last50MsTimeStamp = 0;
            bool hasNotes = false;
            int lastChannel = 0;
            int lastVelocity = 0;

            List<Note> thisNotes = new();
            for (var j = 0; j < events.Count(); j++)
            {
                long startTime = events[j].GetTimedNoteOnEvent().Time;
                long stopTime;
                long dur = events[j].Length;
                int noteNum = events[j].NoteNumber;
                int channel = events[j].Channel;
                int velocity = events[j].Velocity;
                hasNotes = true;
                if (j == 0)
                {
                    if (startTime % 100 != 0 && startTime % 100 != 50)
                    {
                        if (startTime % 100 < 25 || (startTime % 100 < 75 && startTime % 100 > 50)) startTime = 50 * ((startTime + 49) / 50);
                        else startTime = 50 * (startTime / 50);
                    }

                    if (dur % 100 != 0 && dur % 100 != 25 && dur % 100 != 50 && dur % 100 != 75)
                    {
                        dur = 25 * ((dur + 24) / 25);
                        if (dur < 25) dur = 25;
                    }

                    stopTime = startTime + dur;
                    last50MsTimeStamp = startTime;
                    lastStartTime = startTime;
                    lastStopTime = stopTime;
                    lastDur = dur;
                    lastNoteNum = noteNum;
                    lastChannel = channel;
                    lastVelocity = velocity;
                }
                else
                {
                    while (last50MsTimeStamp < startTime) last50MsTimeStamp += 50;

                    startTime = last50MsTimeStamp;
                    stopTime = startTime + dur;

                    if (startTime - lastStopTime < 25)
                        lastDur = startTime - lastStartTime - 25;

                    if (dur % 100 != 0 && dur % 100 != 25 && dur % 100 != 50 && dur % 100 != 75)
                    {
                        dur = 25 * ((dur + 24) / 25);
                        if (dur < 25) dur = 25;
                    }

                    if (lastDur > 0)
                        thisNotes.Add(new Note((SevenBitNumber) lastNoteNum, lastDur, lastStartTime)
                        {
                            Channel = (FourBitNumber) lastChannel,
                            Velocity = (SevenBitNumber) lastVelocity,
                            OffVelocity = (SevenBitNumber) lastVelocity
                        });

                    lastStartTime = startTime;
                    lastStopTime = stopTime;
                    lastDur = dur;
                    lastNoteNum = noteNum;
                    lastChannel = channel;
                    lastVelocity = velocity;
                }

            }

            if (hasNotes)
                thisNotes.Add(new Note((SevenBitNumber)lastNoteNum, lastDur, lastStartTime)
                {
                    Channel = (FourBitNumber)lastChannel,
                    Velocity = (SevenBitNumber)lastVelocity,
                    OffVelocity = (SevenBitNumber)lastVelocity
                });


            processedChunk.AddObjects(thisNotes);
            return processedChunk;
        }
        internal static TrackChunk FixEndSpacing(TrackChunk trackChunk)
        {
            var events = trackChunk.GetNotes().ToArray();
            var tempChunk = new TrackChunk();
            var thisNotes = new List<Note>();
            for (var j = 0; j < events.Count(); j++)
            {
                var noteNum = events[j].NoteNumber;
                var time = events[j].Time;
                var dur = events[j].Length;
                var channel = events[j].Channel;
                var velocity = events[j].Velocity;

                if (j + 1 < events.Length)
                {
                    if (events[j + 1].Time <= events[j].Time + events[j].Length + 25)
                    {
                        dur = events[j + 1].Time - events[j].Time - 25;
                        dur = dur < 25 ? 1 : dur;
                    }
                }
                thisNotes.Add(new Note(noteNum, dur, time)
                {
                    Channel = channel,
                    Velocity = velocity,
                    OffVelocity = velocity
                });
            }
            tempChunk.AddObjects(thisNotes);
            return tempChunk;
        }
    }

    internal static class ProcessGroupExtensions
    {
        internal static bool MapNote(this Dictionary<int, int> mapper, ref int noteNumber) => mapper.ContainsKey(noteNumber) && mapper.TryGetValue(noteNumber, out noteNumber);
        internal static long GetNoteMs(this TimedEvent note, TempoMap tempoMap) => note.TimeAs<MetricTimeSpan>(tempoMap.Clone()).TotalMicroseconds / 1000;
    }
    
}
