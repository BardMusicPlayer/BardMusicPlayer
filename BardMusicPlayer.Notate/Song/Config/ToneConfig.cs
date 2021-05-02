/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Notate.Song.Config.Interfaces;
using System.Collections.Generic;

namespace BardMusicPlayer.Notate.Song.Config
{
    public class ToneConfig : IConfig
    {
        public int Track { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public List<long> IncludedTracks { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int PlayerCount { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
