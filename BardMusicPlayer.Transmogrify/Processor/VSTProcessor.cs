/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Transmogrify.Song.Config;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Transmogrify.Processor
{
    internal class VSTProcessor : BaseProcessor
    {
        public VSTProcessorConfig ProcessorConfig { get; set; }

        internal VSTProcessor(VSTProcessorConfig processorConfig, BmpSong song) : base(song)
        {
            ProcessorConfig = processorConfig;
        }

        public override Task<List<TrackChunk>> Process()
        {
            var trackChunks = new List<TrackChunk> { Song.TrackContainers[ProcessorConfig.Track].SourceTrackChunk }.Concat(ProcessorConfig.IncludedTracks.Select(track => Song.TrackContainers[track].SourceTrackChunk)).ToList();

            return Task.FromResult(new List<TrackChunk>());
        }
    }
}
