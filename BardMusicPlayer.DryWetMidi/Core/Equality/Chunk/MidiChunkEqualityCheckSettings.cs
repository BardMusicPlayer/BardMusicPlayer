﻿using BardMusicPlayer.DryWetMidi.Core.Chunks;
using BardMusicPlayer.DryWetMidi.Core.Equality.Event;
using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Core.Equality.Chunk
{
    /// <summary>
    /// Holds settings according to which <see cref="MidiChunk"/> objects should
    /// be compared for equality.
    /// </summary>
    public sealed class MidiChunkEqualityCheckSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets settings according to which <see cref="MidiEvent"/> objects should
        /// be compared for equality.
        /// </summary>
        public MidiEventEqualityCheckSettings EventEqualityCheckSettings { get; set; } = new MidiEventEqualityCheckSettings();

        #endregion
    }
}
