#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro.Native;

public sealed class NativeFormat
{
    public static bool[] availableChannels = new bool[16];


    private readonly List<int> notesInMeasures = new();
    public string album = "";
    public List<Annotation> annotations = new();
    public string artist = "";
    public List<MasterBar> barMaster = new();

    public List<DirectionSign> directions = new();
    public List<Lyrics> lyrics = new();
    public string music = "";
    public string subtitle = "";
    public List<Tempo> tempos = new();

    public string title = "";
    public List<Track> tracks = new();
    public string words = "";


    public NativeFormat(GPFile fromFile)
    {
        title = fromFile.title;
        subtitle = fromFile.subtitle;
        artist = fromFile.interpret;
        album = fromFile.album;
        words = fromFile.words;
        music = fromFile.music;
        tempos = retrieveTempos(fromFile);
        directions = fromFile.directions;
        barMaster = retrieveMasterBars(fromFile);
        tracks = retrieveTracks(fromFile);
        lyrics = fromFile.lyrics;
        updateAvailableChannels();
    }

    public MidiExport toMidi()
    {
        var mid = new MidiExport();
        mid.midiTracks.Add(getMidiHeader()); //First, untitled track
        foreach (var t in tracks.Select(static track => track.getMidi()).Where(static t => t != null))
            mid.midiTracks.Add(t);

        return mid;
    }

    private MidiTrack getMidiHeader()
    {
        var midiHeader = new MidiTrack();
        //text(s) - name of song, artist etc., created by Gitaro
        //copyright - by Gitaro
        //midi port 0
        //time signature
        //key signature
        //set tempo
        ///////marker text (will be seen in file) - also Gitaro copyright blabla
        //end_of_track
        midiHeader.messages.Add(new MidiMessage("track_name", new[] { "untitled" }, 0));
        midiHeader.messages.Add(new MidiMessage("text", new[] { title }, 0));
        midiHeader.messages.Add(new MidiMessage("text", new[] { subtitle }, 0));
        midiHeader.messages.Add(new MidiMessage("text", new[] { artist }, 0));
        midiHeader.messages.Add(new MidiMessage("text", new[] { album }, 0));
        midiHeader.messages.Add(new MidiMessage("text", new[] { words }, 0));
        midiHeader.messages.Add(new MidiMessage("text", new[] { music }, 0));
        midiHeader.messages.Add(new MidiMessage("copyright", new[] { "Copyright 2017 by Gitaro" }, 0));
        midiHeader.messages.Add(new MidiMessage("marker",
            new[] { title + " / " + artist + " - Copyright 2017 by Gitaro" }, 0));
        midiHeader.messages.Add(new MidiMessage("midi_port", new[] { "0" }, 0));

        //Get tempos from List tempos, get key_signature and time_signature from barMaster
        var tempoIndex = 0;
        var masterBarIndex = 0;
        var currentIndex = 0;
        var oldTimeSignature = "";
        var oldKeySignature = "";
        if (tempos.Count == 0) tempos.Add(new Tempo());
        while (tempoIndex < tempos.Count || masterBarIndex < barMaster.Count)
            //Compare next entry of both possible sources
            if (tempoIndex == tempos.Count ||
                tempos[tempoIndex].position >= barMaster[masterBarIndex].index) //next measure comes first
            {
                if (!barMaster[masterBarIndex].keyBoth.Equals(oldKeySignature))
                {
                    //Add Key-Sig to midiHeader
                    midiHeader.messages.Add(new MidiMessage("key_signature",
                        new[] { "" + barMaster[masterBarIndex].key, "" + barMaster[masterBarIndex].keyType },
                        barMaster[masterBarIndex].index - currentIndex));
                    currentIndex = barMaster[masterBarIndex].index;

                    oldKeySignature = barMaster[masterBarIndex].keyBoth;
                }

                if (!barMaster[masterBarIndex].time.Equals(oldTimeSignature))
                {
                    //Add Time-Sig to midiHeader
                    midiHeader.messages.Add(new MidiMessage("time_signature",
                        new[] { "" + barMaster[masterBarIndex].num, "" + barMaster[masterBarIndex].den, "24", "8" },
                        barMaster[masterBarIndex].index - currentIndex));
                    currentIndex = barMaster[masterBarIndex].index;

                    oldTimeSignature = barMaster[masterBarIndex].time;
                }

                masterBarIndex++;
            }
            else //next tempo signature comes first
            {
                //Add Tempo-Sig to midiHeader
                var _tempo = (int)Math.Round(60 * 1000000 / tempos[tempoIndex].value);
                midiHeader.messages.Add(new MidiMessage("set_tempo", new[] { "" + _tempo },
                    tempos[tempoIndex].position - currentIndex));
                currentIndex = tempos[tempoIndex].position;
                tempoIndex++;
            }


        midiHeader.messages.Add(new MidiMessage("end_of_track", new string[] { }, 0));


        return midiHeader;
    }

    private void updateAvailableChannels()
    {
        for (var x = 0; x < 16; x++)
            if (x != 9)
                availableChannels[x] = true;
            else
                availableChannels[x] = false;

        foreach (var track in tracks) availableChannels[track.channel] = false;
    }

    public List<Track> retrieveTracks(GPFile file)
    {
        var tracks = new List<Track>();
        foreach (var tr in file.tracks)
        {
            var track = new Track
            {
                name = tr.name,
                patch = tr.channel.instrument,
                port = tr.port,
                channel = tr.channel.channel,
                playbackState = PlaybackState.def,
                capo = tr.offset
            };
            if (tr.isMute) track.playbackState = PlaybackState.mute;

            if (tr.isSolo) track.playbackState = PlaybackState.solo;

            track.tuning = getTuning(tr.strings);

            track.notes = retrieveNotes(tr, track.tuning, track);
            tracks.Add(track);
        }

        return tracks;
    }

    public static void addToTremoloBarList(int index, int duration, BendEffect bend, Track myTrack)
    {
        myTrack.tremoloPoints.Add(new TremoloPoint(0.0f,
            index)); //So that it can later be recognized as the beginning
        foreach (var point in from bp in bend.points
                 let at = index + (int)(bp.GP6position * duration / 100.0f)
                 select new TremoloPoint
                 {
                     index = at,
                     value = bp.GP6value
                 })
            myTrack.tremoloPoints.Add(point);

        var tp = new TremoloPoint
        {
            index = index + duration,
            value = 0
        };
        myTrack.tremoloPoints
            .Add(tp); //Back to 0 -> Worst case there will be on the same index the final of tone 1, 0, and the beginning of tone 2.
    }

