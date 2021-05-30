/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Utilities
{
    internal static class KeyDictionary
    {
        internal static readonly IReadOnlyDictionary<int, int> MainKeyMap = new Dictionary<int, int>
        {
            { 130, 187 }, // =
            { 131, 188 }, // ,
            { 132, 189 }, // ~
            { 133, 190 }, // .
            { 134, 191 }, // /
            { 135, 186 }, // ;
            { 136, 192 }, // '
            { 137, 219 }, // [
            { 138, 220 }, // \
            { 139, 221 }, // ]
            { 140, 222 }, // #
            { 141, 223 }  // `
        };

        internal static readonly IReadOnlyDictionary<string, string> OemKeyFix = new Dictionary<string, string>
        {
            { "OemQuestion", "?" },
            { "Oemplus", "+" },
            { "Oem5", @"\" },
            { "OemPeriod", "." },
            { "Oemcomma", "," },
            { "OemMinus", "-" },
            { "Oem8", "`" },
            { "OemOpenBrackets", "[" },
            { "Oem6", "]" },
            { "Oem7", "#" },
            { "Oemtilde", "'" },
            { "Oem1", ";" }
        };
    }
}