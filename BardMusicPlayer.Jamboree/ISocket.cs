/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    internal interface ISocket : IDisposable
    {
        /// <summary>
        /// Closes this socket.
        /// </summary>
        void Close();

        /// <summary>
        /// Returns true if this socket is closed.
        /// </summary>
        /// <returns></returns>
        bool IsClosed();

        /// <summary>
        /// Returns true if this socket is connected.
        /// </summary>
        /// <returns></returns>
        bool IsConnected();

        /// <summary>
        /// Writes the data to the socket.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        int Send(byte[] data, int offset, int len);

        /// <summary>
        /// Reads the data from the socket.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        int Receive(byte[] buffer, int offset, int len);
    }
}
