/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Lists all harmonic types.
    /// </summary>
    internal enum HarmonicType
    {
        /// <summary>
        /// No harmonics. 
        /// </summary>
        None,

        /// <summary>
        /// Natural harmonic
        /// </summary>
        Natural,

        /// <summary>
        /// Artificial harmonic
        /// </summary>
        Artificial,

        /// <summary>
        /// Pinch harmonics
        /// </summary>
        Pinch,

        /// <summary>
        /// Tap harmonics
        /// </summary>
        Tap,

        /// <summary>
        /// Semi harmonics
        /// </summary>
        Semi,

        /// <summary>
        /// Feedback harmonics
        /// </summary>
        Feedback
    }
}
