/*
 * MogLib/Notate/ProcessBard.cs
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Common.Structs;
using BardMusicPlayer.Notate.Objects;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using static BardMusicPlayer.Common.Structs.OctaveRange;
using static BardMusicPlayer.Notate.Objects.NotateConfig;
using static BardMusicPlayer.Notate.Objects.NotateConfig.NotateGroup;

namespace BardMusicPlayer.Notate
{
    internal sealed class ProcessBard : ProcessGroup<ProcessBard, NotateConfig.BardGroup>
    {
        internal ProcessBard(BardGroup notateGroup, TempoMap tempoMap,
            ConcurrentDictionary<int, TrackChunk> trackChunks) : base(notateGroup, tempoMap, trackChunks)
        {
        }

        internal ConcurrentDictionary<int, TrackChunk> GetGroups(out long discardedNotes)
        {
            discardedNotes = 0;

            var trackChunk = ProcessModifiers(ref discardedNotes);

            if (!notateGroup.instruments.ContainsKey(VST.VST0) ||
                notateGroup.instruments.Any(vst => vst.Value.instrument.Equals(Instrument.None)))
            {
                discardedNotes += trackChunk.GetNotes()
                    .Count(x => x.NoteNumber < Tone.LowerNote);
                return new ConcurrentDictionary<int, TrackChunk>();
            }
            
            var step1Vst = trackChunk.GetNotes().Where(x => x.NoteNumber >= Tone.LowerNote).Where(note => notateGroup.instruments.ContainsKey((VST) (int) note.NoteNumber)).ToDictionary(note => note.GetTimedNoteOnEvent().GetNoteMs(tempoMap), note => (VST) (int) note.NoteNumber);

            step1Vst.Add(-100, VST.VST0);
            var step2Vst = new Dictionary<int, Dictionary<long, Note>>();
            for (var i = 0; i >= Tone.LowerNote; i++) step2Vst.Add(i, new Dictionary<long, Note>());

            var step1Notes = new Dictionary<int, Dictionary<long, Note>>();
            var step2Notes = new Dictionary<int, Dictionary<long, Note>>();
            for (var i = 0; i < Tone.LowerNote; i++)
            {
                step1Notes.Add(i, new Dictionary<long, Note>());
                step2Notes.Add(i, new Dictionary<long, Note>());
            }
            
            foreach (var note in trackChunk.GetNotes().Where(x => x.NoteNumber < Tone.LowerNote))
            {
                int noteNumber = note.NoteNumber;
                var timeStamp = note.GetTimedNoteOnEvent().Time;
                if (step1Notes[noteNumber].ContainsKey(timeStamp))
                {
                    var previousNote = step1Notes[noteNumber][timeStamp];
                    if (previousNote.Length < note.Length) step1Notes[noteNumber][timeStamp] = note.Clone();
                }
                else step1Notes[noteNumber].Add(timeStamp, note.Clone());
            }
            
            for (var i = 0; i < Tone.LowerNote; i++)
            {
                long lastNoteTimeStamp = -1;
                var noteEvents = step1Notes[i];
                foreach (var noteEvent in noteEvents)
                {
                    if (lastNoteTimeStamp >= 0 &&
                        noteEvents[lastNoteTimeStamp].Length + lastNoteTimeStamp >= noteEvent.Key)
                        noteEvents[lastNoteTimeStamp].Length -= (noteEvents[lastNoteTimeStamp].Length +
                            lastNoteTimeStamp + 1 - noteEvent.Key);

                    lastNoteTimeStamp = noteEvent.Key;
                }

                var voiceIndex1 = 0;
                var voiceIndex2 = -1;
                foreach (var noteEvent in noteEvents)
                {
                    voiceIndex1++;
                    if (voiceIndex1 > 127) voiceIndex1 = 1;
                    voiceIndex2++;
                    if (voiceIndex2 == 9) voiceIndex2 = 10;
                    if (voiceIndex2 == 15) voiceIndex2 = 0;

                    var noteOn = noteEvent.Value.GetTimedNoteOnEvent().GetNoteMs(tempoMap);
                    var noteOff = noteEvent.Value.GetTimedNoteOffEvent().GetNoteMs(tempoMap);
                    var vstProperties = notateGroup.instruments[step1Vst.Where(map => map.Key < noteOn)
                        .OrderByDescending(map => map.Key)
                        .FirstOrDefault().Value];

                    var instrument = vstProperties.instrument;

                    var noteNumber = (int) noteEvent.Value.NoteNumber;

                    if (vstProperties.octaveRange.Equals(Mapper) &&
                        !vstProperties.noteMapper.MapNote(ref noteNumber) ||
                        !instrument.TryShiftNoteToDefaultOctave(vstProperties.octaveRange, ref noteNumber))
                    {
                        discardedNotes++;
                        continue;
                    }

                    noteOn += 5000;
                    noteOff += 5000;

                    noteOn += instrument.NoteSampleOffset(noteNumber);
                    noteOff += instrument.NoteSampleOffset(noteNumber);
                    var dur = noteOff - noteOn;

                    C1toC4.TryShiftNoteToOctave(instrument.DefaultOctaveRange, ref noteNumber);

                    try
                    {
                        step2Notes[i].Add(noteOn,
                            new Note((SevenBitNumber) noteNumber, dur, noteOn)
                            {
                                Velocity = (SevenBitNumber) voiceIndex1, OffVelocity = (SevenBitNumber) voiceIndex1,
                                Channel = (FourBitNumber) voiceIndex2
                            });
                    }
                    catch (Exception)
                    {
                        discardedNotes += 1;
                    }
                }
            }

            var lastVst = VST.VST0;
            foreach (var vstKvp in step1Vst.Where(vstKvp => lastVst != vstKvp.Value))
            {
                lastVst = vstKvp.Value;
                step2Vst[(int) vstKvp.Value].Add(vstKvp.Key + 5000,
                    new Note((SevenBitNumber) (int) vstKvp.Value, 25, vstKvp.Key + 5000));
            }

            trackChunk = TimedObjectUtilities.ToTrackChunk(step2Notes.SelectMany(note => note.Value).Select(note => note.Value));
            
            var step3Notes = new Dictionary<int, Dictionary<int, Dictionary<long, Note>>>();

            for (var i = 0; i < notateGroup.distributionBardCount; i++)
            {
                var notesDictionary = new Dictionary<int, Dictionary<long, Note>>();
                for (var j = 0; j < Tone.LowerNote; j++) notesDictionary[j] = new Dictionary<long, Note>();
                step3Notes.Add(i, notesDictionary);
            }

            var loadBalancer = new LoadBalancer(notateGroup.distributionBardCount);

            using (var timedEventsManager = trackChunk.ManageTimedEvents())
            {
                foreach (var trackEvent in timedEventsManager.Events)
                {
                    switch (trackEvent.Event.EventType)
                    {
                        case MidiEventType.NoteOn:
                        {
                            var note = (NoteOnEvent) trackEvent.Event;

                            var (stoppedBard, stoppedNote) = loadBalancer.NotifyNoteOn(trackEvent.Time, note.Channel, note.NoteNumber, note.Velocity);
                            if (stoppedBard > -1)
                            {
                                step3Notes[stoppedBard][stoppedNote.NoteNumber][stoppedNote.Time] = stoppedNote;
                            }
                            break;
                        }
                        case MidiEventType.NoteOff:
                        {
                            var note = (NoteOffEvent) trackEvent.Event; 
                            var (stoppedBard, stoppedNote) = loadBalancer.NotifyNoteOff(trackEvent.Time, note.Channel, note.NoteNumber, note.Velocity);
                            if (stoppedBard > -1)
                            {
                                step3Notes[stoppedBard][stoppedNote.NoteNumber][stoppedNote.Time] = stoppedNote;
                            }
                            break;
                        }
                    }
                }
            }
            
            trackChunks.Clear();
            for (var i = 0; i < notateGroup.distributionBardCount; i++) trackChunks.TryAdd(i, new TrackChunk());

            var step4Notes = new ConcurrentDictionary<int, Note[]>();

            for (var i = 0; i < step3Notes.Count; i++)
            {
                trackChunks[i].AddObjects(step3Notes[i].SelectMany(x => x.Value.Values));
                step4Notes[i] = trackChunks[i].GetNotes().ToArray();
            }

            var discards = new ConcurrentDictionary<long, long>();
            trackChunks.Clear();
            for (var i = 0; i < notateGroup.distributionBardCount; i++)
            {
                trackChunks.TryAdd(i, new TrackChunk());
                discards.TryAdd(i, 0);
            }

            Parallel.For(0, notateGroup.distributionBardCount, iteration =>
            {
                var notes = step4Notes[iteration].Where(c => c != null).Reverse().ToArray();

                for (var i = 1; i < notes.Length; i++)
                {
                    var time = notes[i].GetTimedNoteOnEvent().Time;
                    var dur = notes[i].Length;
                    var lowestParent = notes[0].GetTimedNoteOnEvent().Time;
                    for (var k = i - 1; k >= 0; k--)
                    {
                        var lastOn = notes[k].GetTimedNoteOnEvent().Time;
                        if (lastOn < lowestParent) lowestParent = lastOn;
                    }

                    if (lowestParent > time + 50) continue;
                    time = lowestParent - 50;
                    if (time < 0)
                    {
                        discards[iteration] += 1;
                        continue;
                    }

                    notes[i].Time = time;
                    notes[i].Length = 25;
                }

                trackChunks[iteration].AddObjects(notes);
                trackChunks[iteration].AddObjects(step2Vst.SelectMany(x => x.Value.Values));
            });

            discardedNotes += discards.Values.Sum();
            
            Parallel.ForEach(trackChunks,
                (thisChunk, _, iteration) =>
                {
                    trackChunks[int.Parse(iteration.ToString())] =
                        OffSet50Ms(FixEndSpacing((TrackChunk) thisChunk.Value.Clone()));
                });
            
            return trackChunks;
        }
    }
}
