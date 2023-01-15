#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro;

public abstract class GPFile
{
    public TripletFeel _tripletFeel;
    public string album;
    public string author;


    public GP4File.Clipboard clipboard = null;
    public string copyright;
    public List<DirectionSign> directions = new();
    public bool hideTempo;
    public string instructional;
    public string interpret;
    public List<Lyrics> lyrics = new();
    public RSEMasterEffect masterEffect = null;
    public List<MeasureHeader> measureHeaders = new();
    public string music;
    public PageSetup pageSetup = null;
    public GPFile self;
    public string subtitle;
    public string tab_author;
    public int tempo;
    public string tempoName;
    public string title;
    public List<Track> tracks = new();
    public string version = "";
    public int[] versionTuple = { };

    public string words;

    //Parent class for common type
    public abstract void readSong();
}

public class GPBase
{
    public const int bendPosition = 60;
    public const int bendSemitone = 25;
    public static int pointer;
    public static byte[] data;

    public static void skip(int count)
    {
        pointer += count;
    }

    public static byte[] readByte(int count = 1)
    {
        return extract(pointer, count, true);
    }

    public static sbyte[] readSignedByte(int count = 1)
    {
        var unsigned = extract(pointer, count, true);
        var ret_val = new sbyte[unsigned.Length];
        for (var x = 0; x < unsigned.Length; x++) ret_val[x] = (sbyte)unsigned[x];

        return ret_val;
    }

    public static bool[] readBool(int count = 1)
    {
        var vals = extract(pointer, count, true);
        var ret_val = new bool[vals.Length];
        for (var x = 0; x < vals.Length; x++) ret_val[x] = vals[x] != 0x0;

        return ret_val;
    }

    public static short[] readShort(int count = 1)
    {
        var vals = extract(pointer, count * 2, true);
        var ret_val = new short[count];
        for (var x = 0; x < vals.Length; x += 2) ret_val[x / 2] = (short)(vals[x] + (vals[x + 1] << 8));

        return ret_val;
    }

    public static int[] readInt(int count = 1)
    {
        var vals = extract(pointer, count * 4, true);
        var ret_val = new int[count];
        for (var x = 0; x < vals.Length; x += 4)
            ret_val[x / 4] = vals[x] + (vals[x + 1] << 8) + (vals[x + 2] << 16) + (vals[x + 3] << 24);

        return ret_val;
    }

    public static float[] readFloat(int count = 1)
    {
        var vals = extract(pointer, count * 4, true);
        var ret_val = new float[count];
        for (var x = 0; x < vals.Length; x += 4) ret_val[x / 4] = BitConverter.ToSingle(vals, x);

        return ret_val;
    }

    public static double[] readDouble(int count = 1)
    {
        var vals = extract(pointer, count * 8, true);
        var ret_val = new double[count];
        for (var x = 0; x < vals.Length; x += 8) ret_val[x / 8] = BitConverter.ToDouble(vals, x);

        return ret_val;
    }

    public static string readString(int size, int length = 0)
    {
        if (length == 0) length = size;

        var count = size > 0 ? size : length;
        var ss = length >= 0 ? extract(pointer, length, true) : extract(pointer, size, true);
        skip(count - ss.Length);
        return Encoding.Default.GetString(ss);
    }

    public static string readByteSizeString(int size)
    {
        return readString(size, readByte()[0]);
    }

    public static string readIntSizeString()
    {
        return readString(readInt()[0]);
    }

    public static string readIntByteSizeString()
    {
        //Read length of the string increased by 1 and stored in 1 integer
        //followed by length of the string in 1 byte and finally followed by
        //character bytes.

        var d = readInt()[0] - 1;

        return readByteSizeString(d);
    }


    public static byte[] extract(int start, int length, bool advance_pointer)
    {
        if (length <= 0) return new byte[Math.Max(0, length)];
        if (length + start > data.Length) return new byte[Math.Max(0, length)];

        var ret = new byte[length];
        for (var x = start; x < start + length; x++) ret[x - start] = data[x];

        if (advance_pointer) pointer += length;

        return ret;
    }
}

public sealed class Barre
{
    public int end;
    public int fret;
    public int start;

    public Barre(int fret = 0, int start = 0, int end = 0)
    {
        this.start = start;
        this.fret = fret;
        this.end = end;
    }

    public int[] range()
    {
        return new[] { start, end };
    }
}

