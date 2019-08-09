using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sharlayan.Core;
using Sharlayan;
using Sharlayan.Models.ReadResults;
using FFBardMusicCommon;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpOrchestra : UserControl {

		public EventHandler<PerformerControl> OnPerformerAdded;
		public EventHandler<PerformerControl> OnPerformerClick;

		public class PerformerList : Dictionary<int, PerformerControl> { };
		private PerformerList performerMembers = new PerformerList();
		public PerformerList PerformerMembers {
			get {
				return performerMembers;
			}
		}

		public PerformerControl this[int id] {
			get {
				if(!performerMembers.ContainsKey(id)) {
					return null;
				}
				return performerMembers[id];
			}
			set {
				performerMembers[id] = value;
			}
		}

		private uint conductorId = 0;
		private string conductorName = string.Empty;
		public string ConductorName {
			get { return conductorName; }
			set {
				conductorName = value;
				conductorId = 0;

				if(!string.IsNullOrEmpty(conductorName)) {
					if(Reader.CanGetActors()) {
						if(Reader.GetActors() is ActorResult res) {
							foreach(KeyValuePair<uint, ActorItem> item in res.CurrentPCs) {
								if(item.Value == null)
									continue;
								if(item.Value.Name == conductorName) {
									conductorId = item.Key;
								}
							}
						}
					}
				}
				ConductorNameLabel.Text = conductorName;
				OrchestraTable.Visible = !string.IsNullOrEmpty(conductorName);
			}
		}

		public bool IsConducting {
			get {
				return !(string.IsNullOrEmpty(conductorName));
			}
		}

		public BmpOrchestra() {
			InitializeComponent();

			this.UpdateVisibility();
		}

		public void Reset() {
			foreach(PerformerControl perf in performerMembers.Values) {
				perf.trackSyncs.Clear();
			}
		}

		public void Update(Dictionary<uint, ActorItemBase> actors = null) {
			if(actors == null) {
				if(Reader.CanGetActors()) {
					ActorResult res = Reader.GetActors();
					actors = res.CurrentPCs.ToDictionary(k => k.Key, k => k.Value as ActorItemBase);
				}
			}
			bool conductorExists = false;
			if(actors != null) {
				foreach(int i in actors.Keys) {
					ActorItemBase actor = actors[(uint) i];
					if(performerMembers.TryGetValue(i, out PerformerControl perf)) {
						perf.Update(actor);
					}
					if(IsConducting) {
						if(actor.Name == conductorName) {
							conductorExists = true;
						}
					}
				}
			}
			if(IsConducting) {
				if(!conductorExists) {
					// If conductor is not present in the current world anymore,
					// remove them
					this.ConductorName = string.Empty;
				}
			}

			this.UpdateVisibility();
		}

		public void UpdateVisibility() {
			bool gameReady = false;
			if(Reader.CanGetPlayerInfo()) {
				if(Reader.GetCurrentPlayer().CurrentPlayer.Job != Sharlayan.Core.Enums.Actor.Job.Unknown) {
					gameReady = true;
				}
			}
		}

		public bool IsConductorName(string name) {
			if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(conductorName)) {
				return false;
			}
			return conductorName.Equals(name);
		}

		public void Add(PerformerControl ctl) {
			ctl.OnPerformerClick += OnPerformerClick;
			ctl.OnSpeedShiftChange += OnPerformerSpeedShiftChange;
			ctl.OnOctaveShiftChange += OnPerformerOctaveShiftChange;
			ctl.OnTrackNumChange += OnPerformerTrackNumChange;

			performerMembers[ctl.Id] = ctl;

			ctl.Dock = DockStyle.Top;
			PartyPanel.Controls.Add(ctl);

			OnPerformerAdded?.Invoke(this, ctl);

			this.Update(null);
		}

		public EventHandler<BmpPerformerSettingData> OnPerformerDataChange;

		private void OnPerformerSpeedShiftChange(Object o, float speed) {
			Console.WriteLine(string.Format("Performer {0} changed ss to {1}", (o as PerformerControl).CharName, speed));
			OnPerformerDataChange?.Invoke(this, BmpSocketClientHelper.PerformerToSettingData(o as PerformerControl));

		}
		private void OnPerformerOctaveShiftChange(Object o, int octave) {
			Console.WriteLine(string.Format("Performer {0} changed os to {1}", (o as PerformerControl).CharName, octave));
			OnPerformerDataChange?.Invoke(this, BmpSocketClientHelper.PerformerToSettingData(o as PerformerControl));
		}
		private void OnPerformerTrackNumChange(Object o, int track) {
			Console.WriteLine(string.Format("Performer {0} changed track to {1}", (o as PerformerControl).CharName, track));
			OnPerformerDataChange?.Invoke(this, BmpSocketClientHelper.PerformerToSettingData(o as PerformerControl));
		}

		public void ShowAddPerformerDialog() {
			using(BmpAddPlayer addPerformer = new BmpAddPlayer()) {

				addPerformer.ExcludeActors = performerMembers.Keys.Select(t => (uint) t).ToList();
				addPerformer.ExcludeActors.Add(conductorId);

				DialogResult res = addPerformer.ShowDialog(this);
				if(res == DialogResult.Yes) {
					if(addPerformer.MenuItem is ActorMenuItem item) {
						this.Add(new PerformerControl {
							Id = item.id,
							CharName = item.name,
						});
					}
				}
			}
		}
	}
}
