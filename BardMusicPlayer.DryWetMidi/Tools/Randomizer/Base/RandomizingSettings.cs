﻿namespace BardMusicPlayer.DryWetMidi.Tools.Randomizer.Base;

/// <summary>
/// Settings according to which objects should be randomized.
/// </summary>
[Obsolete("OBS10")]
public abstract class RandomizingSettings<TObject>
{
    #region Properties

    /// <summary>
    /// Gets or sets a predicate to filter objects that should be randomized. Use <c>null</c> if
    /// all objects should be processed.
    /// </summary>
    public Predicate<TObject> Filter { get; set; }

    #endregion
}