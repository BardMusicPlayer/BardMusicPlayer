using Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Sequencing.Track_Classes;

namespace Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.EventArgs
{
    public class ChannelMessageEventArgs : System.EventArgs
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
