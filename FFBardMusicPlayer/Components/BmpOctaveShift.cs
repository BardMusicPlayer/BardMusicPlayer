using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components {
	public partial class BmpOctaveShift : NumericUpDown {
		public BmpOctaveShift() {
			InitializeComponent();

			this.BackColor = Color.FromArgb(50, 50, 50);
			this.ForeColor = Color.FromArgb(250, 250, 250);

			this.Minimum = -4;
			this.Maximum = 4;
			this.TextAlign = HorizontalAlignment.Center;
		}

		protected override void UpdateEditText() {
            this.ChangingText = true;
			this.Text = string.Format("ø{0:+#;-#;0}", this.Value);
            this.ChangingText = false;
		}
	}
}
