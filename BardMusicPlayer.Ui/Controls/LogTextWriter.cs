using System;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace BardMusicPlayer.Ui.Controls
{
    public class LogTextWriter : TextWriter
    {
        private readonly TextBox _textBox;

        public LogTextWriter(TextBox output) { _textBox = output; }

        public override void Write(string value)
        {
            _textBox.Dispatcher.BeginInvoke(new Action(() => { _textBox.AppendText(value + Environment.NewLine); }));
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}