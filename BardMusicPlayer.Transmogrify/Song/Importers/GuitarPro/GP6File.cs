#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers.GuitarPro;

public sealed class GP6File : GPFile
{
    private static readonly List<GP6Tempo> tempos = new();
    private static List<GP6Chord> chords = new();
    private static List<GP6Rhythm> rhythms = new();

    public static int currentMeasure;
    public static int currentTrack;

    public static int totalLength;
    public static int lengthPassed;

    private readonly byte[] udata; //uncompressed data

    //public List<Track> tracks;
    public GP6File(byte[] _data)
    {
        GPBase.pointer = 0;
        udata = _data;
    }


    private void decompressFile()
    {
        var data = new List<byte>();
        var bs = new BitStream(udata);
        var estFileSize = BitConverter.ToInt32(udata, 4);
        bs.SkipBytes(8);
        while (!bs.finished)
        {
            var isCompressed = bs.GetBit();

            if (isCompressed)
            {
                var wordSize = bs.GetBitsBE(4);
                var offset = bs.GetBitsLE(wordSize);
                var length = bs.GetBitsLE(wordSize);
                var sourcePosition = data.Count - offset;
                if (sourcePosition < 0) break;

                var to_read = Math.Min(length, offset);
                for (var r = sourcePosition; r < sourcePosition + to_read; r++) data.Add(data[r]);
            }
            else
            {
                var byteLength = bs.GetBitsLE(2);
                for (var x = 0; x < byteLength; x++) data.Add(bs.GetByte());
            }
        }

        GPBase.data = data.ToArray();
    }


    public override void readSong()
    {
        decompressFile();

        var sb = new StringBuilder();
        var startOfXml = 0;
        for (var x = 0; x < GPBase.data.Length; x++) //8150
        {
            //string hex = BitConverter.ToString(GPBase.data.ToList().GetRange(x,Math.Min(8150,GPBase.data.Length-x)).ToArray()).Replace("-", string.Empty);
            sb.Append((char)GPBase.data[x]);
            if (startOfXml == 0 && (char)GPBase.data[x] == '<' && (char)GPBase.data[x + 1] == 'G') startOfXml = x;
        }

        var xml = sb.ToString();
        // for (var x = startOfXml; x < xml.Length; x += 8000)
        // {
        //     //Debug.Log(xml.Substring(x, 8000));
        // }

        var parsedXml = ParseGP6(xml, startOfXml);

        var gp5File = GP6NodeToGP5File(parsedXml.subnodes[0]);
        tracks = gp5File.tracks;
        self = gp5File;
    }

    public static GP5File GP6NodeToGP5File(Node node) //node = GPIF tag
    {
        var file = new GP5File(new byte[] { })
        {
            version = "GUITAR PRO 6.0",
            versionTuple = new[] { 6, 0 },
            //set direct members of song:
            title = node.getSubnodeByName("Score", true).subnodes[0].content,
            subtitle = node.getSubnodeByName("Score").subnodes[1].content,
            interpret = node.getSubnodeByName("Score").subnodes[2].content,
            album = node.getSubnodeByName("Score").subnodes[3].content,
            words = node.getSubnodeByName("Score").subnodes[4].content,
            music = node.getSubnodeByName("Score").subnodes[5].content,
            copyright = node.getSubnodeByName("Score").subnodes[7].content,
            tab_author = node.getSubnodeByName("Score").subnodes[8].content,
            instructional = node.getSubnodeByName("Score").subnodes[9].content,
            notice = node.getSubnodeByName("Score").subnodes[10].content.Split('\n') //?
        };

        //Page Layout
        var nPageLayout = node.getSubnodeByName("Score", true).getSubnodeByName("PageSetup", true);
        file.pageSetup = new PageSetup();
        if (nPageLayout != null)
        {
            file.pageSetup.pageSize = new Point(int.Parse(nPageLayout.subnodes[0].content),
                int.Parse(nPageLayout.subnodes[1].content, CultureInfo.InvariantCulture));
            file.pageSetup.pageMargin = new Padding(
                int.Parse(nPageLayout.subnodes[5].content, CultureInfo.InvariantCulture),
                int.Parse(nPageLayout.subnodes[3].content, CultureInfo.InvariantCulture),
                int.Parse(nPageLayout.subnodes[4].content, CultureInfo.InvariantCulture),
                int.Parse(nPageLayout.subnodes[6].content, CultureInfo.InvariantCulture));
            file.pageSetup.scoreSizeProportion =
                float.Parse(nPageLayout.subnodes[7].content, CultureInfo.InvariantCulture);
        }

        file.lyrics = transferLyrics(node.getSubnodeByName("Tracks"));
        //tempo, key, midiChannels, directions only on a per track / per measureHeader (MasterBar) basis

        file.measureCount = node.getSubnodeByName("MasterBars", true).subnodes.Count;
        file.trackCount = node.getSubnodeByName("Tracks", true).subnodes.Count;

        var nAutomations = node.getSubnodeByName("MasterTrack", true).getSubnodeByName("Automations", true);
        foreach (var nAutomation in nAutomations.subnodes) tempos.Add(new GP6Tempo(nAutomation));

        file.measureHeaders = transferMeasureHeaders(node.getSubnodeByName("MasterBars"), file);
        file.tracks = transferTracks(node.getSubnodeByName("Tracks", true), file);
        rhythms = readRhythms(node.getSubnodeByName("Rhythms", true));
        chords = readChords(node.getSubnodeByName("Tracks", true));
        transferBars(node, file); //Bars > Voices > Beats > Notes

        //TODO update global vars tempo, key, midiChannels, directions based on first value?
        return file;
    }

    public static void transferBars(Node node, GP5File song)
    {
        var nBars = node.getSubnodeByName("Bars", true);
        var cnt = 0;
        var barCnt = -1;
        foreach (var nBar in nBars.subnodes)
        {
            var _bar = new Measure();
            var clef = nBar.getSubnodeByName("Clef").content;
            _bar.clef = clef switch
            {
                "G2" => MeasureClef.treble,
                "F4" => MeasureClef.bass,
                "Neutral" => MeasureClef.neutral,
                _ => _bar.clef
            };
            //.. not important for this app.

            var voices = nBar.getSubnodeByName("Voices").content.Split(' ');
            _bar.track = song.tracks[cnt % song.trackCount];
            if (cnt % song.trackCount == 0) barCnt++;
            _bar.header = song.measureHeaders[barCnt];
            currentMeasure = barCnt;
            currentTrack = cnt % song.trackCount;

            cnt++;
            var nSimileMark = nBar.getSubnodeByName("SimileMark", true);
            if (nSimileMark != null)
                _bar.simileMark = nSimileMark.content switch
                {
                    "Simple" => SimileMark.simple,
                    "FirstOfDouble" => SimileMark.firstOfDouble,
                    "SecondOfDouble" => SimileMark.secondOfDouble,
                    _ => _bar.simileMark
                };

            _bar.voices = new List<Voice>();

            foreach (var voiceParsed in voices.Select(static voice => int.Parse(voice, CultureInfo.InvariantCulture))
                         .Where(static voiceParsed => voiceParsed >= 0))
                _bar.voices.Add(transferVoice(node, voiceParsed, _bar));

            song.tracks[(cnt - 1) % song.trackCount].addMeasure(_bar);
        }
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

        return result;
    }

