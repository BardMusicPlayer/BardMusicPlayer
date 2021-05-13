using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal EventHandler<BmpDownloadEvent> OnDownloadRequested;
        internal EventHandler<Util.BmpVersion> OnDownloadComplete;
        internal EventHandler<Util.BmpVersion> OnLaunchRequested;

        private Util.BmpVersion LocalVersion;
        private Dictionary<string, Util.BmpVersion> RemoteVersions;

        public MainWindow()
        {
            InitializeComponent();
        }

        internal void ProvideVersions(Util.BmpVersion localVersion, Dictionary<string, Util.BmpVersion> remoteVersions)
        {
            this.LocalVersion = localVersion;
            this.RemoteVersions = remoteVersions;

            this.label_CurrentVersion.Content = "Current version: " + localVersion.build;
            this.label_NewVersionAvailable.Content = "BMP version " + this.RemoteVersions.First().Value.build + " is available for download.";

            this.tbox_PatchNotes.Text = this.RemoteVersions.Select(version => "website url: " + version.Key + @"/" + Environment.NewLine + "beta: " + version.Value.beta + Environment.NewLine + "commit: " + version.Value.commit + Environment.NewLine + "build: " + version.Value.build + Environment.NewLine).First();
        }

        private void button_LaunchBMP_Click(object sender, RoutedEventArgs e)
        {
            OnLaunchRequested?.Invoke(this, this.LocalVersion);
            this.Close();
        }

        private async void button_InstallUpdate_Click(object sender, RoutedEventArgs e)
        {
            // TODO: show progress bar UI
            await Task.Run(() =>
            {
                var version = this.RemoteVersions.First().Value;
                foreach (var item in version.items)
                {
                    // TODO: update progress bar UI with progress
                    Debug.WriteLine($"Downloading {item.source}");
                    BmpDownloadEvent downloadEvent = new BmpDownloadEvent(this.RemoteVersions.First().Key, version, item);
                    OnDownloadRequested?.Invoke(this, downloadEvent);
                }

                OnDownloadComplete?.Invoke(this, this.RemoteVersions.First().Value);
            });
        }

        private void button_NavigationClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
