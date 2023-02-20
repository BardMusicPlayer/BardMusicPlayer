using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Multimedia.DevicesConnector
{
    /// <summary>
    /// Callback used to process events coming from <see cref="DevicesConnector.InputDevice"/> before
    /// they will be sent to <see cref="DevicesConnector.OutputDevices"/>.
    /// </summary>
    /// <param name="inputMidiEvent">A MIDI event to process.</param>
    /// <returns>New event (or the input one) which is the processed original one.</returns>
    public delegate MidiEvent DevicesConnectorEventCallback(MidiEvent inputMidiEvent);
}
