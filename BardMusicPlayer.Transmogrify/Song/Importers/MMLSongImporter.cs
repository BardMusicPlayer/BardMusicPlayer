#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BardMusicPlayer.Quotidian.Structs;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers;

public enum Chap
{
    NULL = 0,
    SETTINGS = 1,
    CHANNEL = 2,
    FINISHED = 3
}

public static class MMLSongImporter
{
    private static readonly Dictionary<int, List<MMLCommand>> musicData = new();
    private static readonly Dictionary<int, string> musicInstrument = new();

    private static readonly string mmlPatterns =
        @"[tT]\d{1,3}|[lL](16|2|4|8|1|32|64)\.?|[vV]\d+|[oO]\d|<|>|[a-gA-G](\+|#|-)?(16|2|4|8|1|32|64)?\.?|[rR](16|2|4|8|1|32|64)?\.?|[nN]\d+\.?|&";

    private static readonly Dictionary<string, int> noteNumbers = new()
    {
        { "c", 0 }, { "c.", 1 }, { "d", 2 }, { "d.", 3 }, { "e", 4 }, { "f", 5 }, { "f.", 6 }, { "g", 7 },
        { "g.", 8 }, { "a", 9 }, { "a.", 10 }, { "b", 11 }, { "b.", 12 }
    };

    private static Chap _chap { get; set; } = Chap.NULL;

    private static string Title { get; set; } = "None";
    private static int _Tempo { get; set; } = 120 + 16; //Tempo + 33/2
    private static double _Length { get; set; } = 4; //length of a note
    private static int _Octave { get; set; } = 60; //The octave in midi notes


    private static int CurrentChannel { get; set; }

    /// <summary>
    ///     Opens and process a mmslong file
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static MidiFile OpenMMLSongFile(string path)
    {
        if (!File.Exists(path)) throw new BmpTransmogrifyException("File " + path + " does not exist!");

        var rfile = new StreamReader(path);
        var musicdata = "";
        while (rfile.Peek() >= 0)
        {
            var line = rfile.ReadLine();
            if (line.Contains("[Settings]")) _chap = Chap.SETTINGS;

            if (line.StartsWith("[Channel", StringComparison.Ordinal))
            {
                if (musicdata.Length > 0)
                {
                    var matches = Regex.Matches(StripComments(musicdata), mmlPatterns);
                    for (var i = 0; i < matches.Count; ++i)
                        musicData[CurrentChannel].Add(MMLCommand.Parse(matches[i].Value));
                    musicdata = "";
                }

                _chap = Chap.CHANNEL;
                CurrentChannel++;
                musicData.Add(CurrentChannel, new List<MMLCommand>());
                Console.WriteLine(CurrentChannel);
                continue;
            }

            if (line.Contains("[3MLE EXTENSION]"))
            {
                if (musicdata.Length > 0)
                {
                    var matches = Regex.Matches(StripComments(musicdata), mmlPatterns);
                    for (var i = 0; i < matches.Count; ++i)
                        musicData[CurrentChannel].Add(MMLCommand.Parse(matches[i].Value));
                    musicdata = "";
                }

                _chap = Chap.FINISHED;
            }

            switch (_chap)
            {
                case Chap.SETTINGS:
                {
                    if (line.Split('=')[0] == "Title") Title = line.Split('=')[1];

                    break;
                }
                case Chap.CHANNEL when line.StartsWith("//", StringComparison.Ordinal):
                {
                    var result = Instrument.Parse(StripComments(line));
                    if (result.Index != 0) musicInstrument[CurrentChannel] = result.Name;

                    continue;
                }
                case Chap.CHANNEL:
                    musicdata += StripComments(line);
                    break;
            }
        }

        return CreateMidi();
    }

    private static string StripComments(string code)
    {
        if (code.StartsWith("/*", StringComparison.Ordinal))
        {
            var idx = code.IndexOf("*/", StringComparison.Ordinal);
            code = code.Substring(idx + 2);
        }

        if (code.StartsWith("//", StringComparison.Ordinal))
        {
            var idx = code.IndexOf("//", StringComparison.Ordinal);
            code = code.Substring(idx + 2);
        }

        code = string.Concat(code.Where(static c => !char.IsWhiteSpace(c)));
        return code;
    }

