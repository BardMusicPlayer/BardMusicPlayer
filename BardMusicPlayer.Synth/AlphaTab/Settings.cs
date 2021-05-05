using BardMusicPlayer.Synth.AlphaTab.Util;

namespace BardMusicPlayer.Synth.AlphaTab
{
    /// <summary>
    /// Lists the different modes on how rhythm notation is shown on the tab staff.
    /// </summary>
    [JsonSerializable]
    internal enum TabRhythmMode
    {
        /// <summary>
        /// Rhythm notation is hidden.
        /// </summary>
        Hidden,

        /// <summary>
        /// Rhythm notation is shown with individual beams per beat.
        /// </summary>
        ShowWithBeams,

        /// <summary>
        /// Rhythm notation is shown and behaves like normal score notation with connected bars.
        /// </summary>
        ShowWithBars
    }

    /// <summary>
    /// The notation settings control how various music notation elements are shown and behaving
    /// </summary>
    [JsonSerializable]
    internal class NotationSettings
    {


        /// <summary>
        /// Whether to display the song information or not.
        /// </summary>
        [JsonName("hideInfo")]
        public bool HideInfo { get; set; } = false;

        /// <summary>
        /// Whether to display the tuning information or not.
        /// </summary>
        [JsonName("hideTuning")]
        public bool HideTuning { get; set; } = false;

        /// <summary>
        /// Whether to display the track names in the accolade or not.
        /// </summary>
        [JsonName("hideTrackNames")]
        public bool HideTrackNames { get; set; } = false;

        /// <summary>
        /// Whether to display the chord diagrams or not.
        /// </summary>
        [JsonName("hideChordDiagrams")]
        public bool HideChordDiagrams { get; set; } = false;

        /// <summary>
        /// Whether to show rhythm notation in the guitar tablature.
        /// </summary>
        [JsonName("rhythmMode")]
        public TabRhythmMode RhythmMode { get; set; } = TabRhythmMode.Hidden;

        /// <summary>
        /// The height of the rythm bars.
        /// </summary>
        [JsonName("rhythmHeight")]
        public float RhythmHeight { get; set; } = 15;

        /// <summary>
        /// The transposition pitch offsets for the individual tracks.
        /// They apply to rendering and playback.
        /// </summary>
        [JsonName("transpositionPitches")]
        public int[] TranspositionPitches { get; set; } = new int[0];

        /// <summary>
        /// The transposition pitch offsets for the individual tracks.
        /// They apply to rendering only.
        /// </summary>
        [JsonName("displayTranspositionPitches")]
        public int[] DisplayTranspositionPitches { get; set; } = new int[0];

        /// <summary>
        /// If set to true the guitar tabs on grace beats are rendered smaller.
        /// </summary>
        [JsonName("smallGraceTabNotes")]
        public bool SmallGraceTabNotes { get; set; } = true;

        /// <summary>
        /// If set to true bend arrows expand to the end of the last tied note
        /// of the string. Otherwise they end on the next beat.
        /// </summary>
        [JsonName("extendBendArrowsOnTiedNotes")]
        public bool ExtendBendArrowsOnTiedNotes { get; set; } = true;

        /// <summary>
        /// If set to true the note heads on tied notes
        /// will have parenthesis if they are preceeded by bends.
        /// </summary>
        [JsonName("showParenthesisForTiedBends")]
        public bool ShowParenthesisForTiedBends { get; set; } = true;

        /// <summary>
        /// If set to true a tab number will be shown in case
        /// a bend is increased on a tied note.
        /// </summary>
        [JsonName("showTabNoteOnTiedBend")]
        public bool ShowTabNoteOnTiedBend { get; set; } = true;

        /// <summary>
        /// If set to true, 0 is shown on dive whammy bars.
        /// </summary>
        [JsonName("showZeroOnDiveWhammy")]
        public bool ShowZeroOnDiveWhammy { get; set; } = false;

        /// <summary>
        /// If set to true, line effects (like w/bar, let-ring etc)
        /// are drawn until the end of the beat instead of the start.
        /// </summary>
        [JsonName("extendLineEffectsToBeatEnd")]
        public bool ExtendLineEffectsToBeatEnd { get; set; } = false;

        /// <summary>
        /// Gets or sets the height for slurs. The factor is multiplied with the a logarithmic distance
        /// between slur start and end.
        /// </summary>
        [JsonName("slurHeight")]
        public float SlurHeight { get; set; } = 7.0f;
    }
    

    /// <summary>
    /// This public class contains instance specific settings for alphaTab
    /// </summary>
    [JsonSerializable]
    internal partial class Settings
    {


        /// <summary>
        /// The notation settings control how various music notation elements are shown and behaving.
        /// </summary>
        [JsonName("notation")]
        public NotationSettings Notation { get; } = new NotationSettings();

    }
}
