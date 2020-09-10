using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Controls {
	public partial class SpeedShiftComponent : NumericUpDown {
		public SpeedShiftComponent() {
			InitializeComponent();

			this.BackColor = Color.FromArgb(120, 120, 120);
			this.ForeColor = Color.FromArgb(250, 250, 250);

			this.Minimum = 10;
			this.Maximum = 200;
			this.Increment = 5;
			this.Value = 100;

			this.TextAlign = HorizontalAlignment.Center;
		}
		protected override void UpdateEditText() {
            this.ChangingText = true;
			this.Text = this.Value.ToString() + "%";
            this.ChangingText = false;
		}
	}
}
