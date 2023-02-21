using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.Tools.NotesMerger.VelocityMergers;

internal sealed class AverageVelocityMerger : VelocityMerger
{
    #region Fields

    private readonly List<SevenBitNumber> _velocities = new List<SevenBitNumber>();

    #endregion

    #region Overrides

    public override SevenBitNumber Velocity => (SevenBitNumber)MathUtilities.Round(_velocities.Average(v => v));

    public override void Initialize(SevenBitNumber velocity)
    {
        _velocities.Clear();
        _velocities.Add(velocity);
    }

    public override void Merge(SevenBitNumber velocity)
    {
        _velocities.Add(velocity);
    }

    #endregion
}