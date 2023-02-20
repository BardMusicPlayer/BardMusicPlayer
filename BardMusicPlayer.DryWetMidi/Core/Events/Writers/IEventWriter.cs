using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Core.Events.Writers
{
    internal interface IEventWriter
    {
        #region Methods

        void Write(MidiEvent midiEvent, MidiWriter writer, WritingSettings.WritingSettings settings, bool writeStatusByte);

        int CalculateSize(MidiEvent midiEvent, WritingSettings.WritingSettings settings, bool writeStatusByte);

        byte GetStatusByte(MidiEvent midiEvent);

        #endregion
    }
}
