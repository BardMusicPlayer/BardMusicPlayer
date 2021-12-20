/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;

namespace BardMusicPlayer.Jamboree.PartyClient.WebSocket
{
    internal class WebSocketPartyClient : IPartyClient
    {
        internal WebSocketPartyClient()
        {
        }

        public string RequestParty()
        {
            throw new System.NotImplementedException();
        }

        public bool JoinParty(string party)
        {
            throw new System.NotImplementedException();
        }

        public List<string> RequestPartyMembers()
        {
            throw new System.NotImplementedException();
        }

        public event IPartyClient.PartyMembersChangedHandler PartyMembersChanged
        {
            add { throw new System.NotImplementedException(); }
            remove { throw new System.NotImplementedException(); }
        }

        public bool AssignPartyLeader(string partyMember)
        {
            throw new System.NotImplementedException();
        }

        public string RequestPartyLeader()
        {
            throw new System.NotImplementedException();
        }

        public event IPartyClient.PartyLeaderChangedHandler PartyLeaderChanged
        {
            add { throw new System.NotImplementedException(); }
            remove { throw new System.NotImplementedException(); }
        }

        public List<PartyGame> RequestPartyGames(string partyMember)
        {
            throw new System.NotImplementedException();
        }

        public event IPartyClient.PartyGamesChangedHandler PartyGamesChanged
        {
            add { throw new System.NotImplementedException(); }
            remove { throw new System.NotImplementedException(); }
        }

        public void Dispose()
        {
        }
    }
}
