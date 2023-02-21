using System.Reflection;
using BardMusicPlayer.DryWetMidi.Common;

namespace BardMusicPlayer.DryWetMidi.MusicTheory.Scale;

/// <summary>
/// Provides intervals sequences for known musical scales.
/// </summary>
public static class ScaleIntervals
{
    #region Constants

    /// <summary>
    /// 'Aeolian' scale's intervals sequence.
    /// </summary>
    [DisplayName("aeolian")]
    public static readonly IEnumerable<Interval.Interval> Aeolian = GetIntervals(2, 1, 2, 2, 1, 2, 2);

    /// <summary>
    /// 'Altered' scale's intervals sequence.
    /// </summary>
    [DisplayName("altered")]
    public static readonly IEnumerable<Interval.Interval> Altered = GetIntervals(1, 2, 1, 2, 2, 2, 2);

    /// <summary>
    /// 'Arabian' scale's intervals sequence.
    /// </summary>
    [DisplayName("arabian")]
    public static readonly IEnumerable<Interval.Interval> Arabian = GetIntervals(2, 2, 1, 1, 2, 2, 2);

    /// <summary>
    /// 'Augmented' scale's intervals sequence.
    /// </summary>
    [DisplayName("augmented")]
    public static readonly IEnumerable<Interval.Interval> Augmented = GetIntervals(3, 1, 3, 1, 3, 1);

    /// <summary>
    /// 'Augmented Heptatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("augmented heptatonic")]
    public static readonly IEnumerable<Interval.Interval> AugmentedHeptatonic = GetIntervals(3, 1, 1, 2, 1, 3, 1);

    /// <summary>
    /// 'Balinese' scale's intervals sequence.
    /// </summary>
    [DisplayName("balinese")]
    public static readonly IEnumerable<Interval.Interval> Balinese = GetIntervals(1, 2, 2, 2, 1, 3, 1);

    /// <summary>
    /// 'Bebop' scale's intervals sequence.
    /// </summary>
    [DisplayName("bebop")]
    public static readonly IEnumerable<Interval.Interval> Bebop = GetIntervals(2, 2, 1, 2, 2, 1, 1, 1);

    /// <summary>
    /// 'Bebop Dominant' scale's intervals sequence.
    /// </summary>
    [DisplayName("bebop dominant")]
    public static readonly IEnumerable<Interval.Interval> BebopDominant = GetIntervals(2, 2, 1, 2, 2, 1, 1, 1);

    /// <summary>
    /// 'Bebop Locrian' scale's intervals sequence.
    /// </summary>
    [DisplayName("bebop locrian")]
    public static readonly IEnumerable<Interval.Interval> BebopLocrian = GetIntervals(1, 2, 2, 1, 1, 1, 2, 2);

    /// <summary>
    /// 'Bebop Major' scale's intervals sequence.
    /// </summary>
    [DisplayName("bebop major")]
    public static readonly IEnumerable<Interval.Interval> BebopMajor = GetIntervals(2, 2, 1, 2, 1, 1, 2, 1);

    /// <summary>
    /// 'Bebop Minor' scale's intervals sequence.
    /// </summary>
    [DisplayName("bebop minor")]
    public static readonly IEnumerable<Interval.Interval> BebopMinor = GetIntervals(2, 1, 1, 1, 2, 2, 1, 2);

    /// <summary>
    /// 'Blues' scale's intervals sequence.
    /// </summary>
    [DisplayName("blues")]
    public static readonly IEnumerable<Interval.Interval> Blues = GetIntervals(3, 2, 1, 1, 3, 2);

    /// <summary>
    /// 'Chinese' scale's intervals sequence.
    /// </summary>
    [DisplayName("chinese")]
    public static readonly IEnumerable<Interval.Interval> Chinese = GetIntervals(4, 2, 1, 4, 1);

    /// <summary>
    /// 'Chromatic' scale's intervals sequence.
    /// </summary>
    [DisplayName("chromatic")]
    public static readonly IEnumerable<Interval.Interval> Chromatic = GetIntervals(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);

    /// <summary>
    /// 'Composite Blues' scale's intervals sequence.
    /// </summary>
    [DisplayName("composite blues")]
    public static readonly IEnumerable<Interval.Interval> CompositeBlues = GetIntervals(2, 1, 1, 1, 1, 1, 2, 1, 2);

