namespace Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.EventArgs
{
    public class SysCommonMessageEventArgs : System.EventArgs
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
