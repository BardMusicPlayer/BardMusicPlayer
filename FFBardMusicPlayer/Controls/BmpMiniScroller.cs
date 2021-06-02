using System;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpMiniScroller : UserControl
    {
        public new EventHandler<int> OnScroll;
        public EventHandler OnStatusClick;

        private delegate void TextDelegate(string text);

        private void SetText(string text)
        {
            if (InvokeRequired)
            {
                var d = new TextDelegate(SetText);
                Invoke(d, text);
            }
            else
            {
                Status.Text = text;
            }
        }

        public override string Text
        {
            get => Status.Text;
            set => SetText(value);
        }

        public BmpMiniScroller()
        {
            InitializeComponent();

            LeftButton.Click  += LeftRightButton_Click;
            RightButton.Click += LeftRightButton_Click;
            Status.Click      += Status_Click;
        }

        private void LeftRightButton_Click(object sender, EventArgs e)
        {
            var scroll = 50;
            switch (ModifierKeys)
            {
                case Keys.Shift:   scroll = 100; break;
                case Keys.Control: scroll = 10; break;
            }

            scroll *= sender as Button == LeftButton ? -1 : 1;
            OnScroll?.Invoke(this, scroll);

            Status_Click(sender, e);
        }

        private void Status_Click(object sender, EventArgs e) { OnStatusClick?.Invoke(this, e); }
    }
}