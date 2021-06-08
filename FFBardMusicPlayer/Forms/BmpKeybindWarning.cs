using System;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Forms
{
    public partial class BmpKeybindWarning : Form
    {
        public BmpKeybindWarning() { InitializeComponent(); }

        private void CloseButton_Click(object sender, EventArgs e) { Close(); }
    }
}