﻿#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro;

public sealed class GP4File : GPFile
{
    public RepeatGroup _currentRepeatGroup = new();
    public MidiChannel[] channels;

    public KeySignature key;
    public int measureCount;


    //Members of GPFile
    /*
    public string version;
    public int[] versionTuple;
    public string title = "";
    public string subtitle = "";
    public string interpret = "";
    public string album = "";
    public string author = "";
    public string copyright = "";
    public string tab_author = "";
    public string instructional = "";
    public Lyrics lyrics;
    public int tempo;
    public List<Track> tracks = new List<Track>();
    public List<MeasureHeader> measureHeaders = new List<MeasureHeader>();
    public TripletFeel _tripletFeel;
    */
    public string[] notice;
    public int trackCount;


    public GP4File(byte[] _data)
    {
        GPBase.pointer = 0;
        GPBase.data = _data;
    }

    public void addMeasureHeader(MeasureHeader header)
    {
        header.song = this;
        measureHeaders.Add(header);
        if (header.isRepeatOpen || (header.repeatAlternatives.Count > 0 && _currentRepeatGroup.isClosed &&
                                    header.repeatAlternatives[0] <= 0))
            _currentRepeatGroup = new RepeatGroup();

        _currentRepeatGroup.addMeasureHeader(header);
    }

    public void addTrack(Track track)
    {
        track.song = this;
        tracks.Add(track);
    }


    public override void readSong()
    {
        //HEADERS
        //VERSION
        version = readVersion();
        versionTuple = readVersionTuple();
        clipboard = readClipboard();

        //INFORMATION ABOUT THE PIECE
        readInfo();
        _tripletFeel = GPBase.readBool()[0] ? TripletFeel.eigth : TripletFeel.none;
        readLyrics();
        tempo = GPBase.readInt()[0];
        key = (KeySignature)(GPBase.readInt()[0] * 10); //key + 0
        GPBase.readSignedByte(); //octave
        readMidiChannels();
        measureCount = GPBase.readInt()[0];
        trackCount = GPBase.readInt()[0];

        readMeasureHeaders(measureCount);
        readTracks(trackCount, channels);
        readMeasures();
    }

    public Clipboard readClipboard()
    {
        if (!isClipboard()) return null;

        var clipboard = new Clipboard
        {
            startMeasure = GPBase.readInt()[0],
            stopMeasure = GPBase.readInt()[0],
            startTrack = GPBase.readInt()[0],
            stopTrack = GPBase.readInt()[0]
        };
        return clipboard;
    }

    private bool isClipboard()
    {
        return version.StartsWith("CLIPBOARD", StringComparison.Ordinal);
    }

    private string readVersion()
    {
        var version = GPBase.readByteSizeString(30);
        return version;
    }

    private int[] readVersionTuple() //bl0.12
    {
        if (version.Equals("")) return new[] { 4, 0 };

        var tuple = version.Substring(version.Length - 4).Split('.');
        return new[] { Convert.ToInt32(tuple[0]), Convert.ToInt32(tuple[1]) };
    }

    private void readMeasures()
    {
        /*Read measures.

        Measures are written in the following order:

        - measure 1/track 1
        - measure 1/track 2
        - ...
        - measure 1/track m
        - measure 2/track 1
        - measure 2/track 2
        - ...
        - measure 2/track m
        - ...
        - measure n/track 1
        - measure n/track 2
        - ...
        - measure n/track m
*/
        var tempo = new Tempo(this.tempo);
        var start = Duration.quarterTime;
        foreach (var header in measureHeaders)
        {
            header.start = start;
            foreach (var track in tracks)
            {
                var measure = new Measure(track, header);
                tempo = header.tempo;
                track.measures.Add(measure);
                readMeasure(measure);
            }

            header.tempo = tempo;
            start += header.length();
        }
    }

    private void readMeasure(Measure measure)
    {
        /*The measure is written as number of beats followed by sequence
        of beats.*/
        var start = measure.start();
        var voice = measure.voices[0];
        readVoice(start, voice);
    }

    private void readVoice(int start, Voice voice)
    {
        //TODO: The pointer is 13 bytes too early here (when reading for measure 0xa of track 0x2, beats should return 1, not 898989)
        var beats = GPBase.readInt()[0];
        for (var beat = 0; beat < beats; beat++) start += readBeat(start, voice);
    }

