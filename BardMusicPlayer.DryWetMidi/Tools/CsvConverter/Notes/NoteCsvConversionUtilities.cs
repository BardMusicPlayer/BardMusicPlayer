using BardMusicPlayer.DryWetMidi.Common.DataTypes;
using BardMusicPlayer.DryWetMidi.MusicTheory.Note;

namespace BardMusicPlayer.DryWetMidi.Tools.CsvConverter.Notes;

internal static class NoteCsvConversionUtilities
{
    #region Methods

    public static object FormatNoteNumber(SevenBitNumber noteNumber, NoteNumberFormat noteNumberFormat)
    {
        switch (noteNumberFormat)
        {
            case NoteNumberFormat.NoteNumber:
                return noteNumber;
            case NoteNumberFormat.Letter:
                return Note.Get(noteNumber);
        }

        return null;
    }

    #endregion
}