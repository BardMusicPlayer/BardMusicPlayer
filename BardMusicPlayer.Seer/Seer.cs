using System;
using System.Collections.Generic;
using System.Text;

namespace BardMusicPlayer.Seer
{
    class Seer
    {
        private static readonly Lazy<Seer> LazyInstance = new(() => new Seer());
        public static Seer Instance => LazyInstance.Value;


    }
}
