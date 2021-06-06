using BardMusicPlayer.Coffer;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class BmpPlaylistViewModel : Screen
    {
        private readonly IPlaylist _bmpPlaylist;

        public BmpPlaylistViewModel(IPlaylist bmpPlaylist) { _bmpPlaylist = bmpPlaylist; }

        public bool IsReadOnly { get; set; }

        public string Name { get; set; }

        public void OnNameChanged()
        {
            _bmpPlaylist.SetName(Name);
            BmpCoffer.Instance.SavePlaylist(_bmpPlaylist);
            IsReadOnly = true;
        }

        public void RenamePlaylist() { IsReadOnly = false; }
    }
}