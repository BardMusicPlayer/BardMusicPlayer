/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader;

internal sealed partial class Reader
{
    public bool CanGetHomeWorld()
    {
        return Scanner.Locations.ContainsKey(Signatures.WorldKey);
    }

    public string GetHomeWorld()
    {
        if (!CanGetHomeWorld() || !MemoryHandler.IsAttached) return string.Empty;

        var worldMap = (IntPtr)Scanner.Locations[Signatures.WorldKey];
        try
        {
            var source = MemoryHandler.GetByteArray(worldMap, MemoryHandler.Structures.World.SourceSize);
            var homeworld = ""; // MemoryHandler.GetStringFromBytes(source, MemoryHandler.Structures.World.HomeWorld);
            return homeworld;
        }
        catch (Exception ex)
        {
            MemoryHandler?.RaiseException(ex);
        }

        return string.Empty;
    }
}