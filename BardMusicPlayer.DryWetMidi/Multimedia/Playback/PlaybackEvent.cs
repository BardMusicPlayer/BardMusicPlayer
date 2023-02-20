using System;
using BardMusicPlayer.DryWetMidi.Core;

namespace BardMusicPlayer.DryWetMidi.Multimedia
{
    internal sealed class PlaybackEvent
    {
        #region Constructor

        public PlaybackEvent(MidiEvent midiEvent, TimeSpan time, long rawTime)
        {
            Event = midiEvent;
            Time = time;
            RawTime = rawTime;
        }

        #endregion

        #region Properties

        public MidiEvent Event { get; }

        public TimeSpan Time { get; }

        public long RawTime { get; }

        public PlaybackEventMetadata Metadata { get; } = new PlaybackEventMetadata();

        #endregion
    }
}
