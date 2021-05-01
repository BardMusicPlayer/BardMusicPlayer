/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects
{
    internal class HotbarSlot : IDisposable
    {
        private byte _hotbar;
        public byte Hotbar { 
            get => _hotbar;
            set => _hotbar = Convert.ToByte(value+1);
        }
        private byte _slot;
        public byte Slot
        {
            get  {
                var ss = _slot % 10;
                if(_slot > 10) {
                    ss += _slot / 10 * 10 - 1;
                }
                return Convert.ToByte(ss);
            }
            set => _slot = Convert.ToByte(value+1);
        }
        public byte Action { get; set; } = 0; // Higher level? 0D for 60-70 spells
        public byte Flag { get; set; } = 0;
        public byte Unk1 { get; set; } = 0;
        public byte Unk2 { get; set; } = 0;
        public byte Job { get; set; } = 0;
        public byte Type { get; set; } = 0;

        public override string ToString() => string.Format("HOTBAR_{0}_{1:X}", Hotbar, Slot);

        ~HotbarSlot() => Dispose();
        public void Dispose()
        {
        }
    }
}
