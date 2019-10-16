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
	public partial class BmpConductor : UserControl {
		
		
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
			}
		}

		public bool IsConducting {
			get {
				return !(string.IsNullOrEmpty(conductorName));
			}
		}

		public BmpConductor() {
			InitializeComponent();
		}

		public void Update(Dictionary<uint, ActorItemBase> actors = null) {
			if(actors == null) {
				if(Reader.CanGetActors()) {
					ActorResult res = Reader.GetActors();
					actors = res.CurrentPCs.ToDictionary(k => k.Key, k => k.Value as ActorItemBase);
				}
			}
		}

		public bool IsConductorName(string name) {
			if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(conductorName)) {
				return false;
			}
			return conductorName.Equals(name);
		}
		
	}
}
