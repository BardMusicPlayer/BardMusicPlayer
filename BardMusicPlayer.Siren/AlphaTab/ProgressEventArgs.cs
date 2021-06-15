/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab
{
    /// <summary>
    /// Represents the progress of any data being loaded.
    /// </summary>
    internal class ProgressEventArgs
    {
        /// <summary>
        /// Gets the currently loaded bytes.
        /// </summary>
        public int Loaded { get; }

        /// <summary>
        /// Gets the total number of bytes to load.
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressEventArgs"/> class.
        /// </summary>
        /// <param name="loaded"></param>
        /// <param name="total"></param>
        public ProgressEventArgs(int loaded, int total)
        {
            Loaded = loaded;
            Total = total;
        }
    }
}
