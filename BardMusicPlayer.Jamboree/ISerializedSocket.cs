/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    public interface ISerializedSocket : IDisposable
    {
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
        /// Read an object from the socket.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Read<T>();

        /// <summary>
        /// Write an object to the socket.
        /// </summary>
        /// <param name="obj"></param>
        void Write(Object obj);

        /// <summary>
        /// Close the socket.
        /// </summary>
        void Close();
    }
}