    private int readBeat(int start, Voice voice)
    {
        /* The first byte is the beat flags. It lists the data present in
        the current beat:

        - *0x01*: dotted notes
        - *0x02*: presence of a chord diagram
        - *0x04*: presence of a text
        - *0x08*: presence of effects
        - *0x10*: presence of a mix table change event
        - *0x20*: the beat is a n-tuplet
        - *0x40*: status: True if the beat is empty of if it is a rest
        - *0x80*: *blank*

        Flags are followed by:

        - Status: :ref:`byte`. If flag at *0x40* is true, read one byte.
          If value of the byte is *0x00* then beat is empty, if value is
          *0x02* then the beat is rest.

        - Beat duration: :ref:`byte`. See :meth:`readDuration`.

        - Chord diagram. See :meth:`readChord`.

        - Text. See :meth:`readText`.

        - Beat effects. See :meth:`readBeatEffects`.

        - Mix table change effect. See :meth:`readMixTableChange`.*/

        var flags = GPBase.readByte()[0];
        var beat = getBeat(voice, start);
        if ((flags & 0x40) != 0)
            beat.status = (BeatStatus)GPBase.readByte()[0];
        else
            beat.status = BeatStatus.normal;

        var duration = readDuration(flags);
        var effect = new NoteEffect();
        if ((flags & 0x02) != 0) beat.effect.chord = readChord(voice.measure.track.strings.Count);

        if ((flags & 0x04) != 0) beat.text = readText();

        if ((flags & 0x08) != 0) beat.effect = readBeatEffects(effect);

        if ((flags & 0x10) != 0)
        {
            var mixTableChange = readMixTableChange(voice.measure);
            beat.effect.mixTableChange = mixTableChange;
        }

        readNotes(voice.measure.track, beat, duration, effect);
        return beat.status != BeatStatus.empty ? duration.time() : 0;
    }

    private void readNotes(Track track, Beat beat, Duration duration, NoteEffect effect)
    {
        /* First byte lists played strings:

        - *0x01*: 7th string
        - *0x02*: 6th string
        - *0x04*: 5th string
        - *0x08*: 4th string
        - *0x10*: 3th string
        - *0x20*: 2th string
        - *0x40*: 1th string
        - *0x80*: *blank**/

        var stringFlags = GPBase.readByte()[0];
        foreach (var str in track.strings)
        {
            if ((stringFlags & (1 << (7 - str.number))) != 0)
            {
                var note = new Note(beat);
                beat.notes.Add(note);
                readNote(note, str, track);
            }

            beat.duration = duration;
        }
    }

    private void readNote(Note note, GuitarString guitarString, Track track)
    {
        /*The first byte is note flags:

        - *0x01*: time-independent duration
        - *0x02*: heavy accentuated note
        - *0x04*: ghost note
        - *0x08*: presence of note effects
        - *0x10*: dynamics
        - *0x20*: fret
        - *0x40*: accentuated note
        - *0x80*: right hand or left hand fingering

        Flags are followed by:

        - Note type: :ref:`byte`. Note is normal if values is 1, tied if
          value is 2, dead if value is 3.

        - Time-independent duration: 2 :ref:`SignedBytes <signed-byte>`.
          Correspond to duration and tuplet. See :meth:`readDuration`
          for reference.

        - Note dynamics: :ref:`signed-byte`. See :meth:`unpackVelocity`.

        - Fret number: :ref:`signed-byte`. If flag at *0x20* is set then
          read fret number.

        - Fingering: 2 :ref:`SignedBytes <signed-byte>`. See
          :class:`guitarpro.models.Fingering`.

        - Note effects. See :meth:`readNoteEffects`.*/

        var flags = GPBase.readByte()[0];
        note.str = guitarString.number;
        note.effect.ghostNote = (flags & 0x04) != 0;
        if ((flags & 0x20) != 0) note.type = (NoteType)GPBase.readByte()[0];

        if ((flags & 0x01) != 0)
        {
            note.duration = GPBase.readSignedByte()[0];
            note.tuplet = GPBase.readSignedByte()[0];
        }

        if ((flags & 0x10) != 0)
        {
            var dyn = GPBase.readSignedByte()[0];
            note.velocity = unpackVelocity(dyn);
        }

        if ((flags & 0x20) != 0)
        {
            var fret = GPBase.readSignedByte()[0];
            var value = note.type == NoteType.tie ? getTiedNoteValue(guitarString.number, track) : fret;
            note.value = Math.Max(0, Math.Min(99, value));
        }

        if ((flags & 0x80) != 0)
        {
            note.effect.leftHandFinger = (Fingering)GPBase.readSignedByte()[0];
            note.effect.rightHandFinger = (Fingering)GPBase.readSignedByte()[0];
        }

        if ((flags & 0x08) == 0) return;

        note.effect = readNoteEffects(note);
        if (note.effect.isHarmonic() && note.effect.harmonic is TappedHarmonic)
            note.effect.harmonic.fret = note.value + 12;
    }

    private NoteEffect readNoteEffects(Note note)
    {
        /*First byte is note effects flags:

        - *0x01*: bend presence
        - *0x02*: hammer-on/pull-off
        - *0x04*: slide
        - *0x08*: let-ring
        - *0x10*: grace note presence

        Flags are followed by:

        - Bend. See :meth:`readBend`.

        - Grace note. See :meth:`readGrace`.*/

        var noteEffect = note.effect ?? new NoteEffect();
        var flags1 = GPBase.readSignedByte()[0];
        var flags2 = GPBase.readSignedByte()[0];

        noteEffect.hammer = (flags1 & 0x02) != 0;
        noteEffect.letRing = (flags1 & 0x08) != 0;
        noteEffect.staccato = (flags2 & 0x01) != 0;
        noteEffect.palmMute = (flags2 & 0x02) != 0;
        noteEffect.vibrato = (flags2 & 0x40) != 0 || noteEffect.vibrato;

        if ((flags1 & 0x01) != 0) noteEffect.bend = readBend();

        if ((flags1 & 0x10) != 0) noteEffect.grace = readGrace();

        if ((flags2 & 0x04) != 0) noteEffect.tremoloPicking = readTremoloPicking();

        if ((flags2 & 0x08) != 0) noteEffect.slides = readSlides();

        if ((flags2 & 0x10) != 0) noteEffect.harmonic = readHarmonic(note);

        if ((flags2 & 0x20) != 0) noteEffect.trill = readTrill();

        return noteEffect;
    }

