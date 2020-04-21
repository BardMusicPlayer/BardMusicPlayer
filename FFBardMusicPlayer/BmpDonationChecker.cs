using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FFBardMusicPlayer {
	public class BmpDonationChecker {

		BackgroundWorker worker = new BackgroundWorker();
		public EventHandler<DonatorResponse> OnDonatorResponse;

		public class DonatorResponse {
			public bool donator;
			public string donationMessage;
		};

		public BmpDonationChecker(string characterName, string characterWorld) {

			worker.WorkerSupportsCancellation = true;
			worker.DoWork += UpdateApp;
			worker.RunWorkerCompleted += delegate (Object o, RunWorkerCompletedEventArgs a) {
				DonatorResponse don = (a.Result as DonatorResponse);
				if(don != null) {
					OnDonatorResponse?.Invoke(this, don);
				}
			};
			Uri donatorJson = new Uri(Program.urlBase + string.Format("donator?n={0}&w={1}", characterName, characterWorld));
			worker.RunWorkerAsync(donatorJson);
		}
		public void UpdateApp(Object sender, DoWorkEventArgs args) {
			Uri url = (args.Argument as Uri);
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
			
			IAsyncResult res = request.BeginGetResponse(null, null);
			while(!res.IsCompleted) {
				if(worker.CancellationPending) {
					return;
				}
			}
			HttpWebResponse response = request.EndGetResponse(res) as HttpWebResponse;
			args.Result = new DonatorResponse();
			if(response.StatusCode == HttpStatusCode.OK) {
				using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
					args.Result = JsonConvert.DeserializeObject<DonatorResponse>(reader.ReadToEnd());
				}
			}
		}
	}
}
