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

            pbar_DownloadProgress.Opacity = 0;
        }

        internal void ProvideVersions(Util.BmpVersion localVersion, Dictionary<string, Util.BmpVersion> remoteVersions)
        {
            LocalVersion   = localVersion;
            RemoteVersions = remoteVersions;

            label_CurrentVersion.Content = "Current version: " + localVersion.build;
            label_NewVersionAvailable.Content =
                "BMP version " + RemoteVersions.First().Value.build + " is available for download.";

            tbox_PatchNotes.Text = "⡆⣐⢕⢕⢕⢕⢕⢕⢕⢕⠅⢗⢕⢕⢕⢕⢕⢕⢕⠕⠕⢕⢕⢕⢕⢕⢕⢕⢕⢕\n" +
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
            tbox_PatchNotes.Text =
                tbox_PatchNotes.Text + "\ntags within tags, like happy bonerjams and sad bonerjams\n";
            tbox_PatchNotes.Text =
                tbox_PatchNotes.Text + "\ntags within tags, like happy bonerjams and sad bonerjams\n";
            tbox_PatchNotes.Text =
                tbox_PatchNotes.Text + "\ntags within tags, like happy bonerjams and sad bonerjams\n";
            tbox_PatchNotes.Text =
                tbox_PatchNotes.Text + "\ntags within tags, like happy bonerjams and sad bonerjams\n";
        }

        private void button_LaunchBMP_Click(object sender, RoutedEventArgs e)
        {
            OnLaunchRequested?.Invoke(this, LocalVersion);
            Close();
        }

        private async void button_InstallUpdate_Click(object sender, RoutedEventArgs e)
        {
            pbar_DownloadProgress.Opacity = 1;
            tbox_PatchNotes.Text          = string.Empty;

            await Task.Run(() =>
            {
                var version = RemoteVersions.First().Value;
                foreach (var item in version.items)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var currText = tbox_PatchNotes.Text;
                        tbox_PatchNotes.Text = $"{currText}\nDownloading {item.source}...";
                        tbox_PatchNotes.ScrollToEnd();
                    });

                    var downloadEvent = new BmpDownloadEvent(RemoteVersions.First().Key, version, item);
                    OnDownloadRequested?.Invoke(this, downloadEvent);

                    Dispatcher.Invoke(() =>
                    {
                        float iter = version.items.FindIndex(i => i.source.Equals(item.source));
                        var currPercent = Math.Floor(iter / (float) version.items.Count * 100.0f);
                        Debug.WriteLine($"{currPercent}");
                        pbar_DownloadProgress.Value = currPercent;
                    });
                }

                OnDownloadComplete?.Invoke(this, RemoteVersions.First().Value);
            });
        }

        private void button_NavigationClose_Click(object sender, RoutedEventArgs e) { Close(); }
    }
}