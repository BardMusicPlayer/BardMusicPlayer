/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Jamboree
{
    interface ISerialziedSocketCreator
    {
        /// <summary>
        /// This should never return null; this should throw on error.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        ISerializedSocket Create(ISocket socket);
    }
}
