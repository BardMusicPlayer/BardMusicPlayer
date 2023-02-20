﻿namespace BardMusicPlayer.DryWetMidi.Core
{
    /// <summary>
    /// Represents a single MIDI token from a MIDI file.
    /// </summary>
    /// <seealso cref="MidiTokensReader"/>
    public abstract class MidiToken
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MidiToken"/> with the
        /// specified token type.
        /// </summary>
        /// <param name="tokenType">The type of a MIDI token.</param>
        protected MidiToken(MidiTokenType tokenType)
        {
            TokenType = tokenType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of the current MIDI token.
        /// </summary>
        public MidiTokenType TokenType { get; }

        #endregion
    }
}
