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
        private Util.BmpVersion LocalVersion;
        private Dictionary<string, Util.BmpVersion> RemoteVersions;

        public MainWindow()
        {
            InitializeComponent();
        }

        internal void ProvideVersions(Util.BmpVersion localVersion, Dictionary<string, Util.BmpVersion> inputRead)
        {
            this.LocalVersion = localVersion;
            this.RemoteVersions =
                inputRead.OrderBy(version => version.Value.beta)
                         .ThenByDescending(version => version.Value.build)
                         .ToDictionary<KeyValuePair<string, Util.BmpVersion>, string, Util.BmpVersion>(pair => pair.Key, pair => pair.Value);

            this.label_CurrentVersion.Content = "Current version: " + localVersion.build;
            this.label_NewVersionAvailable.Content = "BMP version " + this.RemoteVersions.First().Value.build + " is available for download.";

            this.tbox_PatchNotes.Text = this.RemoteVersions.Select(version => "website url: " + version.Key + @"/" + Environment.NewLine + "beta: " + version.Value.beta + Environment.NewLine + "commit: " + version.Value.commit + Environment.NewLine + "build: " + version.Value.build + Environment.NewLine).First();
        }
    }
}
