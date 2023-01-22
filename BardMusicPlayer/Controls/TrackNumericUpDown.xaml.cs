using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Functions;

namespace BardMusicPlayer.Controls
{
    /// <summary>
    /// Interaktionslogik für TrackNumericUpDown.xaml
    /// </summary>
    public partial class TrackNumericUpDown : UserControl
    {
        public EventHandler<int> OnValueChanged;
        public TrackNumericUpDown()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(TrackNumericUpDown), new PropertyMetadata(OnValueChangedCallBack));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TrackNumericUpDown c = sender as TrackNumericUpDown;
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
        private int _numValue = 1;
        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                this.Text.Text = "T" + NumValue.ToString();
                OnValueChanged?.Invoke(this, _numValue);
                return;
            }
        }

        private void NumUp_Click(object sender, RoutedEventArgs e)
        {
            if (PlaybackFunctions.CurrentSong == null)
                return;
            if (NumValue + 1 > PlaybackFunctions.CurrentSong.TrackContainers.Count)
                return;
            NumValue++;
        }

        private void NumDown_Click(object sender, RoutedEventArgs e)
        {
            if (NumValue - 1 < 0)
                return;
            NumValue--;
        }

        private void TrackNumericUpDown_Key(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Up:
                    NumUp_Click(sender, e);
                    break;
                case System.Windows.Input.Key.Down:
                    NumDown_Click(sender, e);
                    break;
            }
        }
        
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text == null)
                return;

            int val = 0;
            string str = Regex.Replace(Text.Text, "[^0-9]", "");
            if (int.TryParse(str, out val))
            {
                if (PlaybackFunctions.CurrentSong == null)
                    return;

                if ((val < 0) || (NumValue + 1 > PlaybackFunctions.CurrentSong.TrackContainers.Count))
                {
                    NumValue = NumValue;
                    return;
                }
                NumValue = val;
            }
        }

    }
}
