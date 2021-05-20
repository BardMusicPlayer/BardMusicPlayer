/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

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
        internal string ResourcePath { get; private set; }
        internal string DataPath { get; private set; }
        internal string[] Args { get; private set; }

        public async void StartUp(bool localDev, int launcherVersion, string exePath, string resourcePath, string dataPath, string[] args)
        {
            LocalDev = localDev;
            LauncherVersion = launcherVersion;
            ExePath = exePath;
            ResourcePath = resourcePath;
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
            // Read Version or default values if failure.
            try
            {
                if (File.Exists(ResourcePath + @"version.json"))
                    LocalVersion = File.ReadAllText(ResourcePath + @"version.json", Encoding.UTF8)
                        .DeserializeFromJson<BmpVersion>();
            }
            catch (Exception)
            {
                LocalVersion = new BmpVersion {items = new List<BmpVersionItem>()};
            }

            var currentVersionBad = false;
            if (LocalVersion.build != 0)
            {
                Parallel.ForEach(LocalVersion.items, item =>
                {
                    if (!File.Exists(ResourcePath + item.destination) ||
                        File.Exists(ResourcePath + item.destination) && !item.sha256.ToLower()
                            .Equals(Sha256.GetChecksum(ResourcePath + item.destination).ToLower()))
                        currentVersionBad = true;
                });
                if (currentVersionBad) LocalVersion = new BmpVersion {items = new List<BmpVersionItem>()};
            }

            // Fetch UpdateInfo and Versions or default values if failure.
            var remoteVersionBad = false;
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
                        remoteVersionBad = true;
                    }
                }
            }
            catch (Exception)
            {
                // Failed to grab the list of remote versions available. ignore.
                remoteVersionBad = true;
            }

            // sort the list of remote versions first by 'if !beta', then 'latest build', then convert back to dictionary
            RemoteVersions =
                RemoteVersions.OrderBy(version => version.Value.beta)
                              .ThenByDescending(version => version.Value.build)
                              .ToDictionary<KeyValuePair<string, Util.BmpVersion>, string, Util.BmpVersion>(pair => pair.Key, pair => pair.Value);

            // 1. remote version was not able to be read, try and load local
            // 2. remote version was able to be read but it's same as local, try and load local
            if ((remoteVersionBad && !currentVersionBad) || (RemoteVersions.First().Value.build == LocalVersion.build))
            {
                await Loader.Load(LocalVersion, ExePath, ResourcePath, DataPath, Args);
                return;
            }

            // 1. we don't have a current version
            // 2. remote version shows an update
            if (currentVersionBad || (RemoteVersions.First().Value.build > LocalVersion.build))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.ProvideVersions(LocalVersion, RemoteVersions);

                mainWindow.OnDownloadRequested += new EventHandler<BmpDownloadEvent>( (object sender, BmpDownloadEvent e) =>
                {
                    var key = e.Key;
                    var version = e.Version;
                    var item = e.Item;

                    string sourceUrl = $"{BaseUrl}{key}/{item.source}";
                    string destFPath = ResourcePath + item.destination;

                    Downloader downloader = new Downloader();

                    try
                    {
                        Debug.WriteLine($"Attempting to download {sourceUrl} and save to {destFPath}.");

                        var dlTask = downloader.GetFileFromUrl(sourceUrl);
                        dlTask.Wait();
                        byte[] file = dlTask.Result;
                        if (Sha256.GetChecksum(file).Equals(item.sha256))
                        {
                            Directory.CreateDirectory(ResourcePath);
                            File.WriteAllBytes(destFPath, file);
                        }
                    }
                    catch (Exception)
                    {
                        // unable to download file, show message to the user and throw
                        throw new Exception("Unable to download file: " + sourceUrl);
                    }
                });

                var launchHandler = new EventHandler<BmpVersion>(async (object sender, BmpVersion version) =>
                {
                    await Loader.Load(version, ExePath, ResourcePath, DataPath, Args);
                });

                mainWindow.OnDownloadComplete += launchHandler;
                mainWindow.OnLaunchRequested  += launchHandler;

                mainWindow.ShowDialog();
            }

#elif LOCAL
            
            var version = new BmpVersion
            {
                beta = true,
                build = 0,
                commit = "DEBUG",
                entryClass = "BardMusicPlayer.Ui.Main",
                entryDll = "BardMusicPlayer.Ui.dll",
                items = new List<BmpVersionItem>()
            };

            var items = Directory.GetFiles(ResourcePath, "*.dll").Where(name => !name.Equals("libzt.dll"));
            foreach (var item in items)
            {
                version.items.Add(new BmpVersionItem
                {
                    destination = Path.GetFileName(item),
                    sha256 = Sha256.GetChecksum(item),
                    load = true
                });
            }

            await Loader.Load(version, ExePath, ResourcePath, DataPath, Args);
#endif
        }
    }
}
