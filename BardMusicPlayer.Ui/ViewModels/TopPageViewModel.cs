using BardMusicPlayer.Maestro;
using BardMusicPlayer.Ui.Notifications;
using BardMusicPlayer.Ui.ViewModels.Playlist;
using BardMusicPlayer.Ui.ViewModels.SongEditor;
using Melanchall.DryWetMidi.Interaction;
using Stylet;
using StyletIoC;
using System;

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


            BmpMaestro.Instance.OnPlaybackTimeChanged += OnPlaybackTimeChanged;
            BmpMaestro.Instance.OnSongMaxTime += OnSongMaxTime;
        }

        public BardViewModel BardsViewModel { get; }

        public PlaylistViewModel Playlist { get; }

        public TimeSpan SongMaxTime { get; set; }

        public TimeSpan CurrentPlaybackTime { get; set; }

        public int SongProgress => GetSongProgress();

        private void OnSongMaxTime(object sender, ITimeSpan e)
        {
            if (e is MetricTimeSpan mts)
            {
                SongMaxTime = mts;
            }
        }

        private int GetSongProgress()
        {
            if (CurrentPlaybackTime != null && SongMaxTime != null)
            {
                var totalPlayTime = CurrentPlaybackTime.TotalSeconds;
                var maxPlayTime = SongMaxTime.TotalSeconds;

                var totalPercentage = (int)((totalPlayTime * 100) / maxPlayTime);
                return totalPercentage;
            }
            return 0;
        }

        private void OnPlaybackTimeChanged(object sender, ITimeSpan e)
        {
            if (e is MetricTimeSpan mts)
            {
                CurrentPlaybackTime = mts;
            }
        }

        public async void LoadSong() {
            Playlist.RemoveSong();
            await Playlist.AddSongs();
            if (Playlist.CurrentSong != null)
            {
                BmpMaestro.Instance.PlayWithLocalPerformer(Playlist.CurrentSong, -1);
            }
        }

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

        public /*async*/ void PlaySong() {
            BmpMaestro.Instance.StartLocalPerformer();
        }

        public /*async*/ void StopSong()
        {
            BmpMaestro.Instance.StopLocalPerformer();
        }

        public /*async*/ void PauseSong()
        {
            BmpMaestro.Instance.PauseLocalPerformer();
        }
    }
}