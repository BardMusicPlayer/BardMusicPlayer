/*
 * MogLib/Common/Structs/OctaveRange.cs
 *
 * Copyright (C) 2021  MoogleTroupe
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;

namespace BardMusicPlayer.Common.Structs
{
    /// <summary>
    /// Represents available octave ranges.
    /// </summary>
    public readonly struct OctaveRange : IComparable, IConvertible, IComparable<OctaveRange>, IEquatable<OctaveRange>
    {
        public static OctaveRange VSTRange { get; } = new("VST", -3,-3, -3, 121, 127, "");

        public static OctaveRange Invalid { get; } = new("Invalid", -2, -2, -2, -1, -1, "");
        public static OctaveRange Mapper { get; } = new("Mapper", -1, -1, 2, 0, 36, "+4");
        public static OctaveRange C0toC3 { get; } = new("C0toC3", 0, 0, 3, 12, 48, "+3");
        public static OctaveRange C1toC4 { get; } = new("C1toC4", 1, 1, 4, 24, 60, "+2");
        public static OctaveRange C2toC5 { get; } = new("C2toC5", 2, 2, 5, 36, 72, "+1");
        public static OctaveRange C3toC6 { get; } = new("C3toC6", 3, 3, 6, 48, 84, "");
        public static OctaveRange C4toC7 { get; } = new("C4toC7", 4, 4, 7, 60, 96, "-1");
        public static OctaveRange C5toC8 { get; } = new("C5toC8", 5, 5, 8, 72, 108, "-2");
        public static OctaveRange C6toC9 { get; } = new("C6toC9", 6, 6, 9, 84, 120, "-3");
        
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
        public int LowerNote { get; }

        /// <summary>
        /// Gets the highest note.
        /// </summary>
        /// <value>The highest note.</value>
        public int UpperNote { get; }

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
        /// <param name="lowerNote">Lowest possible note.</param>
        /// <param name="upperNote">Highest possible note.</param>
        /// <param name="trackNameOffset">Track Name offset</param>
        private OctaveRange(string name, int index, int lower, int upper, int lowerNote, int upperNote, string trackNameOffset)
        {
            Name = name;
            Index = index;
            Lower = lower;
            Upper = upper;
            LowerNote = lowerNote;
            UpperNote = upperNote;
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
        public static OctaveRange Parse(int octaveRange) => Parse(octaveRange.ToString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octaveRange"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(int octaveRange, out OctaveRange result) => TryParse(octaveRange.ToString(), out result);

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
            if (octaveRange == null)
            {
                result = Invalid;
                return false;
            }
            octaveRange = octaveRange.ToLower().Trim().Replace(" ", string.Empty).Replace("_", string.Empty);

            switch (octaveRange)
            {
                case "+4":
                case "mapper":
                    result = Mapper;
                    return true;
                case "+3":
                case "0":
                case "c0":
                case "c0toc3":
                    result = C0toC3;
                    return true;
                case "+2":
                case "1":
                case "c1":
                case "c1toc4":
                    result = C1toC4;
                    return true;
                case "+1":
                case "2":
                case "c2":
                case "c2toc5":
                    result = C2toC5;
                    return true;
                case "":
                case "+0":
                case "-0":
                case "3":
                case "c3":
                case "c3toc6":
                    result = C3toC6;
                    return true;
                case "-1":
                case "4":
                case "c4":
                case "c4toc7":
                    result = C4toC7;
                    return true;
                case "-2":
                case "5":
                case "c5":
                case "c5toc8":
                    result = C5toC8;
                    return true;
                case "-3":
                case "6":
                case "c6":
                case "c6toc9":
                    result = C6toC9;
                    return true;
                default: 
                    result = Invalid;
                    return false;
            }
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
    }
}