public sealed class Beat
{
    public BeatDisplay display = new();
    public Duration duration = new();
    public BeatEffect effect = new();
    public Measure measure;
    public List<Note> notes = new();
    public Octave octave = Octave.none;
    public int start = Duration.quarterTime;
    public BeatStatus status;
    public BeatText text = new();
    public Voice voice;
    public List<Voice> voices = new();

    public Beat(Voice voice = null)
    {
        this.voice = voice;
    }

    public int realStart()
    {
        var offset = start + measure.start();
        return measure.header.realStart + offset;
    }

    public bool hasVibrato()
    {
        return notes.Any(static note => note.effect.vibrato);
    }

    public bool hasHarmonic()
    {
        return notes.Any(static note => note.effect.isHarmonic());
    }

    public void addNote(Note note)
    {
        note.beat = this;
        notes.Add(note);
    }
}

public sealed class BeatDisplay
{
    public VoiceDirection beamDirection = VoiceDirection.none;
    public bool breakBeam = false;
    public int breakSecondary = 0;
    public bool breakSecondaryTuplet = false;
    public bool forceBeam = false;
    public bool forceBracket = false;
    public TupletBracket tupletBracket = TupletBracket.none;
}

public sealed class BeatEffect
{
    public Chord chord = null;
    public bool fadeIn = false;
    public bool fadeOut = false;
    public bool hasRasgueado = false;
    public MixTableChange mixTableChange;

    public BeatStrokeDirection pickStroke = BeatStrokeDirection.none;
    public SlapEffect slapEffect = SlapEffect.none;
    public BeatStroke stroke = null;
    public BendEffect tremoloBar = null;
    public bool vibrato = false;
    public bool volumeSwell = false;

    public bool isChord()
    {
        return chord != null;
    }

    public bool isTremoloBar()
    {
        return tremoloBar != null;
    }

    public bool isSlapEffect()
    {
        return slapEffect != SlapEffect.none;
    }

    public bool hasPickStroke()
    {
        return pickStroke != BeatStrokeDirection.none;
    }

    public bool isDefault()
    {
        var def = new BeatEffect();
        return stroke == def.stroke && hasRasgueado == def.hasRasgueado &&
               pickStroke == def.pickStroke && fadeIn == def.fadeIn &&
               vibrato == def.vibrato && tremoloBar == def.tremoloBar &&
               slapEffect == def.slapEffect;
    }
}

public sealed class BeatStroke
{
    public BeatStrokeDirection direction = BeatStrokeDirection.none;
    public float startTime; //0 = falls on time, 1 = starts on time
    public int value; //4 = quarter etc.

    public BeatStroke()
    {
    }

    public BeatStroke(BeatStrokeDirection d, int v, float s)
    {
        direction = d;
        value = v;
        startTime = s;
    }

    public void setByGP6Standard(int GP6Duration)
    {
        //GP6 will use value as 30 to 480 (64th to quarter note)
        int[] possibleVals = { 1, 2, 4, 8, 16, 32, 64 };
        var translated = 64 / (GP6Duration / 30);
        var lastVal = 0;
        foreach (var val in possibleVals)
        {
            if (val == translated)
            {
                value = val;
                break;
            }

            if (val > translated && lastVal < translated)
            {
                value = translated - lastVal > val - translated ? val : lastVal;
                break;
            }

            lastVal = val;
        }
    }

    public int getIncrementTime(Beat beat)
    {
        var duration = 0;
        if (value <= 0) return 0;

        foreach (var currentDuration in from voice in beat.voices
                 where !voice.isEmpty()
                 select voice.duration.time())
        {
            if (duration == 0 || currentDuration < duration)
                duration = currentDuration <= Duration.quarterTime ? currentDuration : Duration.quarterTime;

            if (duration > 0) return (int)Math.Round(duration / 8.0f * (4.0f / value));
        }

        return 0;
    }

    public BeatStroke swapDirection()
    {
        direction = direction switch
        {
            BeatStrokeDirection.up => BeatStrokeDirection.down,
            BeatStrokeDirection.down => BeatStrokeDirection.up,
            _ => direction
        };
        return new BeatStroke(direction, value, 0.0f);
    }
}

public sealed class BeatText
{
    public string value;

    public BeatText(string value = "")
    {
        this.value = value;
    }
}

public sealed class BendEffect
{
    public const int semitoneLength = 1;
    public const int maxPosition = 12;
    public int maxValue = semitoneLength * 12;
    public List<BendPoint> points = new();
    public BendType type = BendType.none;
    public int value = 0;
}

