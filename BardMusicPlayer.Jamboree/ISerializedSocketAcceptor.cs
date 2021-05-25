/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Jamboree
{
    internal interface ISerializedSocketAcceptor
    {
        /// <summary>
        /// This method is used for any initial processing or socket validation required. This should return null if the socket is to be rejected.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref="SocketException"></exception>
        ISerializedSocket Accept(ISocket accepted);
    }
}