    public static Voice transferVoice(Node node, int index, Measure bar)
    {
        totalLength = flipDuration(bar.header.timeSignature.denominator) * bar.header.timeSignature.numerator;
        lengthPassed = 0;
        var voice = new Voice();
        var beats = node.getSubnodeByName("Voices", true).subnodes[index].getSubnodeByName("Beats", true).content
            .Split(' ');
        voice.beats = new List<Beat>();
        voice.measure = bar;

        foreach (var beat in beats)
            voice.beats.Add(transferBeat(node, int.Parse(beat, CultureInfo.InvariantCulture), voice));
        return voice;
    }


    public static Beat transferBeat(Node node, int index, Voice voice)
    {
        var beat = new Beat();
        var nBeat = node.getSubnodeByName("Beats", true).subnodes[index];
        var nNotes = nBeat.getSubnodeByName("Notes");
        beat.notes = new List<Note>();
        beat.voice = voice;
        beat.effect = new BeatEffect
        {
            mixTableChange = new MixTableChange()
        };


        //Beat Duration
        beat.duration = new Duration();
        var rhythmRef = int.Parse(nBeat.getSubnodeByName("Rhythm", true).propertyValues[0],
            CultureInfo.InvariantCulture);
        beat.duration.value = rhythms[rhythmRef].noteValue;
        beat.duration.isDotted = rhythms[rhythmRef].augmentationDots == 1;
        beat.duration.isDoubleDotted = rhythms[rhythmRef].augmentationDots == 2;
        beat.duration.tuplet = new Tuplet();
        beat.duration.tuplet = rhythms[rhythmRef].primaryTuplet;

        //Check if should add tempo mark
        if (currentTrack == 0)
        {
            lengthPassed += flipDuration(beat.duration);

            foreach (var tempo in tempos)
                if (tempo.bar == currentMeasure && tempo.transferred == false)
                    if ((float)lengthPassed / totalLength > tempo.position)
                    {
                        //Place tempo value
                        float myTempo = tempo.tempo;
                        switch (tempo.tempoType)
                        {
                            case 1:
                                myTempo /= 2.0f;
                                break;
                            case 3:
                                myTempo *= 1.5f;
                                break;
                            case 4:
                                myTempo *= 2.0f;
                                break;
                            case 5:
                                myTempo *= 3.0f;
                                break;
                        }


                        beat.effect.mixTableChange.tempo = new MixTableItem((int)myTempo, 0, true);
                        tempo.transferred = true;
                    }
        }

        if (nNotes == null) //No notes
        {
            beat.status = BeatStatus.rest;
            return beat;
        }

        var notes = nNotes.content.Split(' ');

        var nChord = nBeat.getSubnodeByName("Chord");
        if (nChord != null)
        {
            beat.effect.chord = new Chord(0);
            foreach (var chord in chords.Where(chord => chord.forTrack == beat.voice.measure.track.number &&
                                                        chord.id == int.Parse(nChord.content,
                                                            CultureInfo.InvariantCulture)))
                beat.effect.chord.name = chord.name;
            //Here later can go further infos..
        }

        var velocity = Velocities.forte;

        var nDynamic = nBeat.getSubnodeByName("Dynamic");
        if (nDynamic != null)
        {
            var dynamicSymbol = nDynamic.content;
            string[] GP6symbols = { "PPP", "PP", "P", "MP", "MF", "F", "FF", "FFF" };
            int[] velocities =
            {
                Velocities.pianoPianissimo, Velocities.pianissimo, Velocities.piano,
                Velocities.mezzoPiano, Velocities.mezzoForte, Velocities.forte, Velocities.fortissimo,
                Velocities.forteFortissimo
            };
            for (var x = 0; x < GP6symbols.Length; x++)
                if (GP6symbols[x].Equals(dynamicSymbol))
                {
                    velocity = velocities[x];
                    break;
                }
        }

        beat.effect.fadeIn = nBeat.getSubnodeByName("Fadding", true) != null &&
                             nBeat.getSubnodeByName("Fadding", true).content.Equals("FadeIn");
        beat.effect.fadeOut = nBeat.getSubnodeByName("Fadding", true) != null &&
                              nBeat.getSubnodeByName("Fadding", true).content.Equals("FadeOut");
        beat.effect.volumeSwell = nBeat.getSubnodeByName("Fadding", true) != null &&
                                  nBeat.getSubnodeByName("Fadding", true).content.Equals("VolumeSwell");

        if (nBeat.getSubnodeByName("FreeText", true) != null)
            beat.text = new BeatText(nBeat.getSubnodeByName("FreeText", true).content);

        var searchArpeggioParams = false;
        var nArpeggio = nBeat.getSubnodeByName("Arpeggio");
        if (nArpeggio != null)
        {
            var direction = nArpeggio.content;
            var bsd = direction.Equals("Up") ? BeatStrokeDirection.up : BeatStrokeDirection.down;
            beat.effect.stroke = new BeatStroke
            {
                direction = bsd
            };
            searchArpeggioParams = true;
        }


        var searchBrushParams = false;
        var nProperties = nBeat.getSubnodeByName("Properties");
        if (nProperties != null)
        {
            //Whammy values in GP6 format: (GP7 below in subnode "Whammy")
            var whammyBarOriginValue = 0.0f;
            var whammyBarMiddleValue = 0.0f;
            var whammyBarDestinationValue = 0.0f;
            var whammyBarOriginOffset = 0.0f;
            var whammyBarMiddleOffset1 = -1.0f;
            var whammyBarMiddleOffset2 = -1.0f;
            var whammyBarDestinationOffset = 100.0f;
            var hasWhammy = false;

            foreach (var nProperty in nProperties.subnodes)
                switch (nProperty.propertyValues[0])
                {
                    case "Slapped":
                        beat.effect.slapEffect = SlapEffect.slapping;
                        break;
                    case "Popped":
                        beat.effect.slapEffect = SlapEffect.popping;
                        break;
                    case "Brush":
                    {
                        var direction = nProperty.subnodes[0].content;
                        var bsd = direction.Equals("Up") ? BeatStrokeDirection.up : BeatStrokeDirection.down;
                        beat.effect.stroke = new BeatStroke
                        {
                            direction = bsd
                        };
                        searchBrushParams = true; //search in Xproperty
                        break;
                    }
                    case "PickStroke":
                    {
                        var direction = nProperty.subnodes[0].content;
                        var bsd = direction.Equals("Up") ? BeatStrokeDirection.up : BeatStrokeDirection.down;
                        beat.effect.pickStroke = bsd;
                        break;
                    }
                    case "VibratoWTremBar":
                        beat.effect.vibrato = true;
                        break;
                    case "WhammyBar":
                        hasWhammy = true;
                        break;
                    case "WhammyBarOriginValue":
                        whammyBarOriginValue = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "WhammyBarMiddleValue":
                        whammyBarMiddleValue = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "WhammyBarDestinationValue":
                        whammyBarDestinationValue =
                            float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "WhammyBarMiddleOffset1":
                        whammyBarMiddleOffset1 =
                            float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "WhammyBarMiddleOffset2":
                        whammyBarMiddleOffset2 =
                            float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "WhammyBarOriginOffset":
                        whammyBarOriginOffset =
                            float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "WhammyBarDestinationOffset":
                        whammyBarDestinationOffset =
                            float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                }

            if (hasWhammy)
            {
                if (whammyBarMiddleOffset1 == -1.0f)
                {
                    whammyBarMiddleOffset1 = whammyBarOriginOffset +
                                             (whammyBarDestinationOffset - whammyBarOriginOffset) / 2.0f;
                    whammyBarMiddleValue =
                        whammyBarOriginValue + (whammyBarDestinationValue - whammyBarOriginValue) / 2.0f;
                }

                if (whammyBarMiddleOffset2 == -1.0f) whammyBarMiddleOffset2 = whammyBarMiddleOffset1;
                beat.effect.tremoloBar = new BendEffect
                {
                    type = BendType.none, //Not defined in GP6
                    points = new List<BendPoint>
                    {
                        new(0.0f, whammyBarOriginValue),
                        new(whammyBarOriginOffset, whammyBarOriginValue)
                    }
                };

                //Peak or Valley
                if ((whammyBarMiddleValue - whammyBarOriginValue) * (whammyBarDestinationValue - whammyBarMiddleValue) <
                    0)
                {
                    beat.effect.tremoloBar.points.Add(new BendPoint(whammyBarMiddleOffset1, whammyBarMiddleValue));
                    beat.effect.tremoloBar.points.Add(new BendPoint(whammyBarMiddleOffset2, whammyBarMiddleValue));
                }

                beat.effect.tremoloBar.points.Add(new BendPoint(whammyBarDestinationOffset, whammyBarDestinationValue));
                beat.effect.tremoloBar.points.Add(new BendPoint(100.0f, whammyBarDestinationValue));
            }
        }

        var nWhammy = nBeat.getSubnodeByName("Whammy", true);
        if (nWhammy != null)
        {
            beat.effect.tremoloBar = new BendEffect
            {
                type = BendType.none, //Not defined in GP6
                points = new List<BendPoint>()
            };
            var originValue = float.Parse(nWhammy.propertyValues[0], CultureInfo.InvariantCulture);
            var middleValue = float.Parse(nWhammy.propertyValues[1], CultureInfo.InvariantCulture);
            var destinationValue = float.Parse(nWhammy.propertyValues[2], CultureInfo.InvariantCulture);
            var originOffset = float.Parse(nWhammy.propertyValues[3], CultureInfo.InvariantCulture);
            var middleOffset1 = float.Parse(nWhammy.propertyValues[4], CultureInfo.InvariantCulture);
            var middleOffset2 = float.Parse(nWhammy.propertyValues[5], CultureInfo.InvariantCulture);
            var destinationOffset = float.Parse(nWhammy.propertyValues[6], CultureInfo.InvariantCulture);

            beat.effect.tremoloBar.points.Add(new BendPoint(0.0f, originValue));
            beat.effect.tremoloBar.points.Add(new BendPoint(originOffset, originValue));
            //Peak or Valley
            if ((middleValue - originValue) * (destinationValue - middleValue) < 0)
            {
                beat.effect.tremoloBar.points.Add(new BendPoint(middleOffset1, middleValue));
                beat.effect.tremoloBar.points.Add(new BendPoint(middleOffset2, middleValue));
            }

            beat.effect.tremoloBar.points.Add(new BendPoint(destinationOffset, destinationValue));
            beat.effect.tremoloBar.points.Add(new BendPoint(100.0f, destinationValue));
        }

        var nXProperty = nBeat.getSubnodeByName("XProperties");
        if (nXProperty != null)
        {
            if (searchBrushParams)
            {
                var duration = int.Parse(nXProperty.getSubnodeByProperty("id", "687935489").subnodes[0].content,
                    CultureInfo.InvariantCulture);
                var startsOnTime = float.Parse(nXProperty.getSubnodeByProperty("id", "687935490").subnodes[0].content,
                    CultureInfo.InvariantCulture);
                beat.effect.stroke.setByGP6Standard(duration);
                beat.effect.stroke.startTime = startsOnTime;
            }

            if (searchArpeggioParams)
            {
                var duration = int.Parse(nXProperty.getSubnodeByProperty("id", "687931393").subnodes[0].content,
                    CultureInfo.InvariantCulture);
                var startsOnTime = float.Parse(nXProperty.getSubnodeByProperty("id", "687931394").subnodes[0].content,
                    CultureInfo.InvariantCulture);
                beat.effect.stroke.setByGP6Standard(duration);
                beat.effect.stroke.startTime = startsOnTime;
            }
        }

        if (nBeat.getSubnodeByName("Wah") != null)
            beat.effect.mixTableChange.wah = new WahEffect
            {
                state = nBeat.getSubnodeByName("Wah").content.Equals("Open")
                    ? WahState.opened
                    : WahState.closed
            };

        GraceEffect graceEffect = null; //Stay null if there is none
        var nGraceEffect = nBeat.getSubnodeByName("GraceNotes");
        if (nGraceEffect != null)
        {
            graceEffect = new GraceEffect();
            var beforeBeat = nGraceEffect.content.Equals("BeforeBeat");
            graceEffect.isOnBeat = !beforeBeat;
            //All other infos will be filled in by the note
        }

        var nTremolo = nBeat.getSubnodeByName("Tremolo");
        var tremolo = "";
        if (nTremolo != null) tremolo = nTremolo.content;

        beat.notes = new List<Note>();
        foreach (var note in notes)
        {
            //Give each Note a GraceEffect obj & Velocities val
            //velocity;
            beat.notes.Add(transferNote(node, int.Parse(note, CultureInfo.InvariantCulture), beat, velocity,
                graceEffect, tremolo, out var tapping));
            if (tapping) beat.effect.slapEffect = SlapEffect.tapping;
        }

        return beat;
    }


