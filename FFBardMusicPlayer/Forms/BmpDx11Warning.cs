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
	public partial class BmpDx11Warning : Form {
		public BmpDx11Warning() {
			InitializeComponent();
		}

		private void CloseButton_Click(object sender, EventArgs e) {
			this.Close();
		}
	}
}
