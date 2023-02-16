using System;

namespace BardMusicPlayer.Maestro.Old.Lib.Sanford.Multimedia
{
    public class ErrorEventArgs : EventArgs
    {
        private Exception ex;

        public ErrorEventArgs(Exception ex)
        {
            this.ex = ex;
        }

        public Exception Error
        {
            get
            {
                return ex;
            }
        }
    }
}