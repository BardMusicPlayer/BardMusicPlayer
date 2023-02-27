using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.UI_Classic;
using Microsoft.Win32;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for MacroEditWindow.xaml
/// </summary>
public sealed partial class MacroEditWindow
{
    public MacroEditWindow(Macro macro)
    {
        InitializeComponent();
        Macro                 = macro;
        MacroName.Text        = Macro.DisplayedText;
        MacroFileName.Content = Macro.File;
    }

    private Macro Macro { get; }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter      = "Basic file | *.bas",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        if (!openFileDialog.FileName.ToLower().EndsWith(".bas", StringComparison.Ordinal))
            return;

        Macro.File            = openFileDialog.FileName;
        MacroFileName.Content = openFileDialog.FileName;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void MacroName_TextChanged(object sender, TextChangedEventArgs e)
    {
        Macro.DisplayedText = MacroName.Text;
    }
}