public sealed class BendPoint
{
    public float GP6position;
    public float GP6value;
    public int position;
    public int value;
    private bool vibrato;

    public BendPoint(int position = 0, int value = 0, bool vibrato = false)
    {
        this.position = position;
        this.value = value;
        this.vibrato = vibrato;
        GP6position = position * 100.0f / BendEffect.maxPosition;
        GP6value = value * 25.0f / BendEffect.semitoneLength;
    }

    public BendPoint(float position, float value, bool isGP6Format = true)
    {
        if (isGP6Format)
        {
            //GP6 Format: position: 0-100, value: 100 = 1 whole tone up
            this.position = (int)(position * BendEffect.maxPosition / 100);
            this.value = (int)(value * 2 * BendEffect.semitoneLength / 100);
            GP6position = position;
            GP6value = value;
        }
        else
        {
            this.position = (int)position;
            this.value = (int)value;
            GP6position = position * 100.0f / BendEffect.maxPosition;
            GP6value = value * 50.0f / BendEffect.semitoneLength;
        }
    }

    public int getTime(int duration)
    {
        return (int)(duration * (float)position / BendEffect.maxPosition);
    }
}

public sealed class Chord
{
    public bool add;
    public List<Barre> barres = new();
    public PitchClass bass;
    public ChordAlteration eleventh;
    public ChordExtension extension;
    public ChordAlteration fifth;
    public List<Fingering> fingerings = new();
    public int firstFret;
    public string name = "";
    public bool newFormat;
    public ChordAlteration ninth;
    public bool[] omissions = new bool[7];
    public PitchClass root;
    public bool sharp;
    public bool show = true;
    public int[] strings;
    public ChordAlteration tonality;
    public ChordType type;

    public Chord(int length)
    {
        strings = new int[length];
        for (var x = 0; x < length; x++) strings[x] = -1;
    }

    public int[] notes()
    {
        return strings.Where(static s => s >= 0).ToArray();
    }
}

public sealed class DirectionSign
{
    public short measure;
    public string name;

    public DirectionSign(string name = "", short measure = 0)
    {
        this.name = name;
        this.measure = measure;
    }
}

public sealed class Duration
{
    public const int quarterTime = 960;
    public const int whole = 1;
    public const int half = 2;
    public const int quarter = 4;
    public const int eigth = 8;
    public const int sixteenth = 16;
    public const int thirtySecond = 32;
    public const int sixtyFourth = 64;
    public const int hundredTwentyEigth = 128;

    private const int minTime = (int)((int)(quarterTime * (4.0f / sixtyFourth)) * 2.0f / 3.0f);
    public bool isDotted;
    public bool isDoubleDotted;
    public Tuplet tuplet = new();

    public int value = quarter;

    public Duration()
    {
    }

    public Duration(int time) //Does not recognize tuplets
    {
        //From GP6 Format -> 30 = 64th, 480 = quarter, 1920 = whole
        var substract = 0;
        if (time >= 15)
        {
            value = hundredTwentyEigth;
            substract = 15;
        }

        if (time >= 30)
        {
            value = sixtyFourth;
            substract = 30;
        }

        if (time >= 60)
        {
            value = thirtySecond;
            substract = 60;
        }

        if (time >= 120)
        {
            value = sixteenth;
            substract = 120;
        }

        if (time >= 240)
        {
            value = eigth;
            substract = 240;
        }

        if (time >= 480)
        {
            value = quarter;
            substract = 480;
        }

        if (time >= 960)
        {
            value = half;
            substract = 960;
        }

        if (time >= 1920)
        {
            value = whole;
            substract = 1920;
        }

        time -= substract;
        if (time >= value * 0.5f) isDotted = true;

        if (!(time >= value * 0.75f)) return;

        isDotted = false;
        isDoubleDotted = true;
    }

    public int time()
    {
        var result = (int)(quarterTime * (4.0f / value));
        if (isDotted) result += (int)(result / 2.0f);

        if (isDoubleDotted) result += (int)(result / 4.0f * 3);

        return tuplet.convertTime(result);
    }
}

public sealed class GraceEffect
{
    public int duration = -1;
    public int fret = 0;
    public bool isDead = false;
    public bool isOnBeat = false;
    public GraceEffectTransition transition = GraceEffectTransition.none;
    public int velocity = Velocities.def;

    public int durationTime()
    {
        return (int)(Duration.quarterTime / 16.0f * duration);
    }
}

public sealed class GuitarString
{
    public int number, value;

