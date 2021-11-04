namespace BardMusicPlayer.Maestro.Events
{
    public sealed class PlaybackStoppedEvent : MaestroEvent
    {

        internal PlaybackStoppedEvent() : base(0, false)
        {
            EventType = GetType();
            Stopped = true;
        }

        public bool Stopped;
        public override bool IsValid() => true;
    }
}
