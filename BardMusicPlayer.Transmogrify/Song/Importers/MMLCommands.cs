#region License
// The MIT License (MIT)
// 
// Copyright (c) 2014 Emma 'Eniko' Maassen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BardMusicPlayer.Transmogrify.Song.Importers
{
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
    /// An MML command, including its type and arguments. Corresponds to a single MML token such as "T120" or "C8".
    /// </summary>
    public struct MMLCommand
    {
        public MMLCommandType Type;
        public List<string> Args;

        public static MMLCommand Parse(string token)
        {
            MMLCommand cmd = new MMLCommand();
            MMLCommandType t = MMLCommandType.Unknown;
            List<string> args = new List<string>();

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

        private static void AddPart(List<string> args, string token, string pattern)
        {
            string s = Regex.Match(token, pattern).Value;
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

        public TimeSpan ToTimeSpan(double secondsPerMeasure)
        {
            double length = 1.0 / (double)Length;
            if (Dotted)
                length *= 1.5;

            return new TimeSpan((long)(secondsPerMeasure * length * TimeSpan.TicksPerSecond));
        }
    }
}
