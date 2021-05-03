using System;
using System.Collections.Generic;
using System.Text;

namespace Sanford.Multimedia.Midi
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

		public Track MidiTrack {
			get { return track; }
		}
    }
}
