using System.Drawing;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components
{
    public partial class BmpOctaveShift : NumericUpDown
    {
        public BmpOctaveShift()
        {
            InitializeComponent();

            BackColor = Color.FromArgb(50, 50, 50);
            ForeColor = Color.FromArgb(250, 250, 250);

            Minimum   = -4;
            Maximum   = 4;
            TextAlign = HorizontalAlignment.Center;
        }

        protected override void UpdateEditText()
        {
            ChangingText = true;
            Text         = $"ø{Value:+#;-#;0}";
            ChangingText = false;
        }
    }
}