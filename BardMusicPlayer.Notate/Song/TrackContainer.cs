﻿/*
 * Copyright(c) 2021 MoogleTroupe
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
        public Dictionary<long, ConfigContainer> ConfigContainers { get; set; } = new();
    }
}
