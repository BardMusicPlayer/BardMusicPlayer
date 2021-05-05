using BardMusicPlayer.Synth.AlphaTab.Audio.Synth.Midi.Event;

namespace BardMusicPlayer.Synth.AlphaTab.Audio.Synth
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
