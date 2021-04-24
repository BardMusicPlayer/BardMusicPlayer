using System;
using System.Collections.Generic;
using System.Text;

namespace BardMusicPlayer.Seer
{
    public class Seer
    {
        private static readonly Lazy<Seer> LazyInstance = new(() => new Seer());
        public static Seer Instance => LazyInstance.Value;

        public void OnGameUpdated(Game theGame)
        {
            // notify subscribers they just lost the game
        }
    }
}
