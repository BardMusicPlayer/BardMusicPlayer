using System.Windows;
using System.Windows.Media;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.UI_Classic;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace BardMusicPlayer;

/// <summary>
/// Interaction Logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        Title              = "BardMusicPlayer BETA Version: 2.X-CUSTOM";
        DataContext        = new Classic_MainView();
        AllowsTransparency = false;
        WindowStyle        = WindowStyle.SingleBorderWindow;
        Height             = 738;
        Width              = 910;
        ResizeMode         = ResizeMode.CanResizeWithGrip;
        
        if (!BmpPigeonhole.Instance.DarkStyle)
            LightModeStyle();
        else
            DarkModeStyle();
    }

    private static void LightModeStyle()
    {
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(Theme.Light);
        theme.SetPrimaryColor(Colors.LightSlateGray);
        theme.SetSecondaryColor(Colors.Green);
        //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);
        paletteHelper.SetTheme(theme);
    }

    private static void DarkModeStyle()
    {
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(Theme.Dark);
        theme.SetPrimaryColor(Colors.DarkOrange);
        theme.SetSecondaryColor(Colors.Blue);
        //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);
        paletteHelper.SetTheme(theme);
    }
}