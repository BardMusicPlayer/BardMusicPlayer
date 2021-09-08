using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Notifications;
using BardMusicPlayer.Ui.Utilities;
using BardMusicPlayer.Ui.ViewModels.Dialogue;
using Microsoft.Win32;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class PlaylistViewModel : Screen,
        IHandle<SelectPlaylistNotification>
    {
        private readonly IContainer _ioc;
        private readonly IEventAggregator _events;

        public PlaylistViewModel(IContainer ioc)
        {
            _ioc    = ioc;

            _events = ioc.Get<IEventAggregator>();
            _events.Subscribe(this);

            var names = BmpCoffer.Instance.GetPlaylistNames();
            Playlists = names.Select(BmpCoffer.Instance.GetPlaylist)
                .Select(playlist => playlist.ToViewModel(ioc))
                .ToBindableCollection();
        }

        public BindableCollection<BmpPlaylistViewModel> Playlists { get; set; }

        public BmpPlaylistViewModel? SelectedPlaylist { get; set; }

        public BmpSong? CurrentSong { get; set; }

        public BmpSong? SelectedSong { get; set; }

        public bool DialogIsOpen { get; set; }

        public DialogueViewModel Dialog { get; set; }

        public IEnumerable<BmpSong>? Songs => SelectedPlaylist?.Songs;

        public void Handle(SelectPlaylistNotification message) { SelectPlaylist(message.Playlist); }

        public void ChangeSong() { Console.WriteLine(SelectedSong.Title); }
        
        /// <summary>
        ///     This opens a song or adds it to the current <see cref="IPlaylist" /> provided.
        /// </summary>
        /// <param name="playlist">The <see cref="IPlaylist" /> to add the user selected <see cref="BmpSong" />.</param>
        public async Task AddSongs(IPlaylist? playlist = null)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter      = "MIDI file|*.mid;*.midi|All files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            foreach (var file in openFileDialog.FileNames)
            {
                try
                {
                    var bmpSong = await BmpSong.OpenMidiFile(file);

                    BmpCoffer.Instance.SaveSong(bmpSong);
                    if (playlist is not null)
                    {
                        playlist.Add(bmpSong);
                        BmpCoffer.Instance.SavePlaylist(playlist);
                    }

                    // Select the first song when there is nothing selected yet
                    CurrentSong ??= bmpSong;
                    SelectedPlaylist?.Add(bmpSong);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void CreatePlaylist()
        {
            var regex = new Regex(@"New Playlist [(](\d+)[)]", RegexOptions.Compiled);
            var conflicts = BmpCoffer.Instance
                .GetPlaylistNames()
                .Select(p => regex.Match(p))
                .Where(m => m.Success)
                .ToList();

            var name = "New Playlist";
            if (conflicts.Count > 0)
                name += $" ({conflicts.Max(m => int.Parse(m.Groups[1].Value)) + 1})";
            else
                name += " (1)";

            SelectedPlaylist = BmpCoffer.Instance.CreatePlaylist(name).ToViewModel(_ioc);
            Playlists.Add(SelectedPlaylist);

            BmpCoffer.Instance.SavePlaylist(SelectedPlaylist.Playlist);
        }

        public void SelectPlaylist(BmpPlaylistViewModel model)
        {
            foreach (BmpPlaylistViewModel idx in Playlists)
            {
                idx.IsActivePlaylist = false;
            }

            SelectedPlaylist       = model;
            model.IsActivePlaylist = true;
        }

        public void RemoveSong()
        {
            if (SelectedSong is null || SelectedPlaylist is null)
                return;

            var index = SelectedPlaylist.Songs
                .ToList().IndexOf(SelectedSong);

            if (index > -1)
            {
                SelectedPlaylist.Remove(index);
                BmpCoffer.Instance.SavePlaylist(SelectedPlaylist);
            }
        }

        public void ClearPlaylist()
        {
            if (SelectedPlaylist == null)
                return;

            for (var i = 0; i < SelectedPlaylist.Count(); i++) SelectedPlaylist.Remove(0);
            BmpCoffer.Instance.SavePlaylist(SelectedPlaylist);
        }

        public void DeletePlaylist()
        {
            if (SelectedPlaylist == null)
                return;

            BmpCoffer.Instance.DeletePlaylist(SelectedPlaylist.Playlist);
            SelectedPlaylist = null;
            foreach (BmpPlaylistViewModel idx in Playlists)
            {
                if (idx.IsActivePlaylist)
                {
                    Playlists.Remove(idx);
                    return;
                }
            }
        }
    }
}