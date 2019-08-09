using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer {
	public partial class BmpConfirmConductor : Form {

		public EventHandler<string> OnConfirmConductor;

		string conductorName = string.Empty;
		public string ConductorName {
			set {
				conductorName = value;
				ConductorNameLabel.Invoke(t => t.Text = conductorName);
			}
		}

		public BmpConfirmConductor() {
			InitializeComponent();
		}

		private void YesButton_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Yes;
			OnConfirmConductor?.Invoke(this, conductorName);
			this.Close();
		}
		private void NoButton_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.No;
			OnConfirmConductor?.Invoke(this, conductorName);
			this.Close();
		}
	}
}
