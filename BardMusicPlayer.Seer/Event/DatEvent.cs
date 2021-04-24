using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardMusicPlayer.Seer.Event
{
    internal class DatEvent : Event
    {
        internal DatEvent() : base(typeof(DatEvent))
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
