using BardMusicPlayer.DryWetMidi.Core;

namespace BardMusicPlayer.DryWetMidi.Tools
{
    internal delegate MidiEvent EventParser(string[] parameters, MidiFileCsvConversionSettings settings);
}
