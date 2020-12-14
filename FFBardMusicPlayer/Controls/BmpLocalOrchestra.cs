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

using System.Timers;
using System.Diagnostics;

using Timer = System.Timers.Timer;
using FFMemoryParser;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpLocalOrchestra : UserControl {

		public EventHandler<bool> onMemoryCheck;
		public class SyncData {
			public Dictionary<uint, long> idTimestamp = new Dictionary<uint, long>();
		};

		public BmpSequencer Sequencer {
			set {
				this.PerformersInvoke(t => t.Sequencer = value);
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
				perf.TrackNum = i + 1;
				PerformerPanel.Controls.Add(perf);
			}
		}

		public void PerformersInvoke(Action<BmpLocalPerformer> action) {
			foreach (Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if (performer != null) {
					action.Invoke(performer);
				}
			}
		}

		public void PerformerProgress(int prog) {
			this.PerformersInvoke(t => t.SetProgress(prog));
		}

		public void PerformerPlay(bool play) {
			this.PerformersInvoke(t => t.Play(play));
		}
		public void PerformerStop() {
			this.PerformersInvoke(t => t.Stop());
		}

		public void PerformerUpdate(SigActorsData actors, SigPerfData data) {
			if (actors == null) return;
			Dictionary<string, BmpLocalPerformer> performerNames = this.GetPerformerNames();
			foreach(ActorData ad in actors.currentActors.Values.ToList()) {
				this.PerformersInvoke(t => {
					if(performerNames.ContainsKey(ad.name)) {
						// Needs name cache because they might zone in or out
						uint perfId = ad.perfid / 2;
						if (perfId >= 0 && perfId < 99) {
							if (!data.Performances.ContainsKey(perfId)) return;
							PerformanceData pdata = data.Performances[perfId];
							Console.WriteLine(string.Format("{0}: {1}", ad.name, pdata.IsReady()));
							performerNames[ad.name].PerformanceUp = pdata.IsReady();
						}
					}
				});
			}
		}

		public Dictionary<string, BmpLocalPerformer> GetPerformerNames() {
			Dictionary<string, BmpLocalPerformer> performerNames = new Dictionary<string, BmpLocalPerformer>();
			foreach (BmpLocalPerformer performer in PerformerPanel.Controls) {
				performerNames.Add(performer.PerformerName, performer);
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
			this.PerformersInvoke(t => { if (t.PerformerEnabled) { t.OpenInstrument(); } });
		}

		private void closeInstruments_Click(object sender, EventArgs e) {
			this.PerformersInvoke(t => { if (t.PerformerEnabled) { t.CloseInstrument(); } });
		}

		private void muteAll_Click(object sender, EventArgs e) {
			this.PerformersInvoke(t => { if (t.PerformerEnabled) { t.ToggleMute(); } });
		}

		private void ensembleCheck_Click(object sender, EventArgs e) {

			Timer openTimer = new Timer {
				Interval = 500
			};
			openTimer.Elapsed += delegate (object o, ElapsedEventArgs ev) {
				openTimer.Stop();
				openTimer = null;
				
				this.PerformersInvoke(t => { if (t.PerformerEnabled && !t.hostProcess) { t.EnsembleAccept(); } });
			};
			openTimer.Start();
		}

		private void testC_Click(object sender, EventArgs e) {

			//StartSyncWorker();
			this.PerformersInvoke(t => { if (t.PerformerEnabled) { t.NoteKey("C"); } });
		}
	}
}
