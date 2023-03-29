namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages.EventArg;

public class InvalidShortMessageEventArgs : EventArgs
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