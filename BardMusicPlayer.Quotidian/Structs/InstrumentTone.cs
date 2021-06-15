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
    /// Represents available instrument tone groups in game.
    /// </summary>
    public readonly struct InstrumentTone : IComparable, IConvertible, IComparable<InstrumentTone>, IEquatable<InstrumentTone>
    {
        public static readonly InstrumentTone None = new("None", 0, Instrument.None, Instrument.None,Instrument.None, Instrument.None, Instrument.None);

        public static readonly InstrumentTone Strummed = new("Strummed", 1,  Instrument.Harp,Instrument.Piano,Instrument.Lute,Instrument.Fiddle, Instrument.None);
        public static readonly InstrumentTone Wind = new("Wind", 2, Instrument.Flute, Instrument.Oboe, Instrument.Clarinet, Instrument.Fife, Instrument.Panpipes);
        public static readonly InstrumentTone Drums = new("Drums", 3,  Instrument.Timpani, Instrument.Bongo, Instrument.BassDrum, Instrument.SnareDrum, Instrument.Cymbal);
        public static readonly InstrumentTone Brass = new("Brass", 4,  Instrument.Trumpet, Instrument.Trombone, Instrument.Tuba, Instrument.Horn, Instrument.Saxophone);
        public static readonly InstrumentTone Strings = new("Strings", 5, Instrument.Violin, Instrument.Viola, Instrument.Cello, Instrument.DoubleBass,Instrument.None);
        public static readonly InstrumentTone ElectricGuitar = new("ElectricGuitar", 6,  Instrument.ElectricGuitarOverdriven, Instrument.ElectricGuitarClean, Instrument.ElectricGuitarMuted, Instrument.ElectricGuitarPowerChords, Instrument.ElectricGuitarSpecial);

        public static readonly IReadOnlyList<InstrumentTone> All = new ReadOnlyCollection<InstrumentTone>(new List<InstrumentTone> { Strummed, Wind, Drums, Brass, Strings, ElectricGuitar });

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
        /// 
        /// </summary>
        public Instrument Tone0 { get; }

        /// <summary>
        /// 
        /// </summary>
        public Instrument Tone1 { get; }

        /// <summary>
        /// 
        /// </summary>
        public Instrument Tone2 { get; }

        /// <summary>
        /// 
        /// </summary>
        public Instrument Tone3 { get; }

        /// <summary>
        /// 
        /// </summary>
        public Instrument Tone4 { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentTone"/> struct.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="index">Index.</param>
        /// <param name="tone0"></param>
        /// <param name="tone1"></param>
        /// <param name="tone2"></param>
        /// <param name="tone3"></param>
        /// <param name="tone4"></param>
        private InstrumentTone(string name, int index, Instrument tone0, Instrument tone1, Instrument tone2, Instrument tone3, Instrument tone4)
        {
            Name = name;
            Index = index;
            Tone0 = tone0;
            Tone1 = tone1;
            Tone2 = tone2;
            Tone3 = tone3;
            Tone4 = tone4;
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

        public Instrument GetInstrumentFromChannel(int channel)
        {
            return channel switch
            {
                0 => Tone0,
                1 => Tone1,
                2 => Tone2,
                3 => Tone3,
                4 => Tone4,
                _ => Instrument.None
            };
        }

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
            if (All.Any(x => x.Index.Equals(instrumentTone)))
            {
                result = All.First(x => x.Index.Equals(instrumentTone));
                return true;
            }
            result = None;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrumentTone"></param>
        /// <returns></returns>
        public static InstrumentTone Parse(string instrumentTone)
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
        public static bool TryParse(string instrumentTone, out InstrumentTone result)
        {
            if (instrumentTone is null)
            {
                result = None;
                return false;
            }
            instrumentTone = instrumentTone.Replace(" ", "").Replace("_", "");
            if (int.TryParse(instrumentTone, out var number)) return TryParse(number, out result);
            if (All.Any(x => x.Name.Equals(instrumentTone, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = All.First(x => x.Name.Equals(instrumentTone, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            result = None;
            return false;
        }
    }
}