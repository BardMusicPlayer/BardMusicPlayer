/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// This class represents the rendering stylesheet.
    /// It contains settings which control the display of the score when rendered. 
    /// </summary>
    internal class RenderStylesheet
    {
        /// <summary>
        /// Gets or sets whether dynamics are hidden.
        /// </summary>
        public bool HideDynamics { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderStylesheet"/> class.
        /// </summary>
        public RenderStylesheet()
        {
            HideDynamics = false;
        }

        internal static void CopyTo(RenderStylesheet src, RenderStylesheet dst)
        {
            dst.HideDynamics = src.HideDynamics;
        }
    }
}
