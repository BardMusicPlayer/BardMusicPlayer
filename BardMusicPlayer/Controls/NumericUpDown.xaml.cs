using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for NumericUpDown.xaml
/// </summary>
public sealed partial class NumericUpDown
{
    public EventHandler<int>? OnValueChanged;

    public NumericUpDown()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumericUpDown), new PropertyMetadata(OnValueChangedCallBack));

    public string? Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var c = sender as NumericUpDown;
        c?.OnValueChangedC(c.Value);
    }

    private void OnValueChangedC(string? c)
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
            Text.Text = NumValue.ToString();
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
        if (NumValue <= 0)
            return;

        NumValue--;
    }

    private void TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Text == null)
            return;

        var str = MyRegex().Replace(Text.Text, "");
        if (int.TryParse(str, out var val))
        {
            if (NumValue is <= 0 or >= 5)
                return;

            NumValue = val;
        }
    }

    [GeneratedRegex("[^\\d|\\.\\-]")]
    private static partial Regex MyRegex();
}