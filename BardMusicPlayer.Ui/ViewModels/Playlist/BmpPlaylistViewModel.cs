using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Notifications;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class BmpPlaylistViewModel : Screen, IPlaylist
    {
        private readonly IEventAggregator _events;
        private readonly IContainer _ioc;

        public BmpPlaylistViewModel(IContainer ioc, IPlaylist bmpPlaylist)
        {
            _ioc    = ioc;
            _events = ioc.Get<IEventAggregator>();

            Playlist = bmpPlaylist;

            IsActivePlaylist = false;
        }

        public bool IsActivePlaylist { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsReadOnly { get; set; }

        public IEnumerable<BmpSong> Songs => Playlist;

        public IPlaylist Playlist { get; }

        public string ActiveColor { get; set; } = "White";

        public string Name
        {
            get => Playlist.GetName();
            set => Playlist.SetName(value);
        }

        public IEnumerator<BmpSong> GetEnumerator() => Playlist.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Playlist).GetEnumerator();

        public void Add(BmpSong song) { Playlist.Add(song); }

        public void Add(int idx, BmpSong song) { Playlist.Add(idx, song); }

        public void Move(int source, int target) { Playlist.Move(source, target); }

        public void Remove(int idx) { Playlist.Remove(idx); }
        public void Remove(BmpSong song) { Playlist.Remove(song); }

        public string GetName() => Playlist.GetName();

        public void SetName(string name) { Playlist.SetName(name); }

        public void OnIsActivePlaylistChanged() { ActiveColor = IsActivePlaylist ? "Orange" : "White"; }

        public void OnNameChanged()
        {
            Playlist.SetName(Name);
            BmpCoffer.Instance.SavePlaylist(Playlist);
            IsReadOnly = true;
        }

        public void RenamePlaylist()
        {
            IsReadOnly = false;
            IsEnabled  = true;
        }

        public void SetReadOnly()
        {
            IsReadOnly = true;
            IsEnabled  = false;
        }

        public void SelectPlaylist(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                IsEnabled  = true;
                IsReadOnly = false;
            }

            if (IsActivePlaylist)
                return;

            IsActivePlaylist = true;
            _events.Publish(new SelectPlaylistNotification(this));
        }
    }
}