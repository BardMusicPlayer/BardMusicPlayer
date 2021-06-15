/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.Collections;

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// A chord definition.
    /// </summary>
    internal class Chord
    {
        /// <summary>
        /// Gets or sets the name of the chord
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates the first fret of the chord diagram.
        /// </summary>
        public int FirstFret { get; set; }

        /// <summary>
        /// Gets or sets the frets played on the individual strings for this chord. 
        /// - The order in this list goes from the highest string to the lowest string.  
        /// - -1 indicates that the string is not played. 
        /// </summary>
        public FastList<int> Strings { get; set; }

        /// <summary>
        /// Gets or sets a list of frets where the finger should hold a barre 
        /// </summary>
        public FastList<int> BarreFrets { get; set; }

        /// <summary>
        /// Gets or sets the staff the chord belongs to. 
        /// </summary>
        public Staff Staff { get; set; }

        /// <summary>
        /// Gets or sets whether the chord name is shown above the chord diagram. 
        /// </summary>
        public bool ShowName { get; set; }

        /// <summary>
        /// Gets or sets whether the chord diagram is shown.
        /// </summary>
        public bool ShowDiagram { get; set; }

        /// <summary>
        /// Gets or sets whether the fingering is shown below the chord diagram. 
        /// </summary>
        public bool ShowFingering { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chord"/> class.
        /// </summary>
        public Chord()
        {
            Strings = new FastList<int>();
            BarreFrets = new FastList<int>();
            ShowDiagram = true;
            ShowName = true;
            ShowFingering = true;
            FirstFret = 1;
        }

        internal static void CopyTo(Chord src, Chord dst)
        {
            dst.FirstFret = src.FirstFret;
            dst.Name = src.Name;
            dst.Strings = src.Strings.Clone();
            dst.BarreFrets = src.BarreFrets.Clone();
            dst.ShowName = src.ShowName;
            dst.ShowDiagram = src.ShowDiagram;
            dst.ShowFingering = src.ShowFingering;
        }
    }
}