    private TremoloPickingEffect readTremoloPicking()
    {
        var value = GPBase.readSignedByte()[0];
        var tp = new TremoloPickingEffect
        {
            duration =
            {
                value = fromTremoloValue(value)
            }
        };
        return tp;
    }

    private int fromTremoloValue(sbyte value)
    {
        return value switch
        {
            1 => Duration.eigth,
            2 => Duration.sixteenth,
            3 => Duration.thirtySecond,
            _ => 8
        };
    }

    private List<SlideType> readSlides()
    {
        var ret_val = new List<SlideType> { (SlideType)GPBase.readSignedByte()[0] };
        return ret_val;
    }

    private HarmonicEffect readHarmonic(Note note)
    {
        /*Harmonic is encoded in :ref:`signed-byte`. Values correspond to:

        - *1*: natural harmonic
        - *3*: tapped harmonic
        - *4*: pinch harmonic
        - *5*: semi-harmonic
        - *15*: artificial harmonic on (*n + 5*)th fret
        - *17*: artificial harmonic on (*n + 7*)th fret
        - *22*: artificial harmonic on (*n + 12*)th fret
*/
        var harmonicType = GPBase.readSignedByte()[0];
        HarmonicEffect harmonic = null;
        switch (harmonicType)
        {
            case 1:
                harmonic = new NaturalHarmonic();
                break;
            case 3:
                harmonic = new TappedHarmonic();
                break;
            case 4:
                harmonic = new PinchHarmonic();
                break;
            case 5:
                harmonic = new SemiHarmonic();
                break;
            case 15:
                var pitch = new PitchClass((note.realValue() + 7) % 12, -1, "", "", 7.0f);
                var octave = Octave.ottava;
                harmonic = new ArtificialHarmonic(pitch, octave);
                break;
            case 17:
                pitch = new PitchClass(note.realValue(), -1, "", "", 12.0f);
                octave = Octave.quindicesima;
                harmonic = new ArtificialHarmonic(pitch, octave);
                break;
            case 22:
                pitch = new PitchClass(note.realValue(), -1, "", "", 5.0f);
                octave = Octave.ottava;
                harmonic = new ArtificialHarmonic(pitch, octave);
                break;
        }

        return harmonic;
    }

    private TrillEffect readTrill()
    {
        var trill = new TrillEffect
        {
            fret = GPBase.readSignedByte()[0],
            duration =
            {
                value = fromTrillPeriod(GPBase.readSignedByte()[0])
            }
        };
        return trill;
    }

    private int fromTrillPeriod(sbyte period)
    {
        return period switch
        {
            1 => Duration.sixteenth,
            2 => Duration.thirtySecond,
            3 => Duration.sixtyFourth,
            _ => Duration.sixteenth
        };
    }

    private GraceEffect readGrace()
    {
        /*- Fret: :ref:`signed-byte`. Number of fret.

        - Dynamic: :ref:`byte`. Dynamic of a grace note, as in
          :attr:`guitarpro.models.Note.velocity`.

        - Transition: :ref:`byte`. See
          :class:`guitarpro.models.GraceEffectTransition`.

        - Duration: :ref:`byte`. Values are:

          - *1*: Thirty-second note.
          - *2*: Twenty-fourth note.
          - *3*: Sixteenth note.*/
        var grace = new GraceEffect
        {
            fret = GPBase.readSignedByte()[0],
            velocity = unpackVelocity(GPBase.readByte()[0]),
            duration = 1 << (7 - GPBase.readByte()[0])
        };
        grace.isDead = grace.fret == -1;
        grace.isOnBeat = false;
        grace.transition = (GraceEffectTransition)GPBase.readSignedByte()[0];
        return grace;
    }

    private BendEffect readBend()
    {
        /*Encoded as:

        -Bend type: :ref:`signed - byte`. See
           :class:`guitarpro.models.BendType`.

        - Bend value: :ref:`int`.

        - Number of bend points: :ref:`int`.

        - List of points.Each point consists of:

          * Position: :ref:`int`. Shows where point is set along
            *x*-axis.

          * Value: :ref:`int`. Shows where point is set along *y*-axis.

          * Vibrato: :ref:`bool`. */
        var bendEffect = new BendEffect
        {
            type = (BendType)GPBase.readSignedByte()[0],
            value = GPBase.readInt()[0]
        };
        var pointCount = GPBase.readInt()[0];
        for (var x = 0; x < pointCount; x++)
        {
            var position =
                (int)Math.Round(GPBase.readInt()[0] * BendEffect.maxPosition / (float)GPBase.bendPosition);
            var value = (int)Math.Round(
                GPBase.readInt()[0] * BendEffect.semitoneLength / (float)GPBase.bendSemitone);
            var vibrato = GPBase.readBool()[0];
            bendEffect.points.Add(new BendPoint(position, value, vibrato));
        }

        return bendEffect;
    }

