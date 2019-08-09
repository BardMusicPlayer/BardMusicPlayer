using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sanford.Multimedia.Midi;

using System.Timers;
using Timer = System.Timers.Timer;

namespace FFBardMusicPlayer {
	public partial class AppMain {

		/*
		AppDebug debug = null;

		public void SetupConductor() {
			Bmp.OnMidiTrackLoad += OnConductorMidiLoad;
			Bmp.OnMidiNote += TriggerConductorNote;

			Orchestra.OnConductorChange += OnConductorChange;
		}

		private long DelayTime {
			get {
				return DateTimeOffset.Now.ToUnixTimeMilliseconds();
			}
		}

		public void OnConductorChange(object o, string name) {
			bool conducting = !(string.IsNullOrEmpty(name));

			if(conducting) {
				if(name.Equals(memory.currentPlayer.CurrentPlayer.Name)) {
					Bmp.Status = BmpStatus.Conducting;
				} else {
					Bmp.Status = BmpStatus.PerformerMulti;
				}
				client.SendConductorPacket(name);

			} else {
				Bmp.Status = BmpStatus.PerformerSolo;
			}

			PerformerSetControl.SetPerformer(null);
		}

		public void OnConductorMidiLoad(object o, Track track) {
			Orchestra.Reset();
		}

		public void DebugPlayLastNote() {
			PerformerControl perf = Orchestra[0];
			if(perf.trackSyncs.Count > 0) {
				int note = perf.trackSyncs.First().note;
				OnOrchestraNote(perf, perf.TrackNum, note);
			}
		}

		public void TriggerConductorNote(object o, NoteEvent e) {
			int trackNum = Bmp.Player.GetTrackNum(e.track);
			OnOrchestraNote(null, trackNum, e.note);

			Timer timer = new Timer(200);
			timer.Elapsed += delegate (object oo, ElapsedEventArgs ee) {
				(oo as Timer).Stop();
				DebugPlayLastNote();
			};
			timer.Start();
		}
		public void TriggerPerformerNote(int id, byte note) {
			if(Orchestra[id] is PerformerControl performer) {
				OnOrchestraNote(performer, performer.TrackNum, note);
			}
		}
		public void OnOrchestraNote(PerformerControl performer, int trackNum, int note) {
			List<PerformerControl> performers;
			bool performing = false;
			if(performer == null) {
				performers = new List<PerformerControl>(Orchestra.PerformerMembers.Values.ToList());
			} else {
				performers = new List<PerformerControl>{ performer };
				performing = true;
			}

			string perfKey = FFXIVKeybindDat.NoteByteToPerformanceKey(note);
			if(string.IsNullOrEmpty(perfKey)) {
				return;
			}
			foreach(PerformerControl perf in performers) {
				if(perf.TrackNum == trackNum) {
					if(perf.trackSyncs.Count > 0) {
						TrackSyncStep step = perf.trackSyncs[0];
						if(step.perfNote == perfKey && (performing != step.performer)) {
							long msDelay = (step.ms - DelayTime);
							if(Math.Abs(msDelay) > 20) {
								// Over 20 ms in delay
								if(Math.Abs(msDelay) > Math.Abs(perf.Delay) || true) {
									perf.Delay = msDelay;
									Console.WriteLine(string.Format("Delay with {0} ({1} - {2})", msDelay, step.ms, DelayTime));
								}
							}
							perf.trackSyncs.RemoveAt(0);
							//goto Update;
						}
					}
					perf.trackSyncs.Add(new TrackSyncStep {
						perfNote = perfKey,
						note = note,
						ms = DelayTime,
						performer = performing,
					});
				}
			}
			//Update:
			//debug.Update(Orch.PerformerMembers.Values.ToList());
		}
		*/
	}
}
