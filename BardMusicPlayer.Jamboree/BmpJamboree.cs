/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Jamboree.PartyClient.PartyManagement;

namespace BardMusicPlayer.Jamboree
{
    public partial class BmpJamboree : IDisposable
    {
        private Pydna _pydna;

#region Instance Constructor/Destructor
        private static readonly Lazy<BmpJamboree> LazyInstance = new(() => new BmpJamboree());

        /// <summary>
        /// 
        /// </summary>
        public bool Started { get; private set; }


        private BmpJamboree()
        {
            _pydna = new Pydna();
        }

        public static BmpJamboree Instance => LazyInstance.Value;

        /// <summary>
        /// Start the eventhandler
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            if (Started) return;
            StartEventsHandler();
            Started = true;
        }

        /// <summary>
        /// Stop the eventhandler
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            if (!Started) return;
            StopEventsHandler();
            Started = false;
        }

        ~BmpJamboree() { Dispose(); }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
#endregion

        public void JoinParty(string networkId, byte type, string name)
        {
            _pydna ??= new Pydna();
            Task.Run(() => _pydna.JoinParty(networkId, type, name));
        }

        public void LeaveParty()
        {
            _pydna ??= new Pydna();
            _pydna.LeaveParty();
        }

        public void SendPerformanceStart()
        {
            _pydna ??= new Pydna();
            Pydna.SendPerformanceStart();
        }

        /// <summary>
        /// Send we joined the party
        /// | type 0 = bard
        /// | type 1 = dancer
        /// </summary>
        /// <param name="type"></param>
        /// <param name="performer_name"></param>
        public void SendPerformerJoin(byte type, string performer_name)
        {
            _pydna ??= new Pydna();
            _pydna.SendPerformerJoin(type, performer_name);
        }

        public void SendClientPacket(byte[] packet)
        {
            _pydna?.SendClientPacket(packet);
        }

        public void SendServerPacket(byte [] packet)
        {
            _pydna?.SendServerPacket(packet);
        }

        public List<PartyClientInfo> GetPartyMembers()
        {
            return PartyManager.Instance.GetPartyMembers();
        }

    }
}
