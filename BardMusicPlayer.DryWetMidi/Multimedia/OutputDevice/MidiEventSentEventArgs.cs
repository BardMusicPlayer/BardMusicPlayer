using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Multimedia.OutputDevice;

/// <summary>
/// Provides data for the <see cref="IOutputDevice.EventSent"/> event.
/// </summary>
public sealed class MidiEventSentEventArgs : EventArgs
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="MidiEventSentEventArgs"/> with
    /// the specified MIDI event.
    /// </summary>
    /// <param name="midiEvent">MIDI event sent by <see cref="IOutputDevice"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="midiEvent"/> is <c>null</c>.</exception>
    public MidiEventSentEventArgs(MidiEvent midiEvent)
    {
        ThrowIfArgument.IsNull(nameof(midiEvent), midiEvent);

        Event = midiEvent;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets MIDI event sent to <see cref="IOutputDevice"/>.
    /// </summary>
    public MidiEvent Event { get; }

    #endregion
}