namespace BardMusicPlayer.Seer.Reader
{
    internal class DatReader : Reader
    {
        public DatReader(Game game) : base(game)
        {
            
        }

        public override void Dispose()
        {
            
        }

        protected override int SleepTimeInMs()
        {
            // this doesn't need to run too often
            return 100;
        }

        protected override bool RunLoop()
        {
            Event.DatEvent newEvent = new Event.DatEvent();
            this.PushEvent(newEvent);
            return true;
        }
    }
}
