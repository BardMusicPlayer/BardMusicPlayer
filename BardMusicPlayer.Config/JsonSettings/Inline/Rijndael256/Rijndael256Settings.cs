/*
 * Copyright(c) 2013 2Toad, LLC. licensing@2toad.com
 * Licensed under the MIT license. See https://github.com/2Toad/Rijndael256/blob/master/LICENSE for full license information.
 */

namespace BardMusicPlayer.Config.JsonSettings.Inline.Rijndael256
{
    /// <summary>
    /// A collection of mutable defaults
    /// </summary>
    internal static class Rijndael256Settings
    {
        static Rijndael256Settings()
        {
            // Set defaults during initialization
            Reset();
        }

        /// <summary>
        /// Resets all the settings to their default values
        /// </summary>
        public static void Reset()
        {
            HashIterations = _hashIterations;
        }

        /// <summary>
        /// The number of iterations used to derive hashes.
        /// Default is 10000.
        /// </summary>
        public static int HashIterations;

        private const int _hashIterations = 10000;
    }
}
