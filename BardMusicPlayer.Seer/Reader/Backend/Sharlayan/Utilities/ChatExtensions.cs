/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities
{
    internal static class ChatExtensions
    {
        private static readonly Regex Romans = new("(?<roman>\\b[IVXLCDM]+\\b)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static readonly Regex Titles = new("(?<num>\\d+)(?<designator>\\w+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static readonly Regex CleanSpaces = new("[ ]+", RegexOptions.Compiled);

        internal static string TrimAndCleanSpaces(this string source) => CleanSpaces.Replace(source, " ").Trim();
        internal static string FromHex(this string source)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i <= source.Length - 2; i += 2)
                stringBuilder.Append(Convert.ToChar(int.Parse(source.Substring(i, 2), NumberStyles.HexNumber)));
            return stringBuilder.ToString();
        }
        internal static string ToTitleCase(this string source, bool all = true)
        {
            if (string.IsNullOrWhiteSpace(source.Trim())) return string.Empty;
            var text = source.TrimAndCleanSpaces();
            var text2 = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(all ? text.ToLower() : text);
            var match = Romans.Match(text);
            if (match.Success)
            {
                var text3 = Convert.ToString(match.Groups["roman"].Value);
                var oldValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text3.ToLower());
                text2 = text2.Replace(oldValue, text3.ToUpper());
            }

            var matchCollection = Titles.Matches(text2);
            foreach (Match item in matchCollection)
            {
                var text4 = Convert.ToString(item.Groups["num"].Value);
                var text5 = Convert.ToString(item.Groups["designator"].Value);
                text2 = text2.Replace(text4 + text5, text4 + text5.ToLower());
            }
            return text2;
        }
    }
}
