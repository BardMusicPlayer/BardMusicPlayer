using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace FFBardMusicPlayer.Controls {
	public partial class SongSearcher : TextBox {

		public EventHandler<KeyEventArgs> OnHandledKeyDown;
		public EventHandler<string> OnTextChange;

		// Events
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			OnTextChange?.Invoke(this, this.Text);
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Down ||
				e.KeyCode == Keys.Up ||
				e.KeyCode == Keys.Enter ||
				e.KeyCode == Keys.Escape ||
				e.KeyCode == Keys.PageUp ||
				e.KeyCode == Keys.PageDown) {

				e.Handled = true;
				e.SuppressKeyPress = true;

				OnHandledKeyDown?.Invoke(this, e);
				return;
			}
			if(e.KeyCode == Keys.Back && e.Control) {
				e.Handled = true;
				e.SuppressKeyPress = true;
				return;
			}
			base.OnKeyDown(e);
		}

		protected override void OnEnter(EventArgs e) {
			this.SelectAll();
			//this.BackColor = Color.White;
			this.TextAlign = HorizontalAlignment.Left;
		}
		protected override void OnLeave(EventArgs e) {
			//this.BackColor = (this.Parent != null ? this.Parent.BackColor : Color.Gainsboro);
			this.TextAlign = HorizontalAlignment.Center;
		}
	}
}
