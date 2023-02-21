﻿using BardMusicPlayer.DryWetMidi.Core.Exceptions;

namespace BardMusicPlayer.DryWetMidi.Core.ReadingSettings;

/// <summary>
/// Specifies how reading engine should react on unexpected track chunks count. The default is
/// <see cref="Ignore"/>.
/// </summary>
public enum UnexpectedTrackChunksCountPolicy
{
    /// <summary>
    /// Ignore unexpected track chunks count.
    /// </summary>
    Ignore = 0,

    /// <summary>
    /// Abort reading and throw an <see cref="UnexpectedTrackChunksCountException"/>.
    /// </summary>
    Abort
}