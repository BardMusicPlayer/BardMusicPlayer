namespace BardMusicPlayer
{
    public partial class Updater
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
