/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;

namespace BardMusicPlayer.Transmogrify.Song.Config
{
    public class DrumToneProcessorConfig : IProcessorConfig
    {
        ///<inheritdoc/>
        public int Track { get; set; } = 0;

        ///<inheritdoc/>
        public List<long> IncludedTracks { get; set; } = new();

        ///<inheritdoc/>
        public int PlayerCount { get; set; } = 1;

        /// <summary>
        /// The mapper of GM drum note to tone and note
        /// </summary>
        public Dictionary<int, (int, int)> Mapper { get; set; } = new(61)
        {
            {27, (-1, -1)},
            {28, (-1, -1)},
            {29, (-1, -1)},
            {30, (-1, -1)},
            {31, (-1, -1)},
            {32, (-1, -1)},
            {33, (-1, -1)},
            {34, (-1, -1)},
            {35, (2, 7)},
            {36, (2, 9)},
            {37, (-1, -1)},
            {38, (3, 19)},
            {39, (-1, -1)},
            {40, (3, 21)},
            {41, (2, 15)},
            {42, (-1, -1)},
            {43, (2, 18)},
            {44, (-1, -1)},
            {45, (2, 22)},
            {46, (-1, -1)},
            {47, (2, 25)},
            {48, (2, 29)},
            {49, (4, 23)},
            {50, (2, 32)},
            {51, (-1, -1)},
            {52, (4, 21)},
            {53, (-1, -1)},
            {54, (-1, -1)},
            {55, (4, 29)},
            {56, (-1, -1)},
            {57, (4, 23)},
            {58, (-1, -1)},
            {59, (-1, -1)},
            {60, (1, 22)},
            {61, (1, 19)},
            {62, (-1, -1)},
            {63, (-1, -1)},
            {64, (-1, -1)},
            {64, (-1, -1)},
            {65, (-1, -1)},
            {66, (-1, -1)},
            {67, (-1, -1)},
            {68, (-1, -1)},
            {69, (-1, -1)},
            {70, (-1, -1)},
            {71, (-1, -1)},
            {72, (-1, -1)},
            {73, (-1, -1)},
            {74, (-1, -1)},
            {74, (-1, -1)},
            {75, (-1, -1)},
            {76, (-1, -1)},
            {77, (-1, -1)},
            {78, (-1, -1)},
            {79, (-1, -1)},
            {80, (-1, -1)},
            {81, (-1, -1)},
            {82, (-1, -1)},
            {83, (-1, -1)},
            {84, (-1, -1)},
            {85, (-1, -1)},
            {86, (-1, -1)},
            {87, (-1, -1)}
        };
    }
}
