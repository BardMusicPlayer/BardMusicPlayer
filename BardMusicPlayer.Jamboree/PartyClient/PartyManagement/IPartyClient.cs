/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using BardMusicPlayer.Jamboree.PartyNetworking.Server_Client;

namespace BardMusicPlayer.Jamboree.PartyClient.PartyManagement;

internal interface IPartyClient : IDisposable
{
    /// <summary>
    /// asks for a new party uuid
    /// </summary>
    /// <returns></returns>
    string RequestParty();

    /// <summary>
    /// joins a party
    /// </summary>
    /// <param name="party">string uuid</param>
    /// <returns>true if successful</returns>
    bool JoinParty(string party);
        
    /// <summary>
    /// requests a list of party members.
    /// </summary>
    /// <returns>list of string uuid</returns>
    List<string> RequestPartyMembers();

    /// <summary>
    /// Fired when the PartyMembers changes
    /// </summary>
    /// <param name="partyMembers"></param>
    delegate void PartyMembersChangedHandler(List<string> partyMembers);

    /// <summary>
    /// Fired when the PartyMembers changes
    /// </summary>
    event PartyMembersChangedHandler PartyMembersChanged;
        
    /// <summary>
    /// assigns someone else to be the party leader.
    /// </summary>
    /// <param name="partyMember">string uuid</param>
    /// <returns>true if successful</returns>
    bool AssignPartyLeader(string partyMember);
        
    /// <summary>
    /// returns the current party leader.
    /// </summary>
    /// <returns>uuid of current party leader</returns>
    string RequestPartyLeader();

    /// <summary>
    /// Fired when the PartyLeader changes
    /// </summary>
    /// <param name="partyLeader"></param>
    delegate void PartyLeaderChangedHandler(string partyLeader);

    /// <summary>
    /// Fired when the PartyLeader changes
    /// </summary>
    event PartyLeaderChangedHandler PartyLeaderChanged;

    /// <summary>
    /// requests all PartyGame structs from a partyMember.
    /// </summary>
    /// <param name="partyMember">string uuid</param>
    /// <returns>list of games</returns>
    List<NetworkSocket> RequestPartyGames(string partyMember);

    /// <summary>
    /// Fired when the PartyGame list changes in a PartyMember
    /// </summary>
    /// <param name="partyMember"></param>
    /// <param name="partyGames"></param>
    delegate void PartyGamesChangedHandler(string partyMember, List<NetworkSocket> partyGames);

    /// <summary>
    /// Fired when the PartyGame list changes in a PartyMember
    /// </summary>
    event PartyGamesChangedHandler PartyGamesChanged;

}