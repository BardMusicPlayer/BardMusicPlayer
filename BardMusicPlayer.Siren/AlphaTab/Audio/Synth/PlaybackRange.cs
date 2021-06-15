/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth
{
    /// <summary>
    /// Represents a range of the song that should be played.
    /// </summary>
    internal class PlaybackRange
    {
        /// <summary>
        /// The position in midi ticks from where the song should start.
        /// </summary>
        public int StartTick { get; set; }

        /// <summary>
        /// The position in midi ticks to where the song should be played.
        /// </summary>
        public int EndTick { get; set; }
    }
}
