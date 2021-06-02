using System.ComponentModel;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components
{
    public partial class BmpCheckButton : CheckBox
    {
        public BmpCheckButton() { InitializeComponent(); }

        public BmpCheckButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override bool ShowFocusCues => false;
    }
}