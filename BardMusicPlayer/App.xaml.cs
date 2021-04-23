using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace BardMusicPlayer
{
    public partial class App : Application
    {
        internal static Version Version = default;
        internal static UpdateInfo UpdateInfo = default;
        internal static List<Version> Versions = new();

        internal static int LauncherVersion = 1;
        internal static string ExePath = Assembly.GetExecutingAssembly().Location;
        internal static string ApiEndpoint = @"https://api.bardmusicplayer.com/v2/";

#if LOCAL
        internal static string DataPath = Directory.GetCurrentDirectory() + @"Data\";
#if DEBUG
        internal static string VersionPath = @"..\..\..\..\BardMusicPlayer.Ui\bin\Debug\net48\";
#else
        internal static string VersionPath = @"..\..\..\..\BardMusicPlayer.Ui\bin\Release\net48\";
#endif
#else
        internal static string DataPath = @Environment.GetFolderPath(@Environment.SpecialFolder.LocalApplicationData) + @"\BardMusicPlayer\";
        internal static string VersionPath = DataPath + @"Version\";
#endif

#pragma warning disable CS1998
        protected override async void OnStartup(StartupEventArgs eventArgs)
#pragma warning restore CS1998
        {
            try
            {
                Directory.CreateDirectory(DataPath);
                Directory.CreateDirectory(VersionPath);

#if LOCAL

                // Clean away local build artifacts.
                foreach (var dll in Directory.EnumerateFiles(@".\","*.dll")) File.Delete(dll);

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

                Loader.Load(version, ExePath, VersionPath, DataPath, eventArgs.Args);

#else

                // Read Version or default values if failure.
                try
                {
                    if (File.Exists(VersionPath + @"version.json"))
                        Version = File.ReadAllText(VersionPath + @"version.json", Encoding.UTF8)
                            .DeserializeFromJson<Version>();
                } catch (Exception)
                {
                    Version = default;
                }
                if (Version.build != 0)
                {
                    var currentVersionBad = false;
                    Parallel.ForEach(Version.items, item =>
                    {
                        if (!File.Exists(VersionPath + item.destination) ||
                            File.Exists(VersionPath + item.destination) && !item.sha256.ToLower().Equals(Sha256.GetChecksum(VersionPath + item.destination).ToLower()))
                            currentVersionBad = true;
                    });
                    if (currentVersionBad) Version = default;
                }

                // Fetch UpdateInfo and Versions or default values if failure.
                try
                {
                        using TimeoutWebClient webClient = new() { Timeout = 3000 };
                        UpdateInfo = webClient.DownloadString(ApiEndpoint + "updateInfo/1")
                            .DeserializeFromJson<UpdateInfo>();
                        Versions = UpdateInfo.versionPaths.Select(versionPath =>
                                webClient.DownloadString(versionPath + "/version.json").DeserializeFromJson<Version>())
                            .ToList();
                } catch (Exception)
                {
                    UpdateInfo = default;
                    UpdateInfo.versionPaths = new List<string>();
                    Versions = new List<Version>();
                }
                
                // Direct user to download new EXE if needed. 
                if (UpdateInfo.deprecated) MessageBox.Show(UpdateInfo.deprecatedMessage, UpdateInfo.deprecatedTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                
                // If we have valid local and remote information, and local is on the latest release build, just launch it directly without a prompt.
                // If such a user wants to pick older/beta version they are smart enough to clear out their appdata folder.
                if (Version.build != 0 && Versions.Contains(Version) && Versions.First(version => version.beta == false).Equals(Version))
                {
                    Loader.Load(Version, ExePath, VersionPath, DataPath, eventArgs.Args);
                    return;
                }

                // Invoke Home.xaml here and do things. Version, UpdateInfo, and Versions may or may not be default/empty and logic will need to be done by the UI to decide what to do.
                MessageBox.Show(string.Join(",",UpdateInfo.versionPaths), "Can haz versionz", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);

#endif
            }
            catch (Exception exception)
            {
                MessageBox.Show("Uh oh, something went wrong and BardMusicPlayer is shutting down.\nPlease ask for support in the Discord Server and provide a picture of this error message:\n\n" + exception.Message,
                    "BardMusicPlayer Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
    }
}
