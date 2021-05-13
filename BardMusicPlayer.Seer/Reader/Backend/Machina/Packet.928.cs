/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Text;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal partial class Packet
    {
        /// <summary>
        /// Contains contentId -> PlayerName.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="otherActorId"></param>
        /// <param name="myActorId"></param>
        /// <param name="message"></param>
        internal void Size928(long timeStamp, uint otherActorId, uint myActorId, byte[] message)
        {
            try
            {
                if (otherActorId != myActorId) return;
                var myZoneId = uint.MaxValue;
                var myWorldId = uint.MaxValue;
                var partyMembers = new SortedDictionary<uint, string>();
                for (var i = 0; i <= 616; i += 88)
                {
                    var contentId = BitConverter.ToUInt64(message, 48 + i);
                    if (contentId == 0) continue; // The party member is not online.

                    if (!_contentId2ActorId.ContainsKey(contentId)) return; // Missing cache. Unable to parse this packet.
                    
                    var actorId = _contentId2ActorId[contentId];
                    if (actorId == 0)
                    {
                        if (i == 0) return; // The first player should always have a contentId.
                        continue; // This player is too far away for us to consider them "in party."
                    }

                    var playerName = Encoding.UTF8.GetString(message, 95 + i, 32).Trim((char) 0);
                    uint currentZoneId = BitConverter.ToUInt16(message, 60 + i);
                    uint currentWorldId = BitConverter.ToUInt16(message, 62 + i);

                    switch (i)
                    {
                        case 0 when actorId != myActorId: // The first ActorId should always be this Game's ActorId.
                            return;
                        case 0 when actorId == myActorId: // Store location of this Game for lookup later.
                            myZoneId = currentZoneId;
                            myWorldId = currentWorldId;
                            break;
                        default:
                            if (myZoneId != currentZoneId || myWorldId != currentWorldId) continue; // The player is in a different location.
                            break;
                    }

                    partyMembers.Add(actorId, playerName);
                }
                if (partyMembers.Count == 1) partyMembers.Clear(); // No party members nearby. Seer only accepts an empty collection for this case.
                _machinaReader.ReaderHandler.Game.PublishEvent(new PartyMembersChanged(EventSource.Machina, partyMembers));
            }
            catch (Exception ex)
            {
                _machinaReader.ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.Machina, new BmpSeerMachinaException("Exception in Packet.Size928 (party): " + ex.Message)));
            }
        }
    }
}