    public GuitarString(int number, int value)
    {
        this.number = number;
        this.value = value;
    }
}

public abstract class HarmonicEffect
{
    public float fret;
    public int type;
}

public sealed class NaturalHarmonic : HarmonicEffect
{
    public NaturalHarmonic()
    {
        type = 1;
    }
}

public sealed class ArtificialHarmonic : HarmonicEffect
{
    public Octave octave;
    public PitchClass pitch;

    public ArtificialHarmonic(PitchClass pitch = null, Octave octave = 0)
    {
        this.pitch = pitch;
        this.octave = octave;
        type = 2;
    }
}

public sealed class TappedHarmonic : HarmonicEffect
{
    public TappedHarmonic(int fret = 0)
    {
        this.fret = fret;
        type = 3;
    }
}

public sealed class PinchHarmonic : HarmonicEffect
{
    public PinchHarmonic()
    {
        type = 4;
    }
}

public sealed class SemiHarmonic : HarmonicEffect
{
    public SemiHarmonic()
    {
        type = 5;
    }
}

public sealed class FeedbackHarmonic : HarmonicEffect
{
    public FeedbackHarmonic()
    {
        type = 6;
    }
}

public sealed class LyricLine
{
    public string lyrics = "";
    public int startingMeasure = 1;
}

public sealed class Lyrics
{
    private static readonly int maxLineCount = 5;
    public LyricLine[] lines;
    public int trackChoice;


    public Lyrics()
    {
        trackChoice = -1;
        lines = new LyricLine[maxLineCount];
        for (var x = 0; x < maxLineCount; x++) lines[x] = new LyricLine();
    }
}

public sealed class Marker
{
    public myColor color = new(255, 0, 0);
    public MeasureHeader measureHeader = null;
    public string title = "Section";
}

public enum SimileMark
{
    none = 0,
    simple = 1,
    firstOfDouble = 2,
    secondOfDouble = 3
}

public sealed class Measure
{
    public const int maxVoices = 2;
    public List<Beat> beats = new();
    public MeasureClef clef = MeasureClef.treble;
    public MeasureHeader header;
    public LineBreak lineBreak = LineBreak.none;
    public SimileMark simileMark = SimileMark.none;
    public Track track;
    public List<Voice> voices = new();

    public Measure(Track track = null, MeasureHeader header = null)
    {
        if (voices.Count == 0)
            for (var x = 0; x < maxVoices; x++)
                voices.Add(new Voice(this));
        this.header = header;
        this.track = track;
    }

    public bool isEmpty()
    {
        if (voices.Any(static v => !v.isEmpty())) return false;

        return beats.Count == 0;
    }

    public int end()
    {
        return start() + length();
    }

    public int number()
    {
        return header.number;
    }

    public KeySignature keySignature()
    {
        return header.keySignature;
    }

    public int repeatClose()
    {
        return header.repeatClose;
    }

    public int start()
    {
        return header.start;
    }

    public int length()
    {
        return header.length();
    }

    public Tempo tempo()
    {
        return header.tempo;
    }

    public TimeSignature timeSignature()
    {
        return header.timeSignature;
    }

    public bool isRepeatOpen()
    {
        return header.isRepeatOpen;
    }

    public TripletFeel tripletFeel()
    {
        return header.tripletFeel;
    }

    public bool hasMarker()
    {
        return header.hasMarker();
    }

    public Marker marker()
    {
        return header.marker;
    }

    public void addVoice(Voice voice)
    {
        voice.measure = this;
        voices.Add(voice);
    }
}

public sealed class MeasureHeader
{
    public List<string> direction = new();
    public List<string> fromDirection = new();
    public bool hasDoubleBar = false;
    public bool isRepeatOpen = false;
    public KeySignature keySignature = KeySignature.CMajor;
    public Marker marker = null;
    public int number = 0;
    public int realStart = -1;
    public List<int> repeatAlternatives = new();
    public int repeatClose = -1;
    public RepeatGroup repeatGroup;
    public GPFile song;
    public int start = Duration.quarterTime;
    public Tempo tempo = new();
    public TimeSignature timeSignature = new();
    public TripletFeel tripletFeel = TripletFeel.none;

    public bool hasMarker()
    {
        return marker != null;
    }

    public int length()
    {
        return timeSignature.numerator * timeSignature.denominator.time();
    }
}

public sealed class MidiChannel
{
    private static readonly int DEFAULT_PERCUSSION_CHANNEL = 9;

