/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;

namespace BardMusicPlayer.Transmogrify.Song.Config.Interfaces
{
    public interface IProcessorConfig
    {
        /// <summary>
        /// The main track this processorConfig is applied against
        /// </summary>
        int Track { get; set; }

        /// <summary>
        /// Tracks to merge into the main track before processing
        /// </summary>
        List<long> IncludedTracks { get; set; }

        /// <summary>
        /// Amount of players to distribute notes to.
        /// </summary>
        int PlayerCount { get; set; }
    }
}
