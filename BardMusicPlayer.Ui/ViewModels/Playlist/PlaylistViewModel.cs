using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.ViewModels.Dialogue;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class PlaylistViewModel : Screen
    {
        private readonly IContainer _ioc;
        private BmpSong? _currentSong;
        private BindableCollection<IPlaylist> _playlists;
        private IPlaylist? _selectedPlaylist;
        private BmpSong? _selectedSong;
        private BindableCollection<BmpSong> _songs;
        private bool _dialogIsOpen;
        private DialogueViewModel _dialog;

        public PlaylistViewModel(IContainer ioc)
        {
            _ioc = ioc;

            var names = BmpCoffer.Instance.GetPlaylistNames();
            Playlists = new(names.Select(BmpCoffer.Instance.GetPlaylist));
        }

        public BindableCollection<BmpSong> Songs
        {
            get => _songs;
            set => SetAndNotify(ref _songs, value);
        }

        public BindableCollection<IPlaylist> Playlists
        {
            get => _playlists;
            set => SetAndNotify(ref _playlists, value);
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

        public void ShowPlaylist()
        {
            Dialog       = new DialogueViewModel("Playlist name");
            DialogIsOpen = true;
        }

        public void RemoveSong() {}

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

        public void ClearPlaylist()
        {
            
        }

        public void DeletePlaylist() { }

        public void LoadPlaylist(IPlaylist playlist) { }
    }
}