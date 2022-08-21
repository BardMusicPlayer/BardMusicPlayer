using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BardMusicPlayer.Jamboree.Events
{
    /// <summary>
    /// if the connection 
    /// </summary>
    public sealed class PartyChangedEvent : JamboreeEvent
    {
        internal PartyChangedEvent() : base(0, false)
        {
            EventType = GetType();
        }

        public override bool IsValid() => true;
    }
}
