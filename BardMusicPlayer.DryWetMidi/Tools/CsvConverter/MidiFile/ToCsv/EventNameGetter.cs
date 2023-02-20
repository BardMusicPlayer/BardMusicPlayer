using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Tools.CsvConverter.MidiFile.ToCsv
{
    internal delegate string EventNameGetter(MidiEvent midiEvent);
}
