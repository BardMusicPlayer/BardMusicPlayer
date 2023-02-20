using BardMusicPlayer.DryWetMidi.Common.DataTypes;
using BardMusicPlayer.DryWetMidi.MusicTheory.Note;
using BardMusicPlayer.DryWetMidi.Tools.CsvConverter.Notes;

namespace BardMusicPlayer.DryWetMidi.Tools.CsvConverter.MidiFile.FromCsv
{
    internal static class TypeParser
    {
        public static readonly ParameterParser Byte = (p, s) => byte.Parse(p);
        public static readonly ParameterParser SByte = (p, s) => sbyte.Parse(p);
        public static readonly ParameterParser Long = (p, s) => long.Parse(p);
        public static readonly ParameterParser UShort = (p, s) => ushort.Parse(p);
        public static readonly ParameterParser String = (p, s) => CsvUtilities.UnescapeString(p);
        public static readonly ParameterParser Int = (p, s) => int.Parse(p);
        public static readonly ParameterParser FourBitNumber = (p, s) => (FourBitNumber)byte.Parse(p);
        public static readonly ParameterParser SevenBitNumber = (p, s) => (SevenBitNumber)byte.Parse(p);
        public static readonly ParameterParser NoteNumber = (p, s) =>
        {
            switch (s.NoteNumberFormat)
            {
                case NoteNumberFormat.NoteNumber:
                    return SevenBitNumber(p, s);
                case NoteNumberFormat.Letter:
                    return Note.Parse(p).NoteNumber;
            }

            return null;
        };
    }
}
