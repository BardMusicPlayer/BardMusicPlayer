namespace BardMusicPlayer.DryWetMidi.Common.Parsing;

internal delegate ParsingResult Parsing<T>(string input, out T result);