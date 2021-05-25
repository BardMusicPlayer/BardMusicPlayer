using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Ui.Notifications
{
    public class EditSongNotification
    {
        public EditSongNotification(BmpSong song) { Song = song; }

        public BmpSong Song { get; }
    }
}