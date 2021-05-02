/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Common.Structs;
using BardMusicPlayer.Notate.Song.Config.Interfaces;
using System.Collections.Generic;

namespace BardMusicPlayer.Notate.Song.Config
{
    public class ClassicConfig : IConfig
    {
        ///<inheritdoc/>
        public int Track { get; set; } = 0;

        ///<inheritdoc/>
        public List<long> IncludedTracks { get; set; } = new();

        ///<inheritdoc/>
        public int PlayerCount { get; set; } = 1;

        /// <summary>
        /// The instrument for this track
        /// </summary>
        public Instrument Instrument { get; set; } = Instrument.Piano;

        /// <summary>
        /// The octave range to use
        /// </summary>
        public OctaveRange OctaveRange { get; set; } = OctaveRange.C3toC6;
    }
}