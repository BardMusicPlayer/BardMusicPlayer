/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal partial class Reader
    {
        public bool CanGetCharacterId() => Scanner.Locations.ContainsKey(Signatures.CharacterIdKey);

        public string GetCharacterId()
        {
            var id = "";
            if (!CanGetCharacterId() || !MemoryHandler.IsAttached) return id;

            var characterIdMap = (IntPtr) Scanner.Locations[Signatures.CharacterIdKey];

            try
            {
                id = MemoryHandler.GetString(characterIdMap, MemoryHandler.Structures.CharacterId.Offset,
                    MemoryHandler.Structures.CharacterId.SourceSize);
            }
            catch (Exception ex)
            {
                MemoryHandler?.RaiseException(ex);
            }

            return id;
        }
    }
}