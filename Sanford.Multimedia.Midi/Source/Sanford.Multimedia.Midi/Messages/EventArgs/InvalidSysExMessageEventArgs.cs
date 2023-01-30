using System;
using System.Collections;

namespace Sanford.Multimedia.Midi
{
    public class InvalidSysExMessageEventArgs : EventArgs
    {
        private byte[] messageData;

        public InvalidSysExMessageEventArgs(byte[] messageData)
        {
            this.messageData = messageData;
        }

        public ICollection MessageData
        {
            get
            {
                return messageData;
            }
        }
    }
}