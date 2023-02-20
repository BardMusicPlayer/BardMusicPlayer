namespace BardMusicPlayer.DryWetMidi.Tools.CsvConverter.MidiFile.FromCsv
{
    internal enum RecordType
    {
        Header,
        TrackChunkStart,
        TrackChunkEnd,
        FileEnd,
        Event,
        Note
    }
}
