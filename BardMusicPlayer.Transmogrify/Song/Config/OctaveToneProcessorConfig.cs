/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;

namespace BardMusicPlayer.Transmogrify.Song.Config
{
    public class OctaveToneProcessorConfig : IProcessorConfig
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
        public InstrumentTone InstrumentTone { get; set; } = InstrumentTone.Strummed;

        /// <summary>
        /// The mapper of octave to tone and octave
        /// </summary>
        public Dictionary<int, (int, int)> Mapper { get; set; } = new(9)
        {
            {0, (-1, -1)},
            {1, (-1, -1)},
            {2, (-1, -1)},
            {3, (-1, -1)},
            {4, (-1, -1)},
            {5, (-1, -1)},
            {6, (-1, -1)},
            {7, (-1, -1)},
            {8, (-1, -1)}
        };
    }
}
