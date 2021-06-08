using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Utilities;
using BardMusicPlayer.Ui.ViewModels.Dialogue;
using Microsoft.Win32;
using Stylet;
using StyletIoC;

using System.Windows.Controls;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class PlaylistViewModel : Screen
    {
        private readonly IContainer _ioc;

        public PlaylistViewModel(IContainer ioc)
        {
            _ioc = ioc;

            var names = BmpCoffer.Instance.GetPlaylistNames();
            Playlists = names.Select(BmpCoffer.Instance.GetPlaylist)
                .Select(playlist => new BmpPlaylistViewModel(playlist, this))
                .ToBindableCollection();


            Songs = new BindableCollection<BmpSong>();
            var titles = BmpCoffer.Instance.GetSongTitles();
            foreach(var s in titles)
            {
                BmpSong song  = BmpCoffer.Instance.GetSong(s);
                Songs.Add(song);
                SelectedSong = song;
            }


        }

        public BindableCollection<BmpPlaylistViewModel> Playlists { get; set; }

        public BindableCollection<BmpSong> Songs { get; set; }

        public BmpSong? CurrentSong { get; set; }

        public BmpSong? SelectedSong { get; set; }

        public bool DialogIsOpen { get; set; }

        public DialogueViewModel Dialog { get; set; }

        public IPlaylist? SelectedPlaylist { get; set; }

        public void ChangeSong()
        {
            Console.WriteLine(SelectedSong.Title);
        }

        /// <summary>
        /// This opens a song or adds it to the current playlist
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
                    var bmpSong = await BmpSong.OpenMidiFile(openFileDialog.FileName);
                    CurrentSong = bmpSong;
                    if (!onthefly)
                    {
                        BmpCoffer.Instance.SaveSong(bmpSong);
                        //TODO: Add to playlist
                        Songs.Add(bmpSong);
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
            Playlists.Add(new BmpPlaylistViewModel(SelectedPlaylist, this));
            BmpCoffer.Instance.SavePlaylist(SelectedPlaylist);
        }

        public void SelectPlaylist(BmpPlaylistViewModel mdl)
        {
            foreach (BmpPlaylistViewModel idx in Playlists)
            {
                idx.IsActivePlaylist = false;
            }
            SelectedPlaylist = mdl.GetPlaylist();
            mdl.IsActivePlaylist = true;
        }

        public void RemoveSong()
        {
            int idx = 0;
            foreach (var item in SelectedPlaylist.ToList())
            {
                if (item.Id.Equals(SelectedSong.Id))
                {
                    SelectedPlaylist.Remove(idx);
                    Songs.Remove(SelectedSong);
                    break;
                }
                idx++;
            }
            BmpCoffer.Instance.SavePlaylist(SelectedPlaylist);
        }

        public void ClearPlaylist() { }

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
                Songs.Add(s);

            if(Songs.Count > 0)
                SelectedSong = Songs.First();
        }
    }
}