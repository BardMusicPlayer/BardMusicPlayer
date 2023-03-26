using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.Interaction;

/// <summary>
/// Musical objects that can be played.
/// </summary>
public interface IMusicalObject
{
    #region Properties

    /// <summary>
    /// Gets the channel which should be used to play an object.
    /// </summary>
    FourBitNumber Channel { get; }

    #endregion
}