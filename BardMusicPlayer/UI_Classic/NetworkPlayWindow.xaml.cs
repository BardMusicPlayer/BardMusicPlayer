using System.ComponentModel;
using System.Windows;

namespace BardMusicPlayer.UI_Classic
{
    /// <summary>
    /// Interaktionslogik für NetworkPlayWindow.xaml
    /// </summary>
    public partial class NetworkPlayWindow : Window
    {
        public NetworkPlayWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

    }
}
