/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    public interface ISerializedSocket : IDisposable
    {
        bool IsClosed();

        bool IsConnected();

        T Read<T>();

        void Write(Object obj);

        void Close();
    }
}