    public static List<BendPoint> getBendPoints(int index, int duration, BendEffect bend)
    {
        return (from bp in bend.points
            let at = index + (int)(bp.GP6position * duration / 100.0f)
            select new BendPoint { index = at, value = bp.GP6value }).ToList();
    }


    public List<Note> retrieveNotes(GuitarPro.Track track, int[] tuning, Track myTrack)
    {
        var notes = new List<Note>();
        var index = 0;
        var last_notes = new Note[10];
        var last_was_tie = new bool[10];
        for (var x = 0; x < 10; x++) last_was_tie[x] = false;

        //GraceNotes if on beat - reducing the next note's length
        var rememberGrace = false;
        var rememberedGrace = false;
        var graceLength = 0;
        var subtractSubindex = 0;

        for (var x = 0; x < 10; x++) last_notes[x] = null;

        var measureIndex = -1;
        foreach (var m in track.measures)
        {
            var notesInMeasure = 0;
            measureIndex++;
            var skipVoice = false;
            switch (m.simileMark)
            {
                //Repeat last measure
                case SimileMark.simple:
                {
                    var amountNotes = notesInMeasures[notesInMeasures.Count - 1]; //misuse prohibited by guitarpro
                    var endPoint = notes.Count;
                    for (var x = endPoint - amountNotes; x < endPoint; x++)
                    {
                        var newNote = new Note(notes[x]);
                        var oldM = track.measures[measureIndex - 1];
                        newNote.index += flipDuration(oldM.header.timeSignature.denominator) *
                                         oldM.header.timeSignature.numerator;
                        notes.Add(newNote);
                        notesInMeasure++;
                    }

                    skipVoice = true;
                    break;
                }
                case SimileMark.firstOfDouble:
                //Repeat first or second of last two measures
                case SimileMark.secondOfDouble:
                {
                    var secondAmount = notesInMeasures[notesInMeasures.Count - 1]; //misuse prohibited by guitarpro
                    var firstAmount = notesInMeasures[notesInMeasures.Count - 2];
                    var endPoint = notes.Count - secondAmount;
                    for (var x = endPoint - firstAmount; x < endPoint; x++)
                    {
                        var newNote = new Note(notes[x]);
                        var oldM1 = track.measures[measureIndex - 2];
                        var oldM2 = track.measures[measureIndex - 1];
                        newNote.index += flipDuration(oldM1.header.timeSignature.denominator) *
                                         oldM1.header.timeSignature.numerator;
                        newNote.index += flipDuration(oldM2.header.timeSignature.denominator) *
                                         oldM2.header.timeSignature.numerator;
                        notes.Add(newNote);
                        notesInMeasure++;
                    }

                    skipVoice = true;
                    break;
                }
            }

            foreach (var v in m.voices)
            {
                if (skipVoice) break;

                var subIndex = 0;
                foreach (var b in v.beats)
                {
                    if (b.text != null && !b.text.value.Equals(""))
                        annotations.Add(new Annotation(b.text.value, index + subIndex));

                    if (b.effect.tremoloBar != null)
                        addToTremoloBarList(index + subIndex, flipDuration(b.duration), b.effect.tremoloBar,
                            myTrack);


                    //Prepare Brush or Arpeggio
                    var hasBrush = false;
                    var brushInit = 0;
                    var brushIncrease = 0;
                    var brushDirection = BeatStrokeDirection.none;

                    if (b.effect.stroke != null)
                    {
                        var notesCnt = b.notes.Count;
                        brushDirection = b.effect.stroke.direction;
                        if (brushDirection != BeatStrokeDirection.none && notesCnt > 1)
                        {
                            hasBrush = true;
                            var temp = new Duration
                            {
                                value = b.effect.stroke.value
                            };
                            var brushTotalDuration = flipDuration(temp);
                            var beatTotalDuration = flipDuration(b.duration);


                            brushIncrease = brushTotalDuration / notesCnt;
                            var startPos = index + subIndex + (int)((brushTotalDuration - brushIncrease) *
                                                                    (b.effect.stroke.startTime - 1));
                            var endPos = startPos + brushTotalDuration - brushIncrease;

                            if (brushDirection == BeatStrokeDirection.down)
                            {
                                brushInit = startPos;
                            }
                            else
                            {
                                brushInit = endPos;
                                brushIncrease = -brushIncrease;
                            }
                        }
                    }

                    foreach (var n in b.notes)
                    {
                        var note = new Note
                        {
                            //Beat values
                            isTremBarVibrato = b.effect.vibrato,
                            fading = Fading.none
                        };
                        if (b.effect.fadeIn) note.fading = Fading.fadeIn;

                        if (b.effect.fadeOut) note.fading = Fading.fadeOut;

                        if (b.effect.volumeSwell) note.fading = Fading.volumeSwell;

                        note.isSlapped = b.effect.slapEffect == SlapEffect.slapping;
                        note.isPopped = b.effect.slapEffect == SlapEffect.popping;
                        note.isHammer = n.effect.hammer;
                        note.isRHTapped = b.effect.slapEffect == SlapEffect.tapping;
                        note.index = index + subIndex;
                        note.duration = flipDuration(b.duration);


                        //Note values
                        note.fret = n.value;
                        note.str = n.str;
                        note.velocity = n.velocity;
                        note.isVibrato = n.effect.vibrato;
                        note.isPalmMuted = n.effect.palmMute;
                        note.isMuted = n.type == NoteType.dead;

                        if (n.effect.harmonic != null)
                        {
                            note.harmonicFret = n.effect.harmonic.fret;
                            if (n.effect.harmonic.fret == 0) //older format..
                                if (n.effect.harmonic.type == 2)
                                    note.harmonicFret =
                                        ((ArtificialHarmonic)n.effect.harmonic).pitch.actualOvertone;

                            note.harmonic = n.effect.harmonic.type switch
                            {
                                1 => HarmonicType.natural,
                                2 => HarmonicType.artificial,
                                3 => HarmonicType.pinch,
                                4 => HarmonicType.tapped,
                                5 => HarmonicType.semi,
                                _ => HarmonicType.natural
                            };
                        }

                        if (n.effect.slides != null)
                            foreach (var sl in n.effect.slides)
                            {
                                note.slidesToNext = note.slidesToNext ||
                                                    sl is SlideType.shiftSlideTo or SlideType.legatoSlideTo;
                                note.slideInFromAbove = note.slideInFromAbove || sl == SlideType.intoFromAbove;
                                note.slideInFromBelow = note.slideInFromBelow || sl == SlideType.intoFromBelow;
                                note.slideOutDownwards = note.slideOutDownwards || sl == SlideType.outDownwards;
                                note.slideOutUpwards = note.slideOutUpwards || sl == SlideType.outUpwards;
                            }

                        if (n.effect.bend != null)
                            note.bendPoints = getBendPoints(index + subIndex, flipDuration(b.duration),
                                n.effect.bend);

                        //Ties

                        var dontAddNote = false;

                        if (n.type == NoteType.tie)
                        {
                            dontAddNote = true;
                            //Find if note can simply be added to previous note

                            var last = last_notes[Math.Max(0, note.str - 1)];


                            if (last != null)
                            {
                                note.fret = last.fret; //For GP3 & GP4
                                if (last.harmonic != note.harmonic || last.harmonicFret != note.harmonicFret
                                   )
                                    dontAddNote = false;

                                if (dontAddNote)
                                {
                                    note.connect = true;
                                    last.duration += note.duration;
                                    last.addBendPoints(note.bendPoints);
                                }
                            }
                        }
                        else // not a tie
                        {
                            last_was_tie[Math.Max(0, note.str - 1)] = false;
                        }

                        //Extra notes to replicate certain effects


                        //Triplet Feel
                        if (!barMaster[measureIndex].tripletFeel.Equals("none"))
                        {
                            var trip = barMaster[measureIndex].tripletFeel;
                            //Check if at regular 8th or 16th beat position
                            var is_8th_pos = subIndex % 480 == 0;
                            var is_16th_pos = subIndex % 240 == 0;
                            var is_first = true; //first of note pair
                            if (is_8th_pos) is_first = subIndex % 960 == 0;

                            if (is_16th_pos) is_first = is_8th_pos;

                            var is_8th = b.duration.value == 8 && !b.duration.isDotted &&
                                         !b.duration.isDoubleDotted && b.duration.tuplet.enters == 1 &&
                                         b.duration.tuplet.times == 1;
                            var is_16th = b.duration.value == 16 && !b.duration.isDotted &&
                                          !b.duration.isDoubleDotted && b.duration.tuplet.enters == 1 &&
                                          b.duration.tuplet.times == 1;

                            switch (trip)
                            {
                                case TripletFeel.eigth when is_8th_pos && is_8th:
                                case TripletFeel.sixteenth when is_16th_pos && is_16th:
                                {
                                    switch (is_first)
                                    {
                                        case true:
                                            note.duration = (int)(note.duration * (4.0f / 3.0f));
                                            break;
                                        case false:
                                            note.duration = (int)(note.duration * (2.0f / 3.0f));
                                            note.resizeValue *= 2.0f / 3.0f;
                                            note.index += (int)(note.duration * (1.0f / 3.0f));
                                            break;
                                    }

                                    break;
                                }
                                case TripletFeel.dotted8th when is_8th_pos && is_8th:
                                case TripletFeel.dotted16th when is_16th_pos && is_16th:
                                {
                                    switch (is_first)
                                    {
                                        case true:
                                            note.duration = (int)(note.duration * 1.5f);
                                            break;
                                        case false:
                                            note.duration = (int)(note.duration * 0.5f);
                                            note.resizeValue *= 0.5f;
                                            note.index += (int)(note.duration * 0.5f);
                                            break;
                                    }

                                    break;
                                }
                                case TripletFeel.scottish8th when is_8th_pos && is_8th:
                                case TripletFeel.scottish16th when is_16th_pos && is_16th:
                                {
                                    switch (is_first)
                                    {
                                        case true:
                                            note.duration = (int)(note.duration * 0.5f);
                                            break;
                                        case false:
                                            note.duration = (int)(note.duration * 1.5f);
                                            note.resizeValue *= 1.5f;
                                            note.index -= (int)(note.duration * 0.5f);
                                            break;
                                    }

                                    break;
                                }
                            }
                        }


                        //Tremolo Picking & Trill
                        if (n.effect.tremoloPicking != null || n.effect.trill != null)
                        {
                            var len = note.duration;
                            if (n.effect.tremoloPicking != null)
                                len = flipDuration(n.effect.tremoloPicking.duration);

                            if (n.effect.trill != null) len = flipDuration(n.effect.trill.duration);

                            var origDuration = note.duration;
                            note.duration = len;
                            note.resizeValue *= (float)len / origDuration;
                            var currentIndex = note.index + len;

                            last_notes[Math.Max(0, note.str - 1)] = note;
                            notes.Add(note);
                            notesInMeasure++;

                            dontAddNote = true; //Because we're doing it here already
                            var originalFret = false;
                            var secondFret = note.fret;

                            if (n.effect.trill != null) secondFret = n.effect.trill.fret - tuning[note.str - 1];

                            while (currentIndex + len <= note.index + origDuration)
                            {
                                var newOne = new Note(note)
                                {
                                    index = currentIndex
                                };
                                if (!originalFret) newOne.fret = secondFret; //For trills

                                last_notes[Math.Max(0, note.str - 1)] = newOne;
                                if (n.effect.trill != null) newOne.isHammer = true;

                                notes.Add(newOne);
                                notesInMeasure++;
                                currentIndex += len;
                                originalFret = !originalFret;
                            }
                        }


                        //Grace Note
                        if (rememberGrace && note.duration > graceLength)
                        {
                            var orig = note.duration;
                            note.duration -= graceLength;
                            note.resizeValue *= (float)note.duration / orig;
                            //subIndex -= graceLength;
                            rememberedGrace = true;
                        }

                        if (n.effect.grace != null)
                        {
                            var isOnBeat = n.effect.grace.isOnBeat;

                            if (n.effect.grace.duration != -1)
                            {
                                //GP3,4,5 format

                                var graceNote = new Note
                                {
                                    index = note.index,
                                    fret = n.effect.grace.fret,
                                    str = note.str
                                };
                                var dur = new Duration
                                {
                                    value = n.effect.grace.duration
                                };
                                graceNote.duration = flipDuration(dur); //works at least for GP5
                                if (isOnBeat)
                                {
                                    var orig = note.duration;
                                    note.duration -= graceNote.duration;
                                    note.index += graceNote.duration;
                                    note.resizeValue *= (float)note.duration / orig;
                                }
                                else
                                {
                                    graceNote.index -= graceNote.duration;
                                }

                                notes.Add(graceNote); //TODO: insert at correct position!
                                notesInMeasure++;
                            }
                            else
                            {
                                if (isOnBeat) // shorten next note
                                {
                                    rememberGrace = true;
                                    graceLength = note.duration;
                                }
                                else //Change previous note
                                {
                                    if (notes.Count > 0)
                                    {
                                        note.index -=
                                            note.duration; //Can lead to negative indices. Midi should handle that
                                        subtractSubindex = note.duration;
                                    }
                                }
                            }
                        }


                        //Dead Notes
                        if (n.type == NoteType.dead)
                        {
                            var orig = note.duration;
                            note.velocity = (int)(note.velocity * 0.9f);
                            note.duration /= 6;
                            note.resizeValue *= (float)note.duration / orig;
                        }

                        //Ghost Notes
                        if (n.effect.palmMute)
                        {
                            var orig = note.duration;
                            note.velocity = (int)(note.velocity * 0.7f);
                            note.duration /= 2;
                            note.resizeValue *= (float)note.duration / orig;
                        }

                        if (n.effect.ghostNote) note.velocity = (int)(note.velocity * 0.8f);


                        //Staccato, Accented, Heavy Accented
                        if (n.effect.staccato)
                        {
                            var orig = note.duration;
                            note.duration /= 2;
                            note.resizeValue *= (float)note.duration / orig;
                        }

                        if (n.effect.accentuatedNote) note.velocity = (int)(note.velocity * 1.2f);

                        if (n.effect.heavyAccentuatedNote) note.velocity = (int)(note.velocity * 1.4f);

                        //Arpeggio / Brush
                        if (hasBrush)
                        {
                            note.index = brushInit;
                            brushInit += brushIncrease;
                        }

                        if (dontAddNote) continue;

                        last_notes[Math.Max(0, note.str - 1)] = note;
                        notes.Add(note);
                        notesInMeasure++;
                    }

                    if (rememberedGrace)
                    {
                        subIndex -= graceLength;
                        rememberGrace = false;
                        rememberedGrace = false;
                    } //After the change in duration for the second beat has been done

                    subIndex -= subtractSubindex;
                    subtractSubindex = 0;
                    subIndex += flipDuration(b.duration);

                    //Sort brushed tones
                    if (hasBrush && brushDirection == BeatStrokeDirection.up)
                    {
                        //Have to reorder them xxx123 -> xxx321
                        var notesCnt = b.notes.Count;
                        var temp = new Note[notesCnt];
                        for (var x = notes.Count - notesCnt; x < notes.Count; x++)
                            temp[x - (notes.Count - notesCnt)] = new Note(notes[x]);

                        for (var x = notes.Count - notesCnt; x < notes.Count; x++)
                            notes[x] = temp[temp.Length - (x - (notes.Count - notesCnt)) - 1];
                    }

                    hasBrush = false;
                }

                break; //Consider only the first voice
            }

            var measureDuration =
                flipDuration(m.header.timeSignature.denominator) * m.header.timeSignature.numerator;
            barMaster[measureIndex].duration = measureDuration;
            barMaster[measureIndex].index = index;
            index += measureDuration;
            notesInMeasures.Add(notesInMeasure);
        }


        return notes;
    }


