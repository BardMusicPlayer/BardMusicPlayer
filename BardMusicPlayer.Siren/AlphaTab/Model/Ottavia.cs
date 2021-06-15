/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

// ReSharper disable InconsistentNaming

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Lists all ottavia.  
    /// </summary>
    internal enum Ottavia
    {
        /// <summary>
        /// 2 octaves higher 
        /// </summary>
        _15ma,

        /// <summary>
        /// 1 octave higher
        /// </summary>
        _8va,

        /// <summary>
        /// Normal
        /// </summary>
        Regular,

        /// <summary>
        /// 1 octave lower
        /// </summary>
        _8vb,

        /// <summary>
        /// 2 octaves lower. 
        /// </summary>
        _15mb
    }
}
