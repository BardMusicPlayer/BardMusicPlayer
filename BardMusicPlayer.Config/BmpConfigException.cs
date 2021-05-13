/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Config
{
    public class BmpConfigException : Exception
    {
        public BmpConfigException(string message) : base(message) { }
        public BmpConfigException(string message, Exception inner) : base(message, inner) { }
    }
}