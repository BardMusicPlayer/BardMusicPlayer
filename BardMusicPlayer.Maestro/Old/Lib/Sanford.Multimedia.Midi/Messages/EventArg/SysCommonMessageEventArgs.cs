using System;

namespace BardMusicPlayer.Maestro.Old.Lib.Sanford.Multimedia.Midi.Messages.EventArg
{
    public class SysCommonMessageEventArgs : EventArgs
    {
        private SysCommonMessage message;

        public SysCommonMessageEventArgs(SysCommonMessage message)
        {
            this.message = message;
        }

        public SysCommonMessage Message
        {
            get
            {
                return message;
            }
        }
    }
}