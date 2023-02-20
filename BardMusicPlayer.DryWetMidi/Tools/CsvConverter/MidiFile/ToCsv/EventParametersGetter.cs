using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Tools.CsvConverter.MidiFile.ToCsv
{
    internal delegate object[] EventParametersGetter(MidiEvent midiEvent, MidiFileCsvConversionSettings settings);
}
