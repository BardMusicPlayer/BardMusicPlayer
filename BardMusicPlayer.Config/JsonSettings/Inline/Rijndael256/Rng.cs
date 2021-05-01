/*
 * Copyright(c) 2013 2Toad, LLC. licensing@2toad.com
 * Licensed under the MIT license. See https://github.com/2Toad/Rijndael256/blob/master/LICENSE for full license information.
 */

using System.Security.Cryptography;

namespace BardMusicPlayer.Config.JsonSettings.Inline.Rijndael256
{
    internal static class Rng
    {
        static Rng()
        {
            Random = RandomNumberGenerator.Create();
        }

        /// <summary>
        /// Generates an array of bytes using a cryptographically strong sequence
        /// of random values.
        /// </summary>
        /// <param name="size">The size of the array.</param>
        /// <returns>The array of bytes.</returns>
        public static byte[] GenerateRandomBytes(int size)
        {
            var bytes = new byte[size];
            Random.GetBytes(bytes);
            return bytes;
        }

        private static readonly RandomNumberGenerator Random;
    }
}
