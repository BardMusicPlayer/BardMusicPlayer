using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpMiniScroller : UserControl {

		public EventHandler<int> OnScroll;
		public EventHandler OnStatusClick;

		public override string Text {
			get { return Status.Text; }
			set {
				Status.Text = value;
			}
		}

		public BmpMiniScroller() {
			InitializeComponent();

			LeftButton.Click += LeftRightButton_Click;
			RightButton.Click += LeftRightButton_Click;
			Status.Click += Status_Click;
		}

		private void LeftRightButton_Click(object sender, EventArgs e) {
			int scroll = 50;
			if(Control.ModifierKeys == Keys.Shift) {
				scroll = 100;
			} else if(Control.ModifierKeys == Keys.Control) {
				scroll = 10;
			}
			scroll *= ((sender as Button) == LeftButton ? -1 : 1);
			OnScroll?.Invoke(this, scroll);

			this.Status_Click(sender, e);
		}

		private void Status_Click(object sender, EventArgs e) {
			OnStatusClick?.Invoke(this, e);
		}
	}
}
