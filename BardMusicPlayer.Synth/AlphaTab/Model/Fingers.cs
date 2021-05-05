namespace BardMusicPlayer.Synth.AlphaTab.Model
{
    /// <summary>
    /// Lists all fingers.
    /// </summary>
    internal enum Fingers
    {
        /// <summary>
        /// Unknown type (not documented)
        /// </summary>
        Unknown = -2,

        /// <summary>
        /// No finger, dead note
        /// </summary>
        NoOrDead = -1,

        /// <summary>
        /// The thumb
        /// </summary>
        Thumb = 0,

        /// <summary>
        /// The index finger
        /// </summary>
        IndexFinger = 1,

        /// <summary>
        /// The middle finger
        /// </summary>
        MiddleFinger = 2,

        /// <summary>
        /// The annular finger
        /// </summary>
        AnnularFinger = 3,

        /// <summary>
        /// The little finger
        /// </summary>
        LittleFinger = 4
    }
}
