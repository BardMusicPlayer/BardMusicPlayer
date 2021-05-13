/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.IO
{
    /// <summary>
    /// Represents a writer where binary data can be written to. 
    /// </summary>
    internal interface IWriteable
    {
        /// <summary>
        /// Write a single byte to the stream. 
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteByte(byte value);

        /// <summary>
        /// Write data from the given buffer. 
        /// </summary>
        /// <param name="buffer">The buffer to get the data from. </param>
        /// <param name="offset">The offset where to start reading the data.</param>
        /// <param name="count">The number of bytes to write</param>
        void Write(byte[] buffer, int offset, int count);
    }
}
