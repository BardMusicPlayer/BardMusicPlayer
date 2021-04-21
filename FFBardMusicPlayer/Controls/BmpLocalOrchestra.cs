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
using Sharlayan.Models.ReadResults;
using Sharlayan.Core;
using System.Diagnostics;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpLocalOrchestra : UserControl {

		public EventHandler<bool> onMemoryCheck;
		public class SyncData {
			public Dictionary<uint, long> idTimestamp = new Dictionary<uint, long>();
		};

        private BmpSequencer parentSequencer;
        public BmpSequencer Sequencer
        {
            set
            {
                parentSequencer = value;

                this.UpdatePerformers(value);
                this.UpdateMemory();
            }

            get
            {
                return parentSequencer;
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

		private void StartSyncWorker() {
			BackgroundWorker syncWorker = new BackgroundWorker();
			syncWorker.WorkerSupportsCancellation = true;
			syncWorker.DoWork += SyncWorker_DoWork;
			syncWorker.RunWorkerCompleted += SyncWorker_RunWorkerCompleted;

			this.UpdateMemory();

			List<uint> actorIds = new List<uint>();
			foreach(Control ctl in PerformerPanel.Controls) {
				BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
				if(performer != null && performer.PerformerEnabled && performer.PerformanceUp) {
					actorIds.Add(performer.actorId);
				}
			}
			syncWorker.RunWorkerAsync(actorIds);

			onMemoryCheck.Invoke(this, true);
		}

		private void SyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			SyncData data = (e.Result as SyncData);

			StringBuilder debugDump = new StringBuilder();
			
			if(Sharlayan.MemoryHandler.Instance.IsAttached) {
				if(Sharlayan.Reader.CanGetActors()) {
					ActorResult actors = Sharlayan.Reader.GetActors();
					foreach(KeyValuePair<uint, long> kvp in data.idTimestamp) {
						if(actors.CurrentPCs.ContainsKey(kvp.Key)) {
							ActorItem item = actors.CurrentPCs[kvp.Key];
							debugDump.AppendLine(string.Format("{0} MS {1}", item.Name, kvp.Value));
						}
					}
				}
			}

			onMemoryCheck.Invoke(this, false);

			//MessageBox.Show(this.Parent, debugDump.ToString());
			Console.WriteLine(debugDump.ToString());
		}

		// Start memory poll sync

		private void SyncWorker_DoWork(object sender, DoWorkEventArgs e) {
			BackgroundWorker worker = (sender as BackgroundWorker);
			List<uint> actorIds = (e.Argument as List<uint>);
			SyncData data = new SyncData();
			Dictionary<uint, uint> performanceToActor = new Dictionary<uint, uint>();

			if(Sharlayan.MemoryHandler.Instance.IsAttached) {
				if(Sharlayan.Reader.CanGetPartyMembers() && Sharlayan.Reader.CanGetActors()) {
					ActorResult actors = Sharlayan.Reader.GetActors();
					foreach(ActorItem actor in actors.CurrentPCs.Values.ToList()) {
						if(actorIds.Contains(actor.ID)) {
							performanceToActor[actor.PerformanceID / 2] = actor.ID;
						}
					}
				}
			}

			if(e.Cancel) {
				return;
			}

			PerformanceResult performanceCache = null;
			List<uint> perfKeys = performanceToActor.Keys.ToList();

			Stopwatch msCounter = Stopwatch.StartNew();
			DateTime now = DateTime.Now;

			while(!worker.CancellationPending) {
				if(e.Cancel) {
					return;
				}
				if(Sharlayan.MemoryHandler.Instance.IsAttached) {
					if(Sharlayan.Reader.CanGetPerformance()) {
						performanceCache = Sharlayan.Reader.GetPerformance();

						foreach(uint pid in perfKeys) {
							if(performanceToActor.ContainsKey(pid)) {
								// Check it
								if(performanceCache.Performances[pid].Animation > 0) {
									uint aid = performanceToActor[pid];
									//data.idTimestamp[aid] = msCounter.ElapsedMilliseconds;
									data.idTimestamp[aid] = (long)((DateTime.Now - now).TotalMilliseconds);
									performanceToActor.Remove(pid);
								}
							}
						}
						if(perfKeys.Count != performanceToActor.Keys.Count) {
							perfKeys = performanceToActor.Keys.ToList();
						}
						if(performanceToActor.Keys.Count == 0) {
							break;
						}
					}
				}
			}

			e.Result = data;
		}

		public void PopulateLocalProcesses(List<MultiboxProcess> processes) {
			PerformerPanel.Controls.Clear();

			List<BmpLocalPerformer> performers = new List<BmpLocalPerformer>();
			int track = 1;
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
				perf.TrackNum = i+1;
				PerformerPanel.Controls.Add(perf);
			}
		}

		public void UpdateMemory() {
			if(Sharlayan.MemoryHandler.Instance.IsAttached) {
				if(Sharlayan.Reader.CanGetActors() && Sharlayan.Reader.CanGetPerformance()) {

					List<string> performerNames = GetPerformerNames();

					int pid = -1;
					PerformanceResult perfs = Sharlayan.Reader.GetPerformance();
					ActorResult ares = Sharlayan.Reader.GetActors();
					if(ares != null) {
						foreach(ActorItem actor in ares.CurrentPCs.Values.ToList()) {
							if(performerNames.Contains(actor.Name)) {
								
								uint perfId = actor.PerformanceID / 2;
								if(perfId >= 0 && perfId < 99 && perfs.Performances.ContainsKey(perfId)) {
									PerformanceItem item = perfs.Performances[perfId];

									BmpLocalPerformer perf = this.FindPerformer(actor.Name, perfId, actor.ID);
									if(perf != null) {
										perf.PerformanceUp = item.IsReady();
										perf.performanceId = perfId;
										perf.actorId = actor.ID;
									}
								}
							}
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

        public BmpLocalPerformer FindPerformer(string name, uint performerId, uint actorId)
        {
            foreach (Control ctl in PerformerPanel.Controls)
            {
                BmpLocalPerformer performer = (ctl as BmpLocalPerformer);
                if (performer != null)
                {
                    // few things can happen here
                    if (performer.PerformerName == name)
                    {
                        if (performer.performanceId == 0 && performer.actorId == 0)
                        {
                            // here, we've found the performer with the name for the first time
                            // after returning here, the perfId and actorId should be set
                            // so, just return them, i guess
                            return performer;
                        }
                        else if (performer.performanceId == performerId && performer.actorId == actorId)
                        {
                            // we've seen this performer before, and can damn well make sure that
                            // this /is/ the character we want to return
                            return performer;
                        }
                    }
                }
            }

            // all else fails, blame the one person in BMP that decided to give
            // muiltiple characters the exact same name.
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
            parentSequencer.Pause();
            foreach (Control ctl in PerformerPanel.Controls) {
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
