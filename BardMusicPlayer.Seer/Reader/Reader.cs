using System;

namespace BardMusicPlayer.Seer.Reader
{
    internal abstract class Reader : IDisposable
    {
        protected readonly Game Seer;
        internal Reader(Game seer)
        {
            Seer = seer;
        }
        ~Reader() => Dispose();
        public abstract void Dispose();
    }
}
