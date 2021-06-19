/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Jamboree
{
    interface ISerializedServerSocketFactory
    {
        /// <summary>
        /// Creaetes a new, bound, listening socket.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        ISerializedServerSocket CreateServerSocket();
    }
}
