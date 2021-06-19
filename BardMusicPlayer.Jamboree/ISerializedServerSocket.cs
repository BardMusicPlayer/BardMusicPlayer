/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    public interface ISerializedServerSocket : IDisposable
    {
        /// <summary>
        /// Accepts an incoming connection.
        /// </summary>
        /// <returns></returns>
        ISerializedSocket Accept();

        /// <summary>
        /// Returns true if this server socket is bound.
        /// </summary>
        /// <returns></returns>
        bool IsBound();

        /// <summary>
        /// Returns true if this server socket is closed.
        /// </summary>
        /// <returns></returns>
        bool IsClosed();

        /// <summary>
        /// Closes this server socket.
        /// </summary>
        void Close();
    }
}
