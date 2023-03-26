using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Multimedia.Recording;

internal sealed class RecordingEvent
{
    #region Constructor

    public RecordingEvent(MidiEvent midiEvent, TimeSpan time)
    {
        Event = midiEvent;
        Time  = time;
    }

    #endregion

    #region Properties

    public MidiEvent Event { get; }

    public TimeSpan Time { get; }

    #endregion
}