    public static int[] getTuning(List<GuitarString> strings)
    {
        var tuning = new int[strings.Count];
        for (var x = 0; x < tuning.Length; x++) tuning[x] = strings[x].value;

        return tuning;
    }

    public static List<MasterBar> retrieveMasterBars(GPFile file)
    {
        var masterBars = new List<MasterBar>();
        foreach (var mh in file.measureHeaders)
        {
            //(mh.timeSignature.denominator) * mh.timeSignature.numerator;
            var mb = new MasterBar
            {
                time = mh.timeSignature.numerator + "/" + mh.timeSignature.denominator.value,
                num = mh.timeSignature.numerator,
                den = mh.timeSignature.denominator.value
            };
            var keyFull = "" + (int)mh.keySignature;
            if (keyFull.Length != 1)
            {
                mb.keyType = int.Parse(keyFull.Substring(keyFull.Length - 1));
                mb.key = int.Parse(keyFull.Substring(0, keyFull.Length - 1));
            }
            else
            {
                mb.key = 0;
                mb.keyType = int.Parse(keyFull);
            }

            mb.keyBoth = keyFull; //Useful for midiExport later

            mb.tripletFeel = mh.tripletFeel;

            masterBars.Add(mb);
        }

        return masterBars;
    }

    public static List<Tempo> retrieveTempos(GPFile file)
    {
        var tempos = new List<Tempo>();
        //Version < 4 -> look at Measure Headers, >= 4 look at mixtablechanges


        var version = file.versionTuple[0];
        if (version < 4) //Look at MeasureHeaders
        {
            //Get inital tempo from file header
            var init = new Tempo
            {
                position = 0,
                value = file.tempo
            };
            if (init.value != 0) tempos.Add(init);

            var pos = 0;
            float oldTempo = file.tempo;
            foreach (var mh in file.measureHeaders)
            {
                var t = new Tempo
                {
                    value = mh.tempo.value,
                    position = pos
                };
                pos += flipDuration(mh.timeSignature.denominator) * mh.timeSignature.numerator;
                if (oldTempo != t.value) tempos.Add(t);
                oldTempo = t.value;
            }
        }
        else //Look at MixtableChanges - only on track 1, voice 1
        {
            var pos = 0;

            //Get inital tempo from file header
            var init = new Tempo
            {
                position = 0,
                value = file.tempo
            };
            if (init.value != 0) tempos.Add(init);
            foreach (var m in file.tracks[0].measures)
            {
                var smallPos = 0; //inner measure position
                if (m.voices.Count == 0) continue;

                foreach (var b in m.voices[0].beats)
                {
                    var tempo = b.effect?.mixTableChange?.tempo;
                    if (tempo != null)
                    {
                        var t = new Tempo
                        {
                            value = tempo.value,
                            position = pos + smallPos
                        };

                        tempos.Add(t);
                    }

                    smallPos += flipDuration(b.duration);
                }

                pos += flipDuration(m.header.timeSignature.denominator) * m.header.timeSignature.numerator;
            }
        }

        return tempos;
    }

