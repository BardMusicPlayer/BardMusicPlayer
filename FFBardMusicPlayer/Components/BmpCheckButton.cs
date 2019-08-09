using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components {
	public partial class BmpCheckButton : CheckBox {
		public BmpCheckButton() {
			InitializeComponent();
		}

		public BmpCheckButton(IContainer container) {
			container.Add(this);

			InitializeComponent();
		}

		protected override bool ShowFocusCues {
			get { return false; }
		}
	}
}
