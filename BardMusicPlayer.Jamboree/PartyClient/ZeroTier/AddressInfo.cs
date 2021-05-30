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

using System.Net;

namespace BardMusicPlayer.Jamboree.PartyClient.ZeroTier
{
    internal class AddressInfo
    {
        public AddressInfo(IPAddress addr, ulong net_id)
        {
            _addr   = addr;
            _net_id = net_id;
        }

        public ulong _net_id;
        public IPAddress _addr;

        public IPAddress Address => _addr;

        public ulong NetworkId => _net_id;
    }
}