    public static Note transferNote(Node node, int index, Beat beat, int velocity, GraceEffect graceEffect,
        string tremolo, out bool tapping)
    {
        tapping = false;
        var note = new Note();
        var nNote = node.getSubnodeByName("Notes", true).subnodes[index];
        note.beat = beat;
        note.effect = new NoteEffect();
        note.type = NoteType.normal;

        //Properties
        var nProperties = nNote.getSubnodeByName("Properties", true);
        if (nProperties != null)
        {
            float harmonicFret = -1;
            var harmonicType = "";
            var bendDestOff = 100.0f;
            var bendDestVal = 0.0f;
            var bendMidOff1 = -1.0f;
            var bendMidOff2 = -1.0f;
            var bendMidVal = 0.0f;
            var bendOrigVal = 0.0f;
            var bendOrigOff = 0.0f;
            var element = -1; //GP6-style drums
            var variation = 0;
            var bendEffect = new BendEffect();
            var hasBendEffect = false;

            foreach (var nProperty in nProperties.subnodes)
                switch (nProperty.propertyValues[0])
                {
                    case "Element":
                        element = int.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "Variation":
                        variation = int.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "Fret":
                        note.value = int.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "String":
                        note.str = int.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture) + 1;
                        break;
                    case "PalmMuted":
                        note.effect.palmMute = true;
                        break;
                    case "Muted":
                        note.type = NoteType.dead;
                        break;
                    case "HarmonicFret":
                        harmonicFret = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "HarmonicType":
                        harmonicType = nProperty.subnodes[0].content;
                        break;
                    case "Bended":
                        hasBendEffect = true;
                        break;
                    case "BendDestinationOffset":
                        bendDestOff = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "BendDestinationValue":
                        bendDestVal = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "BendMiddleOffset1":
                        bendMidOff1 = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "BendMiddleOffset2":
                        bendMidOff2 = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "BendMiddleValue":
                        bendMidVal = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "BendOriginValue":
                        bendOrigVal = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "BendOriginOffset":
                        bendOrigOff = float.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        break;
                    case "Slide":
                    {
                        note.effect.slides = new List<SlideType>();
                        var flags = uint.Parse(nProperty.subnodes[0].content, CultureInfo.InvariantCulture);
                        if (flags % 2 == 1) note.effect.slides.Add(SlideType.shiftSlideTo);

                        if ((flags >> 1) % 2 == 1) note.effect.slides.Add(SlideType.legatoSlideTo);

                        if ((flags >> 2) % 2 == 1) note.effect.slides.Add(SlideType.outDownwards);

                        if ((flags >> 3) % 2 == 1) note.effect.slides.Add(SlideType.outUpwards);

                        if ((flags >> 4) % 2 == 1) note.effect.slides.Add(SlideType.intoFromBelow);

                        if ((flags >> 5) % 2 == 1) note.effect.slides.Add(SlideType.intoFromAbove);

                        if ((flags >> 6) % 2 == 1) note.effect.slides.Add(SlideType.pickScrapeOutDownwards);

                        if ((flags >> 7) % 2 == 1) note.effect.slides.Add(SlideType.pickScrapeOutUpwards);

                        break;
                    }
                    case "LeftHandTapped":
                    case "HopoDestination":
                        note.effect.hammer = true;
                        break;
                    case "Tapped":
                        tapping = true;
                        break;
                }

            if (hasBendEffect)
            {
                if (bendMidOff1 == -1.0f)
                {
                    bendMidOff1 = bendOrigOff + (bendDestOff - bendOrigOff) / 2.0f;
                    bendMidVal = bendOrigVal + (bendDestVal - bendOrigVal) / 2.0f;
                }

                if (bendMidOff2 == -1.0f) bendMidOff2 = bendMidOff1;

                bendEffect.points.Add(new BendPoint(0.0f, bendOrigVal));
                bendEffect.points.Add(new BendPoint(bendOrigOff, bendOrigVal));
                //Peak or Valley
                if ((bendMidVal - bendOrigVal) * (bendDestVal - bendMidVal) < 0)
                {
                    bendEffect.points.Add(new BendPoint(bendMidOff1, bendMidVal));
                    bendEffect.points.Add(new BendPoint(bendMidOff2, bendMidVal));
                }

                bendEffect.points.Add(new BendPoint(bendDestOff, bendDestVal));
                bendEffect.points.Add(new BendPoint(100.0f, bendDestVal));

                note.effect.bend = bendEffect;
            }

            if (harmonicFret != -1)
            {
                switch (harmonicType)
                {
                    case "Natural":
                    case "":
                        note.effect.harmonic = new NaturalHarmonic(); //Ignore the complicated GP3-5 settings
                        break;
                    //There should be during playback a function that reads only fret and type and creates the harmonic + for GP3-5 files that transfers the old format
                    case "Artificial":
                        note.effect.harmonic = new ArtificialHarmonic();
                        break;
                    case "Pinch":
                        note.effect.harmonic = new PinchHarmonic();
                        break;
                    case "Tap":
                        note.effect.harmonic = new TappedHarmonic();
                        break;
                    case "Semi":
                        note.effect.harmonic = new SemiHarmonic();
                        break;
                    case "Feedback":
                        note.effect.harmonic = new FeedbackHarmonic();
                        break;
                }

                note.effect.harmonic.fret = harmonicFret;
            }

            if (element != -1) //GP6-style Drumset
            {
                var midiValue = getGP6DrumValue(element, variation);
                note.value = midiValue;
                note.str = 1;
            }
        }


