using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer {
	public partial class BmpAbout : Form {

		public BmpAbout() {
			InitializeComponent();
		}

		private void DonateLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			Process.Start(Program.urlBase + "donate/");
		}

		private void CloseButton_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void DonationButton_Click(object sender, EventArgs e)
		{
				System.Diagnostics.Process.Start(Program.urlBase + "#donate");
		}
	}
}
