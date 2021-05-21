/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    internal interface ISocket : IDisposable
    {
        void Close();

        bool IsClosed();

        bool IsConnected();

        int Send(byte[] data);

        int Send(byte[] data, int offset, int len);

        int Receive(byte[] buffer);

        int Receive(byte[] buffer, int offset, int len);
    }
}