    public int channel,
        effectChannel,
        instrument,
        volume,
        balance,
        chorus,
        reverb,
        phaser,
        tremolo,
        bank;

    public MidiChannel()
    {
        channel = 0;
        effectChannel = 1;
        instrument = 25;
        volume = 104;
        balance = 64;
        chorus = 0;
        reverb = 0;
        phaser = 0;
        tremolo = 0;
        bank = 0;
    }

    public bool isPercussionChannel()
    {
        return channel % 16 == DEFAULT_PERCUSSION_CHANNEL;
    }
}

public enum WahState
{
    off = -2,
    none = -1,
    opened = 0,
    closed = 100
}

public sealed class WahEffect
{
    public bool display = false;
    public WahState state = WahState.none;
}

public sealed class MixTableChange
{
    public MixTableItem balance = null;
    public MixTableItem chorus = null;
    public bool hideTempo;
    public MixTableItem instrument = null;
    public MixTableItem phaser = null;
    public MixTableItem reverb = null;
    public RSEInstrument rse = null;
    public MixTableItem tempo = null;
    public string tempoName;
    public MixTableItem tremolo = null;
    public bool useRSE;
    public MixTableItem volume = null;
    public WahEffect wah = null;

    public MixTableChange(string tempoName = "", bool hideTempo = true, bool useRSE = true)
    {
        this.tempoName = tempoName;
        this.hideTempo = hideTempo;
        this.useRSE = useRSE;
    }

    public bool isJustWah()
    {
        return instrument == null && volume == null && balance == null && chorus == null && reverb == null &&
               phaser == null && tremolo == null && wah != null;
    }
}

public sealed class MixTableItem
{
    public bool allTracks;
    public int duration;
    public int value;

    public MixTableItem(int value = 0, int duration = 0, bool allTracks = false)
    {
        this.value = value;
        this.duration = duration;
        this.allTracks = allTracks;
    }
}

public sealed class myColor
{
    private float r, g, b, a;

    public myColor(int r, int g, int b, int a = 255)
    {
        this.r = r / 255.0f;
        this.g = g / 255.0f;
        this.b = b / 255.0f;
        this.a = a / 255.0f;
    }
}

public sealed class Note
{
    public Beat beat;
    public int duration;
    public double durationPercent = 1.0;
    public NoteEffect effect = new();
    public int str = 0;
    public bool swapAccidentals = false;
    public int tuplet;
    public NoteType type = NoteType.rest;
    public int value = 0;
    public int velocity = Velocities.def;

    public Note(Beat beat = null)
    {
        this.beat = beat;
    }

    public int realValue()
    {
        return value + beat.voice.measure.track.strings[str - 1].value;
    }
}

public sealed class NoteEffect
{
    public bool accentuatedNote = false;
    public BendEffect bend = null;
    public bool ghostNote = false;
    public GraceEffect grace = null;
    public bool hammer = false;
    public HarmonicEffect harmonic = null;
    public bool heavyAccentuatedNote = false;
    public Fingering leftHandFinger = Fingering.open;
    public bool letRing = false;
    public Note note = null;
    public bool palmMute = false;
    public Fingering rightHandFinger = Fingering.open;
    public List<SlideType> slides = new();
    public bool staccato = false;
    public TremoloPickingEffect tremoloPicking = null;
    public TrillEffect trill = null;
    public bool vibrato = false;

    public bool isBend()
    {
        return bend != null && bend.points.Count > 0;
    }

    public bool isHarmonic()
    {
        return harmonic != null;
    }

    public bool isGrace()
    {
        return grace != null;
    }

    public bool isTrill()
    {
        return trill != null;
    }

    public bool isTremoloPicking()
    {
        return tremoloPicking != null;
    }

    public bool isFingering()
    {
        return (int)leftHandFinger > -1 || (int)rightHandFinger > -1;
    }
}

public sealed class Padding
{
    public int right, top, left, bottom;

    public Padding(int right = 0, int top = 0, int left = 0, int bottom = 0)
    {
        this.right = right;
        this.top = top;
        this.left = left;
        this.bottom = bottom;
    }
}

public sealed class Point
{
    public int x, y;

    public Point(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }
}

public enum HeaderFooterElements
{
    none = 0x000,
    title = 0x001,
    subtitle = 0x002,
    artist = 0x004,
    album = 0x008,
    words = 0x010,
    music = 0x020,
    wordsAndMusic = 0x040,
    copyright = 0x080,
    pageNumber = 0x100,
    all = title | subtitle | artist | album | words | music | wordsAndMusic | copyright | pageNumber
}

