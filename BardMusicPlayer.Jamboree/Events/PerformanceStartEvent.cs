namespace BardMusicPlayer.Jamboree.Events
{
    public sealed class PerformanceStartEvent : JamboreeEvent
    {
        /// <summary>
        /// Start the performance received
        /// </summary>
        /// <param name="timestampinMillis">in milliseconds</param>
        internal PerformanceStartEvent(long timestampinMillis, bool start)
        {
            EventType = GetType();
            SenderTimestamp_in_millis = timestampinMillis;
            Play = start;
        }

        /// <summary>
        /// The host time in milis
        /// </summary>
        public long SenderTimestamp_in_millis { get; }
        public bool Play { get; }

        public override bool IsValid() => true;
    }

}
