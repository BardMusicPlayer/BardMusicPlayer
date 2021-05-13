/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Lists all durations of a beat.
    /// </summary>
    internal enum Duration
    {
        /// <summary>
        /// A quadruple whole note duration 
        /// </summary>
        QuadrupleWhole = -4,

        /// <summary>
        /// A double whole note duration 
        /// </summary>
        DoubleWhole = -2,

        /// <summary>
        /// A whole note duration 
        /// </summary>
        Whole = 1,

        /// <summary>
        /// A 1/2 note duration 
        /// </summary>
        Half = 2,

        /// <summary>
        /// A 1/4 note duration 
        /// </summary>
        Quarter = 4,

        /// <summary>
        /// A 1/8 note duration 
        /// </summary>
        Eighth = 8,

        /// <summary>
        /// A 1/16 note duration 
        /// </summary>
        Sixteenth = 16,

        /// <summary>
        /// A 1/32 note duration 
        /// </summary>
        ThirtySecond = 32,

        /// <summary>
        /// A 1/64 note duration 
        /// </summary>
        SixtyFourth = 64,

        /// <summary>
        /// A 1/128 note duration 
        /// </summary>
        OneHundredTwentyEighth = 128,

        /// <summary>
        /// A 1/256 note duration 
        /// </summary>
        TwoHundredFiftySixth = 256
    }
}
