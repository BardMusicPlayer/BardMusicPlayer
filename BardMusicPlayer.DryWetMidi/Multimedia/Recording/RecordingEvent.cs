using System;
using BardMusicPlayer.DryWetMidi.Core;

namespace BardMusicPlayer.DryWetMidi.Multimedia
{
    internal sealed class RecordingEvent
    {
        #region Constructor

        public RecordingEvent(MidiEvent midiEvent, TimeSpan time)
        {
            Event = midiEvent;
            Time = time;
        }

        #endregion

        #region Properties

        public MidiEvent Event { get; }

        public TimeSpan Time { get; }

        #endregion
    }
}
