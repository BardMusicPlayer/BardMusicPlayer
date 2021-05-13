/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Notate.Song.Config;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Notate.Processor
{
    internal class DrumToneProcessor : BaseProcessor
    {
        public DrumToneProcessorConfig ProcessorConfig { get; set; }

        internal DrumToneProcessor(DrumToneProcessorConfig processorConfig, BmpSong song) : base(song)
        {
            ProcessorConfig = processorConfig;
        }

        public override Task<List<TrackChunk>> Process()
        {
            return Task.FromResult(new List<TrackChunk>());
        }
    }
}
