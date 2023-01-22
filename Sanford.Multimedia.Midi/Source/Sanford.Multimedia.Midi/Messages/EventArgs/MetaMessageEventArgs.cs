using Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Sequencing.Track_Classes;

namespace Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.EventArgs
{
    public class MetaMessageEventArgs : System.EventArgs
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