    private static int flipDuration(Duration d)
    {
        var ticks_per_beat = 960;
        var result = 0;
        switch (d.value)
        {
            case 1:
                result += ticks_per_beat * 4;
                break;
            case 2:
                result += ticks_per_beat * 2;
                break;
            case 4:
                result += ticks_per_beat;
                break;
            case 8:
                result += ticks_per_beat / 2;
                break;
            case 16:
                result += ticks_per_beat / 4;
                break;
            case 32:
                result += ticks_per_beat / 8;
                break;
            case 64:
                result += ticks_per_beat / 16;
                break;
            case 128:
                result += ticks_per_beat / 32;
                break;
        }

        if (d.isDotted) result = (int)(result * 1.5f);

        if (d.isDoubleDotted) result = (int)(result * 1.75f);

        var enters = d.tuplet.enters;
        var times = d.tuplet.times;

        //3:2 = standard triplet, 3 notes in the time of 2
        result = (int)(result * times / (float)enters);


        return result;
    }
}

public sealed class Note
{
    public List<BendPoint> bendPoints = new();
    public bool connect; //= tie
    public int duration;
    public Fading fading = Fading.none;
    public int fret;
    public HarmonicType harmonic = HarmonicType.none;
    public float harmonicFret;
    public int index;
    public bool isHammer;
    public bool isMuted;
    public bool isPalmMuted;
    public bool isPopped;
    public bool isRHTapped;
    public bool isSlapped;
    public bool isTremBarVibrato;
    public bool isVibrato;

