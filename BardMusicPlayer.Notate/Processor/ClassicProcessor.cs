/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Notate.Song.Config;
using Melanchall.DryWetMidi.Core;
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
            var trackChunks = new List<TrackChunk> { Song.TrackContainers[Config.Track].SourceTrackChunk }.Concat(Config.IncludedTracks.Select(track => Song.TrackContainers[track].SourceTrackChunk)).ToList();

            var trackChunk = TimedObjectUtilities.ToTrackChunk(await 
                trackChunks.GetNoteDictionary(Song.SourceTempoMap, Config.Instrument.InstrumentTone,
                        Config.OctaveRange.LowerNote, 
                        Config.OctaveRange.UpperNote, 
                        (int) Config.Instrument.InstrumentToneMenuKey, 
                        false,
                        -Config.OctaveRange.LowerNote)
                .MoveNoteDictionaryToDefaultOctave(Config.OctaveRange)
                .ConcatNoteDictionaryToList());

            var playerNotesDictionary = await trackChunk.GetPlayerNoteDictionary(Config.PlayerCount, OctaveRange.C3toC6.LowerNote, OctaveRange.C3toC6.UpperNote);

            var concurrentPlayerTrackDictionary = new ConcurrentDictionary<long, TrackChunk>(Config.PlayerCount, Config.PlayerCount);

            Parallel.ForEach(playerNotesDictionary.Values, async (notesDictionary, _, iteration) =>
                {
                    concurrentPlayerTrackDictionary[iteration] = TimedObjectUtilities.ToTrackChunk(await notesDictionary.ConcatNoteDictionaryToList().FixChords().OffSet50Ms().FixEndSpacing());
                    concurrentPlayerTrackDictionary[iteration].AddObjects(new List<ITimedObject>{new TimedEvent(new SequenceTrackNameEvent("tone:" + Config.Instrument.InstrumentTone.Name))});
                }
            );

            trackChunks = concurrentPlayerTrackDictionary.Values.ToList();

            return trackChunks;
        }
    }
}
