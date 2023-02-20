using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Core.Events.Readers
{
    internal interface IEventReader
    {
        #region Methods

        MidiEvent Read(MidiReader reader, ReadingSettings.ReadingSettings settings, byte currentStatusByte);

        #endregion
    }
}
