using Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Sequencing.Track_Classes;

namespace Sanford.Multimedia.Midi.Sanford.Multimedia.Midi.Messages.EventArgs
{
    public class SysExMessageEventArgs : System.EventArgs
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
