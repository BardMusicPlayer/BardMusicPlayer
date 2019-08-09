using FFBardMusicPlayer.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FFBardMusicPlayer.Controls.BmpPlayerInterface;

namespace FFBardMusicPlayer {
	public partial class AppMain {

		// System
		/*
		private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
			Visible = !Visible;
			BringFront();
		}

		private void Orch_Click(object sender, PerformerControl ctl) {
			PerformerSetControl setctl = PerformerSetControl;
			if(!(setctl is PerformerSetControl)) {
				return;
			}
			setctl.SetPerformer(ctl);
		}
		

		private void Player_OnStatusChange(object sender, BmpStatus status) {
			bool solo = (status == BmpStatus.PerformerSolo);
			Playlist.Invoke(t => t.Visible = solo);
			Orchestra.Invoke(t => t.Visible = !solo);
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			if(!Bmp.SelectorSongFocus) {
				if(e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter) {
					e.SuppressKeyPress = true;
					Bmp.SelectorSongFocus = true;
				}
			}
			base.OnKeyDown(e);
		}
		*/
	}
}
