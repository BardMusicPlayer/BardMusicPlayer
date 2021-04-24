namespace BardMusicPlayer.Seer.Reader
{
    internal class NetworkReader : Reader
    {
        public NetworkReader(Game seer) : base(seer)
        {
        }

        public override void Dispose()
        {
            
        }

        protected override int SleepTimeInMs()
        {
            return 1;
        }

        protected override bool RunLoop()
        {
            // NOTE: NetworkReader won't need this, so we can just return false
            //       at the start and shutdown the thread entirely
            // OR: alternatively, this can be used to push aggregate information
            //     received from multiple network packets
            return false;
        }
    }
}