    /// <summary>
    /// 'Diminished' scale's intervals sequence.
    /// </summary>
    [DisplayName("diminished")]
    public static readonly IEnumerable<Interval.Interval> Diminished = GetIntervals(2, 1, 2, 1, 2, 1, 2, 1);

    /// <summary>
    /// 'Diminished Whole Tone' scale's intervals sequence.
    /// </summary>
    [DisplayName("diminished whole tone")]
    public static readonly IEnumerable<Interval.Interval> DiminishedWholeTone = GetIntervals(1, 2, 1, 2, 2, 2, 2);

    /// <summary>
    /// 'Dominant' scale's intervals sequence.
    /// </summary>
    [DisplayName("dominant")]
    public static readonly IEnumerable<Interval.Interval> Dominant = GetIntervals(2, 2, 1, 2, 2, 1, 2);

    /// <summary>
    /// 'Dorian' scale's intervals sequence.
    /// </summary>
    [DisplayName("dorian")]
    public static readonly IEnumerable<Interval.Interval> Dorian = GetIntervals(2, 1, 2, 2, 2, 1, 2);

    /// <summary>
    /// 'Dorian #4' scale's intervals sequence.
    /// </summary>
    [DisplayName("dorian #4")]
    public static readonly IEnumerable<Interval.Interval> Dorian4 = GetIntervals(2, 1, 3, 1, 2, 1, 2);

    /// <summary>
    /// 'Dorian b2' scale's intervals sequence.
    /// </summary>
    [DisplayName("dorian b2")]
    public static readonly IEnumerable<Interval.Interval> DorianB2 = GetIntervals(1, 2, 2, 2, 2, 2, 1);

    /// <summary>
    /// 'Double Harmonic Lydian' scale's intervals sequence.
    /// </summary>
    [DisplayName("double harmonic lydian")]
    public static readonly IEnumerable<Interval.Interval> DoubleHarmonicLydian = GetIntervals(1, 3, 2, 1, 1, 3, 1);

    /// <summary>
    /// 'Double Harmonic Major' scale's intervals sequence.
    /// </summary>
    [DisplayName("double harmonic major")]
    public static readonly IEnumerable<Interval.Interval> DoubleHarmonicMajor = GetIntervals(1, 3, 1, 2, 1, 3, 1);

    /// <summary>
    /// 'Egyptian' scale's intervals sequence.
    /// </summary>
    [DisplayName("egyptian")]
    public static readonly IEnumerable<Interval.Interval> Egyptian = GetIntervals(2, 3, 2, 3, 2);

    /// <summary>
    /// 'Enigmatic' scale's intervals sequence.
    /// </summary>
    [DisplayName("enigmatic")]
    public static readonly IEnumerable<Interval.Interval> Enigmatic = GetIntervals(1, 3, 2, 2, 2, 1, 1);

    /// <summary>
    /// 'Flamenco' scale's intervals sequence.
    /// </summary>
    [DisplayName("flamenco")]
    public static readonly IEnumerable<Interval.Interval> Flamenco = GetIntervals(1, 2, 1, 2, 1, 3, 2);

    /// <summary>
    /// 'Flat Six Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("flat six pentatonic")]
    public static readonly IEnumerable<Interval.Interval> FlatSixPentatonic = GetIntervals(2, 2, 3, 1, 4);

    /// <summary>
    /// 'Flat Three Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("flat three pentatonic")]
    public static readonly IEnumerable<Interval.Interval> FlatThreePentatonic = GetIntervals(2, 1, 4, 2, 3);

    /// <summary>
    /// 'Gypsy' scale's intervals sequence.
    /// </summary>
    [DisplayName("gypsy")]
    public static readonly IEnumerable<Interval.Interval> Gypsy = GetIntervals(1, 3, 1, 2, 1, 3, 1);

    /// <summary>
    /// 'Harmonic Major' scale's intervals sequence.
    /// </summary>
    [DisplayName("harmonic major")]
    public static readonly IEnumerable<Interval.Interval> HarmonicMajor = GetIntervals(2, 2, 1, 2, 1, 3, 1);

