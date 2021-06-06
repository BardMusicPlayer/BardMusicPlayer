using BardMusicPlayer.Ui.Notifications;
using BardMusicPlayer.Ui.ViewModels.Playlist;
using BardMusicPlayer.Ui.ViewModels.SongEditor;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class TopPageViewModel : Screen
    {
        private readonly IEventAggregator _events;

        public TopPageViewModel(IContainer ioc, IEventAggregator events)
        {
            _events = events;

            BardsViewModel = ioc.Get<BardViewModel>();
            Playlist       = ioc.Get<PlaylistViewModel>();
        }

        public BardViewModel BardsViewModel { get; }

        public PlaylistViewModel Playlist { get; }

        public void LoadSong() { Playlist.AddSong(); }

        public void OpenPlaylist()
        {
            var navigate = new NavigateToNotification(Playlist);
            _events.Publish(navigate);
        }

        public void EditSong()
        {
            if (Playlist.CurrentSong is not null)
            {
                var songEditor = new SongEditorViewModel(Playlist.CurrentSong);
                _events.Publish(new NavigateToNotification(songEditor));
            }
        }
    }
}