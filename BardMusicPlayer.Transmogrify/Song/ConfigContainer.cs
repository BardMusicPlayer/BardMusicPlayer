/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Transmogrify.Processor;
using BardMusicPlayer.Transmogrify.Song.Config;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Transmogrify.Song
{
    public sealed class ConfigContainer
    {
        /// <summary>
        /// 
        /// </summary>
        public IProcessorConfig ProcessorConfig { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TrackChunk> ProccesedTrackChunks { get; set; }

        public async Task<List<TrackChunk>> RefreshTrackChunks(BmpSong song)
        {
            if (ProcessorConfig is null) throw new BmpTransmogrifyException("No configuration in this container.");
            return ProcessorConfig switch
            {
                ClassicProcessorConfig classicProcessorConfig => await new ClassicProcessor(classicProcessorConfig, song).Process(),
                LyricProcessorConfig lyricProcessorConfig => await new LyricProcessor(lyricProcessorConfig, song).Process(),
                VSTProcessorConfig vstProcessorConfig => await new VSTProcessor(vstProcessorConfig, song).Process(),
                _ => throw new BmpTransmogrifyException(ProcessorConfig.GetType() + " is not a supported configuration type."),
            };
        }
    }
}
