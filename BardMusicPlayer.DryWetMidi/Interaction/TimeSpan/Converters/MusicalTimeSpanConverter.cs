﻿using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Core.TimeDivision;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Representations;

namespace BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Converters
{
    internal sealed class MusicalTimeSpanConverter : ITimeSpanConverter
    {
        #region ITimeSpanConverter

        public ITimeSpan ConvertTo(long timeSpan, long time, TempoMap.TempoMap tempoMap)
        {
            var ticksPerQuarterNoteTimeDivision = tempoMap.TimeDivision as TicksPerQuarterNoteTimeDivision;
            if (ticksPerQuarterNoteTimeDivision == null)
                throw new ArgumentException("Time division is not supported for time span conversion.", nameof(tempoMap));

            if (timeSpan == 0)
                return new MusicalTimeSpan();

            var xy = MathUtilities.SolveDiophantineEquation(4 * ticksPerQuarterNoteTimeDivision.TicksPerQuarterNote, -timeSpan);
            return new MusicalTimeSpan(Math.Abs(xy.Item1), Math.Abs(xy.Item2));
        }

        public long ConvertFrom(ITimeSpan timeSpan, long time, TempoMap.TempoMap tempoMap)
        {
            var ticksPerQuarterNoteTimeDivision = tempoMap.TimeDivision as TicksPerQuarterNoteTimeDivision;
            if (ticksPerQuarterNoteTimeDivision == null)
                throw new ArgumentException("Time division is not supported for time span conversion.", nameof(tempoMap));

            var musicalTimeSpan = (MusicalTimeSpan)timeSpan;
            if (musicalTimeSpan.Numerator == 0)
                return 0;

            return MathUtilities.RoundToLong(4.0 * musicalTimeSpan.Numerator * ticksPerQuarterNoteTimeDivision.TicksPerQuarterNote / musicalTimeSpan.Denominator);
        }

        #endregion
    }
}
