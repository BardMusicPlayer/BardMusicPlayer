using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Multimedia.Playback
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

        public PlaybackEventMetadata.PlaybackEventMetadata Metadata { get; } = new PlaybackEventMetadata.PlaybackEventMetadata();

        #endregion
    }
}
