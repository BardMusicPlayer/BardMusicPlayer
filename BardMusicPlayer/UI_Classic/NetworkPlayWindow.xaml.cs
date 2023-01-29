using System.ComponentModel;
using System.Windows;

namespace BardMusicPlayer.UI_Classic
{
    /// <summary>
    /// Interaction logic for NetworkPlayWindow.xaml
    /// </summary>
    public partial class NetworkPlayWindow
    {
        public NetworkPlayWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

    }
}
