namespace Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.EventArgs
{
    public class InvalidShortMessageEventArgs : System.EventArgs
    {
        private int message;

        public InvalidShortMessageEventArgs(int message)
        {
            this.message = message;
        }

        public int Message
        {
            get
            {
                return message;
            }
        }
    }
}
