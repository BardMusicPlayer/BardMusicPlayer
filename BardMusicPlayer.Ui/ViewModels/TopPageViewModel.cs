using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Notifications;
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
        }

        public BardViewModel BardsViewModel { get; }

        public BmpSong CurrentSong { get; set; }

        public void EditSong() { _events.Publish(new EditSongNotification(CurrentSong)); }
    }
}