/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Lists all dynamics.
    /// </summary>
    internal enum DynamicValue
    {
        /// <summary>
        /// pianississimo (very very soft)
        /// </summary>
        PPP,

        /// <summary>
        /// pianissimo (very soft)
        /// </summary>
        PP,

        /// <summary>
        /// piano (soft)
        /// </summary>
        P,

        /// <summary>
        /// mezzo-piano (half soft)
        /// </summary>
        MP,

        /// <summary>
        /// mezzo-forte (half loud)
        /// </summary>
        MF,

        /// <summary>
        /// forte (loud)
        /// </summary>
        F,

        /// <summary>
        /// fortissimo (very loud)
        /// </summary>
        FF,

        /// <summary>
        /// fortississimo (very very loud)
        /// </summary>
        FFF
    }
    // ReSharper restore InconsistentNaming
}
