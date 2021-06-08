using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using ServiceStack.Text;

namespace FFBardMusicPlayer
{
    public class BmpDonationChecker
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        public EventHandler<DonatorResponse> OnDonatorResponse;

        public class DonatorResponse
        {
            public bool Donator { get; set; }

            public string DonationMessage { get; set; }
        };

        public BmpDonationChecker(string characterName, string characterWorld)
        {
            worker.WorkerSupportsCancellation =  true;
            worker.DoWork                     += UpdateApp;
            worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs a)
            {
                if (a.Result is DonatorResponse don)
                {
                    OnDonatorResponse?.Invoke(this, don);
                }
            };
            var donatorJson =
                new Uri($"{Program.UrlBase}donator?n={characterName}&w={characterWorld}");
            worker.RunWorkerAsync(donatorJson);
        }

        public void UpdateApp(object sender, DoWorkEventArgs args)
        {
            var url = args.Argument as Uri;
            var request = (HttpWebRequest) WebRequest.Create(url);

            var res = request.BeginGetResponse(null, null);
            while (!res.IsCompleted)
            {
                if (worker.CancellationPending)
                {
                    return;
                }
            }

            var response = request.EndGetResponse(res) as HttpWebResponse;
            args.Result = new DonatorResponse();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    args.Result = JsonSerializer.DeserializeFromReader<DonatorResponse>(reader);
                }
            }
        }
    }
}