    /// <summary>
    /// 'Harmonic Minor' scale's intervals sequence.
    /// </summary>
    [DisplayName("harmonic minor")]
    public static readonly IEnumerable<Interval.Interval> HarmonicMinor = GetIntervals(2, 1, 2, 2, 1, 3, 1);

    /// <summary>
    /// 'Hindu' scale's intervals sequence.
    /// </summary>
    [DisplayName("hindu")]
    public static readonly IEnumerable<Interval.Interval> Hindu = GetIntervals(2, 2, 1, 2, 1, 2, 2);

    /// <summary>
    /// 'Hirajoshi' scale's intervals sequence.
    /// </summary>
    [DisplayName("hirajoshi")]
    public static readonly IEnumerable<Interval.Interval> Hirajoshi = GetIntervals(2, 1, 4, 1, 4);

    /// <summary>
    /// 'Hungarian Major' scale's intervals sequence.
    /// </summary>
    [DisplayName("hungarian major")]
    public static readonly IEnumerable<Interval.Interval> HungarianMajor = GetIntervals(3, 1, 2, 1, 2, 1, 2);

    /// <summary>
    /// 'Hungarian Minor' scale's intervals sequence.
    /// </summary>
    [DisplayName("hungarian minor")]
    public static readonly IEnumerable<Interval.Interval> HungarianMinor = GetIntervals(2, 1, 3, 1, 1, 3, 1);

    /// <summary>
    /// 'Ichikosucho' scale's intervals sequence.
    /// </summary>
    [DisplayName("ichikosucho")]
    public static readonly IEnumerable<Interval.Interval> Ichikosucho = GetIntervals(2, 2, 1, 1, 1, 2, 2, 1);

    /// <summary>
    /// 'In-Sen' scale's intervals sequence.
    /// </summary>
    [DisplayName("in-sen")]
    public static readonly IEnumerable<Interval.Interval> InSen = GetIntervals(1, 4, 2, 3, 2);

    /// <summary>
    /// 'Indian' scale's intervals sequence.
    /// </summary>
    [DisplayName("indian")]
    public static readonly IEnumerable<Interval.Interval> Indian = GetIntervals(4, 1, 2, 3, 2);

    /// <summary>
    /// 'Ionian' scale's intervals sequence.
    /// </summary>
    [DisplayName("ionian")]
    public static readonly IEnumerable<Interval.Interval> Ionian = GetIntervals(2, 2, 1, 2, 2, 2, 1);

    /// <summary>
    /// 'Ionian Augmented' scale's intervals sequence.
    /// </summary>
    [DisplayName("ionian augmented")]
    public static readonly IEnumerable<Interval.Interval> IonianAugmented = GetIntervals(2, 2, 1, 3, 1, 2, 1);

    /// <summary>
    /// 'Ionian Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("ionian pentatonic")]
    public static readonly IEnumerable<Interval.Interval> IonianPentatonic = GetIntervals(4, 1, 2, 4, 1);

    /// <summary>
    /// 'Iwato' scale's intervals sequence.
    /// </summary>
    [DisplayName("iwato")]
    public static readonly IEnumerable<Interval.Interval> Iwato = GetIntervals(1, 4, 1, 4, 2);

    /// <summary>
    /// 'Kafi Raga' scale's intervals sequence.
    /// </summary>
    [DisplayName("kafi raga")]
    public static readonly IEnumerable<Interval.Interval> KafiRaga = GetIntervals(3, 1, 1, 2, 2, 1, 1, 1);

    /// <summary>
    /// 'Kumoi' scale's intervals sequence.
    /// </summary>
    [DisplayName("kumoi")]
    public static readonly IEnumerable<Interval.Interval> Kumoi = GetIntervals(2, 1, 4, 2, 3);

    /// <summary>
    /// 'Kumoijoshi' scale's intervals sequence.
    /// </summary>
    [DisplayName("kumoijoshi")]
    public static readonly IEnumerable<Interval.Interval> Kumoijoshi = GetIntervals(1, 4, 2, 1, 4);

    /// <summary>
    /// 'Leading Whole Tone' scale's intervals sequence.
    /// </summary>
    [DisplayName("leading whole tone")]
    public static readonly IEnumerable<Interval.Interval> LeadingWholeTone = GetIntervals(2, 2, 2, 2, 2, 1, 1);

