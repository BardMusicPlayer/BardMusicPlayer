/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Collections.Generic;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal partial class Reader
    {
        public bool CanGetPartyMembers() => Scanner.Locations.ContainsKey(Signatures.CharacterMapKey) && Scanner.Locations.ContainsKey(Signatures.PartyMapKey) && Scanner.Locations.ContainsKey(Signatures.PartyCountKey);
        public SortedDictionary<uint, string> GetPartyMembers()
        {
            var result = new SortedDictionary<uint, string>();

            if (!CanGetPartyMembers() || !MemoryHandler.IsAttached) return result;
            
            var partyInfoMap = (IntPtr) Scanner.Locations[Signatures.PartyMapKey];
            var partyCountMap = Scanner.Locations[Signatures.PartyCountKey];

            try {
                var partyCount = MemoryHandler.GetByte(partyCountMap);
                var sourceSize = MemoryHandler.Structures.PartyMember.SourceSize;

                if (partyCount > 1 && partyCount < 9) {
                    for (uint i = 0; i < partyCount; i++) {
                        var address = partyInfoMap.ToInt64() + i * (uint) sourceSize;
                        var source = MemoryHandler.GetByteArray(new IntPtr(address), sourceSize);

                        var actorId = SBitConverter.TryToUInt32(source, MemoryHandler.Structures.PartyMember.ID);
                        var playerName = MemoryHandler.GetStringFromBytes(source, MemoryHandler.Structures.PartyMember.Name);
                        if(ActorIdTools.RangeOkay(actorId) && !string.IsNullOrEmpty(playerName)) result[actorId] = playerName;
                    }
                }
                if (result.Count == 1) result.Clear();
            }
            catch (Exception ex) {
                MemoryHandler?.RaiseException(ex);
            }

            return result;
        }
    }
}
