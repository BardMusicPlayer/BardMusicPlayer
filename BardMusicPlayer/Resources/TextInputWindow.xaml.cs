using System.Windows;

namespace BardMusicPlayer.Resources;

/// <summary>
/// Interaction logic for TextInputWindow.xaml
/// </summary>
public partial class TextInputWindow
{
    public TextInputWindow(string infoText, int maxInputLength = 40, string windowTitle = "")
    {
        InitializeComponent();
        InfoText.Text = infoText;
        ResponseTextBox.Focus();
        ResponseTextBox.MaxLength = maxInputLength;
        Title                     = windowTitle;
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