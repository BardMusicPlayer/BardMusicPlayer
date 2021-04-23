using System;
using System.Net;

namespace BardMusicPlayer
{
    internal class TimeoutWebClient : WebClient
    {
        internal int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            var lWebRequest = base.GetWebRequest(uri);
            if (lWebRequest == null) throw new Exception("Invalid download path");
            lWebRequest.Timeout = Timeout;
            ((HttpWebRequest) lWebRequest).ReadWriteTimeout = Timeout;
            return lWebRequest;
        }
    }
}
