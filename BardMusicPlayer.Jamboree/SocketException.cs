/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.IO;

namespace BardMusicPlayer.Jamboree
{
    public class SocketException : IOException
    {
        public SocketException(string message) : base(message) { }

        public SocketException(string message, Exception inner) : base(message, inner) { }
    }
}