    private int getTiedNoteValue(int stringIndex, Track track)
    {
        for (var measure = track.measures.Count - 1; measure >= 0; measure--)
        for (var voice = track.measures[measure].voices.Count - 1; voice >= 0; voice--)
            foreach (var note in from beat in track.measures[measure].voices[voice].beats
                     where beat.status != BeatStatus.empty
                     from note in beat.notes
                     where note.str == stringIndex
                     select note)
                return note.value;

        return -1;
    }

    private int unpackVelocity(sbyte dyn)
    {
        return Velocities.minVelocity +
               Velocities.velocityIncrement * dyn -
               Velocities.velocityIncrement;
    }

    private int unpackVelocity(byte dyn)
    {
        return Velocities.minVelocity +
               Velocities.velocityIncrement * dyn -
               Velocities.velocityIncrement;
    }


    private MixTableChange readMixTableChange(Measure measure)
    {
        var tableChange = new MixTableChange();
        readMixTableChangeValues(tableChange, measure);
        readMixTableChangeDurations(tableChange);
        readMixTableChangeFlags(tableChange);
        return tableChange;
    }

    private void readMixTableChangeFlags(MixTableChange tableChange)
    {
        /* The meaning of flags:

        - *0x01*: change volume for all tracks
        - *0x02*: change balance for all tracks
        - *0x04*: change chorus for all tracks
        - *0x08*: change reverb for all tracks
        - *0x10*: change phaser for all tracks
        - *0x20*: change tremolo for all tracks*/

        var flags = GPBase.readSignedByte()[0];
        if (tableChange.volume != null) tableChange.volume.allTracks = (flags & 0x01) != 0;

        if (tableChange.balance != null) tableChange.balance.allTracks = (flags & 0x02) != 0;

        if (tableChange.chorus != null) tableChange.chorus.allTracks = (flags & 0x04) != 0;

        if (tableChange.reverb != null) tableChange.reverb.allTracks = (flags & 0x08) != 0;

        if (tableChange.phaser != null) tableChange.phaser.allTracks = (flags & 0x10) != 0;

        if (tableChange.tremolo != null) tableChange.tremolo.allTracks = (flags & 0x20) != 0;
    }

    private void readMixTableChangeValues(MixTableChange tableChange, Measure measure)
    {
        var instrument = GPBase.readSignedByte()[0];
        var volume = GPBase.readSignedByte()[0];
        var balance = GPBase.readSignedByte()[0];
        var chorus = GPBase.readSignedByte()[0];
        var reverb = GPBase.readSignedByte()[0];
        var phaser = GPBase.readSignedByte()[0];
        var tremolo = GPBase.readSignedByte()[0];
        var tempo = GPBase.readInt()[0];
        if (instrument >= 0) tableChange.instrument = new MixTableItem(instrument);

        if (volume >= 0) tableChange.volume = new MixTableItem(volume);

        if (balance >= 0) tableChange.balance = new MixTableItem(balance);

        if (chorus >= 0) tableChange.chorus = new MixTableItem(chorus);

        if (reverb >= 0) tableChange.reverb = new MixTableItem(reverb);

        if (phaser >= 0) tableChange.phaser = new MixTableItem(phaser);

        if (tremolo >= 0) tableChange.tremolo = new MixTableItem(tremolo);

        if (tempo < 0) return;

        tableChange.tempo = new MixTableItem(tempo);
        measure.tempo().value = tempo;
    }

    private void readMixTableChangeDurations(MixTableChange tableChange)
    {
        if (tableChange.volume != null) tableChange.volume.duration = GPBase.readSignedByte()[0];

        if (tableChange.balance != null) tableChange.balance.duration = GPBase.readSignedByte()[0];

        if (tableChange.chorus != null) tableChange.chorus.duration = GPBase.readSignedByte()[0];

        if (tableChange.reverb != null) tableChange.reverb.duration = GPBase.readSignedByte()[0];

        if (tableChange.phaser != null) tableChange.phaser.duration = GPBase.readSignedByte()[0];

        if (tableChange.tremolo != null) tableChange.tremolo.duration = GPBase.readSignedByte()[0];

        if (tableChange.tempo == null) return;

        tableChange.tempo.duration = GPBase.readSignedByte()[0];
        tableChange.hideTempo = false;
    }

