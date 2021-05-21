/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    public interface ISerializedServerSocket : IDisposable
    {
        ISerializedSocket Accept();

        bool IsBound();

        bool IsClosed();

        void Close();
    }
}
