/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BardMusicPlayer.Quotidian.Structs
{
    /// <summary>
    /// Represents available octave ranges.
    /// </summary>
    public readonly struct OctaveRange : IComparable, IConvertible, IComparable<OctaveRange>, IEquatable<OctaveRange>
    {
        public static readonly OctaveRange Invalid = new("Invalid", -1, -1, -1, null);

        public static readonly OctaveRange C0toC3 = new("C0toC3", 0, 0, 3, "+3");
        public static readonly OctaveRange C1toC4 = new("C1toC4", 1, 1, 4, "+2");
        public static readonly OctaveRange C2toC5 = new("C2toC5", 2, 2, 5, "+1");
        public static readonly OctaveRange C3toC6 = new("C3toC6", 3, 3, 6, "");
        public static readonly OctaveRange C4toC7 = new("C4toC7", 4, 4, 7, "-1");
        public static readonly OctaveRange C5toC8 = new("C5toC8", 5, 5, 8, "-2");
        public static readonly OctaveRange C6toC9 = new("C6toC9", 6, 6, 9, "-3");

        public static readonly IReadOnlyList<OctaveRange> All = new ReadOnlyCollection<OctaveRange>(new List<OctaveRange> { C0toC3, C1toC4, C2toC5, C3toC6, C4toC7, C5toC8, C6toC9 });

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; }

        /// <summary>
        /// Gets the lowest octave.
        /// </summary>
        /// <value>The lowest octave.</value>
        public int Lower { get; }

        /// <summary>
        /// Gets the highest octave.
        /// </summary>
        /// <value>The highest octave.</value>
        public int Upper { get; }

        /// <summary>
        /// Gets the lowest note.
        /// </summary>
        /// <value>The lowest note.</value>
        public int LowerNote => (Lower + 1) * 12;

        /// <summary>
        /// Gets the highest note.
        /// </summary>
        /// <value>The highest note.</value>
        public int UpperNote => (Upper + 1) * 12;

        /// <summary>
        /// Gets the offset used in midi track names.
        /// </summary>
        public string TrackNameOffset { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctaveRange"/> struct.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="index"></param>
        /// <param name="lower">Lower Octave.</param>
        /// <param name="upper">Upper Octave.</param>
        /// <param name="trackNameOffset">Track Name offset</param>
        private OctaveRange(string name, int index, int lower, int upper, string trackNameOffset)
        {
            Name = name;
            Index = index;
            Lower = lower;
            Upper = upper;
            TrackNameOffset = trackNameOffset;
        }

        /// <summary>
        /// Determines whether the specified <see cref="OctaveRange"/> is equal to the
        /// current <see cref="OctaveRange"/>.
        /// </summary>
        /// <param name="other">The <see cref="OctaveRange"/> to compare with the current <see cref="OctaveRange"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="OctaveRange"/> is equal to the current
        /// <see cref="OctaveRange"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(OctaveRange other) => Index == other;

        bool IEquatable<OctaveRange>.Equals(OctaveRange other) => Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="OctaveRange"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="OctaveRange"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="OctaveRange"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is OctaveRange octaveRange && Equals(octaveRange);

        /// <summary>
        /// Serves as a hash function for a <see cref="OctaveRange"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => (Name, Index, Lower, Upper, LowerNote, UpperNote, TrackNameOffset).GetHashCode();

        public static implicit operator string(OctaveRange octaveRange) => octaveRange.Name;
        public static implicit operator OctaveRange(string name) => Parse(name);
        public static implicit operator int(OctaveRange octaveRange) => octaveRange.Index;
        public static implicit operator OctaveRange(int lower) => Parse(lower);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (!(obj is OctaveRange)) throw new ArgumentException("This is not an OctaveRange");
            return Index - ((OctaveRange) obj).Index;
        }

        public int CompareTo(OctaveRange other) => Index - other.Index;
        public TypeCode GetTypeCode() => TypeCode.Int32;
        public bool ToBoolean(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from OctaveRange to Boolean");
        public char ToChar(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from OctaveRange to Char");
        public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Index);
        public byte ToByte(IFormatProvider provider) => Convert.ToByte(Index);
        public short ToInt16(IFormatProvider provider) => Convert.ToInt16(Index);
        public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Index);
        public int ToInt32(IFormatProvider provider) => Convert.ToInt32(Index);
        public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Index);
        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(Index);
        public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Index);
        public float ToSingle(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from OctaveRange to Single");
        public double ToDouble(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from OctaveRange to Double");
        public decimal ToDecimal(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from OctaveRange to Decimal");
        public DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from OctaveRange to DateTime");
        public string ToString(IFormatProvider provider) => Index.ToString();
        public override string ToString() => Index.ToString();
        public object ToType(Type conversionType, IFormatProvider provider) => throw new InvalidCastException("Invalid cast from OctaveRange to " + conversionType);

        /// <summary>
        /// Get's a string with the instrument plus this octave range for use as a midi track name
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public string GetTrackName(Instrument instrument) => instrument.Name + TrackNameOffset;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octaveRange"></param>
        /// <returns></returns>
        public static OctaveRange Parse(int octaveRange)
        {
            TryParse(octaveRange.ToString(), out var result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octaveRange"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(int octaveRange, out OctaveRange result)
        {
            var success = TryParse(octaveRange.ToString(), out var innerResult);
            result = innerResult;
            return success;
        }

        /// <summary>
        /// Gets the octaveRange from a string.
        /// </summary>
        /// <param name="octaveRange">The string with the name of the octaveRange</param>
        /// <returns>The <see cref="OctaveRange"/>, or <see cref="Invalid"/> if invalid.</returns>
        public static OctaveRange Parse(string octaveRange)
        {
            TryParse(octaveRange, out var result);
            return result;
        }

        /// <summary>
        /// Tries to get the octaveRange from a string.
        /// </summary>
        /// <param name="octaveRange">The string with the name of the octaveRange</param>
        /// <param name="result">The <see cref="OctaveRange"/>, or <see cref="Invalid"/> if invalid.</param>
        /// <returns>true if the <see cref="OctaveRange"/> is anything besides <see cref="Invalid"/></returns>
        public static bool TryParse(string octaveRange, out OctaveRange result)
        {
            if (octaveRange is null)
            {
                result = Invalid;
                return false;
            }
            octaveRange = octaveRange.Replace(" ", "").Replace("_", "");
            if (All.Any(x => x.Name.Equals(octaveRange, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = All.First(x => x.Name.Equals(octaveRange, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            if (All.Any(x => x.TrackNameOffset.Equals(octaveRange, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = All.First(x => x.TrackNameOffset.Equals(octaveRange, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            if (All.Any(x => x.Index.ToString().Equals(octaveRange, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = All.First(x => x.Index.ToString().Equals(octaveRange, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            result = Invalid;
            return false;
        }

        /// <summary>
        /// Validates this note is in this octave range
        /// </summary>
        /// <param name="note">The note</param>
        /// <returns></returns>
        public bool ValidateNoteRange(int note) => note >= LowerNote && note <= UpperNote;

        /// <summary>
        /// Gets the new note value for a note that needs to move to a different base octave.
        /// </summary>
        /// <param name="currentOctaveRange">The current octave range this note is in</param>
        /// <param name="note">The note number</param>
        /// <returns>True, if this note was in range to be moved, else false.</returns>
        public bool TryShiftNoteToOctave(OctaveRange currentOctaveRange, ref int note)
        {
            if (!currentOctaveRange.ValidateNoteRange(note)) return false;
            note += LowerNote - currentOctaveRange.LowerNote;
            return true;
        }

        public int ShiftNoteToOctave(OctaveRange currentOctaveRange, int note)
        {
            if (!currentOctaveRange.ValidateNoteRange(note)) throw new BmpException("ShiftNoteToOctave note " + note + " not in range of " + currentOctaveRange.Name);
            return note + LowerNote - currentOctaveRange.LowerNote;
        }
    }
}