using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.Tools.NotesMerger.VelocityMergers
{
    internal sealed class MaxVelocityMerger : VelocityMerger
    {
        #region Overrides

        public override void Merge(SevenBitNumber velocity)
        {
            _velocity = (SevenBitNumber)Math.Max(_velocity, velocity);
        }

        #endregion
    }
}
