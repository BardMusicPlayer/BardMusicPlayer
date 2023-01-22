using System;
using System.Collections;

namespace Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Processing
{
    public class ChasedEventArgs : EventArgs
    {
        private ICollection messages;

        public ChasedEventArgs(ICollection messages)
        {
            this.messages = messages;
        }

        public ICollection Messages
        {
            get
            {
                return messages;
            }
        }
    }
}
