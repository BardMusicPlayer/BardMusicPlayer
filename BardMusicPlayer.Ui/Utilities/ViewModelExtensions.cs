using BardMusicPlayer.Coffer;
using BardMusicPlayer.Ui.ViewModels.Playlist;
using StyletIoC;

namespace BardMusicPlayer.Ui.Utilities
{
    public static class ViewModelExtensions
    {
        public static BmpPlaylistViewModel ToViewModel(this IPlaylist playlist, IContainer ioc) =>
            new(ioc, playlist);
    }
}