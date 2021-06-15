/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.IO;

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event
{
    internal class MetaDataEvent : MetaEvent
    {
        public byte[] Data { get; private set; }

        public MetaDataEvent(int delta, byte status, byte metaId, byte[] data)
            : base(delta, status, metaId, 0)
        {
            Data = data;
        }

        public override void WriteTo(IWriteable s)
        {
            s.WriteByte(0xFF);
            s.WriteByte((byte)MetaStatus);

            var l = Data.Length;
            MidiFile.WriteVariableInt(s, l);
            s.Write(Data, 0, Data.Length);
        }
    }
}
