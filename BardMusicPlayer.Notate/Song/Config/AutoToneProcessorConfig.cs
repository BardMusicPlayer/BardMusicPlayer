/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Notate.Song.Config.Interfaces;
using System.Collections.Generic;
using BardMusicPlayer.Common.Structs;

namespace BardMusicPlayer.Notate.Song.Config
{
    public class AutoToneProcessorConfig : IProcessorConfig
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
        public AutoToneInstrumentGroup AutoToneInstrumentGroup { get; set; } = AutoToneInstrumentGroup.Lute1Harp3Piano1;

        /// <summary>
        /// The octave range to use
        /// </summary>
        public AutoToneOctaveRange AutoToneOctaveRange { get; set; } = AutoToneOctaveRange.C2toC7;
    }
}
