/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Transmogrify.Song.Importers.LRC;

internal static class Helper
{
    public static void CheckString(string paramName, string value, char[] invalidChars)
    {
        var i = value.IndexOfAny(invalidChars);
        if (i >= 0)
            throw new ArgumentException($"Invalid char '{value[i]}' at index: {i}", paramName);
    }
}