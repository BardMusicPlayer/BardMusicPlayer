using System.Windows;

namespace BardMusicPlayer.Resources;

/// <summary>
/// Interaction logic for TextInputWindow.xaml
/// </summary>
public partial class TextInputWindow
{
    public TextInputWindow(string infotext, int maxinputlength = 40)
    {
        InitializeComponent();
        InfoText.Text = infotext;
        ResponseTextBox.Focus();
        ResponseTextBox.MaxLength = maxinputlength;
    }

    public string ResponseText
    {
        get => ResponseTextBox.Text;
        set => ResponseTextBox.Text = value;
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

}