using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.Tools.NotesMerger.VelocityMergers;

internal sealed class LastVelocityMerger : VelocityMerger
{
    #region Overrides

    public override void Merge(SevenBitNumber velocity)
    {
        _velocity = velocity;
    }

    #endregion
}