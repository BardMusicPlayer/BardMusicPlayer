using BardMusicPlayer.DryWetMidi.Interaction.Notes;
using BardMusicPlayer.DryWetMidi.Tools.Quantizer.LengthedObjectsQuantizer;

namespace BardMusicPlayer.DryWetMidi.Tools.Quantizer
{
    /// <summary>
    /// Settings according to which notes should be quantized.
    /// </summary>
    [Obsolete("OBS13")]
    public class NotesQuantizingSettings : LengthedObjectsQuantizingSettings<Note>
    {
        #region Properties

        /// <summary>
        /// Gets or sets settings which define how notes should be detected and built.
        /// </summary>
        public NoteDetectionSettings NoteDetectionSettings { get; set; } = new NoteDetectionSettings();

        #endregion
    }
}
