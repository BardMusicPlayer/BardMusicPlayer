/*
 * Copyright(c) 2018 OpportunityLiu
 * Licensed under Apache License, Version 2.0. See https://raw.githubusercontent.com/OpportunityLiu/LrcParser/master/LICENSE for full license information.
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