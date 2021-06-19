/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Jamboree
{
    public interface ISerializedSocketFactory
    {
        /// <summary>
        /// Create a connected socket.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        ISerializedSocket createSocket();
    }
}
