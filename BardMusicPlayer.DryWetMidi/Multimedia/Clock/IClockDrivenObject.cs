﻿namespace BardMusicPlayer.DryWetMidi.Multimedia.Clock;

/// <summary>
/// Represents an object driven by clock (timer).
/// </summary>
public interface IClockDrivenObject
{
    #region Methods

    /// <summary>
    /// Advances current object's clock time.
    /// </summary>
    void TickClock();

    #endregion
}