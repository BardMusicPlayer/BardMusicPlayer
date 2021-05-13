/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Pigeonhole
{
    public class BmpPigeonholeException : Exception
    {
        public BmpPigeonholeException(string message) : base(message) { }
        public BmpPigeonholeException(string message, Exception inner) : base(message, inner) { }
    }
}