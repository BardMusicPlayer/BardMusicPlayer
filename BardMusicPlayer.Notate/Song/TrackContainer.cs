/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;

namespace BardMusicPlayer.Notate.Song
{
    public sealed class TrackContainer
    {
        /// <summary>
        /// 
        /// </summary>
        public TrackChunk SourceTrackChunk { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, ConfigContainer> ConfigContainers { get; set; } = new();
    }
}
