using System.Net;
using BardMusicPlayer.Jamboree.Events;
using BardMusicPlayer.Jamboree.PartyNetworking.Autodiscover;
using BardMusicPlayer.Jamboree.PartyNetworking.Server_Client;
using BardMusicPlayer.Jamboree.PartyNetworking.ZeroTier;

namespace BardMusicPlayer.Jamboree;

/// <summary>
/// The manager class for the festival
/// </summary>
/// We are creating a mesh network
/// - fire up the ZeroTier
/// - start the autodiscover to tell we are here
/// - start the TCP listener
/// - add the clients we found to the manager
/// - create a Session and connect to the listenserver
/// - add the listensocket from the listenserver to our session
/// - handshake complete
public class Pydna
{
    private bool _online { get; set; }

    private ZeroTierConnector zeroTierConnector;

    public void JoinParty(string networkId, byte type, string name)
    {
        if (zeroTierConnector != null)
            return;
        FoundClients.Instance.OwnName = name;
        FoundClients.Instance.Type    = type;
            
        zeroTierConnector = new ZeroTierConnector();
        var data = zeroTierConnector.ZeroTierConnect(networkId).Result;

        Autodiscover.Instance.StartAutodiscover(data, "0.1.0");
        NetworkPartyServer.Instance.StartServer(new IPEndPoint(IPAddress.Parse(data), 12345), type, name);
        _online = true;
        BmpJamboree.Instance.PublishEvent(new PartyCreatedEvent("Connected...\r\n"));
    }

    public void LeaveParty()
    {
        _online = false;
        //Stop the autodiscover
        Autodiscover.Instance.Stop();
        NetworkPartyServer.Instance.Stop();
        FoundClients.Instance.Clear();
        zeroTierConnector.ZeroTierDisconnect(true);
        BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[Pydna]: Stopped\r\n"));
    }

    #region NetworkSendFunctions
    public static void SendPerformanceStart()
    {
        FoundClients.Instance.SendToAll(ZeroTierPacketBuilder.PerformanceStart());
    }

    /// <summary>
    /// Send we joined the party
    /// | type 0 = bard
    /// | type 1 = dancer
    /// </summary>
    /// <param name="type"></param>
    /// <param name="performer_name"></param>
    public void SendPerformerJoin(byte type, string performername)
    {
        if (!_online)
            return;
        /*if (!_servermode)
        {
            client.SetPlayerData(type, performername);
            client.SendPacket(ZeroTierPacketBuilder.CMSG_JOIN_PARTY(type, performername));
        }*/
    }

    public void SendClientPacket(byte [] packet)
    {
        //if (!_servermode)
        //    client.SendPacket(packet);
    }

    public void SendServerPacket(byte[] packet)
    {
        if (!_online)
            return;
        FoundClients.Instance.SendToAll(packet);
    }

    #endregion

}