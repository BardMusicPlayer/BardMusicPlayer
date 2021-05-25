using BardMusicPlayer.Seer;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class BardViewModel : Screen
    {
        public BindableCollection<Game> Bards { get; } =
            new(BmpSeer.Instance.Games.Values);

        protected override void OnViewLoaded()
        {
            // TODO: Log when these event happens
            BmpSeer.Instance.GameStarted += g => Bards.Add(g.Game);
            BmpSeer.Instance.GameStopped += g => Bards.Remove(g.Game);
        }

        protected override void OnClose()
        {
            BmpSeer.Instance.GameStarted += g => Bards.Add(g.Game);
            BmpSeer.Instance.GameStopped += g => Bards.Remove(g.Game);
        }
    }
}