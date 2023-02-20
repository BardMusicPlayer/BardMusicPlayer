using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.Interaction.ObjectId
{
    internal sealed class NoteId : IObjectId
    {
        #region Constructor

        public NoteId(FourBitNumber channel, SevenBitNumber noteNumber)
        {
            Channel = channel;
            NoteNumber = noteNumber;
        }

        #endregion

        #region Properties

        public FourBitNumber Channel { get; }

        public SevenBitNumber NoteNumber { get; }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
                return true;

            var noteId = obj as NoteId;
            if (ReferenceEquals(noteId, null))
                return false;

            return Channel == noteId.Channel &&
                   NoteNumber == noteId.NoteNumber;
        }

        public override int GetHashCode()
        {
            return Channel * 1000 + NoteNumber;
        }

        #endregion
    }
}
