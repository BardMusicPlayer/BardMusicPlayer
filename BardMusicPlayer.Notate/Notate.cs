/*
 * MogLib/Notate/Notate.cs
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Common;
using BardMusicPlayer.Common.Structs;
using BardMusicPlayer.Notate.Objects;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Notate
{
    internal sealed class Notate
    {
        private static readonly Lazy<Notate> LazyInstance = new(() => new Notate());
        public static Notate Instance => LazyInstance.Value;
        
        internal MMSong Transmogrify(NotateConfig notateConfig, Stream stream)
        {
            using var midiStream = new MemoryStream();

            stream.CopyTo(midiStream);
            stream.Dispose();

            var midiFile = midiStream.Rewind().ReadAsMidiFile();

            var mmSong = new MMSong
            {
                title = notateConfig.title,
                description = notateConfig.description,
                notateConfig = notateConfig,
                sourceMidiFile = midiStream.Rewind().ToArray()
            };

            ConcurrentDictionary<int, TrackChunk> trackChunks = new();

            for (var index = 0; index < midiFile.GetTrackChunks().Count(); index++)
                trackChunks[index] = midiFile.GetTrackChunks().ElementAt(index);

            if (trackChunks.Count < notateConfig.bardGroups.SelectMany(bardGroup => bardGroup.tracks.Keys)
                .Concat(notateConfig.singerGroups.SelectMany(singerGroup => singerGroup.tracks.Keys)).Distinct()
                .OrderBy(track => track)
                .Last())
                throw new BmpFileException("NotateConfig has a higher track number configured then what exists in this MidiFile.");

            var tempoMap = midiFile.GetTempoMap();

            ConcurrentDictionary<long, (NotateConfig.BardGroup, ConcurrentDictionary<int, TrackChunk>)> bards = new();
            ConcurrentDictionary<long, (NotateConfig.SingerGroup, TrackChunk)> singers = new();

            Parallel.Invoke(new ParallelOptions(),
                () =>
                {
                    Parallel.ForEach(notateConfig.bardGroups,
                        (bardGroup, _, iteration) =>
                        {
                            bards[iteration] = (bardGroup,
                                new ProcessBard(bardGroup, tempoMap, trackChunks)
                                    .GetGroups(out var discardedNotes));
                            // TODO: log discarded notes
                        });
                },
                () =>
                {
                    Parallel.ForEach(notateConfig.singerGroups,
                        (singerGroup, _, iteration) =>
                        {
                            singers[iteration] = (singerGroup,
                                new ProcessSinger(singerGroup, tempoMap, trackChunks)
                                    .GetGroup(out var discardedNotes));
                            // TODO: log discarded notes
                        });
                }
            );

            var midiDelta = new MidiFile();
            midiDelta.Chunks.Add(new TrackChunk());
            midiDelta.ReplaceTempoMap(TempoMap.Create(Tempo.FromBeatsPerMinute(100)));
            midiDelta.TimeDivision = new TicksPerQuarterNoteTimeDivision(600);
            midiDelta.Chunks.AddRange(bards.SelectMany(chunk => chunk.Value.Item2.Values));
            if (midiDelta.GetTrackChunks().Count() < 2)
                midiDelta.Chunks.AddRange(singers.Select(chunk => chunk.Value.Item2));
            var delta = midiDelta.GetNotes().Count > 0
                ? midiDelta.GetNotes().First().GetTimedNoteOnEvent().GetNoteMs(midiDelta.GetTempoMap())
                : 0;

            foreach (var (bardGroup, bardGroupTracks) in bards.Select(chunk => chunk.Value))
            {
                foreach (var bardTrack in bardGroupTracks.Values)
                {
                    var bard = new MMSong.Bard
                    {
                        description = bardGroup.description
                    };
                    foreach (var vst in bardGroup.instruments.Keys) bard.instruments[vst] = bardGroup.instruments[vst].instrument;
                    foreach (var noteEvent in bardTrack.GetNotes())
                    {
                        bard.sequence.Add((noteEvent.GetTimedNoteOnEvent().Time - delta) / 25, noteEvent.NoteNumber);
                        if (noteEvent.NoteNumber < OctaveRange.VSTRange.LowerNote) bard.sequence.Add((noteEvent.GetTimedNoteOffEvent().Time - delta) / 25, 254);
                    }
                    mmSong.bards.Add(bard);
                }
            }

            foreach (var (singerGroup, singerTrack) in singers.Select(chunk => chunk.Value))
            {
                var singer = new MMSong.Singer
                {
                    description = singerGroup.description,
                    lines = singerGroup.lyricMapper
                };
                foreach (var noteEvent in singerTrack.GetNotes()) singer.sequence.Add((noteEvent.GetTimedNoteOnEvent().Time - delta) / 25, noteEvent.NoteNumber);
                mmSong.singers.Add(singer);
            }

            return mmSong;
        }
    }
}