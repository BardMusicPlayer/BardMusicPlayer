﻿/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Transmogrify.Song.Config;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Transmogrify.Processor
{
    internal class ManualToneProcessor : BaseProcessor
    {
        public ManualToneProcessorConfig ProcessorConfig { get; set; }

        internal ManualToneProcessor(ManualToneProcessorConfig processorConfig, BmpSong song) : base(song)
        {
            ProcessorConfig = processorConfig;
        }

        public override Task<List<TrackChunk>> Process()
        {
            return Task.FromResult(new List<TrackChunk>());
        }
    }
}
