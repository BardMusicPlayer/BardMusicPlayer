/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.IO;

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event
{
    internal class SystemExclusiveEvent : SystemCommonEvent
    {
        public byte[] Data { get; private set; }


        public int ManufacturerId => Message >> 8;

        public SystemExclusiveEvent(int delta, byte status, short id, byte[] data)
            : base(delta, status, (byte)(id & 0x00FF), (byte)(id >> 8))
        {
            Data = data;
        }

        public override void WriteTo(IWriteable s)
        {
            s.WriteByte(0xF0);
            var l = Data.Length + 2;
            s.WriteByte((byte)ManufacturerId);
            var b = new[]
            {
                (byte)((l >> 24) & 0xFF), (byte)((l >> 16) & 0xFF), (byte)((l >> 8) & 0xFF), (byte)(l & 0xFF)
            };
            s.Write(b, 0, b.Length);
            s.WriteByte(0xF7);
        }
    }
}
