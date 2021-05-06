/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Common
{
    public class BmpException : Exception
    {
        public BmpException() : base()
        {
        }

        public BmpException(string message) : base(message)
        {
        }

        public BmpException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}