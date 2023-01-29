using System;
using System.Collections.Generic;
using System.Linq;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Resources;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Functions
{
    /// <summary>
    /// simplified functions Ui is using
    /// </summary>
    public static class PlaylistFunctions
    {
        /// <summary>
        /// Add file(s) to the playlist
        /// </summary>
        /// <param name="currentPlaylist"></param>
        /// <returns>true if success</returns>
        public static bool AddFilesToPlaylist(IPlaylist currentPlaylist)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = Globals.Globals.FileFilters,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
                return false;

            foreach (var d in openFileDialog.FileNames)
            {
                var song = BmpSong.OpenFile(d).Result;

                if(currentPlaylist.SingleOrDefault(x => x.Title.Equals(song.Title)) == null)
                    currentPlaylist.Add(song);

                BmpCoffer.Instance.SaveSong(song);
            }
            BmpCoffer.Instance.SavePlaylist(currentPlaylist);
            return true;
        }

        /// <summary>
        /// Add a folder + subfolders to the playlist
        /// </summary>
        /// <param name="currentPlaylist"></param>
        /// <returns>true if success</returns>
        public static bool AddFolderToPlaylist(IPlaylist currentPlaylist)
        {
            var dlg = new FolderPicker
            {
                InputPath = System.IO.Directory.Exists(Pigeonhole.BmpPigeonhole.Instance.SongDirectory) ? System.IO.Path.GetFullPath(Pigeonhole.BmpPigeonhole.Instance.SongDirectory) : System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            };

            if (dlg.ShowDialog() == true)
            {
                var path = dlg.ResultPath;

                if (!System.IO.Directory.Exists(path))
                    return false;

                var files = System.IO.Directory.EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".mid") || s.EndsWith(".mml") || s.EndsWith(".mmsong")).ToArray();
                foreach (var d in files)
                {
                    var song = BmpSong.OpenFile(d).Result;
                    if (currentPlaylist.SingleOrDefault(x => x.Title.Equals(song.Title)) == null)
                        currentPlaylist.Add(song);
                    BmpCoffer.Instance.SaveSong(song);
                }
                BmpCoffer.Instance.SavePlaylist(currentPlaylist);
                return true;
            }
            return false;
        }

        /// <summary>
        /// gets the first playlist or null if none was found
        /// </summary>
        public static IPlaylist GetFirstPlaylist()
        {
            return BmpCoffer.Instance.GetPlaylistNames().Count > 0 ? BmpCoffer.Instance.GetPlaylist(BmpCoffer.Instance.GetPlaylistNames()[0]) : null;
        }

        /// <summary>
        /// Creates and return a new playlist or return the existing one with the given name
        /// </summary>
        /// <param name="playlistname"></param>
        public static IPlaylist CreatePlaylist(string playlistname)
        {
            return BmpCoffer.Instance.GetPlaylistNames().Contains(playlistname) ? BmpCoffer.Instance.GetPlaylist(playlistname) : BmpCoffer.Instance.CreatePlaylist(playlistname);
        }

        /// <summary>
        /// Get a song from the playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="songname"></param>
        public static BmpSong GetSongFromPlaylist(IPlaylist playlist, string songname)
        {
            return playlist?.FirstOrDefault(item => item.Title == songname);
        }

        /// <summary>
        /// get the song names as list
        /// </summary>
        /// <param name="playlist"></param>
        /// used: classic view
        public static List<string> GetCurrentPlaylistItems(IPlaylist playlist)
        {
            var data = new List<string>();
            if (playlist == null)
                return data;

            data.AddRange(playlist.Select(item => item.Title));
            return data;
        }

        public static IEnumerable<string> GetCurrentPlaylistItems(IPlaylist playlist, bool withupselector = false)
        {
            var data = new List<string>();
            if (playlist == null)
                return data;
            if (withupselector)
                data.Add("..");
            data.AddRange(playlist.Select(item => item.Title));
            return data;
        }

        public static TimeSpan GetTotalTime(IPlaylist playlist)
        {
            var totalTime = new TimeSpan(0);

            return playlist.Aggregate(totalTime, (current, p) => current + p.Duration);
        }
    }
}
