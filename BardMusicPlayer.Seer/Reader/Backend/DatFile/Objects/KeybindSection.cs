/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects
{
    internal class KeybindSection : IDisposable
    {
        public byte Type { get; set; }

        public int Size { get; set; }

        public byte[] Data { get; set; }

        ~KeybindSection() { Dispose(); }

        public void Dispose() { Data = null; }
    }
}