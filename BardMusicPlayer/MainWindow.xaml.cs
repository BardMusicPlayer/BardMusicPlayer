using System.ComponentModel;
using System.Reflection;
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
        // Set properties before initializing components
        AllowsTransparency = false;
        WindowStyle        = WindowStyle.SingleBorderWindow;
        Height             = 620;
        Width              = 935;
        ResizeMode         = ResizeMode.CanResizeWithGrip;

        InitializeComponent();

        Title       = "BardMusicPlayer - " + Assembly.GetExecutingAssembly().GetName().Version;
        DataContext = new ClassicMainView();

        if (!BmpPigeonhole.Instance.DarkStyle)
            LightModeStyle();
        else
            DarkModeStyle();

        if (!BmpPigeonhole.Instance.KeepOnTop)
            KeepOnTop_Disabled();
        else
            KeepOnTop_Enabled();
    }

    public static void LightModeStyle()
    {
        const BaseTheme baseTheme = BaseTheme.Light;

        const PrimaryColor primary = PrimaryColor.BlueGrey;
        var primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];

        const SecondaryColor secondary = SecondaryColor.Teal;
        var secondaryColor = SwatchHelper.Lookup[(MaterialDesignColor)secondary];

        var theme = Theme.Create(baseTheme, primaryColor, secondaryColor);
        var paletteHelper = new PaletteHelper();
        theme.Background = Color.FromRgb(250, 250, 250);
        paletteHelper.SetTheme(theme);
    }

    public static void DarkModeStyle()
    {
        const BaseTheme baseTheme = BaseTheme.Dark;

        const PrimaryColor primary = PrimaryColor.Grey;
        var primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];

        const SecondaryColor secondary = SecondaryColor.Teal;
        var secondaryColor = SwatchHelper.Lookup[(MaterialDesignColor)secondary];

        var theme = Theme.Create(baseTheme, primaryColor, secondaryColor);
        var paletteHelper = new PaletteHelper();
        theme.Background = Color.FromRgb(30, 30, 30);
        paletteHelper.SetTheme(theme);
    }

    /// <summary>
    /// Helper to bring the player to front
    /// </summary>
    public static void KeepOnTop_Enabled()
    {
        try
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow is { IsVisible: false })
            {
                mainWindow.Show();
            }

            if (mainWindow is { WindowState: WindowState.Minimized })
            {
                mainWindow.WindowState = WindowState.Normal;
            }

            if (mainWindow != null)
            {
                mainWindow.Activate();
                mainWindow.Topmost = true;
                mainWindow.Focus();
            }
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Helper to bring the player to front
    /// </summary>
    public static void KeepOnTop_Disabled()
    {
        try
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow is { IsVisible: false })
            {
                mainWindow.Show();
            }

            if (mainWindow is { WindowState: WindowState.Minimized })
            {
                mainWindow.WindowState = WindowState.Normal;
            }

            if (mainWindow != null)
            {
                mainWindow.Activate();
                mainWindow.Topmost = false;
                mainWindow.Focus();
            }
        }
        catch
        {
            // ignored
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        for (var intCounter = Application.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
        {
            try { Application.Current.Windows[intCounter]?.Close(); }
            catch
            {
                // ignored
            }
        }
        base.OnClosing(e);
    }
}