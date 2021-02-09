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
			// we can't do this here. loading a song will change the text, and call
			// this twice. see: OnKeyPress
			// OnTextChange?.Invoke(this, this.Text);
		}

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

			// here, we can check if the input character in the search bar is either a
			// letter or a number, and invoke the text change
			if (char.IsLetterOrDigit(e.KeyChar))
            {
				OnTextChange?.Invoke(this, this.Text);
			}
			// since backspace and delete are special, we'll want to listen for those as well
			else if (e.KeyChar == (char)Keys.Delete || e.KeyChar == (char)Keys.Back)
            {
				OnTextChange?.Invoke(this, this.Text);
			}
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