    private BeatEffect readBeatEffects(NoteEffect effect)
    {
        /*
         * The first byte is effects flags:

        - *0x01*: vibrato
        - *0x02*: wide vibrato
        - *0x04*: natural harmonic
        - *0x08*: artificial harmonic
        - *0x10*: fade in
        - *0x20*: tremolo bar or slap effect
        - *0x40*: beat stroke direction
        - *0x80*: *blank*

        - Tremolo bar or slap effect: :ref:`byte`. If it's 0 then
          tremolo bar should be read (see :meth:`readTremoloBar`). Else
          it's tapping and values of the byte map to:

          - *1*: tap
          - *2*: slap
          - *3*: pop

        - Beat stroke direction. See :meth:`readBeatStroke`.*/
        var beatEffect = new BeatEffect();
        var flags1 = GPBase.readSignedByte()[0];
        var flags2 = GPBase.readSignedByte()[0];
        //effect.vibrato = ((flags1 & 0x01) != 0) || effect.vibrato;
        beatEffect.vibrato = (flags1 & 0x02) != 0 || beatEffect.vibrato;
        beatEffect.fadeIn = (flags1 & 0x10) != 0;
        if ((flags1 & 0x20) != 0)
        {
            var value = GPBase.readSignedByte()[0];
            beatEffect.slapEffect = (SlapEffect)value;
        }

        if ((flags2 & 0x04) != 0) beatEffect.tremoloBar = readTremoloBar();

        if ((flags1 & 0x40) != 0) beatEffect.stroke = readBeatStroke();

        if ((flags2 & 0x02) == 0) return beatEffect;

        var direction = GPBase.readSignedByte()[0];
        beatEffect.pickStroke = (BeatStrokeDirection)direction;

        return beatEffect;
    }

    private BeatStroke readBeatStroke()
    {
        var strokeDown = GPBase.readSignedByte()[0];
        var strokeUp = GPBase.readSignedByte()[0];
        return strokeUp > 0
            ? new BeatStroke(BeatStrokeDirection.up, toStrokeValue(strokeUp), 0.0f)
            : new BeatStroke(BeatStrokeDirection.down, toStrokeValue(strokeDown), 0.0f);
    }

    private int toStrokeValue(sbyte value)
    {
        /*Unpack stroke value.

        Stroke value maps to:

        - *1*: hundred twenty-eighth
        - *2*: sixty-fourth
        - *3*: thirty-second
        - *4*: sixteenth
        - *5*: eighth
        - *6*: quarter*/
        return value switch
        {
            1 => Duration.hundredTwentyEigth,
            2 => Duration.sixtyFourth,
            3 => Duration.thirtySecond,
            4 => Duration.sixteenth,
            5 => Duration.eigth,
            6 => Duration.quarter,
            _ => Duration.sixtyFourth
        };
    }

    private BendEffect readTremoloBar()
    {
        return readBend();
    }

    private BeatText readText()
    {
        var text = new BeatText
        {
            value = GPBase.readIntByteSizeString()
        };
        return text;
    }

    private Chord readChord(int stringCount)
    {
        var chord = new Chord(stringCount)
        {
            newFormat = GPBase.readBool()[0]
        };
        if (!chord.newFormat)
            readOldChord(chord);
        else
            readNewChord(chord);
        return chord.notes().Length > 0 ? chord : null;
    }


    private void readOldChord(Chord chord)
    {
        /*Read chord diagram encoded in GP3 format.

        Chord diagram is read as follows:

        - Name: :ref:`int-byte-size-string`. Name of the chord, e.g.
          *Em*.

        - First fret: :ref:`int`. The fret from which the chord is
          displayed in chord editor.

        - List of frets: 6 :ref:`Ints <int>`. Frets are listed in order:
          fret on the string 1, fret on the string 2, ..., fret on the
          string 6. If string is untouched then the values of fret is
          *-1*.*/

        chord.name = GPBase.readIntByteSizeString();
        chord.firstFret = GPBase.readInt()[0];
        if (chord.firstFret <= 0) return;

        for (var i = 0; i < 6; i++)
        {
            var fret = GPBase.readInt()[0];
            if (i < chord.strings.Length) chord.strings[i] = fret;
        }
    }