    /// <summary>
    /// 'Locrian' scale's intervals sequence.
    /// </summary>
    [DisplayName("locrian")]
    public static readonly IEnumerable<Interval.Interval> Locrian = GetIntervals(1, 2, 2, 1, 2, 2, 2);

    /// <summary>
    /// 'Locrian #2' scale's intervals sequence.
    /// </summary>
    [DisplayName("locrian #2")]
    public static readonly IEnumerable<Interval.Interval> Locrian2 = GetIntervals(2, 1, 2, 1, 2, 2, 2);

    /// <summary>
    /// 'Locrian Major' scale's intervals sequence.
    /// </summary>
    [DisplayName("locrian major")]
    public static readonly IEnumerable<Interval.Interval> LocrianMajor = GetIntervals(2, 2, 1, 1, 2, 2, 2);

    /// <summary>
    /// 'Locrian Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("locrian pentatonic")]
    public static readonly IEnumerable<Interval.Interval> LocrianPentatonic = GetIntervals(3, 2, 1, 4, 2);

    /// <summary>
    /// 'Lydian' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian")]
    public static readonly IEnumerable<Interval.Interval> Lydian = GetIntervals(2, 2, 2, 1, 2, 2, 1);

    /// <summary>
    /// 'Lydian #5P Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian #5P pentatonic")]
    public static readonly IEnumerable<Interval.Interval> Lydian5PPentatonic = GetIntervals(4, 2, 2, 3, 1);

    /// <summary>
    /// 'Lydian #9' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian #9")]
    public static readonly IEnumerable<Interval.Interval> Lydian9 = GetIntervals(1, 3, 2, 1, 2, 2, 1);

    /// <summary>
    /// 'Lydian Augmented' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian augmented")]
    public static readonly IEnumerable<Interval.Interval> LydianAugmented = GetIntervals(2, 2, 2, 2, 1, 2, 1);

    /// <summary>
    /// 'Lydian b7' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian b7")]
    public static readonly IEnumerable<Interval.Interval> LydianB7 = GetIntervals(2, 2, 2, 1, 2, 1, 2);

    /// <summary>
    /// 'Lydian Diminished' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian diminished")]
    public static readonly IEnumerable<Interval.Interval> LydianDiminished = GetIntervals(2, 1, 3, 1, 2, 2, 1);

    /// <summary>
    /// 'Lydian Dominant' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian dominant")]
    public static readonly IEnumerable<Interval.Interval> LydianDominant = GetIntervals(2, 2, 2, 1, 2, 1, 2);

    /// <summary>
    /// 'Lydian Dominant Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian dominant pentatonic")]
    public static readonly IEnumerable<Interval.Interval> LydianDominantPentatonic = GetIntervals(4, 2, 1, 3, 2);

    /// <summary>
    /// 'Lydian Minor' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian minor")]
    public static readonly IEnumerable<Interval.Interval> LydianMinor = GetIntervals(2, 2, 2, 1, 1, 2, 2);

    /// <summary>
    /// 'Lydian Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("lydian pentatonic")]
    public static readonly IEnumerable<Interval.Interval> LydianPentatonic = GetIntervals(4, 2, 1, 4, 1);

    /// <summary>
    /// 'Major' scale's intervals sequence.
    /// </summary>
    [DisplayName("major")]
    public static readonly IEnumerable<Interval.Interval> Major = GetIntervals(2, 2, 1, 2, 2, 2, 1);

    /// <summary>
    /// 'Major Blues' scale's intervals sequence.
    /// </summary>
    [DisplayName("major blues")]
    public static readonly IEnumerable<Interval.Interval> MajorBlues = GetIntervals(2, 1, 1, 3, 2, 3);

    /// <summary>
    /// 'Major Flat Two Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("major flat two pentatonic")]
    public static readonly IEnumerable<Interval.Interval> MajorFlatTwoPentatonic = GetIntervals(1, 3, 3, 2, 3);

    /// <summary>
    /// 'Major Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("major pentatonic")]
    public static readonly IEnumerable<Interval.Interval> MajorPentatonic = GetIntervals(2, 2, 3, 2, 3);

    /// <summary>
    /// 'Malkos Raga' scale's intervals sequence.
    /// </summary>
    [DisplayName("malkos raga")]
    public static readonly IEnumerable<Interval.Interval> MalkosRaga = GetIntervals(3, 2, 3, 2, 2);

