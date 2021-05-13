/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// Lists all simile mark types as they are assigned to bars. 
    /// </summary>
    internal enum SimileMark
    {
        /// <summary>
        /// No simile mark is applied
        /// </summary>
        None,

        /// <summary>
        /// A simple simile mark. The previous bar is repeated. 
        /// </summary>
        Simple,

        /// <summary>
        /// A double simile mark. This value is assigned to the first
        /// bar of the 2 repeat bars. 
        /// </summary>
        FirstOfDouble,

        /// <summary>
        /// A double simile mark. This value is assigned to the second
        /// bar of the 2 repeat bars. 
        /// </summary>
        SecondOfDouble
    }
}
