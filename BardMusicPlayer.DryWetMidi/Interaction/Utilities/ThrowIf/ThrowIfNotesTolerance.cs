using BardMusicPlayer.DryWetMidi.Common;

namespace BardMusicPlayer.DryWetMidi.Interaction.Utilities.ThrowIf
{
    internal static class ThrowIfNotesTolerance
    {
        #region Methods

        internal static void IsNegative(string parameterName, long notesTolerance)
        {
            ThrowIfArgument.IsNegative(parameterName, notesTolerance, "Notes tolerance is negative.");
        }

        #endregion
    }
}