    public float
        resizeValue =
            1.0f; //Should reflect any later changes made to the note duration, so that bendPoints can be adjusted

    public bool slideInFromAbove;
    public bool slideInFromBelow;
    public bool slideOutDownwards;
    public bool slideOutUpwards;
    public bool slidesToNext;

    //Values from Note
    public int str;

    //Values from Beat
    public List<BendPoint> tremBarPoints = new();
    public int velocity = 100;

    public Note(Note old)
    {
        str = old.str;
        fret = old.fret;
        velocity = old.velocity;
        isVibrato = old.isVibrato;
        isHammer = old.isHammer;
        isPalmMuted = old.isPalmMuted;
        isMuted = old.isMuted;
        harmonic = old.harmonic;
        harmonicFret = old.harmonicFret;
        slidesToNext = old.slidesToNext;
        slideInFromAbove = old.slideInFromAbove;
        slideInFromBelow = old.slideInFromBelow;
        slideOutDownwards = old.slideOutDownwards;
        slideOutUpwards = old.slideOutUpwards;
        bendPoints.AddRange(old.bendPoints);
        tremBarPoints.AddRange(old.tremBarPoints);
        isTremBarVibrato = old.isTremBarVibrato;
        isSlapped = old.isSlapped;
        isPopped = old.isPopped;
        index = old.index;
        duration = old.duration;
        fading = old.fading;
        isRHTapped = old.isRHTapped;
        resizeValue = old.resizeValue;
    }

    public Note()
    {
    }

    public void addBendPoints(IEnumerable<BendPoint> bendPoints)
    {
        //Hopefully no calculation involved
        this.bendPoints.AddRange(bendPoints);
    }
}

public enum Fading
{
    none = 0,
    fadeIn = 1,
    fadeOut = 2,
    volumeSwell = 3
}

public sealed class Annotation
{
    public int position;
    public string value = "";

    public Annotation(string v = "", int pos = 0)
    {
        value = v;
        position = pos;
    }
}

public sealed class TremoloPoint
{
    public int index;
    public float value; //0 nothing, 100 one whole tone up

    public TremoloPoint()
    {
    }

    public TremoloPoint(float value, int index)
    {
        this.value = value;
        this.index = index;
    }
}

public sealed class BendPoint
{
    public int index; //also global index of midi
    public int usedChannel; //After being part of BendingPlan
    public float value;

    public BendPoint(float value, int index)
    {
        this.value = value;
        this.index = index;
    }

    public BendPoint()
    {
    }
}

public enum HarmonicType
{
    none = 0,
    natural = 1,
    artificial = 2,
    pinch = 3,
    semi = 4,
    tapped = 5
}

public sealed class Track
{
    public int capo;
    public int channel;
    public string name = "";
    public List<Note> notes = new();
    public int patch;
    public PlaybackState playbackState = PlaybackState.def;
    public int port;
    public List<TremoloPoint> tremoloPoints = new();
    public int[] tuning = { 40, 45, 50, 55, 59, 64 };
    private List<int[]> volumeChanges = new();


