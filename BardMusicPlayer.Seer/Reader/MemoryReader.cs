namespace BardMusicPlayer.Seer.Reader
{
    internal class MemoryReader : Reader
    {
        public MemoryReader(Game game) : base(game)
        {
        }

        public override void Dispose()
        {
            
        }

        protected override int SleepTimeInMs()
        {
            // idk. i like the number 33
            return 33;
        }

        protected override bool RunLoop()
        {
            Event.MemoryEvent newEvent = new Event.MemoryEvent();
            this.PushEvent(newEvent);
            return true;
        }
    }
}
