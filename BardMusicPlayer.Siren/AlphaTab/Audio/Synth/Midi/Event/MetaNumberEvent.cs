/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.IO;

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event
{
    internal class MetaNumberEvent : MetaEvent
    {
        public int Value { get; private set; }

        public MetaNumberEvent(int delta, byte status, byte metaId, int number)
            : base(delta, status, metaId, 0)
        {
            Value = number;
        }


        public override void WriteTo(IWriteable s)
        {
            s.WriteByte(0xFF);
            s.WriteByte((byte)MetaStatus);

            MidiFile.WriteVariableInt(s, 3);

            var b = new[]
            {
                (byte)((Value >> 16) & 0xFF), (byte)((Value >> 8) & 0xFF), (byte)(Value & 0xFF)
            };
            s.Write(b, 0, b.Length);
        }
    }
}
