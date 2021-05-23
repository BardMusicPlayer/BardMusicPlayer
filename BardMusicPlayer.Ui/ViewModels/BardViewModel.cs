using BardMusicPlayer.Seer;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class BardViewModel : Screen
    {
        // TODO: Reference all the existing Game instances in here.
        public BindableCollection<Game> Bards { get; } = new();
    }
}