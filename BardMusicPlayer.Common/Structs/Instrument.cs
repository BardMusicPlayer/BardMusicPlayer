/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BardMusicPlayer.Common.Enums;
using static BardMusicPlayer.Common.Structs.OctaveRange;

namespace BardMusicPlayer.Common.Structs
{
    /// <summary>
    /// Represents available instrument VST's in game.
    /// </summary>
    public readonly struct Instrument : IComparable, IConvertible, IComparable<Instrument>, IEquatable<Instrument>
    {
        public static readonly Instrument None = new("None", 0, 122, C3toC6, false, 50, InstrumentTone.None, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE0);

        public static readonly Instrument Harp = new("Harp", 1, 46, C3toC6, false,0, InstrumentTone.Strummed, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE0);
        public static readonly Instrument Piano = new("Piano", 2, 0, C4toC7, false, 0, InstrumentTone.Strummed, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE1);
        public static readonly Instrument Lute = new("Lute", 3, 24, C2toC5, false, 0, InstrumentTone.Strummed, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE2);
        public static readonly Instrument Fiddle = new("Fiddle", 4, 45, C2toC5, false, 0, InstrumentTone.Strummed, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE3);
        public static readonly Instrument Flute = new("Flute", 5, 73, C4toC7, true, 0, InstrumentTone.Wind, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE0);
        public static readonly Instrument Oboe = new("Oboe", 6, 68, C4toC7, true, 0, InstrumentTone.Wind, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE1);
        public static readonly Instrument Clarinet = new("Clarinet", 7, 71, C3toC6, true, 0, InstrumentTone.Wind, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE2);
        public static readonly Instrument Fife = new("Fife", 8, 72, C5toC8, true, 0, InstrumentTone.Wind, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE3);
        public static readonly Instrument Panpipes = new("Panpipes", 9, 75, C4toC7, true, 0, InstrumentTone.Wind, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE4);
        public static readonly Instrument Timpani = new("Timpani", 10, 47, C2toC5, false, 0, InstrumentTone.Drums, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE0);
        public static readonly Instrument Bongo = new("Bongo", 11, 116, C3toC6, false, 0, InstrumentTone.Drums, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE1);
        public static readonly Instrument BassDrum = new("BassDrum", 12, 117, C2toC5, false, 0, InstrumentTone.Drums, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE2);
        public static readonly Instrument SnareDrum = new("SnareDrum", 13, 115, C3toC6, false, 0, InstrumentTone.Drums, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE3);
        public static readonly Instrument Cymbal = new("Cymbal", 14, 127, C3toC6, false, 50, InstrumentTone.Drums, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE4);
        public static readonly Instrument Trumpet = new("Trumpet", 15, 56, C3toC6, true, 50, InstrumentTone.Brass, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE0);
        public static readonly Instrument Trombone = new("Trombone", 16, 57, C2toC5, true, 50, InstrumentTone.Brass, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE1);
        public static readonly Instrument Tuba = new("Tuba", 17, 58, C1toC4, true, 50, InstrumentTone.Brass, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE2);
        public static readonly Instrument Horn = new("Horn", 18, 60, C2toC5, true, 50, InstrumentTone.Brass, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE3);
        public static readonly Instrument Saxophone = new("Saxophone", 19, 65, C3toC6, true, 50, InstrumentTone.Brass, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE4);
        public static readonly Instrument Violin = new("Violin", 20, 40, C3toC6, true, 50, InstrumentTone.Strings, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE0);
        public static readonly Instrument Viola = new("Viola", 21, 41, C3toC6, true, 50, InstrumentTone.Strings, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE1);
        public static readonly Instrument Cello = new("Cello", 22, 42, C2toC5, true, 50, InstrumentTone.Strings, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE2);
        public static readonly Instrument DoubleBass = new("DoubleBass", 23, 43, C1toC4, true, 50, InstrumentTone.Strings, InstrumentToneMenuKey.PERFORMANCE_MODE_EX_TONE3);

        public static IReadOnlyList<Instrument> All { get; } = new ReadOnlyCollection<Instrument>(new List<Instrument> { Harp, Piano, Lute, Fiddle, Flute, Oboe, Clarinet, Fife, Panpipes, Timpani, Bongo, BassDrum, SnareDrum, Cymbal, Trumpet, Trombone, Tuba, Horn, Saxophone, Violin, Viola, Cello, DoubleBass });

        private static readonly Instrument FirstInstrument = Harp;
        private static readonly Instrument LastInstrument = DoubleBass;

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
        /// Gets the midi program change code.
        /// </summary>
        /// <value>The midi program change code.</value>
        public int MidiProgramChangeCode { get; }

        /// <summary>
        /// Gets the default octave range.
        /// </summary>
        /// <value>The default octave range.</value>
        public OctaveRange DefaultOctaveRange { get; }

        /// <summary>
        /// Returns true if this instrument supports being sustained.
        /// </summary>
        public bool IsSustained { get; }

        /// <summary>
        /// Gets the sample offset
        /// </summary>
        public int SampleOffset { get; }

        public InstrumentTone InstrumentTone { get; }

        public InstrumentToneMenuKey InstrumentToneMenuKey { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Instrument"/> struct.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="index">Index.</param>
        /// <param name="midiProgramChangeCode">MidiProgramChangeCode.</param>
        /// <param name="defaultOctaveRange">DefaultOctaveRange</param>
        /// <param name="isSustained">IsSustained</param>
        /// <param name="sampleOffset">SampleOffset</param>
        /// <param name="instrumentTone"></param>
        private Instrument(string name, int index, int midiProgramChangeCode, OctaveRange defaultOctaveRange, bool isSustained, int sampleOffset, InstrumentTone instrumentTone, InstrumentToneMenuKey instrumentToneMenuKey)
        {
            Name = name;
            Index = index;
            MidiProgramChangeCode = midiProgramChangeCode;
            DefaultOctaveRange = defaultOctaveRange;
            IsSustained = isSustained;
            SampleOffset = sampleOffset;
            InstrumentTone = instrumentTone;
            InstrumentToneMenuKey = instrumentToneMenuKey;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Instrument"/> is equal to the
        /// current <see cref="Instrument"/>.
        /// </summary>
        /// <param name="other">The <see cref="Instrument"/> to compare with the current <see cref="Instrument"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Instrument"/> is equal to the current
        /// <see cref="Instrument"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(Instrument other) => Index == other;

        bool IEquatable<Instrument>.Equals(Instrument other) => Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="Instrument"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="Instrument"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="Instrument"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is Instrument instrument && Equals(instrument);

        /// <summary>
        /// Serves as a hash function for a <see cref="Instrument"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => (Name, Index, MidiProgramChangeCode, DefaultOctaveRange).GetHashCode();
        
        public static implicit operator string(Instrument instrument) => instrument.Name;
        public static implicit operator Instrument(string name) => Parse(name);
        public static implicit operator int(Instrument instrument) => instrument.Index;
        public static implicit operator Instrument(int index) => Parse(index);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (!(obj is Instrument)) throw new ArgumentException("This is not an Instrument");
            return Index - ((Instrument) obj).Index;
        }

        public int CompareTo(Instrument other) => Index - other.Index;
        public TypeCode GetTypeCode() => TypeCode.Int32;
        public bool ToBoolean(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from Instrument to Boolean");
        public char ToChar(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from Instrument to Char");
        public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Index);
        public byte ToByte(IFormatProvider provider) => Convert.ToByte(Index);
        public short ToInt16(IFormatProvider provider) => Convert.ToInt16(Index);
        public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Index);
        public int ToInt32(IFormatProvider provider) => Convert.ToInt32(Index);
        public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Index);
        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(Index);
        public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Index);
        public float ToSingle(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from Instrument to Single");
        public double ToDouble(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from Instrument to Double");
        public decimal ToDecimal(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from Instrument to Decimal");
        public DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from Instrument to DateTime");
        public string ToString(IFormatProvider provider) => Index.ToString();
        public override string ToString() => Index.ToString();
        public object ToType(Type conversionType, IFormatProvider provider) => throw new InvalidCastException("Invalid cast from Instrument to " + conversionType);

        /// <summary>
        /// Gets the Default Track name of this instrument.
        /// </summary>
        /// <returns></returns>
        public string GetDefaultTrackName() => Name + DefaultOctaveRange.TrackNameOffset;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrument"></param>
        /// <returns></returns>
        public static Instrument Parse(int instrument)
        {
            TryParse(instrument, out var result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(int instrument, out Instrument result)
        {
            switch (instrument)
            {
                case 1:
                    result = Harp;
                    return true;
                case 2:
                    result = Piano;
                    return true;
                case 3:
                    result = Lute;
                    return true;
                case 4:
                    result = Fiddle;
                    return true;
                case 5:
                    result = Flute;
                    return true;
                case 6:
                    result = Oboe;
                    return true;
                case 7:
                    result = Clarinet;
                    return true;
                case 8:
                    result = Fife;
                    return true;
                case 9:
                    result = Panpipes;
                    return true;
                case 10:
                    result = Timpani;
                    return true;
                case 11:
                    result = Bongo;
                    return true;
                case 12:
                    result = BassDrum;
                    return true;
                case 13:
                    result = SnareDrum;
                    return true;
                case 14:
                    result = Cymbal;
                    return true;
                case 15:
                    result = Trumpet;
                    return true;
                case 16:
                    result = Trombone;
                    return true;
                case 17:
                    result = Tuba;
                    return true;
                case 18:
                    result = Horn;
                    return true;
                case 19:
                    result = Saxophone;
                    return true;
                case 20:
                    result = Violin;
                    return true;
                case 21:
                    result = Viola;
                    return true;
                case 22:
                    result = Cello;
                    return true;
                case 23:
                    result = DoubleBass;
                    return true;
                default:
                    result = None;
                    return false;
            }
        }

        /// <summary>
        /// Gets the instrument from a string.
        /// </summary>
        /// <param name="instrument">The string with the name of the instrument</param>
        /// <returns>The <see cref="Instrument"/>, or <see cref="Unknown"/> if invalid.</returns>
        public static Instrument Parse(string instrument)
        {
            TryParse(instrument, out var result);
            return result;
        }

        /// <summary>
        /// Tries to get the instrument from a string.
        /// </summary>
        /// <param name="instrument">The string with the name of the instrument</param>
        /// <param name="result">The <see cref="Instrument"/>, or <see cref="Unknown"/> if invalid.</param>
        /// <returns>true if the <see cref="Instrument"/> is anything besides <see cref="Unknown"/></returns>
        public static bool TryParse(string instrument, out Instrument result)
        {
            if (int.TryParse(instrument, out var number) && number >= FirstInstrument && number <= LastInstrument) return TryParse(number, out result);

            instrument = instrument.ToLower().Trim().Replace(" ", string.Empty).Replace("_", string.Empty);

            switch (instrument)
            {
                case "harp":
                case "orchestralharp":
                    result = Harp;
                    return true;
                case "piano":
                case "acousticgrandpiano":
                    result = Piano;
                    return true;
                case "lute":
                    result = Lute;
                    return true;
                case "fiddle":
                case "pizzicatostrings":
                    result = Fiddle;
                    return true;
                case "flute":
                    result = Flute;
                    return true;
                case "oboe":
                    result = Oboe;
                    return true;
                case "clarinet":
                    result = Clarinet;
                    return true;
                case "fife":
                case "piccolo":
                    result = Fife;
                    return true;
                case "panpipes":
                case "panflute":
                    result = Panpipes;
                    return true;
                case "timpani":
                    result = Timpani;
                    return true;
                case "bongo":
                    result = Bongo;
                    return true;
                case "bassdrum":
                    result = BassDrum;
                    return true;
                case "snaredrum":
                    result = SnareDrum;
                    return true;
                case "cymbal":
                    result = Cymbal;
                    return true;
                case "trumpet":
                    result = Trumpet;
                    return true;
                case "trombone":
                    result = Trombone;
                    return true;
                case "tuba":
                    result = Tuba;
                    return true;
                case "horn":
                case "frenchhorn":
                    result = Horn;
                    return true;
                case "saxophone":
                case "altosaxophone":
                    result = Saxophone;
                    return true;
                case "violin":
                    result = Violin;
                    return true;
                case "viola":
                    result = Viola;
                    return true;
                case "cello":
                    result = Cello;
                    return true;
                case "doublebass":
                case "contrabass":
                    result = DoubleBass;
                    return true;
                default:
                    result = None;
                    return false;
            }
        }
        
        /// <summary>
        /// Gets the per note sound sample offset for a given instrument. Should be combined with the SampleOffest.
        /// </summary>
        /// <param name="note">The in game note in this Instrument's default range</param>
        /// <returns>The millisecond offset</returns>
        public long NoteSampleOffset(int note) => ValidateNoteRange(note) && Equals(Clarinet) && note <= 58 ? 0 : 50;

        /// <summary>
        /// Gets the new note value for a note that needs to move to a different base octave.
        /// </summary>
        /// <param name="currentOctaveRange">The current octave range this note is in</param>
        /// <param name="note">The note number</param>
        /// <returns>True, if this note was in range to be moved, else false.</returns>
        public bool TryShiftNoteToDefaultOctave(OctaveRange currentOctaveRange, ref int note)
        {
            if (Equals(None))
                throw new BmpException(Name + " is not a valid instrument for this function.");

            return DefaultOctaveRange.TryShiftNoteToOctave(currentOctaveRange, ref note);
        }

        /// <summary>
        /// Validates this note is in this Instrument's octave range.
        /// </summary>
        /// <param name="note">The note</param>
        /// <returns></returns>
        public bool ValidateNoteRange(int note) => DefaultOctaveRange.ValidateNoteRange(note);
    }
}