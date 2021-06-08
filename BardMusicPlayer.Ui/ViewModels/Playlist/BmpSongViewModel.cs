using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song;
using Stylet;
using StyletIoC;

namespace BardMusicPlayer.Ui.ViewModels.Playlist
{
    public class BmpSongViewModel : Screen
    {
        private readonly IContainer _ioc;

        public BmpSongViewModel(IContainer ioc, BmpSong song)
        {
            _ioc    = ioc;
            BmpSong = song;
        }

        public BmpSong BmpSong { get; set; }

        public List<string> Tags => BmpSong.Tags;

        public string Title => BmpSong.Title;

        // TODO: Let the user change the tags of the song in here
        public void EditTags() { }
    }
}