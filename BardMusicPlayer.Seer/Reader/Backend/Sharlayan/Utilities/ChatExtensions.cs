/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Globalization;
using System.Text;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities
{
    internal static class ChatExtensions
    {        
        internal static string FromHex(this string source)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i <= source.Length - 2; i += 2)
                stringBuilder.Append(Convert.ToChar(int.Parse(source.Substring(i, 2), NumberStyles.HexNumber)));
            return stringBuilder.ToString();
        }
    }
}
