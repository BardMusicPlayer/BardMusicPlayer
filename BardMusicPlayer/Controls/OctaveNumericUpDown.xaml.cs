using System.Windows;
using System.Windows.Controls;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for NumericUpDown.xaml
/// </summary>
public sealed partial class OctaveNumericUpDown
{
    public EventHandler<int>? OnValueChanged;

    public OctaveNumericUpDown()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(OctaveNumericUpDown), new PropertyMetadata(OnValueChangedCallBack));

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is OctaveNumericUpDown c)
        {
            c.OnValueChangedC(c.Value);
        }
    }

    private void OnValueChangedC(string c)
    {
        NumValue = Convert.ToInt32(c);
    }


    /* Track UP/Down */
    private int _numValue;

    private int NumValue
    {
        get => _numValue;
        set
        {
            _numValue = value;
            Text.Text = "ø" + NumValue;
            OnValueChanged?.Invoke(this, _numValue);
        }
    }
    private void NumUp_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue >= 5)
            return;

        NumValue++;
    }

    private void NumDown_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue <= -5)
            return;

        NumValue--;
    }

    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Text == null)
            return;

        if (_numValue is < -5 or > 5)
        {
            Text.Text = @"ø" + 0;
            NumValue  = 0;
        }

        if (int.TryParse(Text.Text.Replace(@"ø", ""), out _numValue))
        {
            Text.Text = @"ø" + _numValue;
            NumValue  = _numValue;
        }
    }
}