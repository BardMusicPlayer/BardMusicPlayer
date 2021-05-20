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

            this.pbar_DownloadProgress.Opacity = 0;
        }

        internal void ProvideVersions(Util.BmpVersion localVersion, Dictionary<string, Util.BmpVersion> remoteVersions)
        {
            this.LocalVersion = localVersion;
            this.RemoteVersions = remoteVersions;

            this.label_CurrentVersion.Content = "Current version: " + localVersion.build;
            this.label_NewVersionAvailable.Content = "BMP version " + this.RemoteVersions.First().Value.build + " is available for download.";

            this.tbox_PatchNotes.Text = "⡆⣐⢕⢕⢕⢕⢕⢕⢕⢕⠅⢗⢕⢕⢕⢕⢕⢕⢕⠕⠕⢕⢕⢕⢕⢕⢕⢕⢕⢕\n" +
                                        "⢐⢕⢕⢕⢕⢕⣕⢕⢕⠕⠁⢕⢕⢕⢕⢕⢕⢕⢕⠅⡄⢕⢕⢕⢕⢕⢕⢕⢕⢕\n" +
                                        "⢕⢕⢕⢕⢕⠅⢗⢕⠕⣠⠄⣗⢕⢕⠕⢕⢕⢕⠕⢠⣿⠐⢕⢕⢕⠑⢕⢕⠵⢕\n" +
                                        "⢕⢕⢕⢕⠁⢜⠕⢁⣴⣿⡇⢓⢕⢵⢐⢕⢕⠕⢁⣾⢿⣧⠑⢕⢕⠄⢑⢕⠅⢕\n" +
                                        "⢕⢕⠵⢁⠔⢁⣤⣤⣶⣶⣶⡐⣕⢽⠐⢕⠕⣡⣾⣶⣶⣶⣤⡁⢓⢕⠄⢑⢅⢑\n" +
                                        "⠍⣧⠄⣶⣾⣿⣿⣿⣿⣿⣿⣷⣔⢕⢄⢡⣾⣿⣿⣿⣿⣿⣿⣿⣦⡑⢕⢤⠱⢐\n" +
                                        "⢠⢕⠅⣾⣿⠋⢿⣿⣿⣿⠉⣿⣿⣷⣦⣶⣽⣿⣿⠈⣿⣿⣿⣿⠏⢹⣷⣷⡅⢐\n" +
                                        "⣔⢕⢥⢻⣿⡀⠈⠛⠛⠁⢠⣿⣿⣿⣿⣿⣿⣿⣿⡀⠈⠛⠛⠁⠄⣼⣿⣿⡇⢔\n" +
                                        "⢕⢕⢽⢸⢟⢟⢖⢖⢤⣶⡟⢻⣿⡿⠻⣿⣿⡟⢀⣿⣦⢤⢤⢔⢞⢿⢿⣿⠁⢕\n" +
                                        "⢕⢕⠅⣐⢕⢕⢕⢕⢕⣿⣿⡄⠛⢀⣦⠈⠛⢁⣼⣿⢗⢕⢕⢕⢕⢕⢕⡏⣘⢕\n" +
                                        "⢕⢕⠅⢓⣕⣕⣕⣕⣵⣿⣿⣿⣾⣿⣿⣿⣿⣿⣿⣿⣷⣕⢕⢕⢕⢕⡵⢀⢕⢕\n" +
                                        "⢑⢕⠃⡈⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⢃⢕⢕⢕\n" +
                                        "⣆⢕⠄⢱⣄⠛⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⢁⢕⢕⠕⢁\n" +
                                        "⣿⣦⡀⣿⣿⣷⣶⣬⣍⣛⣛⣛⡛⠿⠿⠿⠛⠛⢛⣛⣉⣭⣤⣂⢜⠕⢑⣡⣴⣿\n";
            this.tbox_PatchNotes.Text = this.tbox_PatchNotes.Text + "\ntags within tags, like happy bonerjams and sad bonerjams\n";
            this.tbox_PatchNotes.Text = this.tbox_PatchNotes.Text + "\ntags within tags, like happy bonerjams and sad bonerjams\n";
            this.tbox_PatchNotes.Text = this.tbox_PatchNotes.Text + "\ntags within tags, like happy bonerjams and sad bonerjams\n";
            this.tbox_PatchNotes.Text = this.tbox_PatchNotes.Text + "\ntags within tags, like happy bonerjams and sad bonerjams\n";
        }

        private void button_LaunchBMP_Click(object sender, RoutedEventArgs e)
        {
            OnLaunchRequested?.Invoke(this, this.LocalVersion);
            this.Close();
        }

        private async void button_InstallUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.pbar_DownloadProgress.Opacity = 1;
            this.tbox_PatchNotes.Text = string.Empty;

            await Task.Run(() =>
            {
                var version = this.RemoteVersions.First().Value;
                foreach (var item in version.items)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        string currText = this.tbox_PatchNotes.Text;
                        this.tbox_PatchNotes.Text = $"{currText}\nDownloading {item.source}...";
                        this.tbox_PatchNotes.ScrollToEnd();
                    });

                    BmpDownloadEvent downloadEvent = new BmpDownloadEvent(this.RemoteVersions.First().Key, version, item);
                    OnDownloadRequested?.Invoke(this, downloadEvent);

                    this.Dispatcher.Invoke(() =>
                    {
                        float iter = version.items.FindIndex(i => i.source.Equals(item.source));
                        var currPercent = Math.Floor((iter / (float)version.items.Count) * 100.0f);
                        Debug.WriteLine($"{currPercent}");
                        this.pbar_DownloadProgress.Value = currPercent;
                    });
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
