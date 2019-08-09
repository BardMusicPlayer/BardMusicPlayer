using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpOrchestraPerformer : UserControl {

		private BmpSequencer player;
		public BmpSequencer Player {
			get {
				return player;
			}
			set {
				player = value;
				player.OnLoad += OnMidiLoad;
			}
		}

		public BmpKeyboard Keyboard {
			get { return KeyboardPreview; }
		}

		private PerformerControl performerControl;

		public BmpOrchestraPerformer() {
			InitializeComponent();

			TrackSelect.ValueChanged += TrackSelect_ValueChanged;
			SpeedShift.ValueChanged += SpeedShift_ValueChanged;
			OctaveShift.ValueChanged += OctaveShift_ValueChanged;
		}

		private void OnMidiLoad(object o, EventArgs e) {
			UpdateKeyboard();
		}

		private void TrackSelect_ValueChanged(object sender, EventArgs e) {
			int track = decimal.ToInt32((sender as NumericUpDown).Value);
			if(performerControl is PerformerControl ctl) {
				ctl.SetTrackNum(track);
			}

			UpdateKeyboard();
		}

		private void SpeedShift_ValueChanged(object sender, EventArgs e) {
			float speed = decimal.ToSingle((sender as NumericUpDown).Value);
			if(performerControl is PerformerControl ctl) {
				ctl.SetSpeedShift(speed / 100f);
			}

			UpdateKeyboard();
		}

		private void OctaveShift_ValueChanged(object sender, EventArgs e) {
			int octave = decimal.ToInt32((sender as NumericUpDown).Value);
			if(performerControl is PerformerControl ctl) {
				ctl.SetOctaveShift(octave);
			}

			UpdateKeyboard();
		}

		public void UpdateKeyboard() {
			if(!(performerControl is PerformerControl)) {
				return;
			}
			if(!(player is BmpSequencer)) {
				return;
			}
			int tn = performerControl.TrackNum;
			Sequence seq = player.Sequence;
			if(seq is Sequence && (tn >= 0 && tn < seq.Count) && seq[tn] is Track track) {
				List<int> notes = new List<int>();
				foreach(MidiEvent ev in track.Iterator()) {
					if(ev.MidiMessage.MessageType == MessageType.Channel) {
						ChannelMessage msg = (ev.MidiMessage as ChannelMessage);
						if(msg.Command == ChannelCommand.NoteOn) {
							int note = msg.Data1;
							int vel = msg.Data2;
							if(vel > 0) {
								notes.Add(NoteHelper.ApplyOctaveShift(note, performerControl.OctaveShift));
							}
						}
					}
				}
				KeyboardPreview.UpdateFrequency(notes);
				TrackSelect.Maximum = (seq.Count - 1);
			}
		}


		public void SetPerformer(PerformerControl ctl) {
			performerControl = ctl;

			bool b = (performerControl != null);

			if(b) {
				TrackSelect.Value = performerControl.TrackNum;
				SpeedShift.Value = 100;
				OctaveShift.Value = 0;

				PerformerInfo.Text = string.Format("{0}", ctl.CharName);

				Point loc = ctl.FindForm().PointToClient(ctl.PointToScreen(Point.Empty));
				int x = loc.X + ctl.Width;
				int y = loc.Y + (ctl.Height / 2) - (this.Height / 2);
				this.Location = new Point(x, y);

				this.Focus();
			}

			this.Visible = b;

			UpdateKeyboard();
		}

		private void ConductPerformerControl_Leave(object sender, EventArgs e) {
			this.Visible = false;
		}
	}
}
