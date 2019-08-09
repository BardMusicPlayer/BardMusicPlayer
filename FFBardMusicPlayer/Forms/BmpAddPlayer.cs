using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sharlayan;
using Sharlayan.Models.ReadResults;
using Sharlayan.Core;

namespace FFBardMusicPlayer {
	public partial class BmpAddPlayer : Form {

		public List<uint> ExcludeActors = new List<uint>();

		public ActorMenuItem MenuItem {
			get {
				return ActorList.SelectedItem as ActorMenuItem;
			}
		}
		public BmpAddPlayer() {
			InitializeComponent();
			if(MemoryHandler.Instance.IsAttached) {
				while(Scanner.Instance.IsScanning) {
					// ...
				}
			}
		}

		private void AppAddPerformer_Load(object sender, EventArgs e) {
			Update();
		}

		public new void Update() {

			string filterName = ActorFilter.Text;

			ActorList.BeginUpdate();
			ActorList.DataSource = null;
			List<ActorMenuItem> menuItems = new List<ActorMenuItem>();
			if(Reader.CanGetActors()) {
				ActorResult res = Reader.GetActors();
				foreach(ActorItem item in res.CurrentPCs.Values) {
					if(!ExcludeActors.Contains(item.ID)) {
						if(!string.IsNullOrEmpty(filterName)) {
							if(!item.Name.ToLower().Contains(filterName.ToLower())) {
								continue;
							}
						}
						menuItems.Add(new ActorMenuItem {
							name = item.Name,
							id = (int) item.ID,
						});
					}
				}
			}
			ActorList.DataSource = menuItems;
			ActorList.EndUpdate();

			ActorList.Refresh();
		}

		private void ActorFilter_TextChanged(object sender, EventArgs e) {
			Update();
		}

		private void ActorList_DoubleClick(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Yes;
		}

		private void ActorFilter_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape) {
				this.DialogResult = DialogResult.No;
			}
			if(e.KeyCode == Keys.Enter) {
				this.DialogResult = DialogResult.Yes;
			}
			if(e.KeyCode == Keys.Up) {
				ActorList.SelectedIndex -= 1;
			}
			if(e.KeyCode == Keys.Down) {
				ActorList.SelectedIndex += 1;
			}
		}
	}

	public class ActorMenuItem {
		public string name;
		public int id;

		public override string ToString() {
			return string.Format("{0}\t{1}", id, name);
		}
	}
}
