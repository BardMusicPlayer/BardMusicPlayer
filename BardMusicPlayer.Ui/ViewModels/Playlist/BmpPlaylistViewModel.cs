using BardMusicPlayer.Coffer;
using Stylet;
using System.Windows.Input;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class BmpPlaylistViewModel : Screen
    {
        private readonly IPlaylist _bmpPlaylist;
        private readonly PlaylistViewModel _playlistview;

        public BmpPlaylistViewModel(IPlaylist bmpPlaylist, PlaylistViewModel parent)
        {
            _bmpPlaylist = bmpPlaylist;
            _playlistview = parent;
            Name = _bmpPlaylist.GetName();
        }

        public bool IsReadOnly { get; set; } = false;

        public bool IsActivePlaylist { get; set; } = false;

        public IPlaylist GetPlaylist() { return _bmpPlaylist; }

        public string Name { get; set; }

        public void OnNameChanged()
        {
            _bmpPlaylist.SetName(Name);
            BmpCoffer.Instance.SavePlaylist(_bmpPlaylist);
            IsReadOnly = true;
        }

        public void RenamePlaylist() { IsReadOnly = false; }

        public void SetReadOnly() { IsReadOnly = true; }

        public void SelectPlaylist(object sender, MouseButtonEventArgs e)
        {
            if (IsActivePlaylist)
                return;
            IsActivePlaylist = true;
            _playlistview.SelectPlaylist(this);
        }
    }
}