        //XProperties (are there more?)
        var trillLength = 0;
        var nXProperties = nNote.getSubnodeByName("XProperties", true);
        var nTrillLength = nXProperties?.getSubnodeByProperty("id", "688062467");
        if (nTrillLength != null)
            trillLength = int.Parse(nTrillLength.subnodes[0].content, CultureInfo.InvariantCulture);

        //Other Subnodes
        var nTrill = nNote.getSubnodeByName("Trill");
        if (nTrill != null)
        {
            var secondNote = int.Parse(nTrill.content, CultureInfo.InvariantCulture);
            note.effect.trill = new TrillEffect
            {
                fret = secondNote,
                duration = new Duration(trillLength)
            };
        }

        var nVibrato = nNote.getSubnodeByName("Vibrato");
        if (nVibrato != null) note.effect.vibrato = true;
        if (nNote.getSubnodeByName("LetRing") != null) note.effect.letRing = true;
        var nAntiAccent = nNote.getSubnodeByName("AntiAccent");
        if (nAntiAccent != null) note.effect.ghostNote = true;
        var nAccent = nNote.getSubnodeByName("Accent");
        if (nAccent != null)
        {
            var val = int.Parse(nAccent.content, CultureInfo.InvariantCulture);
            note.effect.accentuatedNote = val == 4;
            note.effect.heavyAccentuatedNote = val == 8;
            note.effect.staccato = val == 1;
        }

