using BardMusicPlayer.DryWetMidi.Core;
using BardMusicPlayer.DryWetMidi.Interaction;

namespace BardMusicPlayer.DryWetMidi.Tools
{
    internal sealed class TimedMidiEvent
    {
        #region Constructor

        public TimedMidiEvent(ITimeSpan time, MidiEvent midiEvent)
        {
            Time = time;
            Event = midiEvent;
        }

        #endregion

        #region Properties

        public ITimeSpan Time { get; }

        public MidiEvent Event { get; }

        #endregion
    }
}
