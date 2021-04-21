using System.Windows;

namespace BardMusicPlayer
{
    public partial class Updater : Window
    {
        public Updater()
        {
            InitializeComponent();
            var home = new Home();
            var download = new Download();
            Content = download;
        }
    }
}
