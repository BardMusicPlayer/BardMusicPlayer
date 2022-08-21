using BardMusicPlayer.Jamboree.Events;
using BardMusicPlayer.Jamboree.PartyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using ZeroTier.Sockets;

namespace BardMusicPlayer.Jamboree.PartyNetworking
{
    public class NetworkPartyServer : IDisposable
    {
        private static readonly Lazy<NetworkPartyServer> lazy = new Lazy<NetworkPartyServer>(() => new NetworkPartyServer());
        public static NetworkPartyServer Instance { get { return lazy.Value; } }
        private NetworkPartyServer() { }
        ~NetworkPartyServer()
        {
#if DEBUG
            Console.WriteLine("Destructor Called."); // Breakpoint here
#endif
        }

        void IDisposable.Dispose()
        {
            svcWorker.Stop();
#if DEBUG
            Console.WriteLine("Dispose Called.");
#endif
            //GC.SuppressFinalize(this);
        }

        private SocketServer svcWorker { get; set; } = null;

        public void StartServer(IPEndPoint iPEndPoint, byte type, string name)
        {
            BackgroundWorker objWorkerServerDiscovery = new BackgroundWorker();
            objWorkerServerDiscovery.WorkerReportsProgress = true;
            objWorkerServerDiscovery.WorkerSupportsCancellation = true;

            svcWorker = new SocketServer(ref objWorkerServerDiscovery, iPEndPoint, type, name);
            objWorkerServerDiscovery.DoWork += new DoWorkEventHandler(svcWorker.Start);
            objWorkerServerDiscovery.ProgressChanged += new ProgressChangedEventHandler(logWorkers_ProgressChanged);
            objWorkerServerDiscovery.RunWorkerAsync();
        }

        public void Stop()
        {
            svcWorker.Stop();
        }

        private void logWorkers_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.UserState.ToString());
        }
    }

    public class SocketServer
    {
        public bool disposing = false;
        public IPEndPoint iPEndPoint;
        public int ServerPort = 0;
        ZeroTierExtendedSocket listener = null;
        private BackgroundWorker worker = null;
        private Dictionary<string, KeyValuePair<long, ZeroTierExtendedSocket> > _pushBacklist = new Dictionary<string, KeyValuePair<long, ZeroTierExtendedSocket> >();

        private List<NetworkSocket> sessions = new List<NetworkSocket>();
        List<NetworkSocket> removed_sessions = new List<NetworkSocket>();



        private PartyClientInfo _clientInfo = new PartyClientInfo();

        public SocketServer(ref BackgroundWorker w, IPEndPoint localEndPoint, byte type, string name)
        {
            worker = w;
            iPEndPoint = localEndPoint;
            worker.ReportProgress(1, "Server");

            _clientInfo.Performer_Type = type;
            _clientInfo.Performer_Name = name;

            FoundClients.Instance.OnNewAddress += Instance_Finished; //Triggered if a new IP was added
        }

        public void Instance_Finished(object sender, string ip)
        {
            KeyValuePair<long, ZeroTierExtendedSocket> val;
            if (!_pushBacklist.TryGetValue(ip, out val))
                return;

            ZeroTierExtendedSocket handler = val.Value;
            if (AddClient(handler))
            {
                _pushBacklist.Remove(ip);
                return;
            }
            
        }

        private bool AddClient(ZeroTierExtendedSocket handler)
        {
            IPEndPoint remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
            if (!FoundClients.Instance.IsIpInList(remoteIpEndPoint.Address.ToString()))
            {
                BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Error Ip not in list\r\n"));
                return false;
            }
            else
            {
                NetworkSocket sockets = FoundClients.Instance.FindSocket(remoteIpEndPoint.Address.ToString());
                if (sockets != null)
                {
                    BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Session added\r\n"));
                    sockets.ListenSocket = handler;
                    lock (sessions)
                    {
                        sessions.Add(sockets);
                    }
                    return true;
                }
                else
                {
                    BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Error handshake sock null\r\n"));
                    return false;
                }
            }
            return true;
        }

        public void Start(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(3000);
            listener = new ZeroTierExtendedSocket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            listener.Bind(iPEndPoint);
            listener.Listen(10);
            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Started\r\n"));

            while (this.disposing == false)
            {
                long da = DateTimeOffset.Now.ToUnixTimeSeconds();

                //Only accept if a autodiscover was triggered
                if (listener.Poll(100, System.Net.Sockets.SelectMode.SelectRead))
                {
                    //Incomming connection
                    ZeroTierExtendedSocket handler = listener.Accept();
                    bool isInList = false;
                    Parallel.ForEach(sessions, session =>
                    {
                        if (session.ListenSocket == handler)
                            isInList = true;
                    });
                    if (!isInList)
                    {
                        IPEndPoint remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
                        if (!AddClient(handler))
                        {
                            KeyValuePair<long, ZeroTierExtendedSocket> val = new KeyValuePair<long, ZeroTierExtendedSocket>(DateTimeOffset.Now.ToUnixTimeSeconds(), handler );
                            _pushBacklist.Add(remoteIpEndPoint.Address.ToString(), val);
                        }
                    }
                }

                lock (sessions)
                {
                    //Update the sessions
                    foreach (NetworkSocket session in sessions)
                    {
                        if (!session.Update())
                            removed_sessions.Add(session);
                    }

                    //Remove dead sessions
                    foreach (NetworkSocket session in removed_sessions)
                    {
                        sessions.Remove(session);
                    }
                }
                //And clear the list
                removed_sessions.Clear();

                //Keep the pushback list clean
                List<string> delPushlist = new List<string>();
                foreach (var data in _pushBacklist)
                {
                    KeyValuePair<long, ZeroTierExtendedSocket> val = data.Value;
                    long currtime = DateTimeOffset.Now.ToUnixTimeSeconds();

                    if (val.Key+60 <= currtime)
                    {
                        delPushlist.Add(data.Key);
                        val.Value.Close();
                    }
                }
                lock (_pushBacklist)
                {
                    foreach (var i in delPushlist)
                        _pushBacklist.Remove(i);
                }

                long db = DateTimeOffset.Now.ToUnixTimeSeconds();
                try{
                    Task.Delay((int)(10 - (db - da)));
                }
                catch{ }
            }

            //Finished serving - close all
            foreach (NetworkSocket s in sessions)
            {
                // Release the socket.
                s.CloseConnection();
            }

            listener.KeepAlive = false;
            try { listener.Shutdown(System.Net.Sockets.SocketShutdown.Both); }
            finally 
            { 
                listener.Close(); 
            }
            
            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Stopped\r\n"));
            return;
        }

        public void SendToAll(byte[] pck)
        {
            foreach (NetworkSocket session in sessions)
                session.SendPacket(pck);
        }

        public void Stop()
        {
            this.disposing = true;
        }
    }
}
