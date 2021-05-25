/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    public interface ISerializationAdapter
    {
        /// <summary>
        /// Decode the length bytes of data in the buffer starting at offset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        T Decode<T>(byte[] buffer, int offset, int length);

        /// <summary>
        /// Encode the specified object into its serialized bytes.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Encode(object obj);
    }
}
