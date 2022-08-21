using System;
using System.Collections.Generic;
using System.Text;

namespace Sanford.Multimedia.Midi
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

		public Track MidiTrack {
			get { return track; }
		}
    }
}
