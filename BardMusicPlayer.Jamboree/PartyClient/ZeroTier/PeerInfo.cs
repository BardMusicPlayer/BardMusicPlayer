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
    internal class PeerInfo
    {
        public PeerInfo(ulong id, short versionMajor, short versionMinor, short versionRev, byte role)
        {
            _id           = id;
            _versionMajor = versionMajor;
            _versionMinor = versionMinor;
            _versionRev   = versionRev;
            _role         = role;
        }

        private ulong _id;
        private short _versionMajor;
        private short _versionMinor;
        private short _versionRev;
        private byte _role;
        internal ConcurrentDictionary<string, IPEndPoint> _paths = new();

        public ICollection<IPEndPoint> Paths => _paths.Values;

        public ulong Id => _id;

        public byte Role => _role;

        public string Version => string.Format("{0}.{1}.{2}", _versionMajor, _versionMinor, _versionRev);
    }
}