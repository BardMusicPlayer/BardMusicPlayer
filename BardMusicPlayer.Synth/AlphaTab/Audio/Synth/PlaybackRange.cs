namespace BardMusicPlayer.Synth.AlphaTab.Audio.Synth
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