    public MidiTrack getMidi()
    {
        //if there is nothing, return null
        if (notes.Count - 1 < 0)
            return null;

        var midiTrack = new MidiTrack();
        midiTrack.messages.Add(new MidiMessage("midi_port", new[] { "" + port }, 0));
        midiTrack.messages.Add(new MidiMessage("track_name", new[] { name }, 0));
        midiTrack.messages.Add(new MidiMessage("program_change", new[] { "" + channel, "" + patch }, 0));


        var noteOffs = new List<int[]>();
        var channelConnections =
            new List<int[]>(); //For bending and trembar: [original Channel, artificial Channel, index at when to delete artificial]
        var activeBendingPlans = new List<BendingPlan>();
        var currentIndex = 0;
        var _temp = new Note
        {
            index = notes[notes.Count - 1].index + notes[notes.Count - 1].duration,
            str = -2
        };
        notes.Add(_temp);

        tremoloPoints = addDetailsToTremoloPoints(tremoloPoints, 60);

        //var _notes = addSlidesToNotes(notes); //Adding slide notes here, as they should not appear as extra notes during playback

        foreach (var n in notes)
        {
            noteOffs.Sort(static (x, y) => x[0].CompareTo(y[0]));


            //Check for active bendings in progress
            var currentBPs = findAndSortCurrentBendPoints(activeBendingPlans, n.index);
            var _tremBarChange = 0.0f;
            foreach (var bp in currentBPs)
            {
                //Check first if there is a note_off event happening in the meantime..
                var newNoteOffs = new List<int[]>();
                foreach (var noteOff in noteOffs)
                    if (noteOff[0] <= bp.index) //between last and this note, a note off event should occur
                    {
                        midiTrack.messages.Add(
                            new MidiMessage("note_off",
                                new[] { "" + noteOff[2], "" + noteOff[1], "0" }, noteOff[0] - currentIndex));
                        currentIndex = noteOff[0];
                    }
                    else
                    {
                        newNoteOffs.Add(noteOff);
                    }

                noteOffs = newNoteOffs;

                //Check if there are active tremPoints to be adjusted for
                var _newTremPoints = new List<TremoloPoint>();

                foreach (var tp in tremoloPoints)
                    if (tp.index <= bp.index) //between last and this note, a note off event should occur
                        _tremBarChange = tp.value;
                    else
                        _newTremPoints.Add(tp);
                tremoloPoints = _newTremPoints;

                //Check if there are active volume changes
                var _newVolumeChanges = new List<int[]>();
                foreach (var vc in volumeChanges)
                    if (vc[0] <= bp.index) //between last and this note, a volume change event should occur
                    {
                        //channel control value
                        midiTrack.messages.Add(
                            new MidiMessage("control_change",
                                new[] { "" + bp.usedChannel, "7", "" + vc[1] }, vc[0] - currentIndex));
                        currentIndex = vc[0];
                    }
                    else
                    {
                        _newVolumeChanges.Add(vc);
                    }

                volumeChanges = _newVolumeChanges;

                midiTrack.messages.Add(
                    new MidiMessage("pitchwheel",
                        new[] { "" + bp.usedChannel, "" + (int)((bp.value + _tremBarChange) * 25.6f) },
                        bp.index - currentIndex));
                currentIndex = bp.index;
            }

            //Delete no longer active Bending Plans
            var final = new List<BendingPlan>();
            foreach (var bpl in activeBendingPlans)
            {
                var newBPL = new BendingPlan(bpl.originalChannel, bpl.usedChannel, new List<BendPoint>());
                foreach (var bp in bpl.bendingPoints.Where(bp => bp.index > n.index))
                    newBPL.bendingPoints.Add(bp);
                if (newBPL.bendingPoints.Count > 0)
                {
                    final.Add(newBPL);
                }
                else //That bending plan has finished
                {
                    midiTrack.messages.Add(new MidiMessage("pitchwheel", new[] { "" + bpl.usedChannel, "-128" },
                        0));
                    midiTrack.messages.Add(new MidiMessage("control_change",
                        new[] { "" + bpl.usedChannel, "101", "127" }, 0));
                    midiTrack.messages.Add(new MidiMessage("control_change",
                        new[] { "" + bpl.usedChannel, "10", "127" }, 0));

                    //Remove the channel from channelConnections
                    var newChannelConnections = channelConnections.Where(cc => cc[1] != bpl.usedChannel).ToList();
                    channelConnections = newChannelConnections;

                    NativeFormat.availableChannels[bpl.usedChannel] = true;
                }
            }

            activeBendingPlans = final;


            var activeChannels = getActiveChannels(channelConnections);
            var newTremPoints = new List<TremoloPoint>();
            foreach (var tp in tremoloPoints)
                if (tp.index <= n.index) //between last and this note, a trembar event should occur
                {
                    var value = tp.value * 25.6f;
                    value = Math.Min(Math.Max(value, -8192), 8191);
                    foreach (var ch in activeChannels)
                    {
                        midiTrack.messages.Add(
                            new MidiMessage("pitchwheel",
                                new[] { "" + ch, "" + (int)value }, tp.index - currentIndex));
                        currentIndex = tp.index;
                    }
                }
                else
                {
                    newTremPoints.Add(tp);
                }

            tremoloPoints = newTremPoints;


            //Check if there are active volume changes
            var newVolumeChanges = new List<int[]>();
            foreach (var vc in volumeChanges)
                if (vc[0] <= n.index) //between last and this note, a volume change event should occur
                    foreach (var ch in activeChannels)
                    {
                        midiTrack.messages.Add(
                            new MidiMessage("control_change",
                                new[] { "" + ch, "7", "" + vc[1] }, vc[0] - currentIndex));
                        currentIndex = vc[0];
                    }
                else
                    newVolumeChanges.Add(vc);

            volumeChanges = newVolumeChanges;


            var temp = new List<int[]>();
            foreach (var noteOff in noteOffs)
                if (noteOff[0] <= n.index) //between last and this note, a note off event should occur
                {
                    midiTrack.messages.Add(
                        new MidiMessage("note_off",
                            new[] { "" + noteOff[2], "" + noteOff[1], "0" }, noteOff[0] - currentIndex));
                    currentIndex = noteOff[0];
                }
                else
                {
                    temp.Add(noteOff);
                }

            noteOffs = temp;

            var velocity = n.velocity;
            int note;

            if (n.str == -2) break; //Last round

            //if (n.str-1 < 0) Debug.WriteLine("String was -1");
            //if (n.str-1 >= tuning.Length && tuning.Length != 0) Debug.Log("String was higher than string amount (" + n.str + ")");
            if (tuning.Length > 0)
                note = tuning[n.str - 1] + capo + n.fret;
            else
                note = capo + n.fret;

            if (n.harmonic != HarmonicType.none) //Has Harmonics
            {
                var harmonicNote = getHarmonic(tuning[n.str - 1], n.fret, capo, n.harmonicFret, n.harmonic);
                note = harmonicNote;
            }

            var noteChannel = channel;

            if (n.bendPoints.Count > 0) //Has Bending
            {
                var usedChannel = tryToFindChannel();
                if (usedChannel == -1) usedChannel = channel;

                NativeFormat.availableChannels[usedChannel] = false;
                channelConnections.Add(new[] { channel, usedChannel, n.index + n.duration });
                midiTrack.messages.Add(new MidiMessage("program_change", new[] { "" + usedChannel, "" + patch },
                    n.index - currentIndex));
                noteChannel = usedChannel;
                currentIndex = n.index;
                activeBendingPlans.Add(createBendingPlan(n.bendPoints, channel, usedChannel, n.duration, n.index,
                    n.resizeValue, n.isVibrato));
            }

            if (n.isVibrato && n.bendPoints.Count == 0) //Is Vibrato & No Bending
            {
                var usedChannel = channel;
                activeBendingPlans.Add(createBendingPlan(n.bendPoints, channel, usedChannel, n.duration, n.index,
                    n.resizeValue, true));
            }

            if (n.fading != Fading.none) //Fading
                volumeChanges = createVolumeChanges(n.index, n.duration, n.velocity, n.fading);

            midiTrack.messages.Add(new MidiMessage("note_on",
                new[] { "" + noteChannel, "" + note, "" + n.velocity }, n.index - currentIndex));
            currentIndex = n.index;

            if (n.bendPoints.Count > 0) //Has Bending cont.
            {
                midiTrack.messages.Add(new MidiMessage("control_change", new[] { "" + noteChannel, "101", "0" },
                    0));
                midiTrack.messages.Add(new MidiMessage("control_change", new[] { "" + noteChannel, "100", "0" },
                    0));
                midiTrack.messages.Add(new MidiMessage("control_change", new[] { "" + noteChannel, "6", "6" }, 0));
                midiTrack.messages.Add(new MidiMessage("control_change", new[] { "" + noteChannel, "38", "0" }, 0));
            }

            noteOffs.Add(new[] { n.index + n.duration, note, noteChannel });
        }


        midiTrack.messages.Add(new MidiMessage("end_of_track", new string[] { }, 0));
        return midiTrack;
    }

