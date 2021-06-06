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
                .Select(playlist => new BmpPlaylistViewModel(playlist))
                .ToBindableCollection();
        }

        public BindableCollection<BmpPlaylistViewModel> Playlists { get; set; }

        public BindableCollection<BmpSong> Songs { get; set; }

        public BmpSong? CurrentSong { get; set; }

        public BmpSong? SelectedSong { get; set; }

        public bool DialogIsOpen { get; set; }

        public DialogueViewModel Dialog { get; set; }

        public IPlaylist? SelectedPlaylist { get; set; }

        public void ChangeSong() { }

        public async Task AddSong()
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
                    BmpCoffer.Instance.SaveSong(bmpSong);
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

            var playlist = BmpCoffer.Instance.CreatePlaylist(name);
            Playlists.Add(new BmpPlaylistViewModel(playlist));
            BmpCoffer.Instance.SavePlaylist(playlist);
        }

        public void RemoveSong() { }

        public void ClearPlaylist() { }

        public void DeletePlaylist() { }

        public void LoadPlaylist(IPlaylist playlist) { }
    }
}