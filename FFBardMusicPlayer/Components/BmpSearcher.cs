using System;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components
{
    public partial class SongSearcher : TextBox
    {
        public EventHandler<KeyEventArgs> OnHandledKeyDown;
        public EventHandler<string> OnTextChange;

        // Events

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // here, we can check if the input character in the search bar is either a
            // letter or a number, and invoke the text change
            // since backspace and delete are special, we'll want to listen for those as well
            if (char.IsLetterOrDigit((char) e.KeyCode) ||
                e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Back)
            {
                OnTextChange?.Invoke(this, Text);
            }

            base.OnKeyUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Enter:
                case Keys.Escape:
                case Keys.PageUp:
                case Keys.PageDown:
                    e.Handled          = true;
                    e.SuppressKeyPress = true;

                    OnHandledKeyDown?.Invoke(this, e);
                    return;
                case Keys.Back when e.Control:
                    e.Handled          = true;
                    e.SuppressKeyPress = true;
                    return;
                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            SelectAll();
            //this.BackColor = Color.White;
            TextAlign = HorizontalAlignment.Left;
        }

        protected override void OnLeave(EventArgs e)
        {
            //this.BackColor = (this.Parent != null ? this.Parent.BackColor : Color.Gainsboro);
            TextAlign = HorizontalAlignment.Center;
        }
    }
}