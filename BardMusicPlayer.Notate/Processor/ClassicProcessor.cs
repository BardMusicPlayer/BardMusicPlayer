/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Notate.Song.Config;
using Melanchall.DryWetMidi.Core;
using BardMusicPlayer.Notate.Processor.Interfaces;
using BardMusicPlayer.Notate.Processor.Utilities;
using BardMusicPlayer.Common.Structs;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Concurrent;
using System.Linq;

namespace BardMusicPlayer.Notate.Processor
{
    internal class ClassicProcessor : BaseProcessor
    {
        public ClassicConfig Config { get; set; }

        internal ClassicProcessor(ClassicConfig config, BmpSong song) : base(song)
        {
            Config = config;
        }

        public override async Task<List<TrackChunk>> Process()
        {
            List<TrackChunk> trackChunks = new();

            trackChunks.Add(Song.TrackContainers[Config.Track].SourceTrackChunk);

            foreach(var track in Config.IncludedTracks) trackChunks.Add(Song.TrackContainers[track].SourceTrackChunk);

            var trackChunk = TimedObjectUtilities.ToTrackChunk(await 
                trackChunks.GetNoteDictionary(Song.SourceTempoMap, Config.OctaveRange.LowerNote, Config.OctaveRange.UpperNote, false)
                .MoveNoteDictionaryToDefaultOctave(Config.OctaveRange)
                .ConcatNoteDictionaryToList());

            var playerNotesDictionary = await trackChunk.GetPlayerNoteDictionary(Config.PlayerCount, OctaveRange.C3toC6.LowerNote, OctaveRange.C3toC6.UpperNote, false);

            var concurrentPlayerTrackDictionary = new ConcurrentDictionary<long, TrackChunk>();

            Parallel.ForEach(playerNotesDictionary.Values, async (notesDictionary, _, iteration) =>
                {
                    concurrentPlayerTrackDictionary[iteration] = await TimedObjectUtilities.ToTrackChunk(await notesDictionary.ConcatNoteDictionaryToList()).FixClassicChords().OffSet50Ms().FixEndSpacing();
                }
            );

            trackChunks = concurrentPlayerTrackDictionary.Values.ToList();

            return trackChunks;
        }
    }
}
