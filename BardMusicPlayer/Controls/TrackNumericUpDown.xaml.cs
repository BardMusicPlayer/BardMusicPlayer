using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Functions;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for TrackNumericUpDown.xaml
/// </summary>
public sealed partial class TrackNumericUpDown
{
    public EventHandler<int> OnValueChanged;
    public TrackNumericUpDown()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(TrackNumericUpDown), new PropertyMetadata(OnValueChangedCallBack));

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is TrackNumericUpDown c)
        {
            c.OnValueChangedC(c.Value);
        }
    }

    private void OnValueChangedC(string c)
    {
        NumValue = Convert.ToInt32(c);
    }


    /* Track UP/Down */
    private int _numValue = 1;
    public int NumValue
    {
        get => _numValue;
        set
        {
            _numValue = value;
            Text.Text = "T" + NumValue;
            OnValueChanged?.Invoke(this, _numValue);
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

    private void TrackNumericUpDown_Key(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                NumUp_Click(sender, e);
                break;
            case Key.Down:
                NumDown_Click(sender, e);
                break;
        }
    }
        
    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Text == null)
            return;

        var str = Regex.Replace(Text.Text, "[^0-9]", "");
        if (int.TryParse(str, out var val))
        {
            if (PlaybackFunctions.CurrentSong == null)
                return;

            if (val < 0 || NumValue + 1 > PlaybackFunctions.CurrentSong.TrackContainers.Count)
            {
                NumValue = NumValue;
                return;
            }
            NumValue = val;
        }
    }

}