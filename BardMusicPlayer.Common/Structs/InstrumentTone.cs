/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BardMusicPlayer.Common.Structs
{
    /// <summary>
    /// Represents available instrument tone groups in game.
    /// </summary>
    public readonly struct InstrumentTone : IComparable, IConvertible, IComparable<InstrumentTone>, IEquatable<InstrumentTone>
    {
        public static readonly InstrumentTone None = new("None", 0, new List<Instrument>{});

        public static readonly InstrumentTone Strummed = new("Strummed", 1, new List<Instrument>{Instrument.Harp, Instrument.Piano, Instrument.Lute, Instrument.Fiddle});
        public static readonly InstrumentTone Wind = new("Wind", 2, new List<Instrument>{Instrument.Flute, Instrument.Oboe, Instrument.Clarinet, Instrument.Fife, Instrument.Panpipes });
        public static readonly InstrumentTone Drums = new("Drums", 3, new List<Instrument>{Instrument.Timpani, Instrument.Bongo, Instrument.BassDrum, Instrument.SnareDrum, Instrument.Cymbal});
        public static readonly InstrumentTone Brass = new("Brass", 4, new List<Instrument>{Instrument.Trumpet, Instrument.Trombone, Instrument.Tuba, Instrument.Horn, Instrument.Saxophone});
        public static readonly InstrumentTone Strings = new("Strings", 5, new List<Instrument>{Instrument.Violin, Instrument.Viola, Instrument.Cello, Instrument.DoubleBass});
        public static readonly InstrumentTone SomethingNew = new("SomethingNew", 6, new List<Instrument>{});

        public static IReadOnlyList<InstrumentTone> All { get; } = new ReadOnlyCollection<InstrumentTone>(new List<InstrumentTone> { Strummed, Wind, Drums, Brass, Strings, SomethingNew });

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

        public IReadOnlyList<Instrument> Instruments { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentTone"/> struct.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="index">Index.</param>
        /// <param name="instruments"></param>
        private InstrumentTone(string name, int index, IReadOnlyList<Instrument> instruments)
        {
            Name = name;
            Index = index;
            Instruments = instruments;
        }

        /// <summary>
        /// Determines whether the specified <see cref="InstrumentTone"/> is equal to the
        /// current <see cref="InstrumentTone"/>.
        /// </summary>
        /// <param name="other">The <see cref="InstrumentTone"/> to compare with the current <see cref="InstrumentTone"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="InstrumentTone"/> is equal to the current
        /// <see cref="InstrumentTone"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(InstrumentTone other) => Index == other;

        bool IEquatable<InstrumentTone>.Equals(InstrumentTone other) => Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="InstrumentTone"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="InstrumentTone"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="InstrumentTone"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is InstrumentTone instrumentTone && Equals(instrumentTone);

        /// <summary>
        /// Serves as a hash function for a <see cref="Instrument"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => (Name, Index).GetHashCode();
        
        public static implicit operator string(InstrumentTone instrumentTone) => instrumentTone.Name;
        public static implicit operator InstrumentTone(string name) => Parse(name);
        public static implicit operator int(InstrumentTone instrumentTone) => instrumentTone.Index;
        public static implicit operator InstrumentTone(int index) => Parse(index);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (!(obj is Instrument)) throw new ArgumentException("This is not an InstrumentTone");
            return Index - ((Instrument) obj).Index;
        }

        public int CompareTo(InstrumentTone other) => Index - other.Index;
        public TypeCode GetTypeCode() => TypeCode.Int32;
        public bool ToBoolean(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from InstrumentTone to Boolean");
        public char ToChar(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from InstrumentTone to Char");
        public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Index);
        public byte ToByte(IFormatProvider provider) => Convert.ToByte(Index);
        public short ToInt16(IFormatProvider provider) => Convert.ToInt16(Index);
        public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Index);
        public int ToInt32(IFormatProvider provider) => Convert.ToInt32(Index);
        public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Index);
        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(Index);
        public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Index);
        public float ToSingle(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from InstrumentTone to Single");
        public double ToDouble(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from InstrumentTone to Double");
        public decimal ToDecimal(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from InstrumentTone to Decimal");
        public DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from InstrumentTone to DateTime");
        public string ToString(IFormatProvider provider) => Name;
        public override string ToString() => Name;
        public object ToType(Type conversionType, IFormatProvider provider) => throw new InvalidCastException("Invalid cast from InstrumentTone to " + conversionType);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrumentTone"></param>
        /// <returns></returns>
        public static InstrumentTone Parse(int instrumentTone)
        {
            TryParse(instrumentTone, out var result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrumentTone"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(int instrumentTone, out InstrumentTone result)
        {
            switch (instrumentTone)
            {
                case 1:
                    result = Strummed;
                    return true;
                case 2:
                    result = Wind;
                    return true;
                case 3:
                    result = Drums;
                    return true;
                case 4:
                    result = Brass;
                    return true;
                case 5:
                    result = Strings;
                    return true;
                case 6:
                    result = SomethingNew;
                    return true;
                default:
                    result = None;
                    return false;
            }
        }

        public static InstrumentTone Parse(string instrumentTone)
        {
            TryParse(instrumentTone, out var result);
            return result;
        }

        public static bool TryParse(string instrumentTone, out InstrumentTone result)
        {
            if (int.TryParse(instrumentTone, out var number) && number >= Strummed && number <= SomethingNew) return TryParse(number, out result);

            instrumentTone = instrumentTone.ToLower().Trim().Replace(" ", string.Empty).Replace("_", string.Empty);

            switch (instrumentTone)
            {
                case "strummed":
                    result = Strummed;
                    return true;
                case "wind":
                    result = Wind;
                    return true;
                case "drums":
                    result = Drums;
                    return true;
                case "brass":
                    result = Brass;
                    return true;
                case "strings":
                    result = Strings;
                    return true;
                case "somethingnew":
                    result = SomethingNew;
                    return true;
                default:
                    result = None;
                    return false;
            }
        }
    }
}