using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels.SongEditor
{
    public class SongEditorViewModel : Screen
    {
        public SongEditorViewModel(BmpSong bmpSong)
        {

            CurrentSong = bmpSong;
            TrackContainers = new BindableCollection<ConfigContainerViewModel>(bmpSong.TrackContainers
                .SelectMany(container => container.Value.ConfigContainers)
                .Select((t, i) => new ConfigContainerViewModel(t, $"Config {i}")));
        }

        public BindableCollection<ConfigContainerViewModel> TrackContainers { get; }

        public BmpCoffer Playlist => BmpCoffer.Instance;

        public BmpSong CurrentSong { get; set; }

        // TODO: Save the song configuration
        public async Task Save() { await Task.Run(() => { throw new System.NotImplementedException(); }); }

        public void NewConfiguration() { throw new System.NotImplementedException(); }
    }
}