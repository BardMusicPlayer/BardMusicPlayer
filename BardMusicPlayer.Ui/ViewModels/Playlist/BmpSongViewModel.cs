using BardMusicPlayer.Transmogrify.Song;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class BmpSongViewModel : Screen
    {
        private readonly BmpSong _bmpSong;
        private readonly PlaylistViewModel _playlistview;

        public BmpSongViewModel(BmpSong bmpsong, PlaylistViewModel parent)
        {
            _bmpSong      = bmpsong;
            _playlistview = parent;
            Title         = _bmpSong.Title;
        }

        public string Title { get; set; }
    }
}