        // Node nAccidental = nNote.getSubnodeByName("Accidental"); Doesn't matter for this app.
        if (nNote.getSubnodeByName("Tie") != null && nNote.getSubnodeByName("Tie").propertyValues[1].Equals("true"))
            note.type = NoteType.tie;

        if (!tremolo.Equals(""))
        {
            note.effect.tremoloPicking = new TremoloPickingEffect
            {
                //1/2 = 8th, 1/4 = 16ths, 1/8 = 32nds
                duration = new Duration()
            };
            note.effect.tremoloPicking.duration.value = tremolo switch
            {
                "1/2" => 8,
                "1/4" => 16,
                "1/8" => 32,
                _ => note.effect.tremoloPicking.duration.value
            };
        }

        note.effect.grace = graceEffect;
        note.velocity = velocity;

        return note;
    }

    public static int getGP6DrumValue(int element, int variation)
    {
        var val = element * 10 + variation;
        return val switch
        {
            0 => 35,
            10 => 38,
            11 => 91,
            12 => 37,
            20 => 99,
            30 => 56,
            40 => 102,
            50 => 43,
            60 => 45,
            70 => 47,
            80 => 48,
            90 => 50,
            100 => 42,
            101 => 92,
            102 => 46,
            110 => 44,
            120 => 57,
            130 => 49,
            140 => 55,
            150 => 51,
            151 => 93,
            152 => 53,
            160 => 52,
            _ => 0
        };
    }

    public static List<GP6Chord> readChords(Node nTracks)
    {
        var ret_val = new List<GP6Chord>();
        var tcnt = 0;
        foreach (var nDiagrams in nTracks.subnodes.Select(static nTrack => nTrack.getSubnodeByName("Properties"))
                     .Select(static nProperties => nProperties?.getSubnodeByProperty("name", "DiagramCollection")))
        {
            if (nDiagrams != null)
            {
                var nItems = nDiagrams.getSubnodeByName("Items");
                var chordcnt = 0;
                foreach (var chord in nItems.subnodes.Select(Item => new GP6Chord
                         {
                             id = chordcnt,
                             forTrack = tcnt,
                             name = Item.propertyValues[1]
                         }))
                {
                    //Here I can later parse the chord picture
                    ret_val.Add(chord);
                    chordcnt++;
                }
            }


            tcnt++;
        }

        return ret_val;
    }

    public static List<GP6Rhythm> readRhythms(Node nRhythms)
    {
        var ret_val = new List<GP6Rhythm>();
        var cnt = 0;
        foreach (var nRhythm in nRhythms.subnodes)
        {
            string[] durations = { "Whole", "Half", "Quarter", "Eighth", "16th", "32nd" };
            var noteValue = nRhythm.getSubnodeByName("NoteValue", true).content;
            var note = 4;
            for (var x = 0; x < durations.Length; x++)
                if (noteValue.Equals(durations[x]))
                    note = (int)Math.Pow(2, x);
            var nAug = nRhythm.getSubnodeByName("AugmentationDot", true);
            var augCnt = 0;
            if (nAug != null) augCnt = int.Parse(nAug.propertyValues[0], CultureInfo.InvariantCulture);
            var nTuplet = nRhythm.getSubnodeByName("PrimaryTuplet");
            int n = 1, m = 1;
            if (nTuplet != null)
            {
                n = int.Parse(nTuplet.propertyValues[0], CultureInfo.InvariantCulture);
                m = int.Parse(nTuplet.propertyValues[1], CultureInfo.InvariantCulture);
            }

            ret_val.Add(new GP6Rhythm(cnt++, note, augCnt, n, m));
        }

        return ret_val;
    }

    public static List<Track> transferTracks(Node nTracks, GP5File song)
    {
        var ret_val = new List<Track>();
        var cnt = 0;
        foreach (var nTrack in nTracks.subnodes)
        {
            var _track = new Track(song, cnt++)
            {
                name = nTrack.getSubnodeByName("Name").content
            };
            var colors = nTrack.getSubnodeByName("Color").content.Split(' ');
            _track.color = new myColor(int.Parse(colors[0]), int.Parse(colors[1]),
                int.Parse(colors[2], CultureInfo.InvariantCulture));
            _track.channel = new MidiChannel();

            var param = nTrack.getSubnodeByName("RSE").getSubnodeByName("ChannelStrip").getSubnodeByName("Parameters")
                .content.Split(' ');
            _track.channel.bank = 0;
            _track.channel.balance = (int)(100 * float.Parse(param[11], CultureInfo.InvariantCulture));
            _track.channel.volume = (int)(100 * float.Parse(param[12], CultureInfo.InvariantCulture));


            var nMidi = nTrack.getSubnodeByName("GeneralMidi", true);
            if (nMidi != null) //GP6
            {
                _track.channel.instrument =
                    int.Parse(nMidi.getSubnodeByName("Program").content, CultureInfo.InvariantCulture);
                _track.channel.channel = int.Parse(nMidi.getSubnodeByName("PrimaryChannel").content,
                    CultureInfo.InvariantCulture);
                _track.channel.effectChannel = int.Parse(nMidi.getSubnodeByName("SecondaryChannel").content,
                    CultureInfo.InvariantCulture);
                _track.port = int.Parse(nMidi.getSubnodeByName("Port").content, CultureInfo.InvariantCulture);
            }
            else
            {
                //GP7
                _track.channel.instrument =
                    int.Parse(
                        nTrack.getSubnodeByName("Sounds").subnodes[0].getSubnodeByName("MIDI")
                            .getSubnodeByName("Program").content, CultureInfo.InvariantCulture);
                _track.channel.channel =
                    int.Parse(nTrack.getSubnodeByName("MidiConnection").getSubnodeByName("PrimaryChannel").content,
                        CultureInfo.InvariantCulture);
                _track.channel.effectChannel =
                    int.Parse(nTrack.getSubnodeByName("MidiConnection").getSubnodeByName("SecondaryChannel").content,
                        CultureInfo.InvariantCulture);
                _track.port = int.Parse(nTrack.getSubnodeByName("MidiConnection").getSubnodeByName("Port").content,
                    CultureInfo.InvariantCulture);
            }

            _track.strings = new List<GuitarString>();

            var nProperties = nTrack.getSubnodeByName("Properties");
            var nTuning = nProperties?.getSubnodeByProperty("name", "Tuning");
            if (nTuning != null)
            {
                var tuning = nTuning.subnodes[0].content.Split(' ');
                var gcnt = 0;
                foreach (var str in tuning)
                    _track.strings.Add(new GuitarString(gcnt++, int.Parse(str, CultureInfo.InvariantCulture)));
            }

            if (nProperties != null)
            {
                var nCapoFret = nProperties.getSubnodeByProperty("name", "CapoFret");
                var nFretCount = nProperties.getSubnodeByProperty("name", "FretCount");
                if (nCapoFret != null)
                    _track.offset = int.Parse(nCapoFret.subnodes[0].content, CultureInfo.InvariantCulture);

                _track.fretCount = 24;
                if (nFretCount != null)
                    _track.fretCount =
                        int.Parse(nFretCount.subnodes[0].content, CultureInfo.InvariantCulture); //Not saved anymore
                var nPropertyName = nProperties.getSubnodeByName("Name", true);
                if (nPropertyName != null)
                    _track.tuningName = nPropertyName.subnodes.Count > 0
                        ? nPropertyName.subnodes[0].content
                        : nPropertyName.content;
            }

            _track.isPercussionTrack = _track.channel.channel == 9;

            _track.settings = new TrackSettings();

            var nPlaybackState = nTrack.getSubnodeByName("PlaybackState");
            if (nPlaybackState != null)
                switch (nPlaybackState.content)
                {
                    case "Solo":
                        _track.isSolo = true;
                        break;
                    case "Mute":
                        _track.isMute = true;
                        break;
                }

            //Do not matter for me:
            //_track.indicateTuning, track.settings
            ret_val.Add(_track);
        }

        return ret_val;
    }

    public static List<MeasureHeader> transferMeasureHeaders(Node nMasterBars, GP5File song)
    {
        var ret_val = new List<MeasureHeader>();
        var cnt = 0;
        foreach (var nMasterBar in nMasterBars.subnodes)
        {
            var _measureHeader = new MeasureHeader();
            var accidentals = int.Parse(nMasterBar.getSubnodeByName("Key", true).subnodes[0].content,
                CultureInfo.InvariantCulture);
            var mode = nMasterBar.getSubnodeByName("Key", true).subnodes[1].content.Equals("Major") ? 0 : 1;
            _measureHeader.keySignature = (KeySignature)(accidentals * 10 + (accidentals < 0 ? -mode : mode));

            _measureHeader.hasDoubleBar = nMasterBar.getSubnodeByName("DoubleBar", true) != null;
            _measureHeader.direction = transferDirections(nMasterBar.getSubnodeByName("Directions", true));
            _measureHeader.fromDirection = transferFromDirections(nMasterBar.getSubnodeByName("Directions", true));
            _measureHeader.isRepeatOpen = nMasterBar.getSubnodeByName("Repeat", true) != null &&
                                          nMasterBar.getSubnodeByName("Repeat", true).propertyValues[0].Equals("true");
            _measureHeader.repeatClose = 0;
            if (nMasterBar.getSubnodeByName("Repeat", true) != null &&
                nMasterBar.getSubnodeByName("Repeat", true).propertyValues[1].Equals("true"))
                _measureHeader.repeatClose = int.Parse(nMasterBar.getSubnodeByName("Repeat", true).propertyValues[2],
                    CultureInfo.InvariantCulture);

            if (nMasterBar.getSubnodeByName("AlternateEndings", true) != null)
            {
                var _aes = nMasterBar.getSubnodeByName("AlternateEndings", true).content.Split(' ');
                foreach (var _ in _aes)
                    _measureHeader.repeatAlternatives.Add(int.Parse(_, CultureInfo.InvariantCulture));
            }

            _measureHeader.timeSignature = new TimeSignature(); //Time
            var timeSig = nMasterBar.getSubnodeByName("Time", true).content.Split('/');

            _measureHeader.timeSignature.numerator = int.Parse(timeSig[0], CultureInfo.InvariantCulture);
            _measureHeader.timeSignature.denominator = new Duration
            {
                value = int.Parse(timeSig[1], CultureInfo.InvariantCulture)
            };

            _measureHeader.tripletFeel = TripletFeel.none;
            if (nMasterBar.getSubnodeByName("TripletFeel", true) != null)
            {
                var feel = nMasterBar.getSubnodeByName("TripletFeel", true).content;
                _measureHeader.tripletFeel = feel switch
                {
                    "Triplet8th" => TripletFeel.eigth,
                    "Triplet16th" => TripletFeel.sixteenth,
                    "Dotted8th" => TripletFeel.dotted8th,
                    "Dotted16th" => TripletFeel.dotted16th,
                    "Scottish8th" => TripletFeel.scottish8th,
                    "Scottish16th" => TripletFeel.scottish16th,
                    _ => _measureHeader.tripletFeel
                };
            }

            _measureHeader.song = song;
            _measureHeader.number = cnt++;


            //Do I really need these:
            //_measureHeader.marker ? repeatGroup ?
            //_measureHeader.start ? realStart ?
            //_measureHeader.tempo - useless, as the real tempo is saved as MixTableChange on the BeatEffect

            ret_val.Add(_measureHeader);
        }

        return ret_val;
    }

    public static List<string> transferDirections(Node nDirections)
    {
        var ret_val = new List<string>();
        if (nDirections == null) return ret_val;

        ret_val.AddRange(from nElement in nDirections.subnodes
            where nElement.name.Equals("Target")
            select nElement.content);
        return ret_val;
    }

    public static List<string> transferFromDirections(Node nDirections)
    {
        var ret_val = new List<string>();
        if (nDirections == null) return ret_val;

        ret_val.AddRange(from nElement in nDirections.subnodes
            where nElement.name.Equals("Jump")
            select nElement.content);
        return ret_val;
    }

    public static List<Lyrics> transferLyrics(Node nTracks)
    {
        var ret_val = new List<Lyrics>();
        if (nTracks == null) return ret_val;

        foreach (var nTrack in nTracks.subnodes)
        {
            var nLyrics = nTrack.getSubnodeByName("Lyrics");
            var lyrics = new Lyrics();
            var cnt = 0;
            foreach (var _line in nLyrics.subnodes.Select(static nLine => new LyricLine
                     {
                         lyrics = nLine.subnodes[0].content,
                         startingMeasure = int.Parse(nLine.subnodes[1].content, CultureInfo.InvariantCulture)
                     }))
                lyrics.lines[cnt++] = _line;

            ret_val.Add(lyrics);
        }

        return ret_val;
    }

    public static Node ParseGP6(string xml, int start)
    {
        //Remove '<' chars inside CDATA tags
        var skipMode = false;
        for (var x = 0; x < xml.Length - 3; x++)
        {
            var sub = xml.Substring(x, 3);

            switch (sub)
            {
                case "<!-":
                    xml = xml.Substring(0, x) + '{' + xml.Substring(x + 1);
                    continue;
                case "<![":
                    skipMode = true;
                    continue;
                case "]]>":
                    skipMode = false;
                    break;
            }

            if (skipMode && xml[x] == '<') xml = xml.Substring(0, x) + '{' + xml.Substring(x + 1);
        }

        var split = xml.Substring(start).Split('<');
        var openTags = 0;
        var stack = new List<Node>();
        var mainNode = new Node(new List<Node>(), new List<string>(), new List<string>());
        stack.Add(mainNode);
        //Parse all Tags
        for (var x = 1; x < split.Length; x++)
        {
            if (split[x].StartsWith("/", StringComparison.Ordinal))
            {
                //Closes a tag.
                openTags--;
                stack[stack.Count - 2].subnodes.Add(stack[stack.Count - 1]);
                stack[stack.Count - 2].content = ""; //content are the subnodes
                stack.RemoveAt(stack.Count - 1);

                continue;
            }

            if (split[x].StartsWith("![", StringComparison.Ordinal))
                //normal string value encased in ![CDATA[ and ]]>
                //Already dealt with below (as content value of previous normal tag)
                continue;

            //Is normal Tag (might have parameters in tag and might be closed with />
            var endOfTag = split[x].IndexOf(">", StringComparison.Ordinal);
            if (endOfTag == -1) break; //File Error

            var sb = new StringBuilder();
            var firstSpace = split[x].IndexOf(' ');
            var firstSlash = split[x].IndexOf('/');
            if (firstSpace == -1 || firstSpace > endOfTag) firstSpace = endOfTag;
            if (firstSlash != -1 && firstSlash < firstSpace) firstSpace = firstSlash;

            var tagName = split[x].Substring(0, firstSpace);

            var pos = firstSpace;
            var isSingleTag = false;
            var collectingPropertyValue = false;
            var property = new StringBuilder();
            var propertyValue = new StringBuilder();

            var propertyNames = new List<string>();
            var propertyValues = new List<string>();
            while (pos < endOfTag)
            {
                switch (collectingPropertyValue)
                {
                    case true when split[x][pos] != '"':
                        propertyValue.Append(split[x][pos]);
                        pos++;
                        continue;
                    case true when split[x][pos] == '"':
                        collectingPropertyValue = false;
                        propertyValues.Add(propertyValue.ToString());
                        propertyValue = new StringBuilder();
                        pos++;
                        continue;
                }

                if (split[x][pos] != ' ' && split[x][pos] != '=' && split[x][pos] != '/')
                {
                    property.Append(split[x][pos]);
                    pos++;
                    continue;
                }

                if (split[x][pos] == '/')
                {
                    isSingleTag = true;
                    break;
                }

                if (split[x][pos] == '=')
                {
                    pos++;
                    propertyNames.Add(property.ToString());
                    property = new StringBuilder();
                    collectingPropertyValue = true;
                }

                pos++;
            }

            if (isSingleTag)
            {
                stack[stack.Count - 1].subnodes.Add(new Node(new List<Node>(), propertyNames, propertyValues, tagName));
                continue;
            }

            openTags++;
            //Collect values outside of tag
            var finalValue = "";
            if (x < split.Length - 1)
                finalValue = split[x + 1].StartsWith("![", StringComparison.Ordinal)
                    ? split[x + 1].Substring(8, split[x + 1].LastIndexOf("]]>", StringComparison.Ordinal) - 8)
                    : split[x].Substring(endOfTag + 1);

            stack.Add(new Node(new List<Node>(), propertyNames, propertyValues, tagName, finalValue));
        }

        return stack[0];
    }
}

