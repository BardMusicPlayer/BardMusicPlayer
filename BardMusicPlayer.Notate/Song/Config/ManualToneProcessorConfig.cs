/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Notate.Song.Config.Interfaces;
using System.Collections.Generic;
using BardMusicPlayer.Common.Structs;

namespace BardMusicPlayer.Notate.Song.Config
{
    public class ManualToneProcessorConfig : IProcessorConfig
    {
        ///<inheritdoc/>
        public int Track { get; set; } = 0;

        ///<inheritdoc/>
        public List<long> IncludedTracks { get; set; } = new();

        ///<inheritdoc/>
        public int PlayerCount { get; set; } = 1;

        /// <summary>
        /// The instrument tone for this track
        /// </summary>
        public InstrumentTone InstrumentTone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Instrument, OctaveRange> OctaveRanges { get; set; }
    }
}
