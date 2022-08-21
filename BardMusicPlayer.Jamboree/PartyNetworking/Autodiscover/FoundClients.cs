using BardMusicPlayer.Jamboree.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BardMusicPlayer.Jamboree.PartyNetworking
{

    public class ClientInfo
    {
        public ClientInfo(string IP, string version)
        {
            IPAddress = IP;
            Version = version;
            Sockets = new NetworkSocket(IP);
        }
        public string IPAddress { get; set; } = "";
        public string Version { get; set; } = "";
        public NetworkSocket Sockets { get; set; } = null;
    }

    internal class FoundClients
    {
        public Dictionary<string, ClientInfo> GetClients() { return _partyClients; }
        private Dictionary<string, ClientInfo> _partyClients = new Dictionary<string, ClientInfo>();
        private List<string> _knownIP = new List<string>();
        public string OwnName { get; set; } = "";
        public byte Type { get; set; } = 255;

        public EventHandler<string> OnNewAddress;

#region Instance Constructor/Destructor
        private static readonly Lazy<FoundClients> LazyInstance = new(() => new FoundClients());
        private FoundClients()
        {
            _partyClients.Clear();
        }

        public static FoundClients Instance => LazyInstance.Value;

        ~FoundClients() { Dispose(); }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Add the found client and create a NetworkSocket
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="version"></param>
        public void Add(string IP, string version)
        {
            if (!_partyClients.ContainsKey(IP))
            {
                lock (_knownIP) {
                    _knownIP.Add(IP);
                }

                ClientInfo client = new ClientInfo(IP, version);
                lock (_partyClients)
                {
                    _partyClients.Add(client.IPAddress, client);
                    OnNewAddress(this, IP);
                }
                BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("Added Client IP "+IP+"\r\n"));
            }
        }

        public void SendToAll(byte[] pck)
        {
            Parallel.ForEach(_partyClients, client =>
            {
                client.Value.Sockets.SendPacket(pck);
            });
        }

        /// <summary>
        /// Find the socket for the IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns>null or NetworkSocket</returns>
        public NetworkSocket FindSocket(string ip)
        {
            ClientInfo info;
            if (_partyClients.TryGetValue(ip, out info))
                return info.Sockets;
            return null;
        }

        /// <summary>
        /// Check if the IP is in IPlist
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public bool IsIpInList(string IP)
        {
            return _knownIP.Contains(IP);
        }

        /// <summary>
        /// Remove the client by its IP
        /// </summary>
        /// <param name="IP"></param>
        public void Remove(string IP)
        {
            lock (_knownIP)
            {
                _knownIP.Remove(IP);
            }

            lock (_partyClients)
            {
                _partyClients.Remove(IP);
            }
        }

        public void Clear()
        {
            _partyClients.Clear();
            _knownIP.Clear();
        }
    }
}
