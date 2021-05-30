using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.SongEditor
{
    public class SongEditorViewModel : Screen
    {
        private BmpSong _currentSong;

        public SongEditorViewModel(BmpSong bmpSong)
        {
            _currentSong = bmpSong;
            TrackContainers = new BindableCollection<ConfigContainerViewModel>(bmpSong.TrackContainers
                .SelectMany(container => container.Value.ConfigContainers)
                .Select((t, i) => new ConfigContainerViewModel(t, $"Config {i}")));
        }

        public BindableCollection<ConfigContainerViewModel> TrackContainers { get; }

        public BmpCoffer Playlist => BmpCoffer.Instance;

        public BmpSong CurrentSong
        {
            get => _currentSong;
            set => SetAndNotify(ref _currentSong, value);
        }

        // TODO: Save the song configuration
        public async Task Save() { }

        public void NewConfiguration() { }
    }
}