public sealed class PageSetup
{
    public string album = "%album%";
    public string artist = "%artist%";
    public string copyright = "Copyright %copyright%\nAll Rights Reserved - International Copyright Secured";
    public HeaderFooterElements headerAndFooter = HeaderFooterElements.all;
    public string music = "Music by %music%";
    public Padding pageMargin = new(10, 15, 10, 10);

    public string pageNumber = "Page %N%/%P%";

    /*The page setup describes how the document is rendered.

        Page setup contains page size, margins, paddings, and how the title
        elements are rendered.

        Following template vars are available for defining the page texts:

        - ``%title%``: will be replaced with Song.title
        - ``%subtitle%``: will be replaced with Song.subtitle
        - ``%artist%``: will be replaced with Song.artist
        - ``%album%``: will be replaced with Song.album
        - ``%words%``: will be replaced with Song.words
        - ``%music%``: will be replaced with Song.music
        - ``%WORDSANDMUSIC%``: will be replaced with the according word
          and music values
        - ``%copyright%``: will be replaced with Song.copyright
        - ``%N%``: will be replaced with the current page number (if
          supported by layout)
        - ``%P%``: will be replaced with the number of pages (if supported
          by layout)*/
    public Point pageSize = new(210, 297);
    public float scoreSizeProportion = 1.0f;
    public string subtitle = "%subtitle%";
    public string title = "%title%";
    public string words = "Words by %words%";
    public string wordsAndMusic = "Words & Music by %WORDSMUSIC%";
}

public sealed class PitchClass
{
    public int accidental;
    public float actualOvertone;

    public string intonation;

    /*Constructor provides several overloads. Each overload provides keyword
    argument *intonation* that may be either "sharp" or "flat".

    First of overloads is (tone, accidental):

    :param tone: integer of whole-tone.
    :param accidental: flat (-1), none (0) or sharp (1).

    >>> p = PitchClass(4, -1)
    >>> vars(p)
    {'accidental': -1, 'intonation': 'flat', 'just': 4, 'value': 3}
    >>> print p
    Eb
    >>> p = PitchClass(4, -1, intonation='sharp')
    >>> vars(p)
    {'accidental': -1, 'intonation': 'flat', 'just': 4, 'value': 3}
    >>> print p
    D#

    Second, semitone number can be directly passed to constructor:

    :param semitone: integer of semitone.

    >>> p = PitchClass(3)
    >>> print p
    Eb
    >>> p = PitchClass(3, intonation='sharp')
    >>> print p
    D#

    And last, but not least, note name:

    :param name: string representing note.

    >>> p = PitchClass('D#')
    >>> print p
    D#*/
    public int just;
    public int value;

    public PitchClass(int arg0i = 0, int arg1i = -1, string arg0s = "", string intonation = "",
        float actualOvertone = 0.0f)
    {
        string[] _notes_sharp = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        string[] _notes_flat = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };
        var value = 0;
        var accidental = 0;
        this.actualOvertone = actualOvertone; //Make it simpler to use later in internal format

        if (arg1i == -1)
        {
            var str = "";
            if (!arg0s.Equals(""))
            {
                str = arg0s;
                for (var x = 0; x < _notes_sharp.Length; x++)
                {
                    if (str.Equals(_notes_sharp[x]))
                    {
                        value = x;
                        break;
                    }

                    if (!str.Equals(_notes_flat[x])) continue;

                    value = x;
                    break;
                }
            }
            else
            {
                value = arg0i % 12;

                str = _notes_sharp[Math.Max(value, 0)];
                if (intonation.Equals("flat")) str = _notes_flat[value];
            }

            if (str.EndsWith("b", StringComparison.Ordinal))
                accidental = -1;
            else if (str.EndsWith("#", StringComparison.Ordinal)) accidental = 1;
        }
        else
        {
            accidental = arg1i;
            just = arg0i % 12;
            this.accidental = accidental;
            this.value = just + accidental;
            this.intonation = intonation ?? "sharp";
        }
    }
}

public sealed class RepeatGroup
{
    public List<MeasureHeader> closings = new();
    public bool isClosed;
    public List<MeasureHeader> measureHeaders = new();
    public List<MeasureHeader> openings = new();

    public void addMeasureHeader(MeasureHeader h)
    {
        if (!(openings.Count > 0)) openings.Add(h);

        measureHeaders.Add(h);
        h.repeatGroup = this;
        if (h.repeatClose > 0)
        {
            closings.Add(h);
            isClosed = true;
        }
        else if (isClosed)
        {
            isClosed = false;
            openings.Add(h);
        }
    }
}

