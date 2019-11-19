using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer {

	public partial class BmpUpdate : Form {

		public UpdateVersion version = new UpdateVersion();
		private float currentVersion;
		BackgroundWorker worker = new BackgroundWorker();

		public BmpUpdate() {
			InitializeComponent();

			worker.WorkerSupportsCancellation = true;
			worker.DoWork += UpdateApp;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

			worker.RunWorkerAsync();
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			this.Close();
		}

		private HttpWebResponse LoadFile(Uri url, out string result) {

			result = string.Empty;

			try {
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
				HttpWebResponse response = (HttpWebResponse) request.GetResponse();

				request.Timeout = 2000;

				if(response != null && response.StatusCode == HttpStatusCode.OK) {
					using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
						result = reader.ReadToEnd();
					}
				}
				return response;
			} catch(Exception e) {
				Console.WriteLine(e);
			}
			return null;
		}

		private bool DownloadToProgramFile(string filename, string outFilename) {
			Uri signatureJson = new Uri(Program.urlBase + filename);
			HttpWebResponse res2 = LoadFile(signatureJson, out string jsonText);
			if(res2.StatusCode == HttpStatusCode.OK) {
				string file = Path.Combine(Program.appBase, outFilename);
				using(StreamWriter writer = new StreamWriter(file)) {
					writer.Write(jsonText);
					return true;
				}
			}
			return false;
		}

		public void UpdateApp(Object sender, EventArgs args) {

			currentVersion = UpdateVersion.Version;

			Uri updateJson = new Uri(Program.urlBase + string.Format("update?v={0}", UpdateVersion.Version.ToString()));
			Console.WriteLine("Updatejson: " + updateJson.ToString());
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(updateJson);

			IAsyncResult res = request.BeginGetResponse(null, null);
			while(!res.IsCompleted) {
				if(worker.CancellationPending) {
					return;
				}
			}
			HttpWebResponse response = request.EndGetResponse(res) as HttpWebResponse;

			if(response.StatusCode == HttpStatusCode.OK) {
				using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
					version = JsonConvert.DeserializeObject<UpdateVersion>(reader.ReadToEnd());
				}

				this.DialogResult = (version.updateVersion > UpdateVersion.Version) ? DialogResult.Yes : DialogResult.No;


				// If not ignoring updates
				if(!Properties.Settings.Default.SigIgnore) {
					// If new version is above current sig version
					// Or if they don't exist

					bool sigExist = File.Exists(Path.Combine(Program.appBase, "signatures.json"));
					bool strExist = File.Exists(Path.Combine(Program.appBase, "structures.json"));
					if((version.sigVersion > Properties.Settings.Default.SigVersion) || version.sigVersion == -1 || !(sigExist && strExist)) {

						Console.WriteLine("Downloading signatures");
						string sigUrl = string.Format("update?f=signatures&v={0}", UpdateVersion.Version.ToString());
						if(DownloadToProgramFile(sigUrl, "signatures.json")) {
							Console.WriteLine("Downloaded signatures");
						}
						Console.WriteLine("Downloading structures");
						string strUrl = string.Format("update?f=structures&v={0}", UpdateVersion.Version.ToString());
						if(DownloadToProgramFile(strUrl, "structures.json")) {
							Console.WriteLine("Downloaded structures");
						}

						Properties.Settings.Default.SigVersion = version.sigVersion;
						Console.WriteLine(string.Format("ver1: {0} ver2: {1}", version.sigVersion, Properties.Settings.Default.SigVersion));
						Properties.Settings.Default.Save();

						// New signature update
						// Reset forced stuff so people don't get stuck on that junk
						if(version.sigVersion > 0) {
							Properties.Settings.Default.ForcedOpen = false;
						}
					}
				}
			}
		}

		private void ButtonSkip_Click(object sender, EventArgs e) {
			worker.CancelAsync();
		}
	}


	public class UpdateVersion {
		// If update, fill these in
		public float updateVersion = 0f;
		public string updateText = "";
		public string updateTitle = "";

		public string appName = "Bard Music Player";
#if DEBUG
		public string appVersion = "Beta";
#else
		public string appVersion = string.Empty;
#endif
		public string creatorName = string.Empty;
		public string creatorWorld = string.Empty;
		public int sigVersion = -1;

		public static float Version {
			get {
				Version v = typeof(Program).Assembly.GetName().Version;
				return (float) v.Major + (float) (v.Minor / 100f);
			}
		}

		public override string ToString() {
			string str = string.Format("{0} {1}", appName, Version);
			if(!string.IsNullOrEmpty(appVersion)) {
				str = string.Format("{0} [{1}]", str, appVersion);
			}
			if(!string.IsNullOrEmpty(creatorName)) {
				str = string.Format("{0} by {1}", str, creatorName);
			}
			if(!string.IsNullOrEmpty(creatorWorld)) {
				str = string.Format("{0} ({1})", str, creatorWorld);
			}
			return str;
		}
	}
}
