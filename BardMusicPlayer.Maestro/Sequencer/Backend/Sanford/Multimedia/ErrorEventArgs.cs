using System;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia;

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