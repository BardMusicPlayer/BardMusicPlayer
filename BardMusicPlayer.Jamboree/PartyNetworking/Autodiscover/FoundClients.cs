using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Jamboree.Events;
using BardMusicPlayer.Jamboree.PartyNetworking.Server_Client;

namespace BardMusicPlayer.Jamboree.PartyNetworking.Autodiscover
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
        public NetworkSocket Sockets { get; set; }
    }

    internal class FoundClients
    {
        public Dictionary<string, ClientInfo> GetClients() { return _partyClients; }
        private Dictionary<string, ClientInfo> _partyClients = new();
        private List<string> _knownIP = new();
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

                var client = new ClientInfo(IP, version);
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
            return _partyClients.TryGetValue(ip, out var info) ? info.Sockets : null;
        }

        /// <summary>
        /// Check if the IP is in IPlist
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public bool IsIpInList(string IP)
        {
            lock (_knownIP)
            {
                return _knownIP.Contains(IP);
            }
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
            lock (_knownIP)
            {
                _knownIP.Clear();
            }
        }
    }
}
