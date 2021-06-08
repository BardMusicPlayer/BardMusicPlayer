using System.Windows.Input;
using BardMusicPlayer.Coffer;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class BmpPlaylistViewModel : Screen
    {
        private readonly IPlaylist _bmpPlaylist;
        private readonly PlaylistViewModel _playlistview;
        private bool _isActivePlaylist;

        public BmpPlaylistViewModel(IPlaylist bmpPlaylist, PlaylistViewModel parent)
        {
            _bmpPlaylist     = bmpPlaylist;
            _playlistview    = parent;
            Name             = _bmpPlaylist.GetName();
            IsActivePlaylist = false;
        }

        public bool IsActivePlaylist
        {
            get => _isActivePlaylist;
            set
            {
                _isActivePlaylist = value;
                if (value)
                    ActiveColor = "Orange";
                else
                    ActiveColor = "White";
            }
        }

        public bool IsEnabled { get; set; }

        public bool IsReadOnly { get; set; }

        public string ActiveColor { get; set; } = "White";

        public string Name { get; set; }

        public IPlaylist GetPlaylist() => _bmpPlaylist;

        public void OnNameChanged()
        {
            _bmpPlaylist.SetName(Name);
            BmpCoffer.Instance.SavePlaylist(_bmpPlaylist);
            IsReadOnly = true;
        }

        public void RenamePlaylist()
        {
            IsReadOnly = false;
            IsEnabled  = true;
        }

        public void SetReadOnly()
        {
            IsReadOnly = true;
            IsEnabled  = false;
        }

        public void SelectPlaylist(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                IsEnabled  = true;
                IsReadOnly = false;
            }

            if (IsActivePlaylist)
                return;

            IsActivePlaylist = true;
            _playlistview.SelectPlaylist(this);
        }

        public void LoadPlaylist() { _playlistview.LoadPlaylist(_bmpPlaylist); }
    }
}