    private static MidiFile CreateMidi()
    {
        var midiFile = new MidiFile();
        foreach (var t in musicData)
        {
            double duration = 0;

            _Octave = 60;
            _Length = 0;
            musicInstrument.TryGetValue(t.Key, out var instrument);
            instrument ??= "Piano";

            var thisTrack = new TrackChunk(new SequenceTrackNameEvent(instrument));
            var result = Instrument.Parse(instrument);
            for (var i = 0; i < t.Value.Count;)
            {
                var cmd = t.Value[i];
                switch (cmd.Type)
                {
                    case MMLCommandType.Tempo:
                        _Tempo = Convert.ToInt32(cmd.Args[0]);
                        if (_Length == 0) _Length = GetLength("4");

                        break;
                    case MMLCommandType.Octave:
                        _Octave = Convert.ToInt32(cmd.Args[0]) * 12 + 12;
                        break;
                    case MMLCommandType.OctaveDown:
                        _Octave -= 12;
                        break;
                    case MMLCommandType.OctaveUp:
                        _Octave += 12;
                        break;
                    case MMLCommandType.Tie:
                        break;
                    case MMLCommandType.Length:
                        _Length = GetLength(cmd.Args[0], cmd.Args[1] == ".");
                        break;
                    case MMLCommandType.Rest:
                        duration += GetLength(cmd.Args[0], cmd.Args[1] == ".");
                        break;
                    case MMLCommandType.NoteNumber:
                        var number = Convert.ToInt32(cmd.Args[0]);
                        if (number == 0)
                        {
                            duration += GetLength("", cmd.Args[1] == ".");
                            break;
                        }

                        SetNoteOn(thisTrack, duration, (SevenBitNumber)(number + 12));
                        duration += GetLength("", cmd.Args[1] == ".");

                        if (i + 1 != t.Value.Count)
                        {
                            var nextcmd = t.Value[i + 1];
                            if (nextcmd.Type == MMLCommandType.Tie)
                                break;
                        }

                        SetNoteOff(thisTrack, duration, (SevenBitNumber)(number + 12));

                        break;
                    case MMLCommandType.Note:
                        var nnumber = GetNote(cmd);
                        SetNoteOn(thisTrack, duration, (SevenBitNumber)nnumber);
                        duration += GetLength(cmd.Args[2], cmd.Args[3] == ".");

                        if (i + 1 != t.Value.Count)
                        {
                            var nextcmd = t.Value[i + 1];
                            if (nextcmd.Type == MMLCommandType.Tie)
                                break;
                        }

                        SetNoteOff(thisTrack, duration, (SevenBitNumber)nnumber);
                        break;
                }

                i++;
            }

            midiFile.Chunks.Add(thisTrack);
        }

        midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision((short)(60000 / _Tempo));
        midiFile.ReplaceTempoMap(TempoMap.Create(Tempo.FromBeatsPerMinute(_Tempo)));

        musicData.Clear();
        return midiFile;
    }

    private static double GetLength(string multiplier, bool dottet = false)
    {
        double length = Tempo.FromBeatsPerMinute(_Tempo).MicrosecondsPerQuarterNote;
        if (multiplier == "")
        {
            length = _Length;
            if (dottet)
                return length * 1.5;

            return length;
        }

        var length_multiplier = Convert.ToInt32(multiplier);
        if (dottet)
            return length / length_multiplier * 1.5;

        return length / length_multiplier;
    }

    private static int GetNote(MMLCommand cmd)
    {
        var noteType = cmd.Args[0].ToLower();
        var noteNum = 0;
        switch (cmd.Args[1])
        {
            case "#":
            case "+":
                noteNum = noteNumbers[noteType + "."];
                break;
            case "-":
                noteNum = noteNumbers[noteType];
                break;
            default:
                noteNum = noteNumbers[noteType];
                break;
        }

        return noteNum + _Octave;
    }

    private static void SetNoteOn(TrackChunk track, double duration, SevenBitNumber noteNumber)
    {
        using var manager = new TimedEventsManager(track.Events);
        var timedEvents = manager.Events;
        timedEvents.Add(new TimedEvent(new NoteOnEvent(noteNumber, (SevenBitNumber)127),
            (long)duration / 1000));
    }

    private static void SetNoteOff(TrackChunk track, double duration, SevenBitNumber noteNumber)
    {
        using var manager = new TimedEventsManager(track.Events);
        var timedEvents = manager.Events;
        timedEvents.Add(
            new TimedEvent(new NoteOffEvent(noteNumber, (SevenBitNumber)127), (long)duration / 1000));
    }
}