//XML Classes

public sealed class GP6Chord
{
    public int forTrack;
    public int id;
    public string name = ""; //Values of this are found in Score->Properties->Property(DiagramCollection)
}

public sealed class GP6Rhythm
{
    public int augmentationDots; //0, 1 or 2
    public int id;
    public int noteValue = 4; //4 = quarter, 16 = 16th etc.
    public Tuplet primaryTuplet = new();

    public GP6Rhythm(int id, int noteValue, int augmentationDots, int n = 1, int m = 1)
    {
        this.id = id;
        this.noteValue = noteValue;
        this.augmentationDots = augmentationDots;

        primaryTuplet = new Tuplet
        {
            enters = n,
            times = m
        };
    }
}

public sealed class GP6Tempo
{
    public int bar;
    public bool linear;
    public float position; //in % of full bar
    public int tempo = 120;
    public int tempoType = 2;
    public bool transferred;
    public bool visible = true;

    public GP6Tempo(Node nAutomation) //Node with a type-subnote "Tempo"
    {
        linear = nAutomation.getSubnodeByName("Linear", true).content.Equals("true");
        bar = int.Parse(nAutomation.getSubnodeByName("Bar", true).content, CultureInfo.InvariantCulture);
        position = float.Parse(nAutomation.getSubnodeByName("Position", true).content, CultureInfo.InvariantCulture);
        visible = nAutomation.getSubnodeByName("Visible", true).content.Equals("true");
        var t = nAutomation.getSubnodeByName("Value", true).content;
        var ts = t.Split(' ');
        tempo = (int)float.Parse(ts[0], CultureInfo.InvariantCulture);
        tempoType = int.Parse(ts[1], CultureInfo.InvariantCulture);
    }
}