    private void readNewChord(Chord chord)
    {
        /*Read new-style (GP4) chord diagram.

        New-style chord diagram is read as follows:

        - Sharp: :ref:`bool`. If true, display all semitones as sharps,
          otherwise display as flats.

        - Blank space, 3 :ref:`Bytes <byte>`.

        - Root: :ref:`int`. Values are:

          * -1 for customized chords
          *  0: C
          *  1: C#
          * ...

        - Type: :ref:`int`. Determines the chord type as followed. See
          :class:`guitarpro.models.ChordType` for mapping.

        - Chord extension: :ref:`int`. See
          :class:`guitarpro.models.ChordExtension` for mapping.

        - Bass note: :ref:`int`. Lowest note of chord as in *C/Am*.

        - Tonality: :ref:`int`. See
          :class:`guitarpro.models.ChordAlteration` for mapping.

        - Add: :ref:`bool`. Determines if an "add" (added note) is
          present in the chord.

        - Name: :ref:`byte-size-string`. Max length is 22.

        - Fifth alteration: :ref:`int`. Maps to
          :class:`guitarpro.models.ChordAlteration`.

        - Ninth alteration: :ref:`int`. Maps to
          :class:`guitarpro.models.ChordAlteration`.

        - Eleventh alteration: :ref:`int`. Maps to
          :class:`guitarpro.models.ChordAlteration`.

        - List of frets: 6 :ref:`Ints <int>`. Fret values are saved as
          in default format.

        - Count of barres: :ref:`int`. Maximum count is 2.

        - Barre frets: 2 :ref:`Ints <int>`.

        - Barre start strings: 2 :ref:`Ints <int>`.

        - Barre end string: 2 :ref:`Ints <int>`.

        - Omissions: 7 :ref:`Bools <bool>`. If the value is true then
          note is played in chord.

        - Blank space, 1 :ref:`byte`.*/

        chord.sharp = GPBase.readBool()[0];
        var intonation = chord.sharp ? "sharp" : "flat";
        GPBase.skip(3);
        chord.root = new PitchClass(GPBase.readByte()[0], -1, "", intonation);
        chord.type = (ChordType)GPBase.readByte()[0];
        chord.extension = (ChordExtension)GPBase.readByte()[0];
        chord.bass = new PitchClass(GPBase.readInt()[0], -1, "", intonation);
        chord.tonality = (ChordAlteration)GPBase.readInt()[0];
        chord.add = GPBase.readBool()[0];
        chord.name = GPBase.readByteSizeString(22);
        chord.fifth = (ChordAlteration)GPBase.readByte()[0];
        chord.ninth = (ChordAlteration)GPBase.readByte()[0];
        chord.eleventh = (ChordAlteration)GPBase.readByte()[0];
        chord.firstFret = GPBase.readInt()[0];
        for (var i = 0; i < 7; i++)
        {
            var fret = GPBase.readInt()[0];
            if (i < chord.strings.Length) chord.strings[i] = fret;
        }

        chord.barres.Clear();
        var barresCount = GPBase.readByte()[0];
        var barreFrets = GPBase.readByte(5);
        var barreStarts = GPBase.readByte(5);
        var barreEnds = GPBase.readByte(5);

        for (var x = 0; x < Math.Min(5, (int)barresCount); x++)
        {
            var barre = new Barre(barreFrets[x], barreStarts[x], barreEnds[x]);
            chord.barres.Add(barre);
        }

        chord.omissions = GPBase.readBool(7);
        GPBase.skip(1);
        var f = new List<Fingering>();
        for (var x = 0; x < 7; x++) f.Add((Fingering)GPBase.readSignedByte()[0]);
        chord.fingerings = f;
        chord.show = GPBase.readBool()[0];
    }


    private Duration readDuration(byte flags)
    {
        /*Duration is composed of byte signifying duration and an integer
        that maps to :class:`guitarpro.models.Tuplet`.

        The byte maps to following values:

        - *-2*: whole note
        - *-1*: half note
        -  *0*: quarter note
        -  *1*: eighth note
        -  *2*: sixteenth note
        -  *3*: thirty-second note

        If flag at *0x20* is true, the tuplet is read.*/

        var duration = new Duration
        {
            value = 1 << (GPBase.readSignedByte()[0] + 2),
            isDotted = (flags & 0x01) != 0
        };
        if ((flags & 0x20) == 0) return duration;

        var iTuplet = GPBase.readInt()[0];
        switch (iTuplet)
        {
            case 3:
                duration.tuplet.enters = 3;
                duration.tuplet.times = 2;
                break;
            case 5:
                duration.tuplet.enters = 5;
                duration.tuplet.times = 4;
                break;
            case 6:
                duration.tuplet.enters = 6;
                duration.tuplet.times = 4;
                break;
            case 7:
                duration.tuplet.enters = 7;
                duration.tuplet.times = 4;
                break;
            case 9:
                duration.tuplet.enters = 9;
                duration.tuplet.times = 8;
                break;
            case 10:
                duration.tuplet.enters = 10;
                duration.tuplet.times = 8;
                break;
            case 11:
                duration.tuplet.enters = 11;
                duration.tuplet.times = 8;
                break;
            case 12:
                duration.tuplet.enters = 12;
                duration.tuplet.times = 8;
                break;
        }

        return duration;
    }

    private Beat getBeat(Voice voice, int start)
    {
        for (var x = voice.beats.Count - 1; x >= 0; x--)
            if (voice.beats[x].start == start)
                return voice.beats[x];

        var newBeat = new Beat(voice)
        {
            start = start
        };
        voice.beats.Add(newBeat);
        return newBeat;
    }

    private void readTracks(int trackCount, IReadOnlyList<MidiChannel> channels)
    {
        for (var i = 0; i < trackCount; i++)
        {
            var track = new Track(this, i + 1, new List<GuitarString>(), new List<Measure>());
            readTrack(track, channels);
            tracks.Add(track);
        }
    }


