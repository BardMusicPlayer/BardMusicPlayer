using System.Windows;
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
        DataContext        = new ClassicMainView();
        AllowsTransparency = false;
        WindowStyle        = WindowStyle.SingleBorderWindow;
        Height             = 738;
        Width              = 930;
        ResizeMode         = ResizeMode.CanResizeWithGrip;
        
        if (!BmpPigeonhole.Instance.DarkStyle)
            LightModeStyle();
        else
            DarkModeStyle();
    }

    public static void LightModeStyle()
    {
        var baseTheme = Theme.Light;
        
        const PrimaryColor primary = PrimaryColor.BlueGrey;
        var primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];

        const SecondaryColor secondary = SecondaryColor.Teal;
        var secondaryColor = SwatchHelper.Lookup[(MaterialDesignColor)secondary];

        var theme = Theme.Create(baseTheme, primaryColor, secondaryColor);
        var paletteHelper = new PaletteHelper();
        paletteHelper.SetTheme(theme);
    }

    public static void DarkModeStyle()
    {
        var baseTheme = Theme.Dark;
        
        const PrimaryColor primary = PrimaryColor.Grey;
        var primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];

        const SecondaryColor secondary = SecondaryColor.Teal;
        var secondaryColor = SwatchHelper.Lookup[(MaterialDesignColor)secondary];

        var theme = Theme.Create(baseTheme, primaryColor, secondaryColor);
        var paletteHelper = new PaletteHelper();
        paletteHelper.SetTheme(theme);
    }
}