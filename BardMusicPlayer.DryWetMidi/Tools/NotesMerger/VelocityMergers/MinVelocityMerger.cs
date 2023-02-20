using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.Tools.NotesMerger.VelocityMergers
{
    internal sealed class MinVelocityMerger : VelocityMerger
    {
        #region Overrides

        public override void Merge(SevenBitNumber velocity)
        {
            _velocity = (SevenBitNumber)Math.Min(_velocity, velocity);
        }

        #endregion
    }
}
