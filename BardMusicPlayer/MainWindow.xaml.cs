using BardMusicPlayer.Ui.Classic;
using System.Windows;
using BardMusicPlayer.Pigeonhole;

namespace BardMusicPlayer.Ui
{
    /// <summary>
    /// Interaction Logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClassicStyle();
        }

        public void ClassicStyle()
        {
            this.Title = "BardMusicPlayer BETA Version: 2.X-CUSTOM";
            this.DataContext = new Classic_MainView();
            this.AllowsTransparency = false;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.Height = 665;
            this.Width = 855;
            this.ResizeMode = ResizeMode.CanResizeWithGrip;
        }
    }
}
