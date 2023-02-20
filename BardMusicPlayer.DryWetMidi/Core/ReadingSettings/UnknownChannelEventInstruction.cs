﻿using BardMusicPlayer.DryWetMidi.Core.Exceptions;

namespace BardMusicPlayer.DryWetMidi.Core.ReadingSettings
{
    /// <summary>
    /// Instruction for reading engine which tells how unknown channel event should be handled.
    /// </summary>
    public enum UnknownChannelEventInstruction
    {
        /// <summary>
        /// Abort reading and throw <see cref="UnknownChannelEventException"/>.
        /// </summary>
        Abort,

        /// <summary>
        /// Skip data bytes.
        /// </summary>
        SkipData
    }
}
