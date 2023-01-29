using System.Windows;
using BardMusicPlayer.UI_Classic;

namespace BardMusicPlayer;

/// <summary>
/// Interaction Logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ClassicStyle();
    }

    public void ClassicStyle()
    {
        Title              = "BardMusicPlayer BETA Version: 2.X-CUSTOM";
        DataContext        = new Classic_MainView();
        AllowsTransparency = false;
        WindowStyle        = WindowStyle.SingleBorderWindow;
        Height             = 665;
        Width              = 855;
        ResizeMode         = ResizeMode.CanResizeWithGrip;
    }
}