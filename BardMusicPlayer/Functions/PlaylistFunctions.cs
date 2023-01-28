using System;
using System.Collections.Generic;
using System.Linq;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Resources;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Functions
{
    /// <summary>
    /// simplyfied functions both Ui are using
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
                BmpSong song = BmpSong.OpenFile(d).Result;

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
            var dlg = new FolderPicker();

            if (System.IO.Directory.Exists(Pigeonhole.BmpPigeonhole.Instance.SongDirectory))
                dlg.InputPath = System.IO.Path.GetFullPath(Pigeonhole.BmpPigeonhole.Instance.SongDirectory);
            else
                dlg.InputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.ResultPath;

                if (!System.IO.Directory.Exists(path))
                    return false;

                string[] files = System.IO.Directory.EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories).Where(s => s.EndsWith(".mid") || s.EndsWith(".mml") || s.EndsWith(".mmsong")).ToArray();
                foreach (var d in files)
                {
                    BmpSong song = BmpSong.OpenFile(d).Result;
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
        /// <param name="playlistname"></param>
        public static IPlaylist GetFirstPlaylist()
        {
            if (BmpCoffer.Instance.GetPlaylistNames().Count > 0)
                return BmpCoffer.Instance.GetPlaylist(BmpCoffer.Instance.GetPlaylistNames()[0]);
            return null;
        }

        /// <summary>
        /// Creates and return a new playlist or return the existing one with the given name
        /// </summary>
        /// <param name="playlistname"></param>
        public static IPlaylist CreatePlaylist(string playlistname)
        {
            if (BmpCoffer.Instance.GetPlaylistNames().Contains(playlistname))
                return BmpCoffer.Instance.GetPlaylist(playlistname);
            return BmpCoffer.CreatePlaylist(playlistname);
        }

        /// <summary>
        /// Get a song fromt the playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="songname"></param>
        public static BmpSong GetSongFromPlaylist(IPlaylist playlist, string songname)
        {
            if (playlist == null)
                return null;

            foreach (var item in playlist)
            {
                if (item.Title == songname)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// get the songnames as list
        /// </summary>
        /// <param name="playlist"></param>
        /// used: classic view
        public static List<string> GetCurrentPlaylistItems(IPlaylist playlist)
        {
            List<string> data = new List<string>();
            if (playlist == null)
                return data;

            foreach (var item in playlist)
                data.Add(item.Title);
            return data;
        }

        public static List<string> GetCurrentPlaylistItems(IPlaylist playlist, bool withupselector = false)
        {
            List<string> data = new List<string>();
            if (playlist == null)
                return data;
            if (withupselector)
                data.Add("..");
            foreach (var item in playlist)
                data.Add(item.Title);
            return data;
        }

        public static TimeSpan GetTotalTime(IPlaylist playlist)
        {
            TimeSpan totalTime = new TimeSpan(0);
            foreach (var p in playlist)
            {
                totalTime += p.Duration;
            };
            return totalTime;
        }
    }
}
