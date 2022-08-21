/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Jamboree.Events;
using BardMusicPlayer.Jamboree.PartyManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ZeroTier.Sockets;

namespace BardMusicPlayer.Jamboree.PartyNetworking
{
    public class NetworkSocket
    {
        private bool _close = false;
        private PartyClientInfo _clientInfo = new PartyClientInfo();

        public PartyClientInfo PartyClient { get { return _clientInfo; } }

        public ZeroTierExtendedSocket ListenSocket { get; set; } = null;
        public ZeroTierExtendedSocket ConnectorSocket { get; set; } = null;
        private string _remoteIP = "";

        Timer _timer;
        bool _await_pong = false;

        public NetworkSocket(string IP)
        {
            _ = ConnectTo(IP).ConfigureAwait(false);
        }

        public async Task<bool> ConnectTo(string IP)
        {
            _remoteIP = IP;
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(IP), 12345);
            byte[] bytes = new byte[1024];
            ConnectorSocket = new ZeroTierExtendedSocket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            //Connect to the server
            ConnectorSocket.Connect(localEndPoint);
            //Wait til connected
            while (!ConnectorSocket.Connected)
            { await Task.Delay(1); }
            //Inform we are connected
            BmpJamboree.Instance.PublishEvent(new PartyConnectionChangedEvent(PartyConnectionChangedEvent.ResponseCode.OK, "Connected"));

            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[NetworkSocket]: Send handshake\r\n"));
            SendPacket(ZeroTierPacketBuilder.MSG_JOIN_PARTY(FoundClients.Instance.Type, FoundClients.Instance.OwnName));

            _timer = new Timer();
            _timer.Interval = 10000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            return false;
        }

        internal NetworkSocket(ZeroTierExtendedSocket socket)
        {
            ListenSocket = socket;
            PartyManager.Instance.Add(_clientInfo);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_await_pong)
            {
                _close = true;
                _timer.Enabled = false;
                return;
            }
            NetworkPacket buffer = new NetworkPacket(NetworkOpcodes.OpcodeEnum.PING);
            SendPacket(buffer.GetData());
            _await_pong = true;
            _timer.Interval = 3000;
        }

        public bool Update()
        {
            byte[] bytes = new byte[60000];
            if (_close)
            {
                CloseConnection();
                return false;
            }
            if (ListenSocket.Poll(0, System.Net.Sockets.SelectMode.SelectError))
            {
                CloseConnection();
                return false;
            }

            if (ListenSocket.Available == -1)
                return false;

            if (ListenSocket.Poll(100, System.Net.Sockets.SelectMode.SelectRead))
            {
                int bytesRec;
                try
                {
                    bytesRec = ListenSocket.Receive(bytes);
                    if (bytesRec == -1)
                    {
                        CloseConnection();
                        return false;
                    }
                    else
                        OpcodeHandling(bytes, bytesRec);
                }
                catch (SocketException err)
                {
                    Console.WriteLine(
                            "ServiceErrorCode={0} SocketErrorCode={1}",
                            err.ServiceErrorCode,
                            err.SocketErrorCode);
                    return false;
                }
            }

            return true;
        }

        public void SendPacket(byte[] pck)
        {
            if (ConnectorSocket.Available == -1)
                _close = true;

            if (!ConnectorSocket.Connected)
                _close = true;

            try 
            { 
                if(ConnectorSocket.Send(pck) == -1 )
                    _close = true;
            }
            catch { _close = true; }
            _close = false;
        }

        private void OpcodeHandling(byte[] bytes, int bytesRec)
        {
            NetworkPacket packet = new NetworkPacket(bytes);
            switch (packet.Opcode)
            {
                case NetworkOpcodes.OpcodeEnum.PING:
                    BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Ping \r\n"));
                    NetworkPacket buffer = new NetworkPacket(NetworkOpcodes.OpcodeEnum.PONG);
                    SendPacket(buffer.GetData());
                    break;
                case NetworkOpcodes.OpcodeEnum.PONG:
                    _await_pong = false;
                    _timer.Interval = 30000;
                    break;
                case NetworkOpcodes.OpcodeEnum.MSG_JOIN_PARTY:
                    _clientInfo.Performer_Type = packet.ReadUInt8();
                    _clientInfo.Performer_Name = packet.ReadCString();
                    BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Received handshake from "+_clientInfo.Performer_Name+"\r\n"));
                    break;
                case NetworkOpcodes.OpcodeEnum.MSG_PLAY:
                    BmpJamboree.Instance.PublishEvent(new PerformanceStartEvent(packet.ReadInt64(), true));
                    break;
                case NetworkOpcodes.OpcodeEnum.MSG_STOP:
                    BmpJamboree.Instance.PublishEvent(new PerformanceStartEvent(packet.ReadInt64(), false));
                    break;
                case NetworkOpcodes.OpcodeEnum.MSG_SONG_DATA:
                    System.Diagnostics.Debug.WriteLine("");
                    break;
                default:
                    break;
            };
        }

        public void CloseConnection()
        {
            _await_pong = false;
            _timer.Enabled = false;
            ListenSocket.LingerState = new System.Net.Sockets.LingerOption(false, 10);
            try { ListenSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both); }
            finally { ListenSocket.Close(); }

            ListenSocket.LingerState = new System.Net.Sockets.LingerOption(false, 10);
            try { ConnectorSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both); }
            finally { ConnectorSocket.Close(); }
            FoundClients.Instance.Remove(_remoteIP);
        }
    }
}
