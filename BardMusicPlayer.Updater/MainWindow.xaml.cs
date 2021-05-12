using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BardMusicPlayer.Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, Util.BmpVersion> RemoteVersions;

        public MainWindow()
        {
            InitializeComponent();
        }

        internal void ProvideVersions(Dictionary<string, Util.BmpVersion> inputRead)
        {
            this.RemoteVersions = inputRead.OrderBy(version => version.Value.beta).ThenByDescending(version => version.Value.build) as Dictionary<string, Util.BmpVersion>;
        }
    }
}
