using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class BardViewModel : Screen
    {
        private readonly IEventAggregator _events;
        private BindableCollection<Game> _bards = new(BmpSeer.Instance.Games.Values);


        public BardViewModel(IEventAggregator events) { _events = events; }

        public BindableCollection<Game> Bards { get; set; }

        public Game? SelectedBard { get; set; }

        public Instrument? InstrumentHeld
        {
            get
            {
                var held = SelectedBard?.InstrumentHeld;
                var holdingNone = held.Equals(Instrument.None);

                return held is null || holdingNone ? null : held;
            }
        }

        protected override void OnViewLoaded()
        {
            // TODO: Log when these event happens
            BmpSeer.Instance.GameStarted += e => EnsureGameExists(e.Game);
            BmpSeer.Instance.GameStopped += OnInstanceOnGameStopped;

            BmpSeer.Instance.PlayerNameChanged     += OnPlayerNameChanged;
            BmpSeer.Instance.InstrumentHeldChanged += OnInstrumentHeldChanged;
        }

        private void OnPlayerNameChanged(PlayerNameChanged e)
        {
            EnsureGameExists(e.Game);

            _events.Publish(e);
        }

        private void OnInstrumentHeldChanged(InstrumentHeldChanged e)
        {
            EnsureGameExists(e.Game);
            NotifyOfPropertyChange(() => InstrumentHeld);
        }

        private void OnInstanceOnGameStopped(GameStopped g)
        {
            if (g.Game is not null)
                Bards.Remove(g.Game);
            else
                Bards = new BindableCollection<Game>(BmpSeer.Instance.Games.Values);
        }

        private void EnsureGameExists(Game game)
        {
            if (!Bards.Contains(game))
            {
                Bards.Add(game);
                SelectedBard ??= game;
            }
        }
    }
}