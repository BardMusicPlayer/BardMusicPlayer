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
    internal sealed partial class Reader
    {
        public KeyValuePair<uint, string> GetPartyLeader()
        {
            KeyValuePair<uint, string> result = new KeyValuePair<uint, string >();

            if (!CanGetPartyMembers() || !MemoryHandler.IsAttached) return result;

            var partyInfoMap = (IntPtr)Scanner.Locations[Signatures.PartyMapKey];
            var partyLeadMap = (IntPtr)Scanner.Locations[Signatures.PartyLeadKey];
            var partyCountMap = (IntPtr)Scanner.Locations[Signatures.PartyCountKey];
            try
            {
                var partyCount = MemoryHandler.GetByte(partyCountMap);
                var sourceSize = MemoryHandler.Structures.PartyMember.SourceSize;

                if (partyCount is > 1 and < 9)
                    for (uint i = 0; i < partyCount; i++)
                    {
                        var address = partyInfoMap.ToInt64() + i * (uint)sourceSize;
                        var source = MemoryHandler.GetByteArray(new IntPtr(address), sourceSize);
                        var actorId = SBitConverter.TryToUInt32(source, MemoryHandler.Structures.PartyMember.ID);
                        var playerName = MemoryHandler.GetStringFromBytes(source, MemoryHandler.Structures.PartyMember.Name);
                        if (ActorIdTools.RangeOkay(actorId) && !string.IsNullOrEmpty(playerName))
                        {
                            if(MemoryHandler.GetByte(partyLeadMap) == i)
                            {
                                result = new KeyValuePair<uint, string>(actorId, playerName);
                                break;
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                MemoryHandler?.RaiseException(ex);
            }

            return result;
        }
    }
}