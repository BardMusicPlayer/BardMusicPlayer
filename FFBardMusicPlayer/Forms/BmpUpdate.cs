using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using ServiceStack;
using ServiceStack.Text;

namespace FFBardMusicPlayer.Forms
{
    public partial class BmpUpdate : Form
    {
        public UpdateVersion Version = new UpdateVersion();
        private float currentVersion;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public BmpUpdate()
        {
            InitializeComponent();

            worker.WorkerSupportsCancellation =  true;
            worker.DoWork                     += UpdateApp;
            worker.RunWorkerCompleted         += Worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { Close(); }

        private HttpWebResponse LoadFile(Uri url, out string result)
        {
            result = string.Empty;

            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                var response = (HttpWebResponse) request.GetResponse();

                request.Timeout = 2000;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        private bool DownloadToProgramFile(string filename, string outFilename)
        {
            var signatureJson = new Uri(Program.UrlBase + filename);
            var res2 = LoadFile(signatureJson, out var jsonText);
            if (res2.StatusCode == HttpStatusCode.OK)
            {
                var file = Path.Combine(Program.AppBase, outFilename);
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(jsonText);
                    return true;
                }
            }

            return false;
        }

        public void UpdateApp(object sender, EventArgs args)
        {
            currentVersion = UpdateVersion.Version;

            var updateJson = $"{Program.UrlBase}update?v={UpdateVersion.Version}";
            Console.WriteLine($"Updatejson: {updateJson}");

            Version = JsonSerializer.DeserializeFromString<UpdateVersion>(updateJson.GetJsonFromUrl());

            if (Version == null)
            {
                return;
            }

            DialogResult = Version.NewVersion > UpdateVersion.Version ? DialogResult.Yes : DialogResult.No;

            if (UpdateVersion.Version > Version.NewVersion)
            {
                DialogResult = DialogResult.Ignore;
            }

            // If not ignoring updates
            if (!Properties.Settings.Default.SigIgnore)
            {
                // If new version is above current sig version
                // Or if they don't exist

                var sigExist = File.Exists(Path.Combine(Program.AppBase, "signatures.json"));
                var strExist = File.Exists(Path.Combine(Program.AppBase, "structures.json"));
                if (Version.SigVersion > Properties.Settings.Default.SigVersion || Version.SigVersion == -1 ||
                    !(sigExist && strExist))
                {
                    Console.WriteLine("Downloading signatures");
                    var sigUrl = $"update?f=signatures&v={UpdateVersion.Version.ToString()}";
                    if (DownloadToProgramFile(sigUrl, "signatures.json"))
                    {
                        Console.WriteLine("Downloaded signatures");
                    }

                    Console.WriteLine("Downloading structures");
                    var strUrl = $"update?f=structures&v={UpdateVersion.Version.ToString()}";
                    if (DownloadToProgramFile(strUrl, "structures.json"))
                    {
                        Console.WriteLine("Downloaded structures");
                    }

                    Properties.Settings.Default.SigVersion = Version.SigVersion;
                    Console.WriteLine($"ver1: {Version.SigVersion} ver2: {Properties.Settings.Default.SigVersion}");
                    Properties.Settings.Default.Save();

                    // New signature update
                    // Reset forced stuff so people don't get stuck on that junk
                    if (Version.SigVersion > 0)
                    {
                        Properties.Settings.Default.ForcedOpen = false;
                    }
                }
            }
        }

        private void ButtonSkip_Click(object sender, EventArgs e) { worker.CancelAsync(); }
    }

    public class UpdateVersion
    {
        // If update, fill these in
        public float NewVersion { get; set; } = 0f;

        public string UpdateText { get; set; } = "";

        public string UpdateTitle { get; set; } = "";

        public string UpdateLog { get; set; } = "";

        public string AppName { get; set; } = "Bard Music Player";
#if DEBUG
        public string AppVersion { get; set; } = "Beta";
#else
		public string appVersion { get; set; } = string.Empty;
#endif
        public string CreatorName { get; set; } = string.Empty;

        public string CreatorWorld { get; set; } = string.Empty;

        public int SigVersion { get; set; } = -1;

        public static float Version
        {
            get
            {
                var v = typeof(Program).Assembly.GetName().Version;
                return v.Major + v.Minor / 100f;
            }
        }

        public override string ToString()
        {
            var str = $"{AppName} {Version}";
            if (!string.IsNullOrEmpty(AppVersion))
            {
                str = $"{str} [{AppVersion}]";
            }

            if (!string.IsNullOrEmpty(CreatorName))
            {
                str = $"{str} by {CreatorName}";
            }

            if (!string.IsNullOrEmpty(CreatorWorld))
            {
                str = $"{str} ({CreatorWorld})";
            }

            return str;
        }
    }
}