public sealed class RSEEqualizer
{
    public float gain;
    public List<float> knobs;

    public RSEEqualizer(List<float> knobs = null, float gain = 0.0f)
    {
        this.gain = gain;
        this.knobs = knobs;
    }
}

public sealed class RSEMasterEffect
{
    public RSEEqualizer equalizer;
    public int reverb;
    public int volume;

    public RSEMasterEffect(int volume = 0, int reverb = 0, RSEEqualizer equalizer = null)
    {
        this.volume = volume;
        this.reverb = reverb;
        this.equalizer = equalizer;
        if (equalizer is { knobs: null })
            equalizer.knobs = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    }
}

public sealed class Tempo
{
    public int value;

    public Tempo(int value = 120)
    {
        this.value = value;
    }
}

public sealed class TimeSignature
{
    public int[] beams = { 0, 0, 0, 0 };
    public Duration denominator = new();
    public int numerator = 4;
}

public sealed class Track
{
    public MidiChannel channel = new();
    public myColor color = new(255, 0, 0);
    public int fretCount = 0;
    public bool indicateTuning = true;
    public bool is12StringedGuitarTrack = false;
    public bool isBanjoTrack = false;
    public bool isMute = false;
    public bool isPercussionTrack = false;
    public bool isSolo = false;
    public bool isVisible = true;
    public List<Measure> measures = new();
    public string name = "";
    public int number;
    public int offset = 0; //Capo
    public int port = 0;
    public TrackRSE rse = null;
    public TrackSettings settings = new();
    public GPFile song;
    public List<GuitarString> strings = new();
    public string tuningName = "";
    public bool useRSE = false;

    public Track(GPFile song, int number, List<GuitarString> strings = null, List<Measure> measures = null)
    {
        this.song = song;
        this.number = number;
        if (strings != null) this.strings = strings;

        if (measures != null) this.measures = measures;
    }

    public void addMeasure(Measure measure)
    {
        measure.track = this;
        measures.Add(measure);
    }
}

public sealed class RSEInstrument
{
    public string effect = "";
    public string effectCategory = "";
    public int effectNumber = -1;
    public int instrument = -1;
    public int soundBank = -1;
    public int unknown = 1;
}

public enum Accentuation
{
    none = 0,
    verySoft = 1,
    soft = 2,
    medium = 3,
    strong = 4,
    veryStrong = 5
}

public sealed class TrackRSE
{
    public Accentuation autoAccentuation = Accentuation.none;
    public RSEEqualizer equalizer = null;
    public int humanize = 0;
    public RSEInstrument instrument = null;

    public TrackRSE()
    {
        if (equalizer is { knobs: null })
            equalizer.knobs = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    }
}

public sealed class TrackSettings
{
    public bool autoBrush = false;
    public bool autoLetRing = false;
    public bool diagramList = true;
    public bool diagramsAreBelow = false;
    public bool diagramsInScore = false;
    public bool extendRhythmic = false;
    public bool forceChannels = false;
    public bool forceHorizontal = false;
    public bool notation = true;
    public bool showRyhthm = false;
    public bool tablature = true;
}

public sealed class TremoloPickingEffect
{
    public Duration duration = new();
}

public sealed class TrillEffect
{
    public Duration duration = new();
    public int fret = 0;
}

public sealed class Tuplet
{
    public int enters = 1;
    public int times = 1;

    public int convertTime(int time)
    {
        return (int)(time * (float)times / enters);
    }
}

public class Velocities
{
    public const int minVelocity = 15;
    public const int velocityIncrement = 16;
    public const int pianoPianissimo = minVelocity;
    public const int pianissimo = minVelocity + velocityIncrement;
    public const int piano = minVelocity + velocityIncrement * 2;
    public const int mezzoPiano = minVelocity + velocityIncrement * 3;
    public const int mezzoForte = minVelocity + velocityIncrement * 4;
    public const int forte = minVelocity + velocityIncrement * 5;
    public const int fortissimo = minVelocity + velocityIncrement * 6;
    public const int forteFortissimo = minVelocity + velocityIncrement * 7;
    public const int def = forte;
}

public sealed class Voice
{
    public List<Beat> beats = new();
    public VoiceDirection direction = VoiceDirection.none;
    public Duration duration;
    public Measure measure;

    public Voice(Measure measure = null)
    {
        this.measure = measure;
    }

