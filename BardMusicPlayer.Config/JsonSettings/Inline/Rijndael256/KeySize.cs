/*
 * Copyright(c) 2013 2Toad, LLC. licensing@2toad.com
 * Licensed under the MIT license. See https://github.com/2Toad/Rijndael256/blob/master/LICENSE for full license information.
 */

namespace BardMusicPlayer.Config.JsonSettings.Inline.Rijndael256
{
    /// <summary>
    /// AES approved cipher key sizes.
    /// </summary>
    internal enum KeySize
    {
        /// <summary>
        /// 128-bit
        /// </summary>
        Aes128 = 128,
        /// <summary>
        /// 192-bit
        /// </summary>
        Aes192 = 192,
        /// <summary>
        /// 256-bit
        /// </summary>
        Aes256 = 256
    }
}