using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroTier;
using ZeroTier.Core;

namespace ZeroTier
{
    public class ZeroTierConnector
    {
        Node node;
        private volatile bool nodeOnline = false;

        /// <summary>
        /// start zerotier
        /// </summary>
        /// <param name="network"></param>
        /// <returns>Ip address</returns>
        public Task<string> ZeroTierConnect(string network)
        {
            node = new Node();
            string ipAddress = "";
            ulong networkId = (ulong)Int64.Parse(network, System.Globalization.NumberStyles.HexNumber);
#if DEBUG
            Console.WriteLine("Connecting to network...");
#endif
            //node.InitFromStorage(configFilePath);
            node.InitAllowNetworkCaching(false);
            node.InitAllowPeerCaching(false);
            // node.InitAllowIdentityCaching(true);
            // node.InitAllowWorldCaching(false);
            node.InitSetEventHandler(ZeroTierEvent);
            // node.InitSetPort(0);   // Will randomly attempt ports if not specified or is set to 0
            node.InitSetRandomPortRange(40000, 50000);
            // node.InitAllowSecondaryPort(false);

            node.Start();   // Network activity only begins after calling Start()
            while (!nodeOnline)
            { Task.Delay(50); }
#if DEBUG
            Console.WriteLine("Id            : " + node.IdString);
            Console.WriteLine("Version       : " + node.Version);
            Console.WriteLine("PrimaryPort   : " + node.PrimaryPort);
            Console.WriteLine("SecondaryPort : " + node.SecondaryPort);
            Console.WriteLine("TertiaryPort  : " + node.TertiaryPort);
#endif
            node.Join(networkId);

#if DEBUG
            Console.WriteLine("Waiting for join to complete...");
#endif
            while (node.Networks.Count == 0)
            {
                Task.Delay(50);
            }

            // Wait until we've joined the network and we have routes + addresses
#if DEBUG
            Console.WriteLine("Waiting for network to become transport ready...");
#endif
            while (!node.IsNetworkTransportReady(networkId))
            {
                Task.Delay(50);
            }

#if DEBUG
            Console.WriteLine("Num of assigned addresses : " + node.GetNetworkAddresses(networkId).Count);
#endif
            if (node.GetNetworkAddresses(networkId).Count == 1)
            {
                IPAddress addr = node.GetNetworkAddresses(networkId)[0];
                ipAddress = addr.ToString();
            }
#if DEBUG
            foreach (IPAddress addr in node.GetNetworkAddresses(networkId))
            {
                Console.WriteLine(" - Address: " + addr);
            }

            Console.WriteLine("Num of routes             : " + node.GetNetworkRoutes(networkId).Count);
            foreach (RouteInfo route in node.GetNetworkRoutes(networkId))
            {
                Console.WriteLine(" -   Route: target={0} via={1} flags={2} metric={3}",
                    route.Target.ToString(),
                    route.Via.ToString(),
                    route.Flags,
                    route.Metric);
            }
#endif
            return Task.FromResult(result: ipAddress);
        }

        public void ZeroTierDisconnect(bool free)
        {
            if (free)
                node.Free();
            else
                node.Stop();
        }

        private void ZeroTierEvent(Event e)
        {
#if DEBUG
            Console.WriteLine("Event.Code = {0} ({1})", e.Code, e.Name);
#endif
            if (e.Code == Constants.EVENT_NODE_ONLINE)
            {
                nodeOnline = true;
            }
            if (e.Code == Constants.EVENT_PEER_PATH_DEAD)
            {
                Console.WriteLine("DEAD");
            }
            /*
        if (e.Code == ZeroTier.Constants.EVENT_NETWORK_OK) {
            Console.WriteLine(" - Network ID: " + e.NetworkInfo.Id.ToString("x16"));
        }
        */
        }

    }
}
