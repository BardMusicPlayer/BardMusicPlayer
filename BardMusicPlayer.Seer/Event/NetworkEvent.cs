using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardMusicPlayer.Seer.Event
{
    internal class NetworkEvent : Event
    {
        internal NetworkEvent() : base(typeof(NetworkEvent))
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
