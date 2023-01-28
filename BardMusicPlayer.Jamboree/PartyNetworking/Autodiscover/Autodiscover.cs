using System;
using System.ComponentModel;
using System.Text;
using BardMusicPlayer.Jamboree.Events;
using BardMusicPlayer.Jamboree.PartyNetworking.ZeroTier;

namespace BardMusicPlayer.Jamboree.PartyNetworking.Autodiscover
{
    /// <summary>
    /// The autodiscover, to get the client IP and version
    /// </summary>
    internal class Autodiscover : IDisposable
    {
        private static readonly Lazy<Autodiscover> lazy = new(() => new Autodiscover());
        public static Autodiscover Instance => lazy.Value;

        private Autodiscover() { }
        ~Autodiscover()
        {
            svcRx.Stop();
#if DEBUG
            Console.WriteLine("Destructor Called.");
#endif
        }

        void IDisposable.Dispose()
        {
            svcRx.Stop();
#if DEBUG
            Console.WriteLine("Dispose Called.");
#endif
            //GC.SuppressFinalize(this);
        }

        private SocketRx svcRx { get; set; }

        public void StartAutodiscover(string address, string version)
        {
            var objWorkerServerDiscoveryRx = new BackgroundWorker();
            objWorkerServerDiscoveryRx.WorkerReportsProgress = true;
            objWorkerServerDiscoveryRx.WorkerSupportsCancellation = true;

            svcRx = new SocketRx(ref objWorkerServerDiscoveryRx, address, version);

            objWorkerServerDiscoveryRx.DoWork          += svcRx.Start;
            objWorkerServerDiscoveryRx.ProgressChanged += logWorkers_ProgressChanged;
            objWorkerServerDiscoveryRx.RunWorkerAsync();
        }

        private static void logWorkers_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.UserState.ToString());
        }

        public void Stop()
        {
            svcRx.Stop();
        }

    }

    public class SocketRx
    {
        public bool disposing;
        public System.Net.IPEndPoint iPEndPoint;
        public string BCAddress = "";
        public string Address = "";
        public string version = "";
        public int ServerPort = 0;
        private byte[] bytes = new byte[255];

        public SocketRx(ref BackgroundWorker w, string address, string ver)
        {
            Address = address;
            BCAddress = address.Split('.')[0] + "." + address.Split('.')[1] + "." + address.Split('.')[2] + ".255";
            version = ver;
            w.ReportProgress(1, "Server");
        }

        public void Start(object sender, DoWorkEventArgs e)
        {
            var listener = new ZeroTierExtendedSocket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
            var transmitter = new ZeroTierExtendedSocket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
            var r = listener.SetBroadcast();
            r = transmitter.SetBroadcast();
            iPEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(BCAddress), 5555);
            listener.ReceiveTimeout = 10;
            listener.BSD_Bind(iPEndPoint);
            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[Autodiscover]: Started\r\n"));
            
            while (disposing == false)
            {
                var bytesRec = listener.ReceiveFrom(bytes);
                if (bytesRec > 0)
                {
                    var all = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    var f = all.Split(' ')[0];               //Get the init
                    if (f.Equals("XIVAmp"))
                    {
                        var ip = all.Split(' ')[1];          //the IP
                        var version = all.Split(' ')[2];     //the version number
                        //Add the client
                        FoundClients.Instance.Add(ip, version);
                    }
                }
                if (!disposing)
                {
                    var t = "XIVAmp " + Address + " " + version; //Send the init ip and version
                    var p = transmitter.SendTo(iPEndPoint, Encoding.ASCII.GetBytes(t));
                    System.Threading.Thread.Sleep(3000);
                }
            }

            try { transmitter.Shutdown(System.Net.Sockets.SocketShutdown.Both); }
            finally { transmitter.Close(); }
            try { listener.Shutdown(System.Net.Sockets.SocketShutdown.Both); }
            finally { listener.Close(); }
            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[Autodiscover]: Stopped\r\n"));
        }

        public void Stop()
        {
            disposing = true;
        }
    }
}
