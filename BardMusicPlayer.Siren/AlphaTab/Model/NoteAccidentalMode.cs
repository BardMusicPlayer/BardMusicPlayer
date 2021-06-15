/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Lists the modes how accidentals are handled for notes
    /// </summary>
    internal enum NoteAccidentalMode
    {
        /// <summary>
        /// Accidentals are calculated automatically.
        /// </summary>
        Default,

        /// <summary>
        /// If the default behavior calculates a Sharp, use flat instead (and vice versa).
        /// </summary>
        SwapAccidentals,

        /// <summary>
        /// This will move the note one line down and applies a Naturalize.
        /// </summary>
        ForceNatural,

        /// <summary>
        /// This will move the note one line down and applies a Sharp.
        /// </summary>
        ForceSharp,

        /// <summary>
        /// This will move the note one line up and applies a Flat.
        /// </summary>
        ForceFlat
    }
}
