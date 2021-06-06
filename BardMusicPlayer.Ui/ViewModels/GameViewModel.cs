using System;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class GameViewModel : Screen, IDisposable,
        IHandle<PlayerNameChanged>,
        IHandle<HomeWorldChanged>
    {
        private readonly IEventAggregator _events;


        public GameViewModel(IContainer ioc, Game game)
        {
            _events = ioc.Get<IEventAggregator>();
            _events.Subscribe(this);

            Game = game;
        }

        public Game Game { get; }

        public string HomeWorld { get; set; }

        public string PlayerName { get; set; }

        public void Dispose()
        {
            Game?.Dispose();
            _events.Unsubscribe(this);
        }

        public void Handle(HomeWorldChanged message)
        {
            if (!message.Game.Equals(Game))
                return;

            HomeWorld = message.HomeWorld;
        }

        public void Handle(PlayerNameChanged message)
        {
            if (!message.Game.Equals(Game))
                return;

            PlayerName = message.PlayerName;
        }
    }
}