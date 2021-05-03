using System;
using System.Collections.Generic;
using System.Text;

namespace Sanford.Multimedia.Midi
{
    public class SysExMessageEventArgs : EventArgs
    {
        private SysExMessage message;
		private Track track;

        public SysExMessageEventArgs(Track track, SysExMessage message)
        {
            this.message = message;
			this.track = track;
        }

        public SysExMessage Message
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
