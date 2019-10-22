using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FFBardMusicPlayer.BmpProcessSelect;
using static FFBardMusicPlayer.Controls.BmpPlayer;
using static Sharlayan.Core.Enums.Performance;
using Sanford.Multimedia.Midi;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpLocalOrchestra : UserControl {

		private bool instrumentToggle;
		public bool InstrumentToggle {
			get { return instrumentToggle; }
			set {
				instrumentToggle = value;
				this.OrchestraGroup.Text = string.Format("Local orchestra ({0})", value ? "on" : "off");
			}
		}

		private BmpSequencer sequencerRef;
		public BmpSequencer SequencerReference {
			get { return sequencerRef; }
			set {
				sequencerRef = value;
				this.UpdatePerformers();
			}
		}

		private bool orchestraEnabled = false;
		public bool OrchestraEnabled {
			get { return orchestraEnabled; }
			set {
				orchestraEnabled = value;
			}
		}

		public BmpLocalOrchestra() {
			InitializeComponent();

			this.Resize += BmpLocalOrchestra_Resize;

			InstrumentToggle = InstrumentToggle;
		}

		private void BmpLocalOrchestra_Resize(object sender, EventArgs e) {
			foreach(Control ctl in PerformerLayout.Controls) {
				ctl.Width = PerformerLayout.Width - PerformerLayout.Padding.Size.Width;
			}
		}

		public void PopulateLocalProcesses(List<MultiboxProcess> processes) {
			PerformerLayout.Controls.Clear();

			int track = 0;
			foreach(MultiboxProcess mp in processes) {
				BmpLocalPerformer performer = new BmpLocalPerformer();
				performer.SetMultiboxProcess(mp);
				performer.TrackNum = track;
				performer.onUpdate += delegate (object o, EventArgs e) {
					this.Invoke(t => t.UpdatePerformer(performer));
				};
				PerformerLayout.Controls.Add(performer);
				track++;
			}

			BmpLocalOrchestra_Resize(this, EventArgs.Empty);
		}

		public void ProcessOnNote(NoteEvent note) {
			if(!InstrumentToggle) {
				return;
			}
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					if(note.trackNum == performer.TrackNum) {

						int po = SequencerReference.GetTrackPreferredOctaveShift(note.track);
						note.note = NoteHelper.ApplyOctaveShift(note.origNote, performer.OctaveNum + po);
						performer.ProcessOnNote(note);
						break;
					}
				}
			}
		}

		public void ProcessOffNote(NoteEvent note) {
			if(!InstrumentToggle) {
				return;
			}
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					if(note.trackNum == performer.TrackNum) {

						int po = SequencerReference.GetTrackPreferredOctaveShift(note.track);
						note.note = NoteHelper.ApplyOctaveShift(note.origNote, performer.OctaveNum + po);
						performer.ProcessOffNote(note);
						break;
					}
				}
			}
		}

		public void UpdatePerformer(BmpLocalPerformer performer) {
			if(sequencerRef == null) {
				return;
			}
			if(performer != null) {
				performer.Update(sequencerRef);
			}
		}

		public void UpdatePerformers() {
			if(sequencerRef == null) {
				return;
			}
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null) {
					this.UpdatePerformer(performer);
				}
			}
		}

		private void openInstruments_Click(object sender, EventArgs e) {

			InstrumentToggle = true;
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					if(performer.TrackNum >= 0 && performer.TrackNum < sequencerRef.MaxTrack) {
						Track track = sequencerRef.Sequence[performer.TrackNum];
						Instrument ins = sequencerRef.GetTrackPreferredInstrument(track);
						performer.OpenInstrument(ins);
					}
				}
			}
		}

		private void closeInstruments_Click(object sender, EventArgs e) {

			InstrumentToggle = false;
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					performer.CloseInstrument();
				}
			}
		}

		private void muteAll_Click(object sender, EventArgs e) {
			
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null) {
					performer.ToggleMute();
				}
			}
		}
	}
}
