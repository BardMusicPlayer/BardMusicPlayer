/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Lists the different bend styles
    /// </summary>
    internal enum BendStyle
    {
        /// <summary>
        /// The bends are as described by the bend points 
        /// </summary>
        Default,

        /// <summary>
        /// The bends are gradual over the beat duration. 
        /// </summary>
        Gradual,

        /// <summary>
        /// The bends are done fast before the next note. 
        /// </summary>
        Fast
    }
}
