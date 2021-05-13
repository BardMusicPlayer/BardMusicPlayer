/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BardMusicPlayer.Common.Structs
{
    /// <summary>
    /// Represents available autotone octave ranges.
    /// </summary>
    public readonly struct AutoToneOctaveRange : IComparable, IConvertible, IComparable<AutoToneOctaveRange>, IEquatable<AutoToneOctaveRange>
    {
        public static readonly AutoToneOctaveRange Invalid = new("Invalid", -1, -1, -1, null);

        public static readonly AutoToneOctaveRange C0toC5 = new("C0toC5", 0, 0, 5, "+2");
        public static readonly AutoToneOctaveRange C1toC6 = new("C1toC6", 1, 1, 6, "+1");
        public static readonly AutoToneOctaveRange C2toC7 = new("C2toC7", 2, 2, 7, "");
        public static readonly AutoToneOctaveRange C3toC8 = new("C3toC8", 3, 3, 8, "-1");
        public static readonly AutoToneOctaveRange C4toC9 = new("C4toC9", 4, 4, 9, "-2");

        public static readonly IReadOnlyList<AutoToneOctaveRange> All = new ReadOnlyCollection<AutoToneOctaveRange>(new List<AutoToneOctaveRange> { C0toC5, C1toC6, C2toC7, C3toC8, C4toC9 });

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
        /// Initializes a new instance of the <see cref="AutoToneOctaveRange"/> struct.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="index"></param>
        /// <param name="lower">Lower Octave.</param>
        /// <param name="upper">Upper Octave.</param>
        /// <param name="trackNameOffset">Track Name offset</param>
        private AutoToneOctaveRange(string name, int index, int lower, int upper, string trackNameOffset)
        {
            Name = name;
            Index = index;
            Lower = lower;
            Upper = upper;
            TrackNameOffset = trackNameOffset;
        }

        /// <summary>
        /// Determines whether the specified <see cref="AutoToneOctaveRange"/> is equal to the
        /// current <see cref="AutoToneOctaveRange"/>.
        /// </summary>
        /// <param name="other">The <see cref="AutoToneOctaveRange"/> to compare with the current <see cref="AutoToneOctaveRange"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="AutoToneOctaveRange"/> is equal to the current
        /// <see cref="AutoToneOctaveRange"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(AutoToneOctaveRange other) => Index == other;

        bool IEquatable<AutoToneOctaveRange>.Equals(AutoToneOctaveRange other) => Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="AutoToneOctaveRange"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="AutoToneOctaveRange"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="AutoToneOctaveRange"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is AutoToneOctaveRange octaveRange && Equals(octaveRange);

        /// <summary>
        /// Serves as a hash function for a <see cref="AutoToneOctaveRange"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => (Name, Index, Lower, Upper, LowerNote, UpperNote, TrackNameOffset).GetHashCode();

        public static implicit operator string(AutoToneOctaveRange octaveRange) => octaveRange.Name;
        public static implicit operator AutoToneOctaveRange(string name) => Parse(name);
        public static implicit operator int(AutoToneOctaveRange octaveRange) => octaveRange.Index;
        public static implicit operator AutoToneOctaveRange(int lower) => Parse(lower);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (!(obj is AutoToneOctaveRange)) throw new ArgumentException("This is not an AutoToneOctaveRange");
            return Index - ((AutoToneOctaveRange) obj).Index;
        }

        public int CompareTo(AutoToneOctaveRange other) => Index - other.Index;
        public TypeCode GetTypeCode() => TypeCode.Int32;
        public bool ToBoolean(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneOctaveRange to Boolean");
        public char ToChar(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneOctaveRange to Char");
        public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Index);
        public byte ToByte(IFormatProvider provider) => Convert.ToByte(Index);
        public short ToInt16(IFormatProvider provider) => Convert.ToInt16(Index);
        public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Index);
        public int ToInt32(IFormatProvider provider) => Convert.ToInt32(Index);
        public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Index);
        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(Index);
        public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Index);
        public float ToSingle(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneOctaveRange to Single");
        public double ToDouble(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneOctaveRange to Double");
        public decimal ToDecimal(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneOctaveRange to Decimal");
        public DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneOctaveRange to DateTime");
        public string ToString(IFormatProvider provider) => Index.ToString();
        public override string ToString() => Index.ToString();
        public object ToType(Type conversionType, IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneOctaveRange to " + conversionType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoToneOctaveRange"></param>
        /// <returns></returns>
        public static AutoToneOctaveRange Parse(int autoToneOctaveRange)
        {
            TryParse(autoToneOctaveRange.ToString(), out var result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoToneOctaveRange"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(int autoToneOctaveRange, out AutoToneOctaveRange result)
        {
            var success = TryParse(autoToneOctaveRange.ToString(), out var innerResult);
            result = innerResult;
            return success;
        }

        /// <summary>
        /// Gets the autoToneOctaveRange from a string.
        /// </summary>
        /// <param name="autoToneOctaveRange">The string with the name of the autoToneOctaveRange</param>
        /// <returns>The <see cref="AutoToneOctaveRange"/>, or <see cref="Invalid"/> if invalid.</returns>
        public static AutoToneOctaveRange Parse(string autoToneOctaveRange)
        {
            TryParse(autoToneOctaveRange, out var result);
            return result;
        }

        /// <summary>
        /// Tries to get the autoToneOctaveRange from a string.
        /// </summary>
        /// <param name="autoToneOctaveRange">The string with the name of the autoToneOctaveRange</param>
        /// <param name="result">The <see cref="AutoToneOctaveRange"/>, or <see cref="Invalid"/> if invalid.</param>
        /// <returns>true if the <see cref="AutoToneOctaveRange"/> is anything besides <see cref="Invalid"/></returns>
        public static bool TryParse(string autoToneOctaveRange, out AutoToneOctaveRange result)
        {
            if (autoToneOctaveRange is null)
            {
                result = Invalid;
                return false;
            }
            autoToneOctaveRange = autoToneOctaveRange.Replace(" ", "").Replace("_", "");
            if (All.Any(x => x.Name.Equals(autoToneOctaveRange, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = All.First(x => x.Name.Equals(autoToneOctaveRange, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            if (All.Any(x => x.TrackNameOffset.Equals(autoToneOctaveRange, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = All.First(x => x.TrackNameOffset.Equals(autoToneOctaveRange, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            if (All.Any(x => x.Index.ToString().Equals(autoToneOctaveRange, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = All.First(x => x.Index.ToString().Equals(autoToneOctaveRange, StringComparison.CurrentCultureIgnoreCase));
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
        public bool TryShiftNoteToOctave(AutoToneOctaveRange currentOctaveRange, ref int note)
        {
            if (!currentOctaveRange.ValidateNoteRange(note)) return false;
            note += LowerNote - currentOctaveRange.LowerNote;
            return true;
        }

        public int ShiftNoteToOctave(AutoToneOctaveRange currentOctaveRange, int note)
        {
            if (!currentOctaveRange.ValidateNoteRange(note)) throw new BmpException("ShiftNoteToOctave note " + note + " not in range of " + currentOctaveRange.Name);
            return note + LowerNote - currentOctaveRange.LowerNote;
        }
    }
}