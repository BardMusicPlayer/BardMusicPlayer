using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Representations;

namespace BardMusicPlayer.DryWetMidi.Interaction.TimeSpan;

/// <summary>
/// Represents a simple math operation used by the <see cref="MathTimeSpan"/>.
/// The default is <see cref="Add"/>.
/// </summary>
public enum MathOperation
{
    /// <summary>
    /// Addition.
    /// </summary>
    Add = 0,

    /// <summary>
    /// Subtraction.
    /// </summary>
    Subtract
}