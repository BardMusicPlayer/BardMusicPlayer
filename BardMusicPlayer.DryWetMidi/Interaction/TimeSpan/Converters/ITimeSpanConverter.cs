namespace BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Converters;

internal interface ITimeSpanConverter
{
    ITimeSpan ConvertTo(long timeSpan, long time, TempoMap.TempoMap tempoMap);

    long ConvertFrom(ITimeSpan timeSpan, long time, TempoMap.TempoMap tempoMap);
}