    private void readTrack(Track track, IReadOnlyList<MidiChannel> channels)
    {
        /*
         * Read track.

        The first byte is the track's flags. It presides the track's
        attributes:

        - *0x01*: drums track
        - *0x02*: 12 stringed guitar track
        - *0x04*: banjo track
        - *0x08*: *blank*
        - *0x10*: *blank*
        - *0x20*: *blank*
        - *0x40*: *blank*
        - *0x80*: *blank*

        Flags are followed by:

        - Name: :ref:`byte-size-string`. A 40 characters long string
          containing the track's name.

        - Number of strings: :ref:`int`. An integer equal to the number
            of strings of the track.

        - Tuning of the strings: List of 7 :ref:`Ints <int>`. The tuning
          of the strings is stored as a 7-integers table, the "Number of
          strings" first integers being really used. The strings are
          stored from the highest to the lowest.

        - Port: :ref:`int`. The number of the MIDI port used.

        - Channel. See :meth:`GP3File.readChannel`.

        - Number of frets: :ref:`int`. The number of frets of the
          instrument.

        - Height of the capo: :ref:`int`. The number of the fret on
          which a capo is set. If no capo is used, the value is 0.

        - Track's color. The track's displayed color in Guitar Pro.*/
        var flags = GPBase.readByte()[0];
        track.isPercussionTrack = (flags & 0x01) != 0;
        track.is12StringedGuitarTrack = (flags & 0x02) != 0;
        track.isBanjoTrack = (flags & 0x04) != 0;
        track.name = GPBase.readByteSizeString(40);
        var stringCount = GPBase.readInt()[0];

        for (var i = 0; i < 7; i++)
        {
            var iTuning = GPBase.readInt()[0];
            if (stringCount <= i) continue;

            var oString = new GuitarString(i + 1, iTuning);
            track.strings.Add(oString);
        }

        track.port = GPBase.readInt()[0];
        track.channel = readChannel(channels);
        if (track.channel.channel == 9) track.isPercussionTrack = true;

        track.fretCount = GPBase.readInt()[0];
        track.offset = GPBase.readInt()[0];
        track.color = readColor();
    }

    private MidiChannel readChannel(IReadOnlyList<MidiChannel> channels)
    {
        /*Read MIDI channel.
       
               MIDI channel in Guitar Pro is represented by two integers. First
               is zero-based number of channel, second is zero-based number of
               channel used for effects.*/
        var index = GPBase.readInt()[0] - 1;
        var trackChannel = new MidiChannel();
        var effectChannel = GPBase.readInt()[0] - 1;
        if (0 > index || index >= channels.Count) return trackChannel;

        trackChannel = channels[index];
        if (trackChannel.instrument < 0) trackChannel.instrument = 0;

        if (!trackChannel.isPercussionChannel()) trackChannel.effectChannel = effectChannel;

        return trackChannel;
    }

    private void readMeasureHeaders(int measureCount)
    {
        /*Read measure headers.

        The *measures* are written one after another, their number have
        been specified previously.

        :param measureCount: number of measures to expect.*/
        MeasureHeader previous = null;
        for (var number = 1; number < measureCount + 1; number++)
        {
            var header = readMeasureHeader(number, previous);
            addMeasureHeader(header);
            previous = header;
        }
    }

    private MeasureHeader readMeasureHeader(int number, MeasureHeader previous = null)
    {
        /*Read measure header.

        The first byte is the measure's flags. It lists the data given in the
        current measure.

        - *0x01*: numerator of the key signature
        - *0x02*: denominator of the key signature
        - *0x04*: beginning of repeat
        - *0x08*: end of repeat
        - *0x10*: number of alternate ending
        - *0x20*: presence of a marker
        - *0x40*: tonality of the measure
        - *0x80*: presence of a double bar

        Each of these elements is present only if the corresponding bit
        is a 1.

        The different elements are written (if they are present) from
        lowest to highest bit.

        Exceptions are made for the double bar and the beginning of
        repeat whose sole presence is enough, complementary data is not
        necessary.

        - Numerator of the key signature: :ref:`byte`.

        - Denominator of the key signature: :ref:`byte`.

        - End of repeat: :ref:`byte`.
          Number of repeats until the previous beginning of repeat.

        - Number of alternate ending: :ref:`byte`.

        - Marker: see :meth:`GP3File.readMarker`.

        - Tonality of the measure: 2 :ref:`Bytes <byte>`. These values
          encode a key signature change on the current piece. First byte
          is key signature root, second is key signature type.
          */

        var flags = GPBase.readByte()[0];
        var header = new MeasureHeader
        {
            number = number,
            start = 0,
            tempo =
            {
                value = tempo
            },
            tripletFeel = _tripletFeel,
            timeSignature =
            {
                numerator = (flags & 0x01) != 0 ? GPBase.readSignedByte()[0] : previous.timeSignature.numerator,
                denominator =
                {
                    value = (flags & 0x02) != 0
                        ? GPBase.readSignedByte()[0]
                        : previous.timeSignature.denominator.value
                }
            },
            isRepeatOpen = (flags & 0x04) != 0
        };
        if ((flags & 0x08) != 0) header.repeatClose = GPBase.readSignedByte()[0];

        if ((flags & 0x10) != 0) header.repeatAlternatives.Add(readRepeatAlternative(measureHeaders));
        if ((flags & 0x20) != 0) header.marker = readMarker(header);

        if ((flags & 0x40) != 0)
        {
            var root = GPBase.readSignedByte()[0];
            var type_ = GPBase.readSignedByte()[0];
            var dir = root < 0 ? -1 : 1;
            header.keySignature = (KeySignature)(root * 10 + dir * type_);
        }
        else if (header.number > 1)
        {
            header.keySignature = previous.keySignature;
        }

        header.hasDoubleBar = (flags & 0x80) != 0;

        return header;
    }

