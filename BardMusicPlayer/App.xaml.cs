using System;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using Polly;

namespace BardMusicPlayer
{
    public partial class App : Application
    {
        private const int LauncherVersion = 1;
        private const string DllName = "BardMusicPlayer.Updater.dll";
        private const string DllType = "BardMusicPlayer.Updater.Main";
        private const string DllSha256Name = "BardMusicPlayer.Updater.dll.sha256";

#if DEBUG
        private static readonly string DllUrl = @"https://dl.bardmusicplayer.com/bmp/updater/debug/" + LauncherVersion + @"/" + DllName ;
        private static readonly string DllSha256Url = @"https://dl.bardmusicplayer.com/bmp/updater/debug/" + LauncherVersion + @"/" + DllSha256Name;
#else
        private static readonly string DllUrl = @"https://dl.bardmusicplayer.com/bmp/updater/release/" + LauncherVersion + @"/" + DllName ;
        private static readonly string DllSha256Url = @"https://dl.bardmusicplayer.com/bmp/updater/release/" + LauncherVersion + @"/" + DllSha256Name;
#endif

        private static readonly string ExePath = Assembly.GetExecutingAssembly().Location;

#if LOCAL

        private static readonly string DataPath = Directory.GetCurrentDirectory() + @"\Data\";

#if DEBUG
        private static readonly string UpdaterPath = Directory.GetCurrentDirectory() + @"\";
#else
        private static readonly string UpdaterPath = Directory.GetCurrentDirectory() + @"\";
#endif

#elif PUBLISH

        private static readonly string DataPath = @Environment.GetFolderPath(@Environment.SpecialFolder.LocalApplicationData) + @"\BardMusicPlayer\";
        private static readonly string UpdaterPath = DataPath + @"Updater\";

#endif

#pragma warning disable CS1998
        protected override async void OnStartup(StartupEventArgs eventArgs)
#pragma warning restore CS1998
        {
            try
            {
                Directory.CreateDirectory(DataPath);
                Directory.CreateDirectory(UpdaterPath);

#if LOCAL

                const bool localDev = true;
                string DllSha256 = GetChecksum(UpdaterPath + DllName);

#elif PUBLISH

                const bool localDev = false;
                string DllSha256 = null;
                try
                {
                    if (File.Exists(UpdaterPath + DllSha256Name) && File.Exists(UpdaterPath + DllName))
                    {
                        DllSha256 = File.ReadAllText(UpdaterPath + DllSha256Name);
                        if (!GetChecksum(UpdaterPath + DllName).Equals(DllSha256)) DllSha256 = null;
                    }
                } catch (Exception)
                {
                    DllSha256 = null;
                }

                string RemoteDllSha256 = null;
                try
                {
                    RemoteDllSha256 = await GetStringFromUrl(DllSha256Url);
                    
                } catch (Exception)
                {
                    RemoteDllSha256 = null;
                }

                if (string.IsNullOrWhiteSpace(RemoteDllSha256) && string.IsNullOrWhiteSpace(DllSha256))
                    throw new Exception("Unable to contact update server and no local files are available.");

                if (!string.IsNullOrWhiteSpace(RemoteDllSha256))
                {
                    if (string.IsNullOrWhiteSpace(DllSha256) || !string.IsNullOrWhiteSpace(DllSha256) && !DllSha256.Equals(RemoteDllSha256))
                    {
                        byte[] RemoteDll = null;
                        try
                        {
                            RemoteDll = await GetFileFromUrl(DllUrl);
                        }
                        catch (Exception)
                        {
                            RemoteDll = null;
                        }
                        if (RemoteDll != null && GetChecksum(RemoteDll).Equals(RemoteDllSha256))
                        {
                            File.WriteAllText(UpdaterPath + DllSha256Name, RemoteDllSha256);
                            File.WriteAllBytes(UpdaterPath + DllName, RemoteDll);
                            DllSha256 = RemoteDllSha256;
                        }
                    }
                } 

#endif

                var updaterType = Assembly.LoadFrom(UpdaterPath + DllName, StringToBytes(DllSha256), AssemblyHashAlgorithm.SHA256).GetType(DllType);
                dynamic main = Activator.CreateInstance(updaterType ?? throw new InvalidOperationException("Unable to run " + DllType + " from " + DllName));
                main.Init(localDev, LauncherVersion, ExePath, DataPath, eventArgs.Args);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Uh oh, something went wrong and BardMusicPlayer is shutting down.\nPlease ask for support in the Discord Server and provide a picture of this error message:\n\n" + exception.Message,
                    "BardMusicPlayer Launcher Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(0);
            }

            base.OnStartup(eventArgs);
        }

        private static byte[] StringToBytes(string text)
        {
            text = text.ToUpper();
            var textArray = new string[text.Length / 2 + (text.Length % 2 == 0 ? 0 : 1)];
            for (var i = 0; i < textArray.Length; i++) textArray[ i ] = text.Substring(i * 2, i * 2 + 2 > text.Length ? 1 : 2);
            return textArray.Select(b => Convert.ToByte(b, 16)).ToArray();
        }

        private static string GetChecksum(string fileName)
        {
            using var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
            return GetChecksum(fileStream);
        }

        private static string GetChecksum(byte[] fileBytes)
        {
            using var memoryStream = new MemoryStream(fileBytes);
            return GetChecksum(memoryStream);
        }

        private static string GetChecksum(Stream stream)
        {
            using var bufferedStream = new BufferedStream(stream, 1024 * 32);
            var sha = new SHA256Managed();
            var checksum = sha.ComputeHash(bufferedStream);
            return BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();
        }

        public async Task<string> GetStringFromUrl(string url)
        {
            var httpClient = new HttpClient();
            return await Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: i => TimeSpan.FromMilliseconds(300))
                .ExecuteAsync(async () =>
                {
                    using var httpResponse = await httpClient.GetAsync(url);
                    httpResponse.EnsureSuccessStatusCode();
                    return await httpResponse.Content.ReadAsStringAsync();
                });
        }

        public async Task<byte[]> GetFileFromUrl(string url)
        {
            var httpClient = new HttpClient();
            return await Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: i => TimeSpan.FromMilliseconds(300))
                .ExecuteAsync(async () =>
                {
                    using var httpResponse = await httpClient.GetAsync(url);
                    httpResponse.EnsureSuccessStatusCode();
                    return await httpResponse.Content.ReadAsByteArrayAsync();
                });
        }
    }
}
