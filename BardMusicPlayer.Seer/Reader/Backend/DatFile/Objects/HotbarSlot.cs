/*
 * Copyright(c) 2023 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects;

internal class HotbarSlot : IDisposable
{
    private byte _hotbar;
    public byte Hotbar
    {
        get => _hotbar;
        set => _hotbar = Convert.ToByte(value + 1);
    }
    private byte _slot;
    public byte Slot
    {
        get
        {
            var ss = _slot % 10;
            if (_slot > 10)
            {
                ss += _slot / 10 * 10 - 1;
            }
            return Convert.ToByte(ss);
        }
        set => _slot = Convert.ToByte(value + 1);
    }
    public byte Action { get; set; } // Higher level? 0D for 60-70 spells
    public byte Flag { get; set; }
    public byte Unk1 { get; set; }
    public byte Unk2 { get; set; }
    public byte Job { get; set; }
    public byte Type { get; set; }

    public bool IsBard => Job == 0x17;

    public override string ToString() => $"HOTBAR_{Hotbar}_{Slot:X}";

    ~HotbarSlot() => Dispose();
    public void Dispose()
    {
    }
}