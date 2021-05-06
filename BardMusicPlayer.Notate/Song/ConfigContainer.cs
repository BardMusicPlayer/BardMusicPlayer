/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
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
        public IConfig Config { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TrackChunk> ProccesedTrackChunks { get; set; }

        public async Task<List<TrackChunk>> RefreshTrackChunks(BmpSong song)
        {
            if (Config is null) throw new BmpNotateException("No configuration in this container.");
            return Config switch
            {
                AutoToneConfig autoToneConfig => await new AutoToneProcessor(autoToneConfig, song).Process(),
                ClassicConfig classicConfig => await new ClassicProcessor(classicConfig, song).Process(),
                DrumToneConfig drumToneConfig => await new DrumToneProcessor(drumToneConfig, song).Process(),
                LyricConfig lyricConfig => await new LyricProcessor(lyricConfig, song).Process(),
                ManualToneConfig manualToneConfig => await new ManualToneProcessor(manualToneConfig, song).Process(),
                _ => throw new BmpNotateException(Config.GetType() + " is not a supported configuration type."),
            };
        }
    }
}
