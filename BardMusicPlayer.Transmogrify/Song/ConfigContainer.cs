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
                AutoToneProcessorConfig autoToneProcessorConfig => await new AutoToneProcessor(autoToneProcessorConfig,
                    song).Process(),
                ClassicProcessorConfig classicProcessorConfig => await new ClassicProcessor(classicProcessorConfig,
                    song).Process(),
                DrumToneProcessorConfig drumToneProcessorConfig => await new DrumToneProcessor(drumToneProcessorConfig,
                    song).Process(),
                LyricProcessorConfig lyricProcessorConfig => await new LyricProcessor(lyricProcessorConfig, song)
                    .Process(),
                ManualToneProcessorConfig manualToneProcessorConfig => await new ManualToneProcessor(
                    manualToneProcessorConfig, song).Process(),
                NoteToneProcessorConfig noteToneProcessorConfig => await new NoteToneProcessor(noteToneProcessorConfig,
                    song).Process(),
                OctaveToneProcessorConfig octaveToneProcessorConfig => await new OctaveToneProcessor(
                    octaveToneProcessorConfig, song).Process(),
                _ => throw new BmpTransmogrifyException(ProcessorConfig.GetType() +
                                                        " is not a supported configuration type.")
            };
        }
    }
}