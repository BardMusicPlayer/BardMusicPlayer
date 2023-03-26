using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.Standards.GeneralMidi;

/// <summary>
/// The class which provides information about the General MIDI Level 1 standard.
/// </summary>
public static class GeneralMidi
{
    #region Constants

    /// <summary>
    /// Channel reserved for percussion according to the General MIDI Level 1 standard.
    /// </summary>
    public static readonly FourBitNumber PercussionChannel = (FourBitNumber)9;

    #endregion
}