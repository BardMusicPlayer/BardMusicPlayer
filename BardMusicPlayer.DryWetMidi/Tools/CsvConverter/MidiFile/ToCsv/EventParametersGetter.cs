using BardMusicPlayer.DryWetMidi.Core;

namespace BardMusicPlayer.DryWetMidi.Tools
{
    internal delegate object[] EventParametersGetter(MidiEvent midiEvent, MidiFileCsvConversionSettings settings);
}
