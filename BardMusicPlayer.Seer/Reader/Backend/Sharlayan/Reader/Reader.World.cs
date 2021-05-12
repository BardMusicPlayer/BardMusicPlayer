/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal partial class Reader
    {
        public bool CanGetWorld() => Scanner.Locations.ContainsKey(Signatures.WorldKey);
        public string GetWorld() {
            if (!CanGetWorld() || !MemoryHandler.IsAttached) return string.Empty;

            var worldMap = (IntPtr) Scanner.Locations[Signatures.WorldKey];
            try {
                var world = MemoryHandler.GetString(worldMap, MemoryHandler.Structures.World.Offset, MemoryHandler.Structures.World.SourceSize);
                return world;
            } catch(Exception ex) {
                MemoryHandler?.RaiseException(ex);
            }
            return string.Empty;
        }
    }
}
