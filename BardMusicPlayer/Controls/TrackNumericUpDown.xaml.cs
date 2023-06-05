using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Pigeonhole;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for TrackNumericUpDown.xaml
/// </summary>
public sealed partial class TrackNumericUpDown
{
    public EventHandler<int>? OnValueChanged;
    public TrackNumericUpDown()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(TrackNumericUpDown), new PropertyMetadata(OnValueChangedCallBack));

    public static readonly DependencyProperty MaxTracksProperty =
        DependencyProperty.Register(nameof(MaxTracks), typeof(int), typeof(TrackNumericUpDown), new PropertyMetadata(OnValueChangedCallBack));

    public int MaxTracks
    {
        get => (int)GetValue(MaxTracksProperty);
        set => SetValue(MaxTracksProperty, value);
    }

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

    private int NumValue
    {
        get => _numValue;
        set
        {
            _numValue = value;
            Text.Text = "t" + NumValue;
            OnValueChanged?.Invoke(this, _numValue);
        }
    }

    private void NumUp_Click(object sender, RoutedEventArgs e)
    {
        if (PlaybackFunctions.CurrentSong == null)
            return;

        if (NumValue >= MaxTracks)
            return;

        NumValue++;
    }

    private void NumDown_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue <= 1)
            return;

        NumValue--;
    }

    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Text == null)
            return;

        if (_numValue <= 0 || _numValue > MaxTracks)
        {
            if (BmpPigeonhole.Instance.EnsembleKeepTrackSetting)
                return;

            if (!BmpPigeonhole.Instance.PlayAllTracks)
            {
                Text.Text = "t" + 0;
                NumValue  = 0;
                return;
            }

            Text.Text = "t" + 1;
            NumValue  = 1;
        }

        if (PlaybackFunctions.CurrentSong == null)
            return;
        
        if (int.TryParse(Text.Text.Replace("t", ""), out _numValue))
        {
            Text.Text = "t" + _numValue;
            NumValue  = _numValue;
        }
    }
}