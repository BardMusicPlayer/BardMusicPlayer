/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Lists all types of how to brush multiple notes on a beat. 
    /// </summary>
    internal enum BrushType
    {
        /// <summary>
        /// No brush. 
        /// </summary>
        None,

        /// <summary>
        /// Normal brush up. 
        /// </summary>
        BrushUp,

        /// <summary>
        /// Normal brush down. 
        /// </summary>
        BrushDown,

        /// <summary>
        /// Arpeggio up. 
        /// </summary>
        ArpeggioUp,

        /// <summary>
        /// Arpeggio down. 
        /// </summary>
        ArpeggioDown
    }
}
