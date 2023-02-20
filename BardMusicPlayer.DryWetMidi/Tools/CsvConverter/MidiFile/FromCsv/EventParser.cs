using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Tools.CsvConverter.MidiFile.FromCsv
{
    internal delegate MidiEvent EventParser(string[] parameters, MidiFileCsvConversionSettings settings);
}
