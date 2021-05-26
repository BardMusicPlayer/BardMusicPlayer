using System;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;
using Microsoft.Win32;
using Stylet;

namespace BardMusicPlayer.Ui.ViewModels
{
    public class PlaylistViewModel : Screen
    {
        private BmpSong? _currentSong;
        private BindableCollection<IPlaylist> _playlists;
        private IPlaylist? _selectedPlaylist;
        private BmpSong? _selectedSong;
        private BindableCollection<BmpSong> _songs;

        public PlaylistViewModel()
        {
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

        public void RemoveSong() { }

        public void CreatePlaylist() { }

        public void ClearPlaylist() { }

        public void DeletePlaylist() { }

        public void LoadPlaylist(IPlaylist playlist) { }
    }
}