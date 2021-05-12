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
        public ClassicProcessorConfig ProcessorConfig { get; set; }

        internal ClassicProcessor(ClassicProcessorConfig processorConfig, BmpSong song) : base(song)
        {
            ProcessorConfig = processorConfig;
        }

        public override async Task<List<TrackChunk>> Process()
        {
            var trackChunks = new List<TrackChunk> { Song.TrackContainers[ProcessorConfig.Track].SourceTrackChunk }.Concat(ProcessorConfig.IncludedTracks.Select(track => Song.TrackContainers[track].SourceTrackChunk)).ToList();

            var trackChunk = TimedObjectUtilities.ToTrackChunk(await 
                trackChunks.GetNoteDictionary(Song.SourceTempoMap, ProcessorConfig.Instrument.InstrumentTone,
                        ProcessorConfig.OctaveRange.LowerNote, 
                        ProcessorConfig.OctaveRange.UpperNote, 
                        (int) ProcessorConfig.Instrument.InstrumentToneMenuKey, 
                        false,
                        -ProcessorConfig.OctaveRange.LowerNote)
                .MoveNoteDictionaryToDefaultOctave(ProcessorConfig.OctaveRange)
                .ConcatNoteDictionaryToList());

            var playerNotesDictionary = await trackChunk.GetPlayerNoteDictionary(ProcessorConfig.PlayerCount, OctaveRange.C3toC6.LowerNote, OctaveRange.C3toC6.UpperNote);

            var concurrentPlayerTrackDictionary = new ConcurrentDictionary<long, TrackChunk>(ProcessorConfig.PlayerCount, ProcessorConfig.PlayerCount);

            Parallel.ForEach(playerNotesDictionary.Values, async (notesDictionary, _, iteration) =>
                {
                    concurrentPlayerTrackDictionary[iteration] = TimedObjectUtilities.ToTrackChunk(await notesDictionary.ConcatNoteDictionaryToList().FixChords().OffSet50Ms().FixEndSpacing());
                    concurrentPlayerTrackDictionary[iteration].AddObjects(new List<ITimedObject>{new TimedEvent(new SequenceTrackNameEvent("tone:" + ProcessorConfig.Instrument.InstrumentTone.Name))});
                }
            );

            trackChunks = concurrentPlayerTrackDictionary.Values.ToList();

            return trackChunks;
        }
    }
}