public sealed class Node
{
    public string content;
    public string name = "";
    public List<string> propertyNames = new();
    public List<string> propertyValues = new();
    public List<Node> subnodes = new();


    public Node(List<Node> subnodes, List<string> propertyNames,
        List<string> propertyValues, string name = "", string content = "")
    {
        this.subnodes = subnodes;
        this.propertyNames = propertyNames;
        this.propertyValues = propertyValues;
        this.content = content;
        this.name = name;
    }

    public Node getSubnodeByProperty(string propertyName, string property)
    {
        foreach (var n in subnodes)
        {
            var cnt = 0;
            var found = false;
            foreach (var pn in n.propertyNames)
            {
                if (pn.Equals(propertyName))
                {
                    found = true;
                    break;
                }

                cnt++;
            }

            if (!found) continue;

            if (n.propertyValues[cnt].Equals(property)) return n;
        }

        return null;
    }

    public Node getSubnodeByName(string name, bool directOnly = false)
    {
        if (this.name.Equals(name)) return this;

        return directOnly
            ? //Only search the direct children
            subnodes.FirstOrDefault(n => n.name.Equals(name))
            : subnodes.Select(n => n.getSubnodeByName(name)).FirstOrDefault(static sub => sub != null);
    }
}

/* //Class Structure following the xml file structure

 public class GPIF //Main node
 {
     public int gpRevision = 0; //version number?
     public Score score = new Score();
     public MasterTrack masterTrack = new MasterTrack();
     public List<Track> tracks = new List<Track>();
     public List<MasterBar> masterBars = new List<MasterBar>();
     public List<Bar> bars = new List<Bar>();
     public List<Voice> voices = new List<Voice>();
     public List<Beat> beats = new List<Beat>();
     public List<Note> notes = new List<Note>();
     public List<Rhythm> rhythms = new List<Rhythm>();
 }
 public class Rhythm
 {
     public int id = 0;
     public string noteValue = "Quarter";
     public int augmentationDots = 0;
     public int primaryTupletNum = 0;
     public int primaryTupletDen = 0;

 }
 public class Note
 {
     public int id = 0;
     public List<NoteProperty> properties = new List<NoteProperty>();
 }

 public class NoteProperty
 {
     public string name = "";
     public int str = 0;
     public int fret = 0;
 }
 public class Beat
 {
     public int id = 0;
     public string dynamic = "MF";
     public int rhythmRef = 0;
     public List<BeatProperty> properties = new List<BeatProperty>();
 }

 public class BeatProperty
 {
     //?
 }

 public class Voice
 {
     public int id = 0;
     public List<int> beats = new List<int>();
 }

 public class Bar
 {
     public int id = 0;
     public string clef = "G2";
     public List<int> voices = new List<int>();
 }

 public class MasterBar
 {
     public Key key = new Key();
     public string time = "4/4";
     public List<int> bars = new List<int>();
     public List<XProperty> xProperties = new List<XProperty>();

 }

 public class XProperty
 {
     int id = 0;
     int value = 0;
 }

 public class Key
 {
     public int accidentalCount = 0;
     public string mode = "Major";
 }

 public class Track
 {
     public int id = 0;
     public string name = "";
     public string shortName = "";
     public string color = "";
     public int systemsDefaultLayout = 3;
     public List<int> systemsLayout = new List<int>();
     public string playingStatus = "Default";
     public string instrumentRef = "";
     public PartSounding partSounding = new PartSounding();
     public TrackRSE rse = new TrackRSE();
     public GeneralMidi generalMidi = new GeneralMidi();
     public string playbackState = "Default";
     public Lyrics lyrics = new Lyrics();
     public TrackProperties trackProperties = new TrackProperties();
 }

 public class TrackProperties
 {
     List<Property> properties = new List<Property>();
 }

 public class Property
 {
     public string name = "";
     public List<int> pitches = new List<int>();
     public List<Item> items = new List<Item>();
 }

 public class Item
 {
     int stringCount = 0;
     int fretCount = 0;
 }

 public class Lyrics
 {
     public bool dispatched = true;
     public List<LyricLine> lines = new List<LyricLine>();
 }

 public class LyricLine
 {
     public string text = "";
     public int offset = 0;
 }

 public class GeneralMidi
 {
     public string table = "instrument";
     public int program = 0;
     public int port = 0;
     public int primaryChannel = 0;
     public int secondaryChannel = 1;
 }

 public class TrackRSE
 {
     public ChannelStrip channelStrip = new ChannelStrip();
     public string bank = "";
     public List<EffectChain> effectChains = new List<EffectChain>();
     public List<Pickup> pickups = new List<Pickup>();
 }

 public class Pickup
 {
     string id = "";
     int volume = 0;
     int tone = 0;
 }

 public class EffectChain
 {
     public string name = "";
     public List<Effect> rail = new List<Effect>();
     public string railName = "";
 }

 public class ChannelStrip
 {
     public string version = "";
     public List<float> parameters = new List<float>();
     public List<Automation> automations = new List<Automation>();
 }

 public class PartSounding
 {
     string nominalKey = "C";
     int transpositionPitch = -12;
 }

 public class MasterTrack
 {
     public List<int> tracks = new List<int>();
     public List<Automation> automations = new List<Automation>();
     public RSE rse = new RSE();
 }

 public class RSE
 {
     public Master master = new Master();
 }

 public class Master
 {
     public List<Effect> effect = new List<Effect>();
 }

 public class Effect
 {
     public string id = "";
     public string byPass = ""; //type?
     public List<float> parameters = new List<float>();
     public List<Automation> automations = new List<Automation>();
 }

 public class Automation
 {
     public string type = "";
     public bool linear = false;
     public int bar = 0;
     public float position = 0;
     public bool visible = true;
     public string value = "";
 }

 public class Score //contains basic infos about the file, like artist & page layout
 {
     public string title = "";
     public string subTitle = "";
     public string artist = "";
     public string album = "";
     public string words = "";
     public string music = "";
     public string wordsAndMusic = "";
     public string copyright = "";
     public string tabber = "";
     public string instructions = "";
     public string notices = "";
     public string firstPageHeader = "";
     public string firstPageFooter = "";
     public string pageHeader = "";
     public string pageFooter = "";
     public int scoreSystemsDefaultLayout = 0;
     public int scoreSystemsLayout = 0; //type?
     public PageSetup pageSetup = new PageSetup();
 }

 public class PageSetup
 {
     public int width = 0;
     public int height = 0;
     public string orientation = "Portrait";
     public int topMargin = 0;
     public int leftMargin = 0;
     public int rightMargin = 0;
     public int bottomMargin = 0;
     public float scale = 1;
 }

*/

