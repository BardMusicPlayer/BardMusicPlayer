﻿namespace BardMusicPlayer.DryWetMidi.Common
{
    internal delegate ParsingResult Parsing<T>(string input, out T result);
}
