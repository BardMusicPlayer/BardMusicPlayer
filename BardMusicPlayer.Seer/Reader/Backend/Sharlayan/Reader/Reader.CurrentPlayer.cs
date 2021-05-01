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
        public bool CanGetPlayerInfo() => Scanner.Locations.ContainsKey(Signatures.PlayerInformationKey);
        public KeyValuePair<uint,string> GetCurrentPlayer() {
            var result = new KeyValuePair<uint,string>();

            if (!CanGetPlayerInfo() || !MemoryHandler.IsAttached) return result;

            var playerInfoMap = (IntPtr) Scanner.Locations[Signatures.PlayerInformationKey];

            if (playerInfoMap.ToInt64() <= 6496)  return result;

            try
            {
                var source = MemoryHandler.GetByteArray(playerInfoMap, MemoryHandler.Structures.CurrentPlayer.SourceSize);
                var actorId = SBitConverter.TryToUInt32(source, MemoryHandler.Structures.CurrentPlayer.ID);
                var playerName = MemoryHandler.GetStringFromBytes(source, MemoryHandler.Structures.CurrentPlayer.Name);
                
                if(ActorIdTools.RangeOkay(actorId) && !string.IsNullOrEmpty(playerName)) result = new KeyValuePair<uint, string>(actorId, playerName);
            }
            catch (Exception ex) {
                MemoryHandler.RaiseException(ex);
            }
            return result;
        }
    }
}
