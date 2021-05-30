/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects
{
    internal class Keybind : IDisposable
    {
        public int MainKey1 { get; set; } = 0;

        public int ModKey1 { get; set; } = 0;

        public int MainKey2 { get; set; } = 0;

        public int ModKey2 { get; set; } = 0;

        public Keys GetKey() => GetKey1() != Keys.None ? GetKey1() : GetKey2();

        public Keys GetKey1() => GetMain(MainKey1) | GetMod(ModKey1);

        public Keys GetKey2() => GetMain(MainKey2) | GetMod(ModKey2);

        private static Keys GetMain(int key)
        {
            if (key < 130) return (Keys) key;
            if (KeyDictionary.MainKeyMap.ContainsKey(key)) return (Keys) KeyDictionary.MainKeyMap[key];

            return Keys.None;
        }

        private static Keys GetMod(int mod)
        {
            var modKeys = Keys.None;
            if ((mod & 1) != 0) modKeys |= Keys.Shift;
            if ((mod & 2) != 0) modKeys |= Keys.Control;
            if ((mod & 4) != 0) modKeys |= Keys.Alt;
            return modKeys;
        }

        public override string ToString()
        {
            var key = GetKey();
            if (key == Keys.None) return string.Empty;

            var str = key.ToString();
            if (KeyDictionary.OemKeyFix.ContainsKey(str)) str = KeyDictionary.OemKeyFix[str];
            return str;
        }

        ~Keybind() { Dispose(); }

        public void Dispose() { }
    }
}