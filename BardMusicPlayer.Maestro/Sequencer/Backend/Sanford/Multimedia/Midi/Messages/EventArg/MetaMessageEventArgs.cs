using System;
using BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Sequencing.TrackClasses;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford.Multimedia.Midi.Messages.EventArg;

public class MetaMessageEventArgs : EventArgs
{
    private MetaMessage message;
    private Track track;

    public MetaMessageEventArgs(Track track, MetaMessage message)
    {
        this.message = message;
        this.track   = track;
    }

    public MetaMessage Message
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