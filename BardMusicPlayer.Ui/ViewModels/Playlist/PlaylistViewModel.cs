using System;
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

        public PlaylistViewModel(IContainer ioc)
        {
            _ioc = ioc;

            var names = BmpCoffer.Instance.GetPlaylistNames();
            Playlists = names.Select(BmpCoffer.Instance.GetPlaylist)
                .Select(playlist => new BmpPlaylistViewModel(ioc, playlist))
                .ToBindableCollection();

            Songs = new BindableCollection<BmpSongViewModel>();
            var titles = BmpCoffer.Instance.GetSongTitles();
            foreach (var s in titles)
            {
                var bmpSong = BmpCoffer.Instance.GetSong(s);
                var songModel = new BmpSongViewModel(_ioc, bmpSong);
                Songs.Add(songModel);
                SelectedSong = songModel;
            }
        }

        public BindableCollection<BmpPlaylistViewModel> Playlists { get; set; }

        public BindableCollection<BmpSongViewModel> Songs { get; set; }

        public BmpSongViewModel? CurrentSong { get; set; }

        public BmpSongViewModel? SelectedSong { get; set; }

        public bool DialogIsOpen { get; set; }

        public DialogueViewModel Dialog { get; set; }

        public IPlaylist? SelectedPlaylist { get; set; }

        public void Handle(SelectPlaylistNotification message) { SelectPlaylist(message.Playlist); }

        public void ChangeSong() { Console.WriteLine(SelectedSong.Title); }

        /// <summary>
        ///     This opens a song or adds it to the current playlist
        /// </summary>
        /// <param name="onthefly"></param>
        public async Task AddSong(bool onthefly = true)
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
                    var songModel = new BmpSongViewModel(_ioc, bmpSong);
                    CurrentSong = songModel;

                    if (!onthefly)
                    {
                        BmpCoffer.Instance.SaveSong(bmpSong);
                        //TODO: Add to playlist
                        Songs.Add(new BmpSongViewModel(_ioc, bmpSong));
                        if (SelectedPlaylist != null)
                        {
                            SelectedPlaylist.Add(bmpSong);
                            BmpCoffer.Instance.SavePlaylist(SelectedPlaylist);
                        }
                    }
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

            SelectedPlaylist = BmpCoffer.Instance.CreatePlaylist(name);
            Playlists.Add(new BmpPlaylistViewModel(_ioc, SelectedPlaylist!));
            BmpCoffer.Instance.SavePlaylist(SelectedPlaylist);
        }

        public void SelectPlaylist(BmpPlaylistViewModel mdl)
        {
            foreach (BmpPlaylistViewModel idx in Playlists)
            {
                idx.IsActivePlaylist = false;
            }

            SelectedPlaylist     = mdl.GetPlaylist();
            mdl.IsActivePlaylist = true;
        }

        public void RemoveSong()
        {
            if (SelectedSong is null)
                return;

            if (SelectedPlaylist is null)
            {
                Songs.Remove(SelectedSong);
                return;
            }

            var idx = 0;
            foreach (var item in SelectedPlaylist)
            {
                if (item.Id.Equals(SelectedSong.BmpSong.Id))
                {
                    SelectedPlaylist.Remove(idx);
                    Songs.Remove(SelectedSong);
                    break;
                }

                idx++;
            }

            BmpCoffer.Instance.SavePlaylist(SelectedPlaylist);
        }

        public void ClearPlaylist()
        {
            if (SelectedPlaylist == null)
                return;

            for (var i = 0; i < SelectedPlaylist.Count(); i++) SelectedPlaylist.Remove(0);
            BmpCoffer.Instance.SavePlaylist(SelectedPlaylist);

            Songs.Clear();
        }

        public void DeletePlaylist()
        {
            if (SelectedPlaylist == null)
                return;

            BmpCoffer.Instance.DeletePlaylist(SelectedPlaylist);
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

        public void LoadPlaylist(IPlaylist playlist)
        {
            Songs.Clear();
            foreach (var s in playlist)
            {
                Songs.Add(new BmpSongViewModel(_ioc, s));
            }

            if (Songs.Count > 0)
                SelectedSong = Songs.First();
        }
    }
}