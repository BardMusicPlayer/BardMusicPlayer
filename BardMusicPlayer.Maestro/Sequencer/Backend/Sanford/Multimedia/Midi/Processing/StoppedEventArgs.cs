using System;
using System.Collections;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Processing
{
    public class StoppedEventArgs : EventArgs
    {
        private ICollection messages;

        public StoppedEventArgs(ICollection messages)
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