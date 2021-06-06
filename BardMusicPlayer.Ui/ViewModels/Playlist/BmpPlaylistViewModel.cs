using BardMusicPlayer.Coffer;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class BmpPlaylistViewModel : Screen
    {
        private readonly IPlaylist _bmpPlaylist;
        private bool _isReadOnly = true;

        public BmpPlaylistViewModel(IPlaylist bmpPlaylist) { _bmpPlaylist = bmpPlaylist; }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetAndNotify(ref _isReadOnly, value);
        }

        public string Name
        {
            get => _bmpPlaylist.GetName();
            set
            {
                _bmpPlaylist.SetName(value);
                IsReadOnly = true;
            }
        }

        public void RenamePlaylist() { IsReadOnly = false; }
    }
}