    /// <summary>
    /// 'Melodic Minor' scale's intervals sequence.
    /// </summary>
    [DisplayName("melodic minor")]
    public static readonly IEnumerable<Interval.Interval> MelodicMinor = GetIntervals(2, 1, 2, 2, 2, 2, 1);

    /// <summary>
    /// 'Melodic Minor Fifth Mode' scale's intervals sequence.
    /// </summary>
    [DisplayName("melodic minor fifth mode")]
    public static readonly IEnumerable<Interval.Interval> MelodicMinorFifthMode = GetIntervals(2, 2, 1, 2, 1, 2, 2);

    /// <summary>
    /// 'Melodic Minor Second Mode' scale's intervals sequence.
    /// </summary>
    [DisplayName("melodic minor second mode")]
    public static readonly IEnumerable<Interval.Interval> MelodicMinorSecondMode = GetIntervals(1, 2, 2, 2, 2, 1, 2);

    /// <summary>
    /// 'Minor' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor")]
    public static readonly IEnumerable<Interval.Interval> Minor = GetIntervals(2, 1, 2, 2, 1, 2, 2);

    /// <summary>
    /// 'Minor #7M Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor #7M pentatonic")]
    public static readonly IEnumerable<Interval.Interval> Minor7MPentatonic = GetIntervals(3, 2, 2, 4, 1);

    /// <summary>
    /// 'Minor Bebop' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor bebop")]
    public static readonly IEnumerable<Interval.Interval> MinorBebop = GetIntervals(2, 1, 2, 2, 1, 2, 1, 1);

    /// <summary>
    /// 'Minor Blues' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor blues")]
    public static readonly IEnumerable<Interval.Interval> MinorBlues = GetIntervals(3, 2, 1, 1, 3, 2);

    /// <summary>
    /// 'Minor Hexatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor hexatonic")]
    public static readonly IEnumerable<Interval.Interval> MinorHexatonic = GetIntervals(2, 1, 2, 2, 4, 1);

    /// <summary>
    /// 'Minor Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor pentatonic")]
    public static readonly IEnumerable<Interval.Interval> MinorPentatonic = GetIntervals(3, 2, 2, 3, 2);

    /// <summary>
    /// 'Minor Seven Flat Five Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor seven flat five pentatonic")]
    public static readonly IEnumerable<Interval.Interval> MinorSevenFlatFivePentatonic = GetIntervals(3, 2, 1, 4, 2);

    /// <summary>
    /// 'Minor Six Diminished' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor six diminished")]
    public static readonly IEnumerable<Interval.Interval> MinorSixDiminished = GetIntervals(2, 1, 2, 2, 1, 1, 2, 1);

    /// <summary>
    /// 'Minor Six Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("minor six pentatonic")]
    public static readonly IEnumerable<Interval.Interval> MinorSixPentatonic = GetIntervals(3, 2, 2, 2, 3);

    /// <summary>
    /// 'Mixolydian' scale's intervals sequence.
    /// </summary>
    [DisplayName("mixolydian")]
    public static readonly IEnumerable<Interval.Interval> Mixolydian = GetIntervals(2, 2, 1, 2, 2, 1, 2);

    /// <summary>
    /// 'Mixolydian b6M' scale's intervals sequence.
    /// </summary>
    [DisplayName("mixolydian b6M")]
    public static readonly IEnumerable<Interval.Interval> MixolydianB6M = GetIntervals(2, 2, 1, 2, 1, 2, 2);

    /// <summary>
    /// 'Mixolydian Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("mixolydian pentatonic")]
    public static readonly IEnumerable<Interval.Interval> MixolydianPentatonic = GetIntervals(4, 1, 2, 3, 2);

    /// <summary>
    /// 'Mystery #1' scale's intervals sequence.
    /// </summary>
    [DisplayName("mystery #1")]
    public static readonly IEnumerable<Interval.Interval> Mystery1 = GetIntervals(1, 3, 2, 2, 2, 2);

    /// <summary>
    /// 'Neopolitan' scale's intervals sequence.
    /// </summary>
    [DisplayName("neopolitan")]
    public static readonly IEnumerable<Interval.Interval> Neopolitan = GetIntervals(1, 2, 2, 2, 1, 3, 1);