    public bool isEmpty()
    {
        return beats.Count == 0;
    }

    public void addBeat(Beat beat)
    {
        beat.voice = this;
        beats.Add(beat);
    }
}

public enum BeatStatus
{
    empty = 0,
    normal = 1,
    rest = 2
}

public enum BeatStrokeDirection
{
    none = 0,
    up = 1,
    down = 2
}

public enum BendType
{
    //: No Preset.
    none = 0,

    // Bends
    // =====
    //: A simple bend.
    bend = 1,

    //: A bend and release afterwards.
    bendRelease = 2,

    //: A bend, then release and rebend.
    bendReleaseBend = 3,

    //: Prebend.
    prebend = 4,

    //: Prebend and then release.
    prebendRelease = 5,

    // Tremolobar
    // ==========
    //: Dip the bar down and then back up.
    dip = 6,

    //: Dive the bar.
    dive = 7,

    //: Release the bar up.
    releaseUp = 8,

    //: Dip the bar up and then back down.
    invertedDip = 9,

    //: Return the bar.
    return_ = 10,

    //: Release the bar down.
    releaseDown = 11
}

public enum ChordType
{
    major = 0,
    seventh = 1,
    majorSeventh = 2,
    sixth = 3,
    minor = 4,
    minorSeventh = 5,
    minorMajor = 6,
    minorSixth = 7,
    suspendedSecond = 8,
    suspendedFourth = 9,
    seventhSuspendedSecond = 10,
    seventhSuspendedFourth = 11,
    diminished = 12,
    augmented = 13,
    power = 14
}

public enum ChordAlteration
{
    perfect = 0,
    diminished = 1,
    augmented = 2
}

public enum ChordExtension
{
    none = 0,
    ninth = 1,
    eleventh = 2,
    thirteenth = 3
}

public enum Fingering
{
    unknown = -2,
    open = -1,
    thump = 0,
    index = 1,
    middle = 2,
    annular = 3,
    little = 4
}

public enum GraceEffectTransition
{
    none = 0,
    slide = 1,
    bend = 2,
    hammer = 3
}

public enum KeySignature
{
    FMajorFlat = -80,
    CMajorFlat = -70,
    GMajorFlat = -60,
    DMajorFlat = -50,
    AMajorFlat = -40,
    EMajorFlat = -30,
    BMajorFlat = -20,
    FMajor = -10,
    CMajor = 00,
    GMajor = 10,
    DMajor = 20,
    AMajor = 30,
    EMajor = 40,
    BMajor = 50,
    FMajorSharp = 60,
    CMajorSharp = 70,
    GMajorSharp = 80,

    DMinorFlat = -81,
    AMinorFlat = -71,
    EMinorFlat = -61,
    BMinorFlat = -51,
    FMinor = -41,
    CMinor = -31,
    GMinor = -21,
    DMinor = -11,
    AMinor = 01,
    EMinor = 11,
    BMinor = 21,
    FMinorSharp = 31,
    CMinorSharp = 41,
    GMinorSharp = 51,
    DMinorSharp = 61,
    AMinorSharp = 71,
    EMinorSharp = 81
}

public enum LineBreak
{
    none = 0,
    break_ = 1,
    protect = 2
}

public enum MeasureClef
{
    treble = 0,
    bass = 1,
    tenor = 2,
    alto = 3,
    neutral = 4 //drums
}

public enum NoteType
{
    rest = 0,
    normal = 1,
    tie = 2,
    dead = 3
}

public enum SlapEffect
{
    none = 0,
    tapping = 1,
    slapping = 2,
    popping = 3
}

public enum SlideType
{
    intoFromAbove = -2,
    intoFromBelow = -1,
    none = 0,
    shiftSlideTo = 1,
    legatoSlideTo = 2,
    outDownwards = 3,
    outUpwards = 4,
    pickScrapeOutDownwards = 5,
    pickScrapeOutUpwards = 6
}

public enum TripletFeel
{
    none = 0,
    eigth = 1,
    sixteenth = 2,
    dotted8th = 3,
    dotted16th = 4,
    scottish8th = 5,
    scottish16th = 6
}

public enum TupletBracket
{
    none = 0,
    start = 1,
    end = 2
}

public enum Octave
{
    none = 0,
    ottava = 1,
    quindicesima = 2,
    ottavaBassa = 3,
    quindicesimaBassa = 4
}

public enum VoiceDirection
{
    none = 0,
    up = 1,
    down = 2
}