using System;
using Sanford.Multimedia.Midi;

namespace BardMusicPlayer.Maestro.Old.Lib.Sanford.Multimedia.Midi.Messages.EventArg
{
    public class MetaMessageEventArgs : EventArgs
    {
        private MetaMessage message;
        private Track track;

        public MetaMessageEventArgs(Track track, MetaMessage message)
        {
            this.message = message;
            this.track = track;
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
}