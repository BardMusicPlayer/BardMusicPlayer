using BardMusicPlayer.Coffer;
using BardMusicPlayer.Ui.ViewModels.Playlist;

namespace BardMusicPlayer.Ui.Notifications
{
    public class SelectPlaylistNotification
    {
        public SelectPlaylistNotification(BmpPlaylistViewModel playlist) { Playlist = playlist; }

        public BmpPlaylistViewModel Playlist { get; }
    }
}