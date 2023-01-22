﻿using System.Collections.Generic;
using BardMusicPlayer.Jamboree.PartyNetworking.Server_Client;

namespace BardMusicPlayer.Jamboree.PartyClient.PartyManagement
{
    public class PartyClientInfo
    {
        /// <summary>
        /// Is this session a (0) bard, (1) dancer
        /// </summary>
        public byte Performer_Type { get; set; } = 254;
        public string Performer_Name { get; set; } = "Unknown";


        private Queue<NetworkPacket> _inPackets = new Queue<NetworkPacket>();

        public PartyClientInfo()
        {}

        public void AddPacket(NetworkPacket packet)
        {
            _inPackets.Enqueue(packet);
        }
    }
}
