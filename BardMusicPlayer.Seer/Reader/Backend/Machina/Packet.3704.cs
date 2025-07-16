/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Text;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal sealed partial class Packet
    {
        /// <summary>
        ///     Contains contentId -> PlayerName.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size3704(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;

                var myZoneId = uint.MaxValue;
                var myWorldId = uint.MaxValue;
                var partyMembers = new SortedDictionary<uint, string>();
                var currentPartyLead = new KeyValuePair<uint, string>();

                byte currentLeader = (byte)BitConverter.ToInt16(message, 3696);
                byte partySize = (byte)BitConverter.ToInt16(message, 3697);

                for (var i = 0; i <= 3136; i += 456)
                {
                    //Check for empty column
                    var actorId = (UInt32)BitConverter.ToUInt16(message, 88 + i) - message[92 + i] + ((UInt32)BitConverter.ToUInt16(message, 90 + i) * 0x10000 ); //What the...
                    // OLD: BitConverter.ToUInt32(message, 88 + i);
                    if (actorId == 0)
                        continue;

                    var playerName = Encoding.UTF8.GetString(message, 32 + i, 32).Trim((char)0);
                    uint currentZoneId = BitConverter.ToUInt16(message, 110 + i);
                    uint homeWorldId = BitConverter.ToUInt16(message, 112 + i);
                    //Change me
                    switch (i)
                    {
                        case 0 when actorId != myActorId: // The first ActorId should always be this Game's ActorId.
                            return;
                        case 0 when actorId == myActorId: // Store location of this Game for lookup later.
                            myZoneId = currentZoneId;
                            myWorldId = homeWorldId;
                            break;
                        default:
                            if (myZoneId != currentZoneId)
                                continue; // The player is in a different location.

                            break;
                    }

                    //Get the party lead
                    if (i / 456 == (currentLeader))
                        currentPartyLead = new KeyValuePair<uint, string>(actorId, playerName);

                    partyMembers.Add(actorId, playerName);
                }

                if (partyMembers.Count != partySize)
                    // No party members nearby. Seer only accepts an empty collection for this case.
                    partyMembers.Clear();
                else
                    _machinaReader.Game.PublishEvent(new PartyLeaderChanged(EventSource.Machina, currentPartyLead));

                _machinaReader.Game.PublishEvent(new PartyMembersChanged(EventSource.Machina, partyMembers));
            }
            catch (Exception ex)
            {
                _machinaReader.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina,
                    new BmpSeerMachinaException("Exception in Packet.Size3704 (party): " + ex.Message)));
            }
        }
    }
}