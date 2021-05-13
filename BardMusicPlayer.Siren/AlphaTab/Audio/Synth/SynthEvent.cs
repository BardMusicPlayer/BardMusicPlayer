/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi.Event;

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth
{
    internal class SynthEvent
    {
        public int EventIndex { get; set; }
        public MidiEvent Event { get; set; }
        public double Time { get; set; }

        public SynthEvent(int eventIndex, MidiEvent e)
        {
            EventIndex = eventIndex;
            Event = e;
        }
    }
}