    private List<Note> addSlidesToNotes(List<Note> notes)
    {
        var ret = new List<Note>();
        var index = -1;
        foreach (var n in notes)
        {
            index++;
            var skipWrite = false;

            if ((n.slideInFromBelow && n.str > 1) || n.slideInFromAbove)
            {
                var myFret = n.fret;
                var start = n.slideInFromAbove ? myFret + 4 : Math.Max(1, myFret - 4);
                var beginIndex = n.index - 960 / 4; //16th before
                var lengthEach = 960 / 4 / Math.Abs(myFret - start);
                for (var x = 0; x < Math.Abs(myFret - start); x++)
                {
                    var newOne = new Note(n)
                    {
                        duration = lengthEach,
                        index = beginIndex + x * lengthEach,
                        fret = start + (n.slideInFromAbove ? -x : +x)
                    };
                    ret.Add(newOne);
                }
            }

            if ((n.slideOutDownwards && n.str > 1) || n.slideOutUpwards)
            {
                var myFret = n.fret;
                var end = n.slideOutUpwards ? myFret + 4 : Math.Max(1, myFret - 4);
                var beginIndex = n.index + n.duration - 960 / 4; //16th before
                var lengthEach = 960 / 4 / Math.Abs(myFret - end);
                n.duration -= 960 / 4;
                ret.Add(n);
                skipWrite = true;
                for (var x = 0; x < Math.Abs(myFret - end); x++)
                {
                    var newOne = new Note(n)
                    {
                        duration = lengthEach,
                        index = beginIndex + x * lengthEach,
                        fret = myFret + (n.slideOutDownwards ? -x : +x)
                    };
                    ret.Add(newOne);
                }
            }
            /*
            if (n.slidesToNext)
            {
                int slideTo = -1;
                //Find next note on same string
                for (int x = index+1; x < notes.Count; x++)
                {
                    if (notes[x].str == n.str)
                    {
                        slideTo = notes[x].fret;
                        break;
                    }
                }

                if (slideTo != -1 && slideTo != n.fret) //Found next tone on string
                {
                    int myStr = n.str;
                    int end = slideTo;
                    int beginIndex = (n.index + n.duration) - 960 / 4; //16th before
                    int lengthEach = (960 / 4) / Math.Abs(myStr - end);
                    n.duration -= 960 / 4;
                    ret.Add(n); skipWrite = true;
                    for (int x = 0; x < Math.Abs(myStr - end); x++)
                    {
                        Note newOne = new Note(n);
                        newOne.duration = lengthEach;
                        newOne.index = beginIndex + x * lengthEach;
                        newOne.fret = myStr + (slideTo < n.fret ? -x : +x);
                        ret.Add(newOne);
                    }
                }
            }
            */

            if (!skipWrite) ret.Add(n);
        }

        return ret;
    }

    private List<int[]> createVolumeChanges(int index, int duration, int velocity, Fading fading)
    {
        const int segments = 20;
        var changes = new List<int[]>();
        switch (fading)
        {
            case Fading.fadeIn:
            case Fading.fadeOut:
            {
                var step = velocity / segments;
                var val = fading == Fading.fadeIn ? 0 : velocity;
                if (fading == Fading.fadeOut) step = (int)(-step * 1.25f);

                for (var x = index; x < index + duration; x += duration / segments)
                {
                    changes.Add(new[] { x, Math.Min(127, Math.Max(0, val)) });
                    val += step;
                }

                break;
            }
            case Fading.volumeSwell:
            {
                var step = (int)(velocity / (segments * 0.8f));
                var val = 0;
                var times = 0;
                for (var x = index; x < index + duration; x += duration / segments)
                {
                    changes.Add(new[] { x, Math.Min(127, Math.Max(0, val)) });
                    val += step;
                    if (times == segments / 2) step = -step;

                    times++;
                }

                break;
            }
        }

        changes.Add(new[] { index + duration, velocity }); //Definitely go back to normal


        return changes;
    }

    private List<int> getActiveChannels(IEnumerable<int[]> channelConnections)
    {
        var ret_val = new List<int> { channel };
        ret_val.AddRange(channelConnections.Select(static cc => cc[1]));
        return ret_val;
    }

