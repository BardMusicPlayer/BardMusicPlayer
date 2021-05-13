﻿/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event
{
    internal enum MetaEventTypeEnum
    {
        SequenceNumber = 0x00,
        TextEvent = 0x01,
        CopyrightNotice = 0x02,
        SequenceOrTrackName = 0x03,
        InstrumentName = 0x04,
        LyricText = 0x05,
        MarkerText = 0x06,
        CuePoint = 0x07,
        PatchName = 0x08,
        PortName = 0x09,
        MidiChannel = 0x20,
        MidiPort = 0x21,
        EndOfTrack = 0x2F,
        Tempo = 0x51,
        SmpteOffset = 0x54,
        TimeSignature = 0x58,
        KeySignature = 0x59,
        SequencerSpecific = 0x7F
    }

    internal abstract class MetaEvent : MidiEvent
    {
        public override int Channel => -1;

        public override MidiEventType Command => (MidiEventType)(Message & 0x00000FF);

        public int MetaStatus => Data1;

        protected MetaEvent(int delta, byte status, byte data1, byte data2)
            : base(delta, status, data1, data2)
        {
        }
    }
}
