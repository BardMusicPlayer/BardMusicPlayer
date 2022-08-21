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
    public sealed class PartyConnectionChangedEvent : JamboreeEvent
    {
        public enum ResponseCode : int
        {
            ERROR =-1,
            OK,
            MESSAGE
        }

        internal PartyConnectionChangedEvent(ResponseCode code, string message) : base(0, false)
        {
            EventType = GetType();
            Code = code;
            Message = message;
        }

        public ResponseCode Code { get; }

        public string Message { get; }

        public override bool IsValid() => true;
    }
}
