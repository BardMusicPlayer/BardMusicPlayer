using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.MusicTheory.Interval
{
    internal static class IntervalUtilities
    {
        #region Methods

        internal static bool IsIntervalValid(int halfSteps)
        {
            return halfSteps >= -SevenBitNumber.MaxValue && halfSteps <= SevenBitNumber.MaxValue;
        }

        #endregion
    }
}
