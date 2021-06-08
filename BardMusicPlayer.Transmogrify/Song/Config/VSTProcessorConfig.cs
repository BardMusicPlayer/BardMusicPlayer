/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;

namespace BardMusicPlayer.Transmogrify.Song.Config
{
    public class VSTProcessorConfig : IProcessorConfig
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
        /// Octave Range for each Tone
        /// </summary>
        public Dictionary<int, OctaveRange> OctaveRanges { get; set; } = new(5)
        {
            {0, OctaveRange.C3toC6},
            {1, OctaveRange.C3toC6},
            {2, OctaveRange.C3toC6},
            {3, OctaveRange.C3toC6},
            {4, OctaveRange.C3toC6}
        };
    }
}
