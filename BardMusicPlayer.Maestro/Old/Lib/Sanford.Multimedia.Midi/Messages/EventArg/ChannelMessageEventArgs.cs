using System;
using Sanford.Multimedia.Midi;

namespace BardMusicPlayer.Maestro.Old.Lib.Sanford.Multimedia.Midi.Messages.EventArg
{
    public class ChannelMessageEventArgs : EventArgs
    {
        private ChannelMessage message;
        private Track track;

        public ChannelMessageEventArgs(Track track, ChannelMessage message)
        {
            this.message = message;
            this.track = track;
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
}