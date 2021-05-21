/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    internal interface IServerSocket : IDisposable
    {
        ISocket Accept();
        bool IsClosed();
        bool IsBound();
        void Close();
    }
}
