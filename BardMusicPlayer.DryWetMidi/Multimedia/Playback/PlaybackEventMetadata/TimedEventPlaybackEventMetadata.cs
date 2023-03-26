namespace BardMusicPlayer.DryWetMidi.Multimedia.Playback.PlaybackEventMetadata;

internal sealed class TimedEventPlaybackEventMetadata
{
    #region Constructor

    public TimedEventPlaybackEventMetadata(object metadata)
    {
        Metadata = metadata;
    }

    #endregion

    #region Properties

    public object Metadata { get; }

    #endregion
}