    private int readRepeatAlternative(IReadOnlyList<MeasureHeader> measureHeaders)
    {
        var value = GPBase.readByte()[0];
        var existingAlternatives = 0;
        for (var x = measureHeaders.Count - 1; x >= 0; x--)
        {
            if (measureHeaders[x].isRepeatOpen) break;

            if (measureHeaders[x].repeatAlternatives.Count > 0)
                existingAlternatives |= measureHeaders[x].repeatAlternatives[0];
        }

        return ((1 << value) - 1) ^ existingAlternatives;
    }

    private Marker readMarker(MeasureHeader header)
    {
        var marker = new Marker
        {
            title = GPBase.readIntByteSizeString(),
            color = readColor(),
            measureHeader = header
        };

        return marker;
    }

    private myColor readColor()
    {
        var r = GPBase.readByte()[0];
        var g = GPBase.readByte()[0];
        var b = GPBase.readByte()[0];
        GPBase.skip(1);
        return new myColor(r, g, b);
    }

    private void readMidiChannels()
    {
        /*Read MIDI channels.

        Guitar Pro format provides 64 channels(4 MIDI ports by 16
        channels), the channels are stored in this order:

        -port1 / channel1
        - port1 / channel2
        - ...
        - port1 / channel16
        - port2 / channel1
        - ...
        - port4 / channel16

        Each channel has the following form:
        -Instrument: :ref:`int`.
        -Volume: :ref:`byte`.
        -Balance: :ref:`byte`.
        -Chorus: :ref:`byte`.
        -Reverb: :ref:`byte`.
        -Phaser: :ref:`byte`.
        -Tremolo: :ref:`byte`.
        -blank1: :ref:`byte`.
        -blank2: :ref:`byte`.*/
        var _channels = new MidiChannel[64];
        for (var i = 0; i < 64; i++)
        {
            var newChannel = new MidiChannel
            {
                channel = i,
                effectChannel = i
            };
            var instrument = GPBase.readInt()[0];
            if (newChannel.isPercussionChannel() && instrument == -1) instrument = 0;

            newChannel.instrument = instrument;
            newChannel.volume = toChannelShort(GPBase.readByte()[0]);
            newChannel.balance = toChannelShort(GPBase.readByte()[0]);
            newChannel.chorus = toChannelShort(GPBase.readByte()[0]);
            newChannel.reverb = toChannelShort(GPBase.readByte()[0]);
            newChannel.phaser = toChannelShort(GPBase.readByte()[0]);
            newChannel.tremolo = toChannelShort(GPBase.readByte()[0]);
            _channels[i] = newChannel;
            GPBase.skip(2);
        }

        channels = _channels;
    }

    private int toChannelShort(byte data)
    {
        var _data = (sbyte)data;
        //transform signed byte to short
        var value = Math.Max(-32768, Math.Min(32767, (_data << 3) - 1));
        return Math.Max(value, -1) + 1;
    }

    private void readLyrics()
    {
        /*Read lyrics.
        First, read an :ref:`int` that points to the track lyrics are
        bound to. Then it is followed by 5 lyric lines. Each one
        constists of number of starting measure encoded in :ref:`int`
        and :ref:`int-size-string` holding text of the lyric line.*/

        lyrics = new List<Lyrics>();
        var _lyrics = new Lyrics
        {
            trackChoice = GPBase.readInt()[0]
        };
        foreach (var t in _lyrics.lines)
        {
            t.startingMeasure = GPBase.readInt()[0];
            t.lyrics = GPBase.readIntSizeString();
        }

        lyrics.Add(_lyrics);
    }


    private void readInfo()
    {
        /*Read score information.

        Score information consists of sequence of
        :ref:`IntByteSizeStrings <int-byte-size-string>`:

        - title
        - subtitle
        - artist
        - album
        - words
        - copyright
        - tabbed by
        - instructions

        The sequence if followed by notice. Notice starts with the
        number of notice lines stored in :ref:`int`. Each line is
        encoded in :ref:`int-byte-size-string`.*/

        title = GPBase.readIntByteSizeString();
        subtitle = GPBase.readIntByteSizeString();
        interpret = GPBase.readIntByteSizeString();
        album = GPBase.readIntByteSizeString();
        author = GPBase.readIntByteSizeString();
        copyright = GPBase.readIntByteSizeString();
        tab_author = GPBase.readIntByteSizeString();
        instructional = GPBase.readIntByteSizeString();
        var notesCount = GPBase.readInt()[0];
        notice = new string[notesCount];
        for (var x = 0; x < notesCount; x++) notice[x] = GPBase.readIntByteSizeString();
    }

    public sealed class Clipboard
    {
        public int startBeat = 1;
        public int startMeasure = 1;
        public int startTrack = 1;
        public int stopBeat = 1;
        public int stopMeasure = 1;
        public int stopTrack = 1;
        public bool subBarCopy = false;
    }
}