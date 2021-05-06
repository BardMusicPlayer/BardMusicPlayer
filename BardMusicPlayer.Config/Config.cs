/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Config
{
    public class Config : JsonSettings.JsonSettings
    {






        public override string FileName { get; set; }
        public Config(string fileName) : base(fileName) { }
    }
}
