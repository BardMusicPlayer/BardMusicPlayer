using System.Collections;

namespace Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.EventArgs
{
    public class InvalidSysExMessageEventArgs : System.EventArgs
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