    /// <summary>
    /// 'Neopolitan Major' scale's intervals sequence.
    /// </summary>
    [DisplayName("neopolitan major")]
    public static readonly IEnumerable<Interval.Interval> NeopolitanMajor = GetIntervals(1, 2, 2, 2, 2, 2, 1);

    /// <summary>
    /// 'Neopolitan Major Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("neopolitan major pentatonic")]
    public static readonly IEnumerable<Interval.Interval> NeopolitanMajorPentatonic = GetIntervals(4, 1, 1, 4, 2);

    /// <summary>
    /// 'Neopolitan Minor' scale's intervals sequence.
    /// </summary>
    [DisplayName("neopolitan minor")]
    public static readonly IEnumerable<Interval.Interval> NeopolitanMinor = GetIntervals(1, 2, 2, 2, 1, 3, 1);

    /// <summary>
    /// 'Oriental' scale's intervals sequence.
    /// </summary>
    [DisplayName("oriental")]
    public static readonly IEnumerable<Interval.Interval> Oriental = GetIntervals(1, 3, 1, 1, 3, 1, 2);

    /// <summary>
    /// 'Pelog' scale's intervals sequence.
    /// </summary>
    [DisplayName("pelog")]
    public static readonly IEnumerable<Interval.Interval> Pelog = GetIntervals(1, 2, 4, 1, 4);

    /// <summary>
    /// 'Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("pentatonic")]
    public static readonly IEnumerable<Interval.Interval> Pentatonic = GetIntervals(2, 2, 3, 2, 3);

    /// <summary>
    /// 'Persian' scale's intervals sequence.
    /// </summary>
    [DisplayName("persian")]
    public static readonly IEnumerable<Interval.Interval> Persian = GetIntervals(1, 3, 1, 1, 2, 3, 1);

    /// <summary>
    /// 'Phrygian' scale's intervals sequence.
    /// </summary>
    [DisplayName("phrygian")]
    public static readonly IEnumerable<Interval.Interval> Phrygian = GetIntervals(1, 2, 2, 2, 1, 2, 2);

    /// <summary>
    /// 'Phrygian Major' scale's intervals sequence.
    /// </summary>
    [DisplayName("phrygian major")]
    public static readonly IEnumerable<Interval.Interval> PhrygianMajor = GetIntervals(1, 3, 1, 2, 1, 2, 2);

    /// <summary>
    /// 'Piongio' scale's intervals sequence.
    /// </summary>
    [DisplayName("piongio")]
    public static readonly IEnumerable<Interval.Interval> Piongio = GetIntervals(2, 3, 2, 2, 1, 2);

    /// <summary>
    /// 'Pomeroy' scale's intervals sequence.
    /// </summary>
    [DisplayName("pomeroy")]
    public static readonly IEnumerable<Interval.Interval> Pomeroy = GetIntervals(1, 2, 1, 2, 2, 2, 2);

    /// <summary>
    /// 'Prometheus' scale's intervals sequence.
    /// </summary>
    [DisplayName("prometheus")]
    public static readonly IEnumerable<Interval.Interval> Prometheus = GetIntervals(2, 2, 2, 3, 1, 2);

    /// <summary>
    /// 'Prometheus Neopolitan' scale's intervals sequence.
    /// </summary>
    [DisplayName("prometheus neopolitan")]
    public static readonly IEnumerable<Interval.Interval> PrometheusNeopolitan = GetIntervals(1, 3, 2, 3, 1, 2);

    /// <summary>
    /// 'Purvi Raga' scale's intervals sequence.
    /// </summary>
    [DisplayName("purvi raga")]
    public static readonly IEnumerable<Interval.Interval> PurviRaga = GetIntervals(1, 3, 1, 1, 1, 1, 3, 1);

    /// <summary>
    /// 'Ritusen' scale's intervals sequence.
    /// </summary>
    [DisplayName("ritusen")]
    public static readonly IEnumerable<Interval.Interval> Ritusen = GetIntervals(2, 3, 2, 2, 3);

    /// <summary>
    /// 'Romanian Minor' scale's intervals sequence.
    /// </summary>
    [DisplayName("romanian minor")]
    public static readonly IEnumerable<Interval.Interval> RomanianMinor = GetIntervals(2, 1, 3, 1, 2, 1, 2);

