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
using Sanford.Multimedia.Midi;

using Timer = System.Timers.Timer;
using System.Timers;
using Sharlayan.Core;
using System.Diagnostics;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpLocalOrchestra : UserControl {

		public EventHandler<bool> onMemoryCheck;
		public class SyncData {
			public Dictionary<uint, long> idTimestamp = new Dictionary<uint, long>();
		};

		public BmpSequencer Sequencer {
			set {
				this.UpdatePerformers(value);
				// this.UpdateMemory();
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
		}

		public void PopulateLocalProcesses(List<MultiboxProcess> processes) {
			PerformerPanel.Controls.Clear();

			List<BmpLocalPerformer> performers = new List<BmpLocalPerformer>();
			int track = 0;
			foreach(MultiboxProcess mp in processes) {
				BmpLocalPerformer perf = new BmpLocalPerformer(mp);
				perf.Dock = DockStyle.Top;

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
				PerformerPanel.Controls.Add(perf);
			}
		}

        public void UpdateMemory(List<ActorItem> actors, List<PerformanceItem> perfs)
        {
            List<string> performerNames = GetPerformerNames();
            foreach (ActorItem actor in actors)
            {
                if (performerNames.Contains(actor.Name))
                {
                    uint perfId = actor.PerformanceID / 2;
                    if (perfId >= 0 && perfId < 99 && perfId < perfs.Count)
                    {
                        BmpLocalPerformer perf = this.FindPerformer(actor.Name);
                        if (perf != null)
                        {
                            PerformanceItem item = perfs[(int)perfId];
                            perf.PerformanceUp = item.IsReady();
                            perf.performanceId = perfId;
                            perf.actorId = actor.ID;
                        }
                    }
                }
            }
        }

        public void UpdatePerformers(BmpSequencer seq) {
			if(seq == null) {
				return;
			}
			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null) {
					performer.Sequencer = seq;
				}
			}
		}

		public void PerformerProgress(int prog) {
			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null) {
					performer.SetProgress(prog);
				}
			}
		}

		public void PerformerPlay(bool play) {
			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null) {
					performer.Play(play);
				}
			}
		}
		public void PerformerStop() {
			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null) {
					performer.Stop();
				}
			}
		}

		public List<string> GetPerformerNames() {
			List<string> performerNames = new List<string>();
			foreach(BmpLocalPerformer performer in PerformerPanel.Controls) {
				performerNames.Add(performer.PerformerName);
			}
			return performerNames;
		}

		public BmpLocalPerformer FindPerformer(string name) {
			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerName == name) {
					return performer;
				}
			}
			return null;
		}

		private void openInstruments_Click(object sender, EventArgs e) {
			
			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					performer.OpenInstrument();
				}
			}
		}

		private void closeInstruments_Click(object sender, EventArgs e) {
			
			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					performer.CloseInstrument();
				}
			}
		}

		private void muteAll_Click(object sender, EventArgs e) {
			
			foreach(Control ctl in PerformerPanel.Controls) {
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
				
				foreach(Control ctl in PerformerPanel.Controls) {
					BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
					if(performer != null && performer.PerformerEnabled && !performer.hostProcess) {
						performer.EnsembleAccept();
					}
				}
			};
			openTimer.Start();
		}

		private void testC_Click(object sender, EventArgs e) {

			//StartSyncWorker();

			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled) {
					performer.NoteKey("C");
				}
			}
		}
	}
}
