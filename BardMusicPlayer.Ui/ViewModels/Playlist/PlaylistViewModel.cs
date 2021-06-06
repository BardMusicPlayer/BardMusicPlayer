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
        private BmpSong? _currentSong;
        private DialogueViewModel _dialog;
        private bool _dialogIsOpen;
        private BindableCollection<BmpPlaylistViewModel> _playlists;
        private IPlaylist? _selectedPlaylist;
        private BmpSong? _selectedSong;
        private BindableCollection<BmpSong> _songs;

        public PlaylistViewModel(IContainer ioc)
        {
            _ioc = ioc;

            var names = BmpCoffer.Instance.GetPlaylistNames();
            Playlists = names.Select(BmpCoffer.Instance.GetPlaylist)
                .Select(playlist => new BmpPlaylistViewModel(playlist))
                .ToBindableCollection();
        }

        public BindableCollection<BmpPlaylistViewModel> Playlists
        {
            get => _playlists;
            set => SetAndNotify(ref _playlists, value);
        }

        public BindableCollection<BmpSong> Songs
        {
            get => _songs;
            set => SetAndNotify(ref _songs, value);
        }

        public BmpSong? CurrentSong
        {
            get => _currentSong;
            set => SetAndNotify(ref _currentSong, value);
        }

        public BmpSong? SelectedSong
        {
            get => _selectedSong;
            set => SetAndNotify(ref _selectedSong, value);
        }

        public bool DialogIsOpen
        {
            get => _dialogIsOpen;
            set => SetAndNotify(ref _dialogIsOpen, value);
        }

        public DialogueViewModel Dialog
        {
            get => _dialog;
            set => SetAndNotify(ref _dialog, value);
        }

        public IPlaylist? SelectedPlaylist
        {
            get => _selectedPlaylist;
            set => SetAndNotify(ref _selectedPlaylist, value);
        }

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