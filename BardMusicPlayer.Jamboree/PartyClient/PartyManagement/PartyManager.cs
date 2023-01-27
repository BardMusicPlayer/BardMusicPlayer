using System;
using System.Collections.Generic;

namespace BardMusicPlayer.Jamboree.PartyClient.PartyManagement
{
    class PartyManager
    {
        public List<PartyClientInfo> GetPartyMembers() { return _partyClients; }
        private List<PartyClientInfo> _partyClients = new List<PartyClientInfo>();

#region Instance Constructor/Destructor
        private static readonly Lazy<PartyManager> LazyInstance = new(() => new PartyManager());
        private PartyManager()
        {
            _partyClients.Clear();
        }

        public static PartyManager Instance => LazyInstance.Value;

        ~PartyManager() { Dispose(); }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
#endregion

        public void Add(PartyClientInfo client)
        {
            foreach(PartyClientInfo info in _partyClients)
                if (info.Performer_Name == client.Performer_Name)
                    return;
            _partyClients.Add(client);
        }

        public void Remove(PartyClientInfo client)
        {
            _partyClients.Remove(client);
        }



    }
}