    /// <summary>
    /// 'Scriabin' scale's intervals sequence.
    /// </summary>
    [DisplayName("scriabin")]
    public static readonly IEnumerable<Interval.Interval> Scriabin = GetIntervals(1, 3, 3, 2, 3);

    /// <summary>
    /// 'Six Tone Symmetric' scale's intervals sequence.
    /// </summary>
    [DisplayName("six tone symmetric")]
    public static readonly IEnumerable<Interval.Interval> SixToneSymmetric = GetIntervals(1, 3, 1, 3, 1, 3);

    /// <summary>
    /// 'Spanish' scale's intervals sequence.
    /// </summary>
    [DisplayName("spanish")]
    public static readonly IEnumerable<Interval.Interval> Spanish = GetIntervals(1, 3, 1, 2, 1, 2, 2);

    /// <summary>
    /// 'Spanish Heptatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("spanish heptatonic")]
    public static readonly IEnumerable<Interval.Interval> SpanishHeptatonic = GetIntervals(1, 2, 1, 1, 2, 1, 2, 2);

    /// <summary>
    /// 'Super Locrian' scale's intervals sequence.
    /// </summary>
    [DisplayName("super locrian")]
    public static readonly IEnumerable<Interval.Interval> SuperLocrian = GetIntervals(1, 2, 1, 2, 2, 2, 2);

    /// <summary>
    /// 'Super Locrian Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("super locrian pentatonic")]
    public static readonly IEnumerable<Interval.Interval> SuperLocrianPentatonic = GetIntervals(3, 1, 2, 4, 2);

    /// <summary>
    /// 'Todi Raga' scale's intervals sequence.
    /// </summary>
    [DisplayName("todi raga")]
    public static readonly IEnumerable<Interval.Interval> TodiRaga = GetIntervals(1, 2, 3, 1, 1, 3, 1);

    /// <summary>
    /// 'Vietnamese 1' scale's intervals sequence.
    /// </summary>
    [DisplayName("vietnamese 1")]
    public static readonly IEnumerable<Interval.Interval> Vietnamese1 = GetIntervals(3, 2, 2, 1, 4);

    /// <summary>
    /// 'Vietnamese 2' scale's intervals sequence.
    /// </summary>
    [DisplayName("vietnamese 2")]
    public static readonly IEnumerable<Interval.Interval> Vietnamese2 = GetIntervals(3, 2, 2, 3, 2);

    /// <summary>
    /// 'Whole Tone' scale's intervals sequence.
    /// </summary>
    [DisplayName("whole tone")]
    public static readonly IEnumerable<Interval.Interval> WholeTone = GetIntervals(2, 2, 2, 2, 2, 2);

    /// <summary>
    /// 'Whole Tone Pentatonic' scale's intervals sequence.
    /// </summary>
    [DisplayName("whole tone pentatonic")]
    public static readonly IEnumerable<Interval.Interval> WholeTonePentatonic = GetIntervals(4, 2, 2, 2, 2);

    #endregion

    #region Methods

    /// <summary>
    /// Gets musical scale's intervals sequence by the scale's name.
    /// </summary>
    /// <param name="name">The name of a scale.</param>
    /// <returns>Intervals sequence for the scale with the name <paramref name="name"/>; or <c>null</c> if
    /// there is no a scale with this name.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <c>null</c> or contains white-spaces only.</exception>
    public static IEnumerable<Interval.Interval> GetByName(string name)
    {
        ThrowIfArgument.IsNullOrWhiteSpaceString(nameof(name), name, "Scale's name");

        foreach (var fieldInfo in typeof(ScaleIntervals).GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            var displayName = (Attribute.GetCustomAttribute(fieldInfo, typeof(DisplayNameAttribute)) as DisplayNameAttribute)?.Name;
            if (string.IsNullOrWhiteSpace(displayName) || !displayName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                continue;

            var intervals = fieldInfo.GetValue(null) as IEnumerable<Interval.Interval>;
            if (intervals == null)
                continue;

            return intervals;
        }

        return null;
    }

    private static IEnumerable<Interval.Interval> GetIntervals(params int[] intervalsInHalfSteps)
    {
        return intervalsInHalfSteps.Select(i => Interval.Interval.FromHalfSteps(i)).ToArray();
    }

    #endregion
}