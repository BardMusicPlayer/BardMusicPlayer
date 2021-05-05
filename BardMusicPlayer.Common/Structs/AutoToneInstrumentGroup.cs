/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BardMusicPlayer.Common.Structs
{
    /// <summary>
    /// Represents available autotone instruments
    /// </summary>
    public readonly struct AutoToneInstrumentGroup : IComparable, IConvertible, IComparable<AutoToneInstrumentGroup>, IEquatable<AutoToneInstrumentGroup>
    {
        public static readonly AutoToneInstrumentGroup Invalid = new("Invalid", -1, 0, 0, 0, InstrumentTone.None, Instrument.None, Instrument.None, Instrument.None, AutoToneOctaveRange.Invalid);

        #region Strummed

        // Lute Harp Piano
        public static readonly AutoToneInstrumentGroup Lute1Harp3Piano1 = new("Lute1Harp3Piano1", 0, 1, 3, 1, InstrumentTone.Strummed, Instrument.Lute, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Lute2Harp2Piano1 = new("Lute2Harp2Piano1", 1, 2, 2, 1, InstrumentTone.Strummed, Instrument.Lute, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Lute3Harp1Piano1 = new("Lute3Harp1Piano1", 2, 3, 1, 1, InstrumentTone.Strummed, Instrument.Lute, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Lute2Harp1Piano2 = new("Lute2Harp1Piano2", 3, 2, 1, 2, InstrumentTone.Strummed, Instrument.Lute, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Lute1Harp1Piano3 = new("Lute1Harp1Piano3", 4, 1, 1, 3, InstrumentTone.Strummed, Instrument.Lute, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Lute1Harp2Piano2 = new("Lute1Harp2Piano2", 5, 1, 2, 2, InstrumentTone.Strummed, Instrument.Lute, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> LuteHarpPiano = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Lute1Harp3Piano1, Lute2Harp2Piano1, Lute3Harp1Piano1, Lute2Harp1Piano2, Lute1Harp1Piano3, Lute1Harp2Piano2 });

        // Lute Piano
        public static readonly AutoToneInstrumentGroup Lute2Piano3 = new("Lute2Piano3", 6, 2, 0, 3, InstrumentTone.Strummed, Instrument.Lute, Instrument.None, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Lute3Piano2 = new("Lute3Piano2", 7, 3, 0, 2, InstrumentTone.Strummed, Instrument.Lute, Instrument.None, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> LutePiano = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Lute2Piano3, Lute3Piano2 });

        // Fiddle Harp Piano
        public static readonly AutoToneInstrumentGroup Fiddle1Harp3Piano1 = new("Fiddle1Harp3Piano1", 8, 1, 3, 1, InstrumentTone.Strummed, Instrument.Fiddle, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Fiddle2Harp2Piano1 = new("Fiddle2Harp2Piano1", 9, 2, 2, 1, InstrumentTone.Strummed, Instrument.Fiddle, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Fiddle3Harp1Piano1 = new("Fiddle3Harp1Piano1", 10, 3, 1, 1, InstrumentTone.Strummed, Instrument.Fiddle, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Fiddle2Harp1Piano2 = new("Fiddle2Harp1Piano2", 11, 2, 1, 2, InstrumentTone.Strummed, Instrument.Fiddle, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Fiddle1Harp1Piano3 = new("Fiddle1Harp1Piano3", 12, 1, 1, 3, InstrumentTone.Strummed, Instrument.Fiddle, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Fiddle1Harp2Piano2 = new("Fiddle1Harp2Piano2", 13, 1, 2, 2, InstrumentTone.Strummed, Instrument.Fiddle, Instrument.Harp, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> FiddleHarpPiano = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Fiddle1Harp3Piano1, Fiddle2Harp2Piano1, Fiddle3Harp1Piano1, Fiddle2Harp1Piano2, Fiddle1Harp1Piano3, Fiddle1Harp2Piano2 });

        // Fiddle Piano
        public static readonly AutoToneInstrumentGroup Fiddle2Piano3 = new("Fiddle2Piano3", 14, 2, 0, 3, InstrumentTone.Strummed, Instrument.Fiddle, Instrument.None, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly AutoToneInstrumentGroup Fiddle3Piano2 = new("Fiddle3Piano2", 15, 3, 0, 2, InstrumentTone.Strummed, Instrument.Fiddle, Instrument.None, Instrument.Piano, AutoToneOctaveRange.C2toC7);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> FiddlePiano = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Fiddle2Piano3, Fiddle3Piano2 });

        public static readonly IReadOnlyList<AutoToneInstrumentGroup> Strummed = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup>().Concat(LuteHarpPiano).Concat(LutePiano).Concat(FiddleHarpPiano).Concat(FiddlePiano).ToList());

        #endregion

        #region Wind

        // Clarinet Panpipes Fife
        public static readonly AutoToneInstrumentGroup Clarinet1Panpipes3Fife1 = new("Clarinet1Panpipes3Fife1", 100, 1, 3, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Panpipes, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet2Panpipes2Fife1 = new("Clarinet2Panpipes2Fife1", 101, 2, 2, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Panpipes, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet3Panpipes1Fife1 = new("Clarinet3Panpipes1Fife1", 102, 3, 1, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Panpipes, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet2Panpipes1Fife2 = new("Clarinet2Panpipes1Fife2", 103, 2, 1, 2, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Panpipes, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet1Panpipes1Fife3 = new("Clarinet1Panpipes1Fife3", 104, 1, 1, 3, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Panpipes, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet1Panpipes2Fife2 = new("Clarinet1Panpipes2Fife2", 105, 1, 2, 2, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Panpipes, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> ClarinetPanpipesFife = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Clarinet1Panpipes3Fife1, Clarinet2Panpipes2Fife1, Clarinet3Panpipes1Fife1, Clarinet2Panpipes1Fife2, Clarinet1Panpipes1Fife3, Clarinet1Panpipes2Fife2 });

        // Clarinet Oboe Fife
        public static readonly AutoToneInstrumentGroup Clarinet1Oboe3Fife1 = new("Clarinet1Oboe3Fife1", 106, 1, 3, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Oboe, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet2Oboe2Fife1 = new("Clarinet2Oboe2Fife1", 107, 2, 2, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Oboe, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet3Oboe1Fife1 = new("Clarinet3Oboe1Fife1", 108, 3, 1, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Oboe, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet2Oboe1Fife2 = new("Clarinet2Oboe1Fife2", 109, 2, 1, 2, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Oboe, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet1Oboe1Fife3 = new("Clarinet1Oboe1Fife3", 110, 1, 1, 3, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Oboe, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet1Oboe2Fife2 = new("Clarinet1Oboe2Fife2", 111, 1, 2, 2, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Oboe, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> ClarinetOboeFife = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Clarinet1Oboe3Fife1, Clarinet2Oboe2Fife1, Clarinet3Oboe1Fife1, Clarinet2Oboe1Fife2, Clarinet1Oboe1Fife3, Clarinet1Oboe2Fife2 });

        // Clarinet Flute Fife
        public static readonly AutoToneInstrumentGroup Clarinet1Flute3Fife1 = new("Clarinet1Flute3Fife1", 112, 1, 3, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Flute, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet2Flute2Fife1 = new("Clarinet2Flute2Fife1", 113, 2, 2, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Flute, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet3Flute1Fife1 = new("Clarinet3Flute1Fife1", 114, 3, 1, 1, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Flute, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet2Flute1Fife2 = new("Clarinet2Flute1Fife2", 115, 2, 1, 2, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Flute, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet1Flute1Fife3 = new("Clarinet1Flute1Fife3", 116, 1, 1, 3, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Flute, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet1Flute2Fife2 = new("Clarinet1Flute2Fife2", 117, 1, 2, 2, InstrumentTone.Wind, Instrument.Clarinet, Instrument.Flute, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> ClarinetFluteFife = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Clarinet1Flute3Fife1, Clarinet2Flute2Fife1, Clarinet3Flute1Fife1, Clarinet2Flute1Fife2, Clarinet1Flute1Fife3, Clarinet1Flute2Fife2 });

        // Clarinet Fife
        public static readonly AutoToneInstrumentGroup Clarinet2Fife3 = new("Clarinet2Fife3", 118, 2, 0, 3, InstrumentTone.Wind, Instrument.Clarinet, Instrument.None, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly AutoToneInstrumentGroup Clarinet3Fife2 = new("Clarinet3Fife2", 119, 3, 0, 2, InstrumentTone.Wind, Instrument.Clarinet, Instrument.None, Instrument.Fife, AutoToneOctaveRange.C3toC8);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> ClarinetFife = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Clarinet2Fife3, Clarinet3Fife2 });

        public static readonly IReadOnlyList<AutoToneInstrumentGroup> Wind = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup>().Concat(ClarinetPanpipesFife).Concat(ClarinetOboeFife).Concat(ClarinetFluteFife).Concat(ClarinetFife).ToList());

        #endregion

        #region Drums

        public static readonly IReadOnlyList<AutoToneInstrumentGroup> Drums = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup>());

        #endregion

        #region Brass

        // Tuba Trombone Trumpet
        public static readonly AutoToneInstrumentGroup Tuba1Trombone3Trumpet1 = new("Tuba1Trombone3Trumpet1", 300, 1, 3, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba2Trombone2Trumpet1 = new("Tuba2Trombone2Trumpet1", 301, 2, 2, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba3Trombone1Trumpet1 = new("Tuba3Trombone1Trumpet1", 302, 3, 1, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba2Trombone1Trumpet2 = new("Tuba2Trombone1Trumpet2", 303, 2, 1, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba1Trombone1Trumpet3 = new("Tuba1Trombone1Trumpet3", 304, 1, 1, 3, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba1Trombone2Trumpet3 = new("Tuba1Trombone2Trumpet2", 305, 1, 2, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> TubaTromboneTrumpet = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Tuba1Trombone3Trumpet1, Tuba2Trombone2Trumpet1, Tuba3Trombone1Trumpet1, Tuba2Trombone1Trumpet2, Tuba1Trombone1Trumpet3, Tuba1Trombone2Trumpet3 });

        // Tube Horn Trumpet
        public static readonly AutoToneInstrumentGroup Tuba1Horn3Trumpet1 = new("Tuba1Horn3Trumpet1", 306, 1, 3, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba2Horn2Trumpet1 = new("Tuba2Horn2Trumpet1", 307, 2, 2, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba3Horn1Trumpet1 = new("Tuba3Horn1Trumpet1", 308, 3, 1, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba2Horn1Trumpet2 = new("Tuba2Horn1Trumpet2", 309, 2, 1, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba1Horn1Trumpet3 = new("Tuba1Horn1Trumpet3", 310, 1, 1, 3, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba1Horn2Trumpet3 = new("Tuba1Horn2Trumpet3", 311, 1, 2, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> TubaHornTrumpet = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Tuba1Horn3Trumpet1, Tuba2Horn2Trumpet1, Tuba3Horn1Trumpet1, Tuba2Horn1Trumpet2, Tuba1Horn1Trumpet3, Tuba1Horn2Trumpet3 });

        // Tuba Trumpet
        public static readonly AutoToneInstrumentGroup Tuba2Trumpet3 = new("Tuba2Trumpet3", 312, 2, 0, 3, InstrumentTone.Brass, Instrument.Tuba, Instrument.None, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba3Trumpet2 = new("Tuba3Trumpet2", 313, 3, 0, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.None, Instrument.Trumpet, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> TubaTrumpet = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Tuba2Trumpet3, Tuba3Trumpet2 });

        // Tuba Trombone Saxophone
        public static readonly AutoToneInstrumentGroup Tuba1Trombone3Saxophone1 = new("Tuba1Trombone3Saxophone1", 314, 1, 3, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba2Trombone2Saxophone1 = new("Tuba2Trombone2Saxophone1", 315, 2, 2, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba3Trombone1Saxophone1 = new("Tuba3Trombone1Saxophone1", 316, 3, 1, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba2Trombone1Saxophone2 = new("Tuba2Trombone1Saxophone2", 317, 2, 1, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba1Trombone1Saxophone3 = new("Tuba1Trombone1Saxophone3", 318, 1, 1, 3, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba1Trombone2Saxophone3 = new("Tuba1Trombone2Saxophone2", 319, 1, 2, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.Trombone, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> TubaTromboneSaxophone= new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Tuba1Trombone3Saxophone1, Tuba2Trombone2Saxophone1, Tuba3Trombone1Saxophone1, Tuba2Trombone1Saxophone2, Tuba1Trombone1Saxophone3, Tuba1Trombone2Saxophone3 });

        // Tube Horn Saxophone
        public static readonly AutoToneInstrumentGroup Tuba1Horn3Saxophone1 = new("Tuba1Horn3Saxophone1", 320, 1, 3, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba2Horn2Saxophone1 = new("Tuba2Horn2Saxophone1", 321, 2, 2, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba3Horn1Saxophone1 = new("Tuba3Horn1Saxophone1", 322, 3, 1, 1, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba2Horn1Saxophone2 = new("Tuba2Horn1Saxophone2", 323, 2, 1, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba1Horn1Saxophone3 = new("Tuba1Horn1Saxophone3", 324, 1, 1, 3, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba1Horn2Saxophone3 = new("Tuba1Horn2Saxophone2", 325, 1, 2, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.Horn, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> TubaHornSaxophone = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Tuba1Horn3Saxophone1, Tuba2Horn2Saxophone1, Tuba3Horn1Saxophone1, Tuba2Horn1Saxophone2, Tuba1Horn1Saxophone3, Tuba1Horn2Saxophone3 });

        // Tuba Saxophone
        public static readonly AutoToneInstrumentGroup Tuba2Saxophone3 = new("Tuba2Saxophone3", 326, 2, 0, 3, InstrumentTone.Brass, Instrument.Tuba, Instrument.None, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup Tuba3Saxophone2 = new("Tuba3Saxophone2", 327, 3, 0, 2, InstrumentTone.Brass, Instrument.Tuba, Instrument.None, Instrument.Saxophone, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> TubaSaxophone = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { Tuba2Saxophone3, Tuba3Saxophone2 });

        public static readonly IReadOnlyList<AutoToneInstrumentGroup> Brass = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup>().Concat(TubaTromboneTrumpet).Concat(TubaHornTrumpet).Concat(TubaTrumpet).Concat(TubaTromboneSaxophone).Concat(TubaHornSaxophone).Concat(TubaSaxophone).ToList());

        #endregion

        #region Strings

        // DoubleBass Cello Viola
        public static readonly AutoToneInstrumentGroup DoubleBass1Cello3Viola1 = new("DoubleBass1Cello3Viola1", 400, 1, 3, 1, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Viola, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass2Cello2Viola1 = new("DoubleBass2Cello2Viola1", 401, 2, 2, 1, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Viola, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass3Cello1Viola1 = new("DoubleBass3Cello1Viola1", 402, 3, 1, 1, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Viola, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass2Cello1Viola2 = new("DoubleBass2Cello1Viola2", 403, 2, 1, 2, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Viola, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass1Cello1Viola3 = new("DoubleBass1Cello1Viola3", 404, 1, 1, 3, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Viola, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass1Cello2Viola3 = new("DoubleBass1Cello2Viola2", 405, 1, 2, 2, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Viola, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> DoubleBassCelloViola = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { DoubleBass1Cello3Viola1, DoubleBass2Cello2Viola1, DoubleBass3Cello1Viola1, DoubleBass2Cello1Viola2, DoubleBass1Cello1Viola3, DoubleBass1Cello2Viola3 });

        // DoubleBass Viola
        public static readonly AutoToneInstrumentGroup DoubleBass2Viola3 = new("DoubleBass2Viola3", 406, 2, 0, 3, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.None, Instrument.Viola, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass3Viola2 = new("DoubleBass3Viola2", 407, 3, 0, 2, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.None, Instrument.Viola, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> DoubleBassViola = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { DoubleBass2Viola3, DoubleBass3Viola2 });

        // DoubleBass Cello Violin
        public static readonly AutoToneInstrumentGroup DoubleBass1Cello3Violin1 = new("DoubleBass1Cello3Violin1", 408, 1, 3, 1, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Violin, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass2Cello2Violin1 = new("DoubleBass2Cello2Violin1", 409, 2, 2, 1, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Violin, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass3Cello1Violin1 = new("DoubleBass3Cello1Violin1", 410, 3, 1, 1, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Violin, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass2Cello1Violin2 = new("DoubleBass2Cello1Violin2", 411, 2, 1, 2, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Violin, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass1Cello1Violin3 = new("DoubleBass1Cello1Violin3", 412, 1, 1, 3, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Violin, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass1Cello2Violin3 = new("DoubleBass1Cello2Violin2", 413, 1, 2, 2, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.Cello, Instrument.Violin, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> DoubleBassCelloViolin = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { DoubleBass1Cello3Violin1, DoubleBass2Cello2Violin1, DoubleBass3Cello1Violin1, DoubleBass2Cello1Violin2, DoubleBass1Cello1Violin3, DoubleBass1Cello2Violin3 });
        
        // DoubleBass Violin
        public static readonly AutoToneInstrumentGroup DoubleBass2Violin3 = new("DoubleBass2Violin3", 414, 2, 0, 3, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.None, Instrument.Violin, AutoToneOctaveRange.C1toC6);
        public static readonly AutoToneInstrumentGroup DoubleBass3Violin2 = new("DoubleBass3Violin2", 415, 3, 0, 2, InstrumentTone.Strings, Instrument.DoubleBass, Instrument.None, Instrument.Violin, AutoToneOctaveRange.C1toC6);
        public static readonly IReadOnlyList<AutoToneInstrumentGroup> DoubleBassViolin = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup> { DoubleBass2Violin3, DoubleBass3Violin2 });

        public static readonly IReadOnlyList<AutoToneInstrumentGroup> Strings = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup>().Concat(DoubleBassCelloViola).Concat(DoubleBassViola).Concat(DoubleBassCelloViolin).Concat(DoubleBassViolin).ToList());

        #endregion

        #region SomethingNew

        public static readonly IReadOnlyList<AutoToneInstrumentGroup> SomethingNew = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup>());

        #endregion

        public static readonly IReadOnlyList<AutoToneInstrumentGroup> All = new ReadOnlyCollection<AutoToneInstrumentGroup>(new List<AutoToneInstrumentGroup>().Concat(Strummed).Concat(Wind).Concat(Drums).Concat(Brass).Concat(Strings).Concat(SomethingNew).ToList());

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
        public int Size1 { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Size2 { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Size3 { get; }

        /// <summary>
        /// 
        /// </summary>
        public InstrumentTone InstrumentTone { get; }

        /// <summary>
        /// 
        /// </summary>
        public Instrument Instrument1 { get; }

        /// <summary>
        /// 
        /// </summary>
        public Instrument Instrument2 { get; }

        /// <summary>
        /// 
        /// </summary>
        public Instrument Instrument3 { get; }

        /// <summary>
        /// 
        /// </summary>
        public AutoToneOctaveRange DefaultAutoToneOctaveRange { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoToneInstrumentGroup"/> struct.
        /// </summary>
        private AutoToneInstrumentGroup(string name, int index, int size1, int size2, int size3, InstrumentTone instrumentTone, Instrument instrument1, Instrument instrument2, Instrument instrument3, AutoToneOctaveRange defaultAutoToneOctaveRange)
        {
            Name = name;
            Index = index;
            Size1 = size1;
            Size2 = size2;
            Size3 = size3;
            InstrumentTone = instrumentTone;
            Instrument1 = instrument1;
            Instrument2 = instrument2;
            Instrument3 = instrument3;
            DefaultAutoToneOctaveRange = defaultAutoToneOctaveRange;
        }

        /// <summary>
        /// Determines whether the specified <see cref="AutoToneInstrumentGroup"/> is equal to the
        /// current <see cref="AutoToneInstrumentGroup"/>.
        /// </summary>
        /// <param name="other">The <see cref="AutoToneInstrumentGroup"/> to compare with the current <see cref="AutoToneInstrumentGroup"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="AutoToneInstrumentGroup"/> is equal to the current
        /// <see cref="AutoToneInstrumentGroup"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(AutoToneInstrumentGroup other) => Index == other;

        bool IEquatable<AutoToneInstrumentGroup>.Equals(AutoToneInstrumentGroup other) => Equals(other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="AutoToneInstrumentGroup"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="AutoToneInstrumentGroup"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="AutoToneInstrumentGroup"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is AutoToneInstrumentGroup octaveRange && Equals(octaveRange);

        /// <summary>
        /// Serves as a hash function for a <see cref="AutoToneInstrumentGroup"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => (Name, Index).GetHashCode();

        public static implicit operator string(AutoToneInstrumentGroup octaveRange) => octaveRange.Name;
        public static implicit operator AutoToneInstrumentGroup(string name) => Parse(name);
        public static implicit operator int(AutoToneInstrumentGroup octaveRange) => octaveRange.Index;
        public static implicit operator AutoToneInstrumentGroup(int lower) => Parse(lower);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (!(obj is AutoToneInstrumentGroup)) throw new ArgumentException("This is not an AutoToneInstrumentGroup");
            return Index - ((AutoToneInstrumentGroup) obj).Index;
        }

        public int CompareTo(AutoToneInstrumentGroup other) => Index - other.Index;
        public TypeCode GetTypeCode() => TypeCode.Int32;
        public bool ToBoolean(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneInstrumentGroup to Boolean");
        public char ToChar(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneInstrumentGroup to Char");
        public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(Index);
        public byte ToByte(IFormatProvider provider) => Convert.ToByte(Index);
        public short ToInt16(IFormatProvider provider) => Convert.ToInt16(Index);
        public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Index);
        public int ToInt32(IFormatProvider provider) => Convert.ToInt32(Index);
        public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Index);
        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(Index);
        public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Index);
        public float ToSingle(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneInstrumentGroup to Single");
        public double ToDouble(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneInstrumentGroup to Double");
        public decimal ToDecimal(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneInstrumentGroup to Decimal");
        public DateTime ToDateTime(IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneInstrumentGroup to DateTime");
        public string ToString(IFormatProvider provider) => Index.ToString();
        public override string ToString() => Index.ToString();
        public object ToType(Type conversionType, IFormatProvider provider) => throw new InvalidCastException("Invalid cast from AutoToneInstrumentGroup to " + conversionType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoToneInstrumentGroup"></param>
        /// <returns></returns>
        public static AutoToneInstrumentGroup Parse(int autoToneInstrumentGroup)
        {
            TryParse(autoToneInstrumentGroup, out var result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoToneInstrumentGroup"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(int autoToneInstrumentGroup, out AutoToneInstrumentGroup result)
        {
            if (All.Any(x => x.Index.Equals(autoToneInstrumentGroup)))
            {
                result = All.First(x => x.Index.Equals(autoToneInstrumentGroup));
                return true;
            }
            result = Invalid;
            return false;
        }

        /// <summary>
        /// Gets the autoToneInstrumentGroup from a string.
        /// </summary>
        /// <param name="autoToneInstrumentGroup">The string with the name of the autoToneInstrumentGroup</param>
        /// <returns>The <see cref="AutoToneInstrumentGroup"/>, or <see cref="Invalid"/> if invalid.</returns>
        public static AutoToneInstrumentGroup Parse(string autoToneInstrumentGroup)
        {
            TryParse(autoToneInstrumentGroup, out var result);
            return result;
        }

        /// <summary>
        /// Tries to get the autoToneInstrumentGroup from a string.
        /// </summary>
        /// <param name="autoToneInstrumentGroup">The string with the name of the autoToneInstrumentGroup</param>
        /// <param name="result">The <see cref="AutoToneInstrumentGroup"/>, or <see cref="Invalid"/> if invalid.</param>
        /// <returns>true if the <see cref="AutoToneInstrumentGroup"/> is anything besides <see cref="Invalid"/></returns>
        public static bool TryParse(string autoToneInstrumentGroup, out AutoToneInstrumentGroup result)
        {
            if (autoToneInstrumentGroup is null)
            {
                result = Invalid;
                return false;
            }
            autoToneInstrumentGroup = autoToneInstrumentGroup.Replace(" ", "").Replace("_", "");
            if (int.TryParse(autoToneInstrumentGroup, out var number)) return TryParse(number, out result);
            if (All.Any(x => x.Name.Equals(autoToneInstrumentGroup, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = All.First(x => x.Name.Equals(autoToneInstrumentGroup, StringComparison.CurrentCultureIgnoreCase));
                return true;
            }
            result = Invalid;
            return false;
        }

        /// <summary>
        /// Gets the new note value for a note that needs to move to a different base octave.
        /// </summary>
        /// <param name="currentOctaveRange">The current octave range this note is in</param>
        /// <param name="note">The note number</param>
        /// <returns>True, if this note was in range to be moved, else false.</returns>
        public bool TryShiftNoteToDefaultOctave(AutoToneOctaveRange currentOctaveRange, ref int note)
        {
            if (Equals(Invalid))
                throw new BmpException(Name + " is not a valid instrument for this function.");

            return DefaultAutoToneOctaveRange.TryShiftNoteToOctave(currentOctaveRange, ref note);
        }

        /// <summary>
        /// Validates this note is in this AutoToneInstrumentGroup's octave range.
        /// </summary>
        /// <param name="note">The note</param>
        /// <returns></returns>
        public bool ValidateNoteRange(int note) => DefaultAutoToneOctaveRange.ValidateNoteRange(note);
    }
}