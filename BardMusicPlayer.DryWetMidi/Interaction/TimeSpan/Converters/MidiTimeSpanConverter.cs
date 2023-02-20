using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Representations;

namespace BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Converters
{
    internal sealed class MidiTimeSpanConverter : ITimeSpanConverter
    {
        #region ITimeSpanConverter

        public ITimeSpan ConvertTo(long timeSpan, long time, TempoMap.TempoMap tempoMap)
        {
            return (MidiTimeSpan)timeSpan;
        }

        public long ConvertFrom(ITimeSpan timeSpan, long time, TempoMap.TempoMap tempoMap)
        {
            return ((MidiTimeSpan)timeSpan).TimeSpan;
        }

        #endregion
    }
}
