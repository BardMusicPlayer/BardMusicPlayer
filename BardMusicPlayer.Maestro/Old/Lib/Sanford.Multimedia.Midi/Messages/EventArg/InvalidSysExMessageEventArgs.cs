using System;
using System.Collections;

namespace BardMusicPlayer.Maestro.Old.Lib.Sanford.Multimedia.Midi.Messages.EventArg
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