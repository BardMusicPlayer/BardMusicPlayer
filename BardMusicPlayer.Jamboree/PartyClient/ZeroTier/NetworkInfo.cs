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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace BardMusicPlayer.Jamboree.PartyClient.ZeroTier
{
    internal class NetworkInfo
    {
        public ulong Id { get; set; }

        public ulong MACAddress;
        public string Name;
        public int Status;
        public int Type;
        public uint MTU;
        public int DHCP;
        public bool Bridge;
        public bool BroadcastEnabled;
        internal bool transportReady; // Synthetic value

        public bool IsPrivate => Type == Constants.NETWORK_TYPE_PRIVATE;

        public bool IsPublic => Type == Constants.NETWORK_TYPE_PUBLIC;

        internal ConcurrentDictionary<string, IPAddress> _addrs = new();

        public ICollection<IPAddress> Addresses => _addrs.Values;

        internal ConcurrentDictionary<string, RouteInfo> _routes =
            new();

        public ICollection<RouteInfo> Routes => _routes.Values;
    }
}