public sealed class BitStream
{
    private static readonly int[] powers_rev = { 128, 64, 32, 16, 8, 4, 2, 1 };
    private static readonly int[] powers = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 };
    public byte[] data;
    public bool finished;
    private int pointer;
    private int subpointer;

    public BitStream(byte[] data)
    {
        this.data = data;
        pointer = 0;
        subpointer = 0;
    }

    public bool GetBit()
    {
        if (finished) return false;

        var ret_val = (data[pointer] >> (7 - subpointer)) % 2 == 1;
        increase_subpointer();
        return ret_val;
    }

    public bool[] GetBits(int amount)
    {
        var ret_val = new bool[amount];
        for (var x = 0; x < amount; x++) ret_val[x] = GetBit();
        return ret_val;
    }

    public byte GetByte()
    {
        byte ret_val = 0x00;
        for (var x = 0; x < 8; x++) ret_val |= (byte)(GetBit() ? powers_rev[x] : 0);
        return ret_val;
    }

    public int GetBitsLE(int amount)
    {
        //returns the number represented by the next n bits, starting with the least significant bit
        var ret_val = 0;

        for (var x = 0; x < amount; x++)
        {
            var val = GetBit();
            ret_val |= val ? powers[x] : 0;
        }

        return ret_val;
    }

    public int GetBitsBE(int amount)
    {
        //returns the number represented by the next n bits, starting with the most significant bit
        var ret_val = 0;

        for (var x = 0; x < amount; x++)
        {
            var val = GetBit();
            ret_val |= val ? powers[amount - x - 1] : 0;
        }

        return ret_val;
    }

    public void SkipBits(int bits)
    {
        for (var x = 0; x < bits; x++) increase_subpointer();
    }

    public void SkipBytes(int bytes)
    {
        for (var x = 0; x < bytes; x++) increase_pointer();
    }

    private void increase_pointer()
    {
        pointer++;
    }

    private void increase_subpointer()
    {
        subpointer++;
        if (subpointer == 8)
        {
            subpointer = 0;
            pointer++;
        }

        if (pointer >= data.Length) finished = true;
    }
}