using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BardMusicPlayer.Updater
{
    public partial class Download : Page
    {
        public Download()
        {
            InitializeComponent();
        }

        private readonly Stopwatch _stopwatch = new();
        private string _currentFile;

        internal async Task DownloadUpdates(List<VersionItem> versionItems)
        {
            using var webClient = new WebClient();
            webClient.DownloadProgressChanged += ProgressChanged;
            
            foreach (var item in versionItems)
            {
                _stopwatch.Start();
                _currentFile = item.destination;
                await webClient.DownloadFileTaskAsync(new Uri(item.source), App.VersionPath + item.destination);
                // TODO: check for errors from WebClient and sha256 the file right now.
                _stopwatch.Reset();
            }
        }
        
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            FileName.Text = "Downloading " + _currentFile + " " + $"{(e.BytesReceived / 1024d / _stopwatch.Elapsed.TotalSeconds):0.00} Kb/s";
            ProgressBar.Value = e.ProgressPercentage;
            CountDown.Text = $"{(e.BytesReceived / 1024d / 1024d):0.00} Mb / {(e.TotalBytesToReceive / 1024d / 1024d):0.00} Mb";
        }
    }
}
