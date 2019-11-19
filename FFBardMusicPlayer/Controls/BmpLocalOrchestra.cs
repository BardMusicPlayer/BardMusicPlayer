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

using Timer = System.Timers.Timer;
using System.Timers;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpLocalOrchestra : UserControl {

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
		}

		private void BmpLocalOrchestra_Resize(object sender, EventArgs e) {
			foreach(Control ctl in PerformerLayout.Controls) {
				ctl.Width = PerformerLayout.Width - PerformerLayout.Padding.Size.Width;
			}
		}

		public void PopulateLocalProcesses(List<MultiboxProcess> processes) {
			PerformerLayout.Controls.Clear();

			List<BmpLocalPerformer> performers = new List<BmpLocalPerformer>();
			int track = 0;
			foreach(MultiboxProcess mp in processes) {
				BmpLocalPerformer perf = new BmpLocalPerformer(mp);
				perf.onUpdate += delegate (object o, EventArgs e) {
					this.Invoke(t => t.UpdatePerformer(perf));
				};

				if(mp.hostProcess == true) {
					perf.hostProcess = true;
					performers.Insert(0, perf);
				} else {
					performers.Add(perf);
				}
				track++;
			}
			for(int i = 0; i < performers.Count; i++) {
				BmpLocalPerformer perf = performers[i];
				perf.TrackNum = i;
				PerformerLayout.Controls.Add(perf);
			}

			BmpLocalOrchestra_Resize(this, EventArgs.Empty);
		}

		public void ProcessOnNote(NoteEvent note) {
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.UiEnabled && performer.PerformerEnabled) {
					if(note.trackNum == performer.TrackNum) {

						int po = SequencerReference.GetTrackPreferredOctaveShift(note.track);
						note.note = NoteHelper.ApplyOctaveShift(note.origNote, performer.OctaveNum + po);
						performer.ProcessOnNote(note);
					}
				}
			}
		}

		public void ProcessOffNote(NoteEvent note) {
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.UiEnabled && performer.PerformerEnabled) {
					if(note.trackNum == performer.TrackNum) {

						int po = SequencerReference.GetTrackPreferredOctaveShift(note.track);
						note.note = NoteHelper.ApplyOctaveShift(note.origNote, performer.OctaveNum + po);
						performer.ProcessOffNote(note);
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

		public List<string> GetPerformerNames() {
			List<string> performerNames = new List<string>();
			foreach(BmpLocalPerformer performer in PerformerLayout.Controls) {
				performerNames.Add(performer.PerformerName);
			}
			return performerNames;
		}

		public BmpLocalPerformer FindPerformer(string name) {
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerName == name) {
					return performer;
				}
			}
			return null;
		}

		private void openInstruments_Click(object sender, EventArgs e) {
			
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					performer.OpenInstrument();
				}
			}
		}

		private void closeInstruments_Click(object sender, EventArgs e) {
			
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
				if(performer != null && performer.PerformerEnabled) {
					performer.ToggleMute();
				}
			}
		}

		private void ensembleCheck_Click(object sender, EventArgs e) {

			Timer openTimer = new Timer {
				Interval = 500
			};
			openTimer.Elapsed += delegate (object o, ElapsedEventArgs ev) {
				openTimer.Stop();
				openTimer = null;
				
				foreach(Control ctl in PerformerLayout.Controls) {
					BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
					if(performer != null && performer.PerformerEnabled && !performer.hostProcess) {
						performer.EnsembleAccept();
					}
				}
			};
			openTimer.Start();
		}

		private void testC_Click(object sender, EventArgs e) {
			foreach(Control ctl in PerformerLayout.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					performer.NoteKey("C");
				}
			}
		}
	}
}
