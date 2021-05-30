/*
 * Copyright (c)2013-2021 ZeroTier, Inc.
 *
 * Use of this software is governed by the Business Source License included
 * in the license here: https://github.com/zerotier/libzt/blob/master/LICENSE.txt
 *
 * Change Date: 2026-01-01
 *
 * On the date above, in accordance with the Business Source License, use
 * of this software will be governed by version 2.0 of the Apache License.
 */
/****/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace BardMusicPlayer.Jamboree.PartyClient.ZeroTier
{
    // Prototype of callback used by ZeroTier to signal events to C# application
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void CSharpCallbackWithStruct(IntPtr msgPtr);

    internal delegate void ZeroTierManagedEventCallback(Event nodeEvent);

    internal class Node
    {
        private static ulong _id = 0x0;
        private static ushort _secondaryPort;
        private static ushort _tertiaryPort;
        private static int _versionMajor;
        private static int _versionMinor;
        private static int _versionRev;
        private static bool _isOnline = false;
        private static bool _hasBeenFreed = false;
        private string _configFilePath;
        private ushort _primaryPort;
        private static ZeroTierManagedEventCallback _managedCallback;
        private CSharpCallbackWithStruct _unmanagedCallback;

        private ConcurrentDictionary<ulong, NetworkInfo> _networks =
            new();

        private ConcurrentDictionary<ulong, PeerInfo> _peers = new();

        public Node() { ClearNode(); }

        private void ClearNode()
        {
            _id             = 0x0;
            _primaryPort    = 0;
            _secondaryPort  = 0;
            _tertiaryPort   = 0;
            _versionMajor   = 0;
            _versionMinor   = 0;
            _versionRev     = 0;
            _isOnline       = false;
            _configFilePath = string.Empty;
            _networks.Clear();
            _peers.Clear();
        }

        public int InitFromStorage(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath)) throw new ArgumentNullException("configFilePath");

            int res = Constants.ERR_OK;
            Console.WriteLine("path = " + configFilePath);
            if ((res = zts_init_from_storage(configFilePath)) == Constants.ERR_OK) _configFilePath = configFilePath;
            return res;
        }

        public int InitSetEventHandler(ZeroTierManagedEventCallback managedCallback)
        {
            if (managedCallback == null) throw new ArgumentNullException("managedCallback");

            int res = Constants.ERR_OK;
            if ((res = zts_init_set_event_handler(OnZeroTierEvent)) == Constants.ERR_OK)
            {
                _unmanagedCallback = OnZeroTierEvent;
                _managedCallback   = new ZeroTierManagedEventCallback(managedCallback);
            }

            return res;
        }

        public int InitSetPort(ushort port)
        {
            int res = Constants.ERR_OK;
            if ((res = zts_init_set_port(port)) == Constants.ERR_OK) _primaryPort = port;
            return res;
        }

        public int InitSetRandomPortRange(ushort startPort, ushort endPort) =>
            zts_init_set_random_port_range(startPort, endPort);

        public int InitSetRoots(byte[] roots_data, int len)
        {
            var unmanagedPointer = Marshal.AllocHGlobal(roots_data.Length);
            Marshal.Copy(roots_data, 0, unmanagedPointer, roots_data.Length);
            var res = zts_init_set_roots(unmanagedPointer, len);
            Marshal.FreeHGlobal(unmanagedPointer);
            return res;
        }

        public int InitAllowNetworkCaching(bool allowed) => zts_init_allow_net_cache(Convert.ToByte(allowed));

        public int InitAllowSecondaryPort(bool allowed) => zts_init_allow_secondary_port(Convert.ToByte(allowed));

        public int InitAllowPortMapping(bool allowed) => zts_init_allow_port_mapping(Convert.ToByte(allowed));

        public int InitAllowPeerCaching(bool allowed) => zts_init_allow_peer_cache(Convert.ToByte(allowed));

        private void OnZeroTierEvent(IntPtr msgPtr)
        {
            var msg = (zts_event_msg_t) Marshal.PtrToStructure(msgPtr, typeof(zts_event_msg_t));
            Event newEvent = null;

            // Node

            if (msg.node != IntPtr.Zero)
            {
                var details = (zts_node_info_t) Marshal.PtrToStructure(msg.node, typeof(zts_node_info_t));
                newEvent       = new Event();
                newEvent.Code  = msg.event_code;
                _id            = details.node_id;
                _primaryPort   = details.primary_port;
                _secondaryPort = details.secondary_port;
                _tertiaryPort  = details.tertiary_port;
                _versionMajor  = details.ver_major;
                _versionMinor  = details.ver_minor;
                _versionRev    = details.ver_rev;
                _isOnline      = Convert.ToBoolean(zts_node_is_online());

                if (msg.event_code == Constants.EVENT_NODE_UP) newEvent.Name              = "EVENT_NODE_UP";
                if (msg.event_code == Constants.EVENT_NODE_ONLINE) newEvent.Name          = "EVENT_NODE_ONLINE";
                if (msg.event_code == Constants.EVENT_NODE_OFFLINE) newEvent.Name         = "EVENT_NODE_OFFLINE";
                if (msg.event_code == Constants.EVENT_NODE_DOWN) newEvent.Name            = "EVENT_NODE_DOWN";
                if (msg.event_code == Constants.ZTS_EVENT_NODE_FATAL_ERROR) newEvent.Name = "EVENT_NODE_FATAL_ERROR";
            }

            // Network

            if (msg.network != IntPtr.Zero)
            {
                var net_info = (zts_net_info_t) Marshal.PtrToStructure(msg.network, typeof(zts_net_info_t));
                newEvent      = new Event();
                newEvent.Code = msg.event_code;

                // Update network info as long as we aren't tearing down the network
                if (msg.event_code != Constants.EVENT_NETWORK_DOWN)
                {
                    var networkId = net_info.net_id;
                    var ni = _networks.GetOrAdd(networkId, new NetworkInfo());

                    newEvent.NetworkInfo                  = ni;
                    newEvent.NetworkInfo.Id               = net_info.net_id;
                    newEvent.NetworkInfo.MACAddress       = net_info.mac;
                    newEvent.NetworkInfo.Name             = System.Text.Encoding.UTF8.GetString(net_info.name);
                    newEvent.NetworkInfo.Status           = net_info.status;
                    newEvent.NetworkInfo.Type             = net_info.type;
                    newEvent.NetworkInfo.MTU              = net_info.mtu;
                    newEvent.NetworkInfo.DHCP             = net_info.dhcp;
                    newEvent.NetworkInfo.Bridge           = Convert.ToBoolean(net_info.bridge);
                    newEvent.NetworkInfo.BroadcastEnabled = Convert.ToBoolean(net_info.broadcast_enabled);

                    zts_core_lock_obtain();

                    // Get assigned addresses

                    var newAddrsDict =
                        new ConcurrentDictionary<string, IPAddress>();
                    var addrBuffer = Marshal.AllocHGlobal(Constants.INET6_ADDRSTRLEN);
                    var addr_count = zts_core_query_addr_count(networkId);

                    for (var idx = 0; idx < addr_count; idx++)
                    {
                        zts_core_query_addr(networkId, idx, addrBuffer, Constants.INET6_ADDRSTRLEN);
                        // Convert buffer to managed string
                        var str = Marshal.PtrToStringAnsi(addrBuffer);
                        var addr = IPAddress.Parse(str);
                        newAddrsDict[addr.ToString()] = addr;
                    }

                    // Update addresses in NetworkInfo object

                    // TODO: This update block works but could use a re-think, I think.
                    // Step 1. Remove addresses not present in new concurrent dict.
                    if (!ni._addrs.IsEmpty)
                    {
                        foreach (var key in ni._addrs.Keys)
                        {
                            if (!newAddrsDict.Keys.Contains(key)) ni._addrs.TryRemove(key, out _);
                        }
                    }
                    else
                        ni._addrs = newAddrsDict;

                    // Step 2. Add addresses not present in existing concurrent dict.
                    foreach (var key in newAddrsDict.Keys)
                    {
                        if (!ni._addrs.Keys.Contains(key)) ni._addrs[key] = newAddrsDict[key];
                    }

                    Marshal.FreeHGlobal(addrBuffer);
                    addrBuffer = IntPtr.Zero;

                    // Get managed routes

                    var newRoutesDict =
                        new ConcurrentDictionary<string, RouteInfo>();
                    var targetBuffer = Marshal.AllocHGlobal(Constants.INET6_ADDRSTRLEN);
                    var viaBuffer = Marshal.AllocHGlobal(Constants.INET6_ADDRSTRLEN);

                    var route_count = zts_core_query_route_count(networkId);

                    ushort flags = 0, metric = 0;

                    for (var idx = 0; idx < route_count; idx++)
                    {
                        zts_core_query_route(
                            networkId,
                            idx,
                            targetBuffer,
                            viaBuffer,
                            Constants.INET6_ADDRSTRLEN,
                            ref flags,
                            ref metric);

                        // Convert buffer to managed string

                        try
                        {
                            var targetStr = Marshal.PtrToStringAnsi(targetBuffer);
                            var targetAddr = IPAddress.Parse(targetStr);
                            var viaStr = Marshal.PtrToStringAnsi(viaBuffer);
                            var viaAddr = IPAddress.Parse(viaStr);
                            var route = new RouteInfo(targetAddr, viaAddr, flags, metric);
                            // Add to NetworkInfo object
                            newRoutesDict[targetStr] = route;
                        }
                        catch
                        {
                            Console.WriteLine("error while parsing route");
                        }
                    }

                    // TODO: This update block works but could use a re-think, I think.
                    // Step 1. Remove routes not present in new concurrent dict.
                    if (!ni._routes.IsEmpty)
                    {
                        foreach (var key in ni._routes.Keys)
                        {
                            if (!newRoutesDict.Keys.Contains(key)) ni._routes.TryRemove(key, out _);
                        }
                    }
                    else
                        ni._routes = newRoutesDict;

                    // Step 2. Add routes not present in existing concurrent dict.
                    foreach (var key in newRoutesDict.Keys)
                    {
                        if (!ni._routes.Keys.Contains(key)) ni._routes[key] = newRoutesDict[key];
                    }

                    Marshal.FreeHGlobal(targetBuffer);
                    Marshal.FreeHGlobal(viaBuffer);
                    targetBuffer = IntPtr.Zero;
                    viaBuffer    = IntPtr.Zero;

                    // Get multicast subscriptions

                    zts_core_lock_release();

                    // Update synthetic "readiness" value
                    ni.transportReady = route_count > 0 && addr_count > 0 ? true : false;
                } // EVENT_NETWORK_DOWN

                if (msg.event_code == Constants.EVENT_NETWORK_NOT_FOUND)
                    newEvent.Name = "EVENT_NETWORK_NOT_FOUND " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_REQ_CONFIG)
                    newEvent.Name = "EVENT_NETWORK_REQ_CONFIG " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_ACCESS_DENIED)
                    newEvent.Name = "EVENT_NETWORK_ACCESS_DENIED " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_READY_IP4)
                    newEvent.Name = "EVENT_NETWORK_READY_IP4 " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_READY_IP6)
                    newEvent.Name = "EVENT_NETWORK_READY_IP6 " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_DOWN)
                    newEvent.Name = "EVENT_NETWORK_DOWN " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_CLIENT_TOO_OLD)
                    newEvent.Name = "EVENT_NETWORK_CLIENT_TOO_OLD " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_REQ_CONFIG)
                    newEvent.Name = "EVENT_NETWORK_REQ_CONFIG " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_OK)
                    newEvent.Name = "EVENT_NETWORK_OK " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_ACCESS_DENIED)
                    newEvent.Name = "EVENT_NETWORK_ACCESS_DENIED " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_READY_IP4_IP6)
                    newEvent.Name = "EVENT_NETWORK_READY_IP4_IP6 " + net_info.net_id.ToString("x16");
                if (msg.event_code == Constants.EVENT_NETWORK_UPDATE)
                    newEvent.Name = "EVENT_NETWORK_UPDATE " + net_info.net_id.ToString("x16");
            }

            // Route

            if (msg.route != IntPtr.Zero)
            {
                var route_info =
                    (zts_route_info_t) Marshal.PtrToStructure(msg.route, typeof(zts_route_info_t));
                newEvent      = new Event();
                newEvent.Code = msg.event_code;
                // newEvent.RouteInfo = default;   // new RouteInfo();

                if (msg.event_code == Constants.EVENT_ROUTE_ADDED) newEvent.Name   = "EVENT_ROUTE_ADDED";
                if (msg.event_code == Constants.EVENT_ROUTE_REMOVED) newEvent.Name = "EVENT_ROUTE_REMOVED";
            }

            // Peer

            if (msg.peer != IntPtr.Zero)
            {
                var peer_info = (zts_peer_info_t) Marshal.PtrToStructure(msg.peer, typeof(zts_peer_info_t));
                newEvent      = new Event();
                newEvent.Code = msg.event_code;
                // newEvent.PeerInfo = default;   // new PeerInfo();

                if (peer_info.role == Constants.PEER_ROLE_PLANET) newEvent.Name  = "PEER_ROLE_PLANET";
                if (msg.event_code == Constants.EVENT_PEER_DIRECT) newEvent.Name = "EVENT_PEER_DIRECT";
                if (msg.event_code == Constants.EVENT_PEER_RELAY) newEvent.Name  = "EVENT_PEER_RELAY";
                // newEvent = new ZeroTier.Core.Event(msg.event_code,"EVENT_PEER_UNREACHABLE");
                if (msg.event_code == Constants.EVENT_PEER_PATH_DISCOVERED)
                    newEvent.Name = "EVENT_PEER_PATH_DISCOVERED";
                if (msg.event_code == Constants.EVENT_PEER_PATH_DEAD) newEvent.Name = "EVENT_PEER_PATH_DEAD";
            }

            // Address

            if (msg.addr != IntPtr.Zero)
            {
                var unmanagedDetails =
                    (zts_addr_info_t) Marshal.PtrToStructure(msg.addr, typeof(zts_addr_info_t));
                newEvent      = new Event();
                newEvent.Code = msg.event_code;
                // newEvent.AddressInfo = default;   // new AddressInfo();

                if (msg.event_code == Constants.EVENT_ADDR_ADDED_IP4) newEvent.Name   = "EVENT_ADDR_ADDED_IP4";
                if (msg.event_code == Constants.EVENT_ADDR_ADDED_IP6) newEvent.Name   = "EVENT_ADDR_ADDED_IP6";
                if (msg.event_code == Constants.EVENT_ADDR_REMOVED_IP4) newEvent.Name = "EVENT_ADDR_REMOVED_IP4";
                if (msg.event_code == Constants.EVENT_ADDR_REMOVED_IP6) newEvent.Name = "EVENT_ADDR_REMOVED_IP6";
            }

            // Storage

            if (msg.cache != IntPtr.Zero)
            {
                newEvent      = new Event();
                newEvent.Code = msg.event_code;
                // newEvent.AddressInfo = default;   // new AddressInfo();

                if (msg.event_code == Constants.EVENT_STORE_IDENTITY_SECRET)
                    newEvent.Name = "EVENT_STORE_IDENTITY_SECRET";
                if (msg.event_code == Constants.EVENT_STORE_IDENTITY_PUBLIC)
                    newEvent.Name = "EVENT_STORE_IDENTITY_PUBLIC";
                if (msg.event_code == Constants.EVENT_STORE_PLANET) newEvent.Name  = "EVENT_STORE_PLANET";
                if (msg.event_code == Constants.EVENT_STORE_PEER) newEvent.Name    = "EVENT_STORE_PEER";
                if (msg.event_code == Constants.EVENT_STORE_NETWORK) newEvent.Name = "EVENT_STORE_NETWORK";
            }

            // Pass the converted Event to the managed callback (visible to user)
            if (newEvent != null) _managedCallback(newEvent);
        }

        public List<NetworkInfo> Networks => new(_networks.Values);

        public List<PeerInfo> Peers => new(_peers.Values);

        public bool IsNetworkTransportReady(ulong networkId)
        {
            try
            {
                return _networks[networkId].transportReady
                       && _networks[networkId].Status == Constants.NETWORK_STATUS_OK;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public List<IPAddress> GetNetworkAddresses(ulong networkId)
        {
            try
            {
                return new List<IPAddress>(_networks[networkId].Addresses);
            }
            catch (KeyNotFoundException)
            {
            }

            return new List<IPAddress>();
        }

        public List<RouteInfo> GetNetworkRoutes(ulong networkId)
        {
            try
            {
                return new List<RouteInfo>(_networks[networkId].Routes);
            }
            catch (KeyNotFoundException)
            {
            }

            return new List<RouteInfo>();
        }

        /*
                public NetworkInfo GetNetwork(ulong networkId)
                {
                    try {
                        return _networks[networkId];
                        } catch (KeyNotFoundException) {

                        }
                        return null;
                }
        */

        public int Start()
        {
            if (_hasBeenFreed == true)
            {
                throw new ObjectDisposedException(
                    "ZeroTier Node has previously been freed. Restart application to create new instance.");
            }

            return zts_node_start();
        }

        public int Free()
        {
            _id           = 0x0;
            _hasBeenFreed = true;
            ClearNode();
            return zts_node_free();
        }

        public int Stop()
        {
            _id = 0x0;
            ClearNode();
            return zts_node_stop();
        }

        public int Join(ulong networkId) =>
            /* The NetworkInfo object will only be added to _networks and populated when a
            response from the controller is received */
            zts_net_join(networkId);

        public int Leave(ulong networkId)
        {
            int res = Constants.ERR_OK;
            if (zts_net_leave(networkId) == Constants.ERR_OK) _networks.TryRemove(networkId, out _);
            return res;
        }

        public bool Online => _isOnline;

        public ulong Id => _id;

        public string IdString => _id.ToString("x16").TrimStart('0');

        public string IdStr => string.Format("{0}", _id.ToString("x16"));

        public ushort PrimaryPort => _primaryPort;

        public ushort SecondaryPort => _secondaryPort;

        public ushort TertiaryPort => _tertiaryPort;

        public string Version => string.Format("{0}.{1}.{2}", _versionMajor, _versionMinor, _versionRev);

        // id

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_id_new")]
        private static extern int
            zts_id_new(string arg1, IntPtr arg2);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_id_pair_is_valid")]
        private static extern int zts_id_pair_is_valid(string arg1, int arg2);

        // init

        [DllImport("libzt", EntryPoint = "CSharp_zts_init_allow_net_cache")]
        private static extern int zts_init_allow_net_cache(int arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_init_allow_peer_cache")]
        private static extern int zts_init_allow_peer_cache(int arg1);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_init_from_storage")]
        private static extern int zts_init_from_storage(string arg1);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_init_from_memory")]
        private static extern int zts_init_from_memory(string arg1, ushort arg2);

        [DllImport("libzt", EntryPoint = "CSharp_zts_init_set_event_handler")]
        private static extern int zts_init_set_event_handler(CSharpCallbackWithStruct arg1);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_init_blacklist_if")]
        private static extern int zts_init_blacklist_if(string arg1, int arg2);

        [DllImport("libzt", EntryPoint = "CSharp_zts_init_set_roots")]
        private static extern int zts_init_set_roots(IntPtr roots_data, int len);

        [DllImport("libzt", EntryPoint = "CSharp_zts_init_set_port")]
        private static extern int zts_init_set_port(ushort arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_init_set_random_port_range")]
        private static extern int zts_init_set_random_port_range(ushort arg1, ushort arg2);

        [DllImport("libzt", EntryPoint = "CSharp_zts_init_allow_secondary_port")]
        private static extern int zts_init_allow_secondary_port(int arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_init_allow_port_mapping")]
        private static extern int zts_init_allow_port_mapping(int arg1);

        // Core query API

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_lock_obtain")]
        private static extern int zts_core_lock_obtain();

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_lock_release")]
        private static extern int zts_core_lock_release();

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_query_addr_count")]
        private static extern int zts_core_query_addr_count(ulong net_id);

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_query_addr")]
        private static extern int zts_core_query_addr(ulong net_id, int idx, IntPtr dst, int len);

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_query_route_count")]
        private static extern int zts_core_query_route_count(ulong net_id);

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_query_route")]
        private static extern int zts_core_query_route(
            ulong net_id,
            int idx,
            IntPtr target,
            IntPtr via,
            int len,
            ref ushort flags,
            ref ushort metric);

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_query_path_count")]
        private static extern int zts_core_query_path_count(ulong peer_id);

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_query_path")]
        private static extern int zts_core_query_path(ulong peer_id, int idx, IntPtr dst, int len);

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_query_mc_count")]
        private static extern int zts_core_query_mc_count(ulong net_id);

        [DllImport("libzt", EntryPoint = "CSharp_zts_core_query_mc")]
        private static extern int zts_core_query_mc(ulong net_id, int idx, ref ulong mac, ref uint adi);

        // addr

        [DllImport("libzt", EntryPoint = "CSharp_zts_addr_is_assigned")]
        private static extern int zts_addr_is_assigned(ulong arg1, int arg2);

        [DllImport("libzt", EntryPoint = "CSharp_zts_addr_get")]
        private static extern int zts_addr_get(ulong arg1, int arg2, IntPtr arg3);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_addr_get_str")]
        private static extern int zts_addr_get_str(ulong arg1, int arg2, IntPtr arg3, int arg4);

        [DllImport("libzt", EntryPoint = "CSharp_zts_addr_get_all")]
        private static extern int zts_addr_get_all(ulong arg1, IntPtr arg2, IntPtr arg3);

        [DllImport("libzt", EntryPoint = "CSharp_zts_addr_compute_6plane")]
        private static extern int zts_addr_compute_6plane(ulong arg1, ulong arg2, IntPtr arg3);

        [DllImport("libzt", EntryPoint = "CSharp_zts_addr_compute_rfc4193")]
        private static extern int zts_addr_compute_rfc4193(ulong arg1, ulong arg2, IntPtr arg3);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_addr_compute_rfc4193_str")]
        private static extern int zts_addr_compute_rfc4193_str(ulong arg1, ulong arg2, string arg3, int arg4);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_addr_compute_6plane_str")]
        private static extern int zts_addr_compute_6plane_str(ulong arg1, ulong arg2, string arg3, int arg4);

        /*
                [DllImport("libzt", EntryPoint="CSharp_zts_get_6plane_addr")]
                static extern int zts_get_6plane_addr(IntPtr arg1, ulong arg2, ulong arg3);

                [DllImport("libzt", EntryPoint="CSharp_zts_get_rfc4193_addr")]
                static extern int zts_get_rfc4193_addr(IntPtr arg1, ulong arg2, ulong arg3);
        */

        // net

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_compute_adhoc_id")]
        public static extern ulong zts_net_compute_adhoc_id(ushort arg1, ushort arg2);

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_join")]
        private static extern int zts_net_join(ulong arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_leave")]
        private static extern int zts_net_leave(ulong arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_transport_is_ready")]
        private static extern int zts_net_transport_is_ready(ulong arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_get_mac")]
        public static extern ulong zts_net_get_mac(ulong arg1);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_net_get_mac_str")]
        private static extern int zts_net_get_mac_str(ulong arg1, string arg2, int arg3);

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_get_broadcast")]
        private static extern int zts_net_get_broadcast(ulong arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_get_mtu")]
        private static extern int zts_net_get_mtu(ulong arg1);

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_net_get_name")]
        private static extern int zts_net_get_name(ulong arg1, string arg2, int arg3);

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_get_status")]
        private static extern int zts_net_get_status(ulong arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_net_get_type")]
        private static extern int zts_net_get_type(ulong arg1);

        // route

        [DllImport("libzt", EntryPoint = "CSharp_zts_route_is_assigned")]
        private static extern int zts_route_is_assigned(ulong arg1, int arg2);

        // node

        [DllImport("libzt", EntryPoint = "CSharp_zts_node_start")]
        private static extern int zts_node_start();

        [DllImport("libzt", EntryPoint = "CSharp_zts_node_is_online")]
        private static extern int zts_node_is_online();

        [DllImport("libzt", EntryPoint = "CSharp_zts_node_get_id")]
        public static extern ulong zts_node_get_id();

        [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_node_get_id_pair")]
        private static extern int zts_node_get_id_pair(string arg1, IntPtr arg2);

        [DllImport("libzt", EntryPoint = "CSharp_zts_node_get_port")]
        private static extern int zts_node_get_port();

        [DllImport("libzt", EntryPoint = "CSharp_zts_node_stop")]
        private static extern int zts_node_stop();

        [DllImport("libzt", EntryPoint = "CSharp_zts_node_free")]
        private static extern int zts_node_free();

        // moon

        [DllImport("libzt", EntryPoint = "CSharp_zts_moon_orbit")]
        private static extern int zts_moon_orbit(ulong arg1, ulong arg2);

        [DllImport("libzt", EntryPoint = "CSharp_zts_moon_deorbit")]
        private static extern int zts_moon_deorbit(ulong arg1);

        // util

        [DllImport("libzt", EntryPoint = "CSharp_zts_util_delay")]
        private static extern void zts_util_delay(int arg1);

        [DllImport("libzt", EntryPoint = "CSharp_zts_errno_get")]
        private static extern int zts_errno_get();

        [StructLayout(LayoutKind.Sequential)]
        private struct zts_node_info_t
        {
            public ulong node_id;
            public ushort primary_port;
            public ushort secondary_port;
            public ushort tertiary_port;
            public byte ver_major;
            public byte ver_minor;
            public byte ver_rev;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct zts_net_info_t
        {
            public ulong net_id;
            public ulong mac;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] name;

            public int status;
            public int type;
            public uint mtu;
            public int dhcp;
            public int bridge;
            public int broadcast_enabled;
            public int port_error;

            public ulong netconf_rev;
            // address, routes, and multicast subs are retrieved using zts_core_ API
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct zts_route_info_t
        {
            private IntPtr target;
            private IntPtr via;
            private ushort flags;
            private ushort metric;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct zts_addr_info_t
        {
            private ulong nwid;
            private IntPtr addr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct zts_peer_info_t
        {
            public ulong address;
            public int ver_major;
            public int ver_minor;
            public int ver_rev;
            public int latency;

            public byte role;
            // TODO
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct zts_event_msg_t
        {
            public short event_code;
            public IntPtr node;
            public IntPtr network;
            public IntPtr netif;
            public IntPtr route;
            public IntPtr peer;
            public IntPtr addr;
            public IntPtr cache;
            public int len;
        }

        public static int ErrNo => zts_errno_get();
    }
}