/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    internal interface IServerSocket : IDisposable
    {
        /// <summary>
        /// Blocks awaiting an incoming connection.
        /// </summary>
        /// <returns></returns>
        ISocket Accept();

        /// <summary>
        /// Returns true if this serversocket is closed.
        /// </summary>
        /// <returns></returns>
        bool IsClosed();

        /// <summary>
        /// Returns true if this server socket is bound.
        /// </summary>
        /// <returns></returns>
        bool IsBound();

        /// <summary>
        /// Closes the socket.
        /// </summary>
        /// <exception cref="SocketException"></exception>
        void Close();
    }
}
