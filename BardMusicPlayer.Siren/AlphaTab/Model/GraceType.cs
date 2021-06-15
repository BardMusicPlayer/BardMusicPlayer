/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Lists all types of grace notes
    /// </summary>
    internal enum GraceType
    {
        /// <summary>
        /// No grace, normal beat. 
        /// </summary>
        None,

        /// <summary>
        /// The beat contains on-beat grace notes. 
        /// </summary>
        OnBeat,

        /// <summary>
        /// The beat contains before-beat grace notes. 
        /// </summary>
        BeforeBeat,

        /// <summary>
        /// The beat contains very special bend-grace notes used in SongBook style displays.
        /// </summary>
        BendGrace
    }
}
