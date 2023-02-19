using System;
using System.Collections;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Processing
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