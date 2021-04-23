using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BardMusicPlayer.Updater
{
    public class Main
    {
        internal const string ApiEndpoint = @"https://api.bardmusicplayer.com/v2/";

        internal UpdateInfo RemoteUpdateInfo { get; set; }
        internal List<Version> RemoteVersions  { get; set; }
        internal Version LocalVersion  { get; set; }

        internal bool LocalDev { get; private set; }
        internal int LauncherVersion { get; private set; }
        internal string ExePath { get; private set; }
        internal string DataPath { get; private set; }
        internal string[] Args { get; private set; }

        internal string VersionPath { get; private set; }

        public void Init(bool localDev, int launcherVersion, string exePath, string dataPath, string[] args)
        {
            LocalDev = localDev;
            LauncherVersion = launcherVersion;
            ExePath = exePath;
            DataPath = dataPath;
            Args = args;

#if PUBLISH
                VersionPath = DataPath + @"Version\";
                // Read Version or default values if failure.
                try
                {
                    if (File.Exists(VersionPath + @"version.json"))
                        LocalVersion = File.ReadAllText(VersionPath + @"version.json", Encoding.UTF8)
                            .DeserializeFromJson<Version>();
                } catch (Exception)
                {
                    LocalVersion = default;
                }
                if (LocalVersion.build != 0)
                {
                    var currentVersionBad = false;
                    Parallel.ForEach(LocalVersion.items, item =>
                    {
                        if (!File.Exists(VersionPath + item.destination) ||
                            File.Exists(VersionPath + item.destination) && !item.sha256.ToLower().Equals(Sha256.GetChecksum(VersionPath + item.destination).ToLower()))
                            currentVersionBad = true;
                    });
                    if (currentVersionBad) LocalVersion = new Version { items = new List<VersionItem>() };
                }

                // Fetch UpdateInfo and Versions or default values if failure.
                try
                {
                        using WebClient webClient = new();
                        RemoteUpdateInfo = webClient.DownloadString(ApiEndpoint + "updateInfo/1")
                            .DeserializeFromJson<UpdateInfo>();
                        RemoteVersions = RemoteUpdateInfo.versionPaths.Select(versionPath =>
                                webClient.DownloadString(versionPath + "/version.json").DeserializeFromJson<Version>())
                            .ToList();
                } catch (Exception)
                {
                    RemoteUpdateInfo = new UpdateInfo {versionPaths = new List<string>()};
                    RemoteVersions = new List<Version>();
                }
                
                // If we have valid local and remote information, and local is on the latest release build, just launch it directly without a prompt.
                // If such a user wants to pick older/beta version they are smart enough to clear out their appdata folder.
                if (LocalVersion.build != 0 && RemoteVersions.Contains(LocalVersion) && RemoteVersions.First(version => version.beta == false).Equals(LocalVersion))
                {
                    Loader.Load(LocalVersion, ExePath, VersionPath, DataPath, args);
                    return;
                }

                // Invoke Home.xaml here and do things. Version, UpdateInfo, and Versions may or may not be default/empty and logic will need to be done by the UI to decide what to do.
                MessageBox.Show(string.Join(",",RemoteUpdateInfo.versionPaths), "Can haz versionz!", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);

#else

#if DEBUG
            VersionPath = @"..\..\..\..\BardMusicPlayer.Ui\bin\Debug\net48\";
#else
            VersionPath = @"..\..\..\..\BardMusicPlayer.Ui\bin\Release\net48\";
#endif

            

#if DEBUG
            var items = Directory.GetFiles(@"..\..\..\..\BardMusicPlayer.Ui\bin\Debug\net48", "*.dll");
#else
                var items = Directory.GetFiles(@"..\..\..\..\BardMusicPlayer.Ui\bin\Release\net48", "*.dll");
#endif

            var version = new Version
            {
                beta = true,
                build = 0,
                commit = "DEBUG",
                entryClass = "BardMusicPlayer.Ui.Main",
                entryDll = "BardMusicPlayer.Ui.dll",
                extra = "",
                items = new List<VersionItem>()
            };

            foreach (var item in items)
            {
                version.items.Add(new VersionItem
                {
                    destination = Path.GetFileName(item),
                    sha256 = Sha256.GetChecksum(item),
                    load = true
                });
            }

            Loader.Load(version, ExePath, VersionPath, DataPath, Args);

#endif
        }

    }
}
