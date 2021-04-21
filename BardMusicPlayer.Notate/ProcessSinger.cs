/*
 * MogLib/Notate/ProcessSinger.cs
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
using BardMusicPlayer.Notate.Objects;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using static BardMusicPlayer.Notate.Objects.NotateConfig;

namespace BardMusicPlayer.Notate
{
    internal sealed class ProcessSinger : ProcessGroup<ProcessSinger, NotateConfig.SingerGroup>
    {
        internal ProcessSinger(SingerGroup notateGroup, TempoMap tempoMap, ConcurrentDictionary<int, TrackChunk> trackChunks) : base(notateGroup, tempoMap, trackChunks)
        {
        }

        internal TrackChunk GetGroup(out long discardedNotes)
        {
            discardedNotes = 0; 

            var trackChunk = ProcessModifiers(ref discardedNotes);

            var step1Notes = new Dictionary<int, Dictionary<long, Note>>();
            var step2Notes = new Dictionary<int, Dictionary<long, Note>>();
            for (var i = 0; i < 128; i++)
            {
                step1Notes.Add(i, new Dictionary<long, Note>());
                step2Notes.Add(i, new Dictionary<long, Note>());
            }
            foreach (var note in trackChunk.GetNotes())
            {
                int noteNumber = note.NoteNumber;
                if (!notateGroup.lyricMapper.ContainsKey(noteNumber))
                {
                    discardedNotes++;
                    continue;
                }

                var timeStamp = note.GetTimedNoteOnEvent().Time;
                if (step1Notes[noteNumber].ContainsKey(timeStamp))
                {
                    var previousNote = step1Notes[noteNumber][timeStamp];
                    if (previousNote.Length < note.Length) step1Notes[noteNumber][timeStamp] = note.Clone();
                }
                else step1Notes[noteNumber].Add(timeStamp, note.Clone());
            }

            for (var i = 0; i < 128; i++)
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
                
                foreach (var noteEvent in noteEvents)
                    step2Notes[i].Add(noteEvent.Value.GetTimedNoteOnEvent().GetNoteMs(tempoMap) + 5200,
                            new Note(noteEvent.Value.NoteNumber, 25, noteEvent.Value.GetTimedNoteOnEvent().GetNoteMs(tempoMap) + 5200));
            }

            trackChunk = new TrackChunk();
            trackChunk.AddObjects(step2Notes.SelectMany(notes => notes.Value.Values));

            return OffSet50Ms(FixEndSpacing(trackChunk));
        }
    }
}
