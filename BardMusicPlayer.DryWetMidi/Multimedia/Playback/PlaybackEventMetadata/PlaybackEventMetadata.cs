namespace BardMusicPlayer.DryWetMidi.Multimedia.Playback.PlaybackEventMetadata;

internal sealed class PlaybackEventMetadata
{
    public NotePlaybackEventMetadata Note { get; set; }

    public TimedEventPlaybackEventMetadata TimedEvent { get; set; }
}