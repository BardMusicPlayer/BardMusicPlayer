using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace BardMusicPlayer.Controls
{
    /// <summary>
    /// Interaktionslogik für NumericUpDown.xaml
    /// </summary>
    public partial class OctaveNumericUpDown : UserControl
    {
        public EventHandler<int> OnValueChanged;

        public OctaveNumericUpDown()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(OctaveNumericUpDown), new PropertyMetadata(OnValueChangedCallBack));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            OctaveNumericUpDown c = sender as OctaveNumericUpDown;
            if (c != null)
            {
                c.OnValueChangedC(c.Value);
            }
        }

        protected virtual void OnValueChangedC(string c)
        {
            NumValue = Convert.ToInt32(c);
        }


        /* Track UP/Down */
        private int _numValue = 0;
        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                this.Text.Text = "ø" + NumValue.ToString();
                OnValueChanged?.Invoke(this, _numValue);
                return;
            }
        }
        private void NumUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void NumDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue--;
        }

        private void OctaveNumericUpDown_Key(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Up:
                    NumUp_Click(sender, e);
                    break;
                case System.Windows.Input.Key.Down:
                    NumDown_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text == null)
                return;

            int val = 0;
            string str = Regex.Replace(Text.Text, @"[^\d|\.\-]", "");
            if (int.TryParse(str, out val))
            {
                NumValue = val;
            }
        }

    }
}