    public static int tryToFindChannel()
    {
        var cnt = 0;
        foreach (var available in NativeFormat.availableChannels)
        {
            if (available) return cnt;

            cnt++;
        }

        return -1;
    }

    public static int getHarmonic(int baseTone, int fret, int capo, float harmonicFret, HarmonicType type)
    {
        var val = 0;
        //Capo, base tone and fret (if not natural harmonic) shift the harmonics simply
        val = baseTone + capo;
        if (type != HarmonicType.natural) val += (int)Math.Round(harmonicFret);

        val += fret;

        switch (harmonicFret)
        {
            case 2.4f:
                val += 34;
                break;
            case 2.7f:
                val += 31;
                break;
            case 3.2f:
                val += 28;
                break;
            case 4f:
                val += 24;
                break;
            case 5f:
                val += 19;
                break;
            case 5.8f:
                val += 28;
                break;
            case 7f:
                val += 12;
                break;
            case 8.2f:
                val += 28;
                break;
            case 9f:
                val += 19;
                break;
            case 9.6f:
                val += 24;
                break;
            case 12f:
                val += 0;
                break;
            case 14.7f:
                val += 19;
                break;
            case 16f:
                val += 12;
                break;
            case 17f:
                val += 19;
                break;
            case 19f:
                val += 0;
                break;
            case 21.7f:
                val += 12;
                break;
            case 24f:
                val += 0;
                break;
        }

        return Math.Min(val, 127);
    }


    public static List<BendPoint> findAndSortCurrentBendPoints(List<BendingPlan> activeBendingPlans, int index)
    {
        var bps = new List<BendPoint>();
        foreach (var bpl in activeBendingPlans)
        foreach (var bp in bpl.bendingPoints.Where(bp => bp.index <= index))
        {
            bp.usedChannel = bpl.usedChannel;
            bps.Add(bp);
        }

        bps.Sort(static (x, y) => x.index.CompareTo(y.index));

        return bps;
    }

    public static List<TremoloPoint> addDetailsToTremoloPoints(List<TremoloPoint> tremoloPoints, int maxDistance)
    {
        var tremPoints = new List<TremoloPoint>();
        var oldValue = 0.0f;
        var oldIndex = 0;
        foreach (var tp in tremoloPoints)
        {
            if (tp.index - oldIndex > maxDistance && !(oldValue == 0.0f && tp.value == 0.0f))
                //Add in-between points
                for (var x = oldIndex + maxDistance; x < tp.index; x += maxDistance)
                {
                    var value = oldValue + (tp.value - oldValue) *
                        (((float)x - oldIndex) / ((float)tp.index - oldIndex));
                    tremPoints.Add(new TremoloPoint(value, x));
                }

            tremPoints.Add(tp);

            oldValue = tp.value;
            oldIndex = tp.index;
        }


        return tremPoints;
    }

    public static BendingPlan createBendingPlan(List<BendPoint> bendPoints, int originalChannel, int usedChannel,
        int duration, int index, float resize, bool isVibrato)
    {
        var maxDistance = duration / 10; //After this there should be a pitchwheel event
        if (isVibrato) maxDistance = Math.Min(maxDistance, 60);

        if (bendPoints.Count == 0)
        {
            //Create Vibrato Plan
            bendPoints.Add(new BendPoint(0.0f, index));
            bendPoints.Add(new BendPoint(0.0f, index + duration));
        }

        var bendingPoints = new List<BendPoint>();


        //Resize the points according to (changed) note duration
        foreach (var bp in bendPoints)
        {
            bp.index = (int)(index + (bp.index - index) * resize);
            bp.usedChannel = usedChannel;
        }

        var old_pos = index;
        var old_value = 0.0f;
        var start = true;
        var vibratoSize = 0;
        var vibratoChange = 0;
        if (isVibrato) vibratoSize = 12;

        if (isVibrato) vibratoChange = 6;

        var vibrato = 0;
        foreach (var bp in bendPoints)
        {
            if (bp.index - old_pos > maxDistance)
                //Add in-between points
                for (var x = old_pos + maxDistance; x < bp.index; x += maxDistance)
                {
                    var value = old_value + (bp.value - old_value) *
                        (((float)x - old_pos) / ((float)bp.index - old_pos));
                    bendingPoints.Add(new BendPoint(value + vibrato, x));
                    if (isVibrato && Math.Abs(vibrato) == vibratoSize) vibratoChange = -vibratoChange;

                    vibrato += vibratoChange;
                }

            if (start || bp.index != old_pos)
            {
                if (isVibrato) bp.value += vibrato;

                bendingPoints.Add(bp);
            }

            old_pos = bp.index;
            old_value = bp.value;
            if ((start || bp.index != old_pos) && isVibrato)
                old_value -= vibrato; //Add back, so not to be influenced by it

            start = false;
            if (isVibrato && Math.Abs(vibrato) == vibratoSize) vibratoChange = -vibratoChange;

            vibrato += vibratoChange;
        }

        if (Math.Abs(index + duration - old_pos) > maxDistance)
            bendingPoints.Add(new BendPoint(old_value, index + duration));

        return new BendingPlan(originalChannel, usedChannel, bendingPoints);
    }
}

public sealed class BendingPlan
{
    public List<BendPoint> bendingPoints = new();

    //List<int> positions = new List<int>(); //index where to put the points
    public int originalChannel;
    public int usedChannel;

    public BendingPlan(int originalChannel, int usedChannel, List<BendPoint> bendingPoints)
    {
        this.bendingPoints = bendingPoints;
        //this.positions = positions;
        this.originalChannel = originalChannel;
        this.usedChannel = usedChannel;
    }
}

public sealed class MasterBar
{
    public int den = 4;
    public int duration;
    public int index; //Midi Index
    public int key; //C, -1 = F, 1 = G
    public string keyBoth = "0";
    public int keyType; //0 = Major, 1 = Minor
    public int num = 4;

    public string time = "4/4";

    public TripletFeel
        tripletFeel = TripletFeel.none; //additional info -> note values are changed in duration and position too
}

public enum PlaybackState
{
    def = 0,
    mute = 1,
    solo = 2
}

public sealed class Tempo
{
    public int position; //total position in song @ 960 ticks_per_beat
    public float value = 120.0f;
}