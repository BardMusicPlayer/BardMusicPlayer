#region

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace BardMusicPlayer.Transmogrify.Song.Importers;

public enum MMLCommandType
{
    Tempo,
    Length,
    Volume,
    Octave,
    OctaveDown,
    OctaveUp,
    Note,
    Rest,
    NoteNumber,
    Tie,
    Unknown
}

/// <summary>
///     An MML command, including its type and arguments. Corresponds to a single MML token such as "T120" or "C8".
/// </summary>
public struct MMLCommand
{
    public MMLCommandType Type;
    public List<string> Args;

    public static MMLCommand Parse(string token)
    {
        var cmd = new MMLCommand();
        var t = MMLCommandType.Unknown;
        var args = new List<string>();

        switch (token.ToLowerInvariant()[0])
        {
            case 't':
                t = MMLCommandType.Tempo;
                AddPart(args, token, @"\d{1,3}");
                break;
            case 'l':
                t = MMLCommandType.Length;
                AddPart(args, token, @"(16|2|4|8|1|32|64)");
                AddPart(args, token, @"\.");
                break;
            case 'v':
                t = MMLCommandType.Volume;
                AddPart(args, token, @"\d+");
                break;
            case 'o':
                t = MMLCommandType.Octave;
                AddPart(args, token, @"\d");
                break;
            case '<':
                t = MMLCommandType.OctaveDown;
                break;
            case '>':
                t = MMLCommandType.OctaveUp;
                break;
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'g':
                t = MMLCommandType.Note;
                AddPart(args, token, @"[a-gA-G]");
                AddPart(args, token, @"(\+|#|-)");
                AddPart(args, token, @"(16|2|4|8|1|32|64)");
                AddPart(args, token, @"\.");
                break;
            case 'r':
                t = MMLCommandType.Rest;
                AddPart(args, token, @"(16|2|4|8|1|32|64)");
                AddPart(args, token, @"\.");
                break;
            case 'n':
                t = MMLCommandType.NoteNumber;
                AddPart(args, token, @"\d+");
                AddPart(args, token, @"\.");
                break;
            case '&':
                t = MMLCommandType.Tie;
                break;
            default:
                t = MMLCommandType.Unknown;
                args.Add(token);
                break;
        }

        cmd.Type = t;
        cmd.Args = args;

        return cmd;
    }

    private static void AddPart(ICollection<string> args, string token, string pattern)
    {
        var s = Regex.Match(token, pattern).Value;
        args.Add(s);
    }
}

public struct MMLLength
{
    public int Length;
    public bool Dotted;

    public MMLLength(int length, bool dotted)
    {
        Length = length;
        Dotted = dotted;
    }

    public readonly TimeSpan ToTimeSpan(double secondsPerMeasure)
    {
        var length = 1.0 / Length;
        if (Dotted) length *= 1.5;

        return new TimeSpan((long)(secondsPerMeasure * length * TimeSpan.TicksPerSecond));
    }
}