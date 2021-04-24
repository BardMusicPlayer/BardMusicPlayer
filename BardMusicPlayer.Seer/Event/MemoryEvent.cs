using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardMusicPlayer.Seer.Event
{
    internal class MemoryEvent : Event
    {
        internal MemoryEvent() : base(typeof(MemoryEvent))
        {
            // do something
        }

        public override bool IsValid()
        {
            // check internal variables for validity
            return true;
        }
    }
}
