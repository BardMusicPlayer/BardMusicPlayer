/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Notate.Processor;
using BardMusicPlayer.Notate.Song.Config;
using BardMusicPlayer.Notate.Song.Config.Interfaces;
using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BardMusicPlayer.Notate.Song
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
            if (ProcessorConfig is null) throw new BmpNotateException("No configuration in this container.");
            return ProcessorConfig switch
            {
                AutoToneProcessorConfig autoToneProcessorConfig => await new AutoToneProcessor(autoToneProcessorConfig, song).Process(),
                ClassicProcessorConfig classicProcessorConfig => await new ClassicProcessor(classicProcessorConfig, song).Process(),
                DrumToneProcessorConfig drumToneProcessorConfig => await new DrumToneProcessor(drumToneProcessorConfig, song).Process(),
                LyricProcessorConfig lyricProcessorConfig => await new LyricProcessor(lyricProcessorConfig, song).Process(),
                ManualToneProcessorConfig manualToneProcessorConfig => await new ManualToneProcessor(manualToneProcessorConfig, song).Process(),
                _ => throw new BmpNotateException(ProcessorConfig.GetType() + " is not a supported configuration type."),
            };
        }
    }
}
