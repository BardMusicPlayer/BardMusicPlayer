using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Sequencing.TrackClasses;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages.EventArg;

public class ChannelMessageEventArgs : EventArgs
{
    private ChannelMessage message;
    private Track track;

    public ChannelMessageEventArgs(Track track, ChannelMessage message)
    {
        this.message = message;
        this.track   = track;
    }

    public ChannelMessage Message
    {
        get
        {
            return message;
        }
    }

    public Track MidiTrack
    {
        get { return track; }
    }
}