/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;

namespace BardMusicPlayer.Notate.Song.Config.Interfaces
{
    public interface IConfig
    {
        /// <summary>
        /// The main track this config is applied against
        /// </summary>
        int Track { get; set; }

        /// <summary>
        /// Tracks to merge into the main track before processing
        /// </summary>
        List<int> IncludedTracks { get; set; }

        /// <summary>
        /// Amount of players to distribute notes to.
        /// </summary>
        int PlayerCount { get; set; }
    }
}
