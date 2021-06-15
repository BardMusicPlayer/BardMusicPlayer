/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Represents a fermata. 
    /// </summary>
    internal class Fermata
    {
        /// <summary>
        /// Gets or sets the type of fermata. 
        /// </summary>
        public FermataType Type { get; set; }

        /// <summary>
        /// Gets or sets the actual lenght of the fermata. 
        /// </summary>
        public float Length { get; set; }

        internal static void CopyTo(Fermata src, Fermata dst)
        {
            dst.Type = src.Type;
            dst.Length = src.Length;
        }
    }
}
