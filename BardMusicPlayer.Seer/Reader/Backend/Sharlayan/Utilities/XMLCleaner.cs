/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System.Linq;
using System.Text;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities
{
    internal static class XMLCleaner
    {
        public static string SanitizeXmlString(string xValue)
        {
            if (xValue == null) return string.Empty;

            var stringBuilder = new StringBuilder(xValue.Length);

            foreach (var item in xValue.Where(xChar => IsLegalXmlChar(xChar)))
            {
                stringBuilder.Append(item);
            }

            return stringBuilder.ToString();
        }

        private static bool IsLegalXmlChar(int xChar) => xChar == 9 || xChar == 10 || xChar == 13 ||
                                                         xChar >= 32 && xChar <= 55295 ||
                                                         xChar >= 57344 && xChar <= 65533 ||
                                                         xChar >= 65536 && xChar <= 1114111;
    }
}