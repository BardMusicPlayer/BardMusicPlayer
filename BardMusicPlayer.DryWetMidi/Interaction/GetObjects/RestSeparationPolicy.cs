﻿namespace BardMusicPlayer.DryWetMidi.Interaction
{
    /// <summary>
    /// Determines a rule for creating rests. The default value is <see cref="NoSeparation"/>.
    /// More info in the
    /// <see href="xref:a_getting_objects#rests">Getting objects: GetObjects: Rests</see> article.
    /// </summary>
    public enum RestSeparationPolicy
    {
        /// <summary>
        /// Rests should be constructed only when there are no notes at all on any channel.
        /// </summary>
        NoSeparation = 0,

        /// <summary>
        /// Rests should be constructed individually for each channel ignoring note number.
        /// </summary>
        SeparateByChannel,

        /// <summary>
        /// Rests should be constructed individually for each note number ignoring channel.
        /// </summary>
        SeparateByNoteNumber,

        /// <summary>
        /// Rests should be constructed individually for each channel and note number.
        /// </summary>
        SeparateByChannelAndNoteNumber
    }
}
