using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BardMusicPlayer.Updater.Util;

namespace BardMusicPlayer.Updater
{
    public class Main
    {
#if DEBUG
        internal const string BaseUrl = @"https://dl.bardmusicplayer.com/bmp/debug/";
#else
        internal const string BaseUrl = @"https://dl.bardmusicplayer.com/bmp/release/";
#endif

        internal int CompatibleLauncherVersion = 1;
        internal Dictionary<string, BmpVersion> RemoteVersions  { get; set; }
        internal BmpVersion LocalVersion  { get; set; }

        internal bool LocalDev { get; private set; }
        internal int LauncherVersion { get; private set; }
        internal string ExePath { get; private set; }
        internal string DataPath { get; private set; }
        internal string[] Args { get; private set; }

        internal string VersionPath { get; private set; }

        public async void StartUp(bool localDev, int launcherVersion, string exePath, string dataPath, string[] args)
        {
            LocalDev = localDev;
            LauncherVersion = launcherVersion;
            ExePath = exePath;
            DataPath = dataPath;
            Args = args;

            if (LauncherVersion != CompatibleLauncherVersion)
            {
                MessageBox.Show("Please download the latest BardMusicPlayer.exe from https://bardmusicplayer.com",
                    "Bard Music Player Update Available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Environment.Exit(0);
            }

#if PUBLISH
            VersionPath = DataPath + @"Version\";
            // Read Version or default values if failure.
            try
            {
                if (File.Exists(VersionPath + @"version.json"))
                    LocalVersion = File.ReadAllText(VersionPath + @"version.json", Encoding.UTF8)
                        .DeserializeFromJson<BmpVersion>();
            }
            catch (Exception)
            {
                LocalVersion = new BmpVersion {items = new List<BmpVersionItem>()};
            }

            if (LocalVersion.build != 0)
            {
                var currentVersionBad = false;
                Parallel.ForEach(LocalVersion.items, item =>
                {
                    if (!File.Exists(VersionPath + item.destination) ||
                        File.Exists(VersionPath + item.destination) && !item.sha256.ToLower()
                            .Equals(Sha256.GetChecksum(VersionPath + item.destination).ToLower()))
                        currentVersionBad = true;
                });
                if (currentVersionBad) LocalVersion = new BmpVersion {items = new List<BmpVersionItem>()};
            }

            // Fetch UpdateInfo and Versions or default values if failure.
            RemoteVersions = new Dictionary<string, BmpVersion>();
            try
            {
                var remoteVersionsListString = await new Downloader().GetStringFromUrl(BaseUrl + @"versionlist.json");
                var remoteVersionsList = remoteVersionsListString.DeserializeFromJson<List<string>>();

                foreach (var remoteVersion in remoteVersionsList)
                {
                    try
                    {
                        var remoteVersionString = await new Downloader().GetStringFromUrl(BaseUrl + remoteVersion + "/version.json");
                        RemoteVersions.Add(remoteVersion, remoteVersionString.DeserializeFromJson<BmpVersion>());
                    }
                    catch (Exception)
                    {
                        // Failed to grab a specific remote version.json. ignore.
                    }
                }
            }
            catch (Exception)
            {
                // Failed to grab the list of remote versions available. ignore.
            }
          
            // Invoke Home.xaml here and do things. Version, UpdateInfo, and Versions may or may not be default/empty and logic will need to be done by the UI to decide what to do.
            MessageBox.Show(string.Join(Environment.NewLine, RemoteVersions.OrderBy(version => version.Value.beta).ThenByDescending(version => version.Value.build).Select(version => "website url: " + BaseUrl + version.Key + @"/" + Environment.NewLine + "beta: " + version.Value.beta + Environment.NewLine + "commit: " + version.Value.commit + Environment.NewLine + "build: " + version.Value.build + Environment.NewLine)), "Can haz versionz", MessageBoxButton.OK, MessageBoxImage.Information);
            Environment.Exit(0);

#elif LOCAL

#if DEBUG
            VersionPath = @"..\..\..\..\BardMusicPlayer.Ui\bin\Debug\net48\";
#else
            VersionPath = @"..\..\..\..\BardMusicPlayer.Ui\bin\Release\net48\";
#endif
            
            var version = new BmpVersion
            {
                beta = true,
                build = 0,
                commit = "DEBUG",
                entryClass = "BardMusicPlayer.Ui.Main",
                entryDll = "BardMusicPlayer.Ui.dll",
                items = new List<BmpVersionItem>()
            };

            var items = Directory.GetFiles(VersionPath, "*.dll");
            foreach (var item in items)
            {
                version.items.Add(new BmpVersionItem
                {
                    destination = Path.GetFileName(item),
                    sha256 = Sha256.GetChecksum(item),
                    load = true
                });
            }

            await Loader.Load(version, ExePath, VersionPath, DataPath, Args);
#endif
        }
    }
}
