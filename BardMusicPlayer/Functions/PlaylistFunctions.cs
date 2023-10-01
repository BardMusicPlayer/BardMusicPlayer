using System.IO;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Resources;
using BardMusicPlayer.Transmogrify.Song;
using Microsoft.Win32;

namespace BardMusicPlayer.Functions;

/// <summary>
/// simplified functions Ui is using
/// </summary>
public static class PlaylistFunctions
{
    /// <summary>
    /// Add file to the playlist
    /// </summary>
    /// <param name="currentPlaylist"></param>
    /// <param name="filename"></param>
    /// <returns>true if success</returns>
    public static bool AddFilesToPlaylist(IPlaylist? currentPlaylist, string filename)
    {
        var song = BmpSong.OpenFile(filename).Result;
        {
            if (currentPlaylist != null && currentPlaylist.SingleOrDefault(x => x.Title.Equals(song.Title)) == null)
                currentPlaylist.Add(song);
            /*else
            {
                if (BmpCoffer.Instance.IsSongInDatabase(song))
                {
                    var sList = BmpCoffer.Instance.GetSongTitles().Where(x => x.StartsWith(song.Title)).ToList();
                    song.Title = song.Title + "(" + sList.Count() + ")";
                    currentPlaylist.Add(song);
                }
            }*/
            BmpCoffer.Instance.SaveSong(song);
        }
        BmpCoffer.Instance.SavePlaylist(currentPlaylist);
        return true;
    }

    /// <summary>
    /// Add file(s) to the playlist
    /// </summary>
    /// <param name="currentPlaylist"></param>
    /// <returns>true if success</returns>
    public static bool AddFilesToPlaylist(IPlaylist? currentPlaylist)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter      = Globals.Globals.FileFilters,
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return false;

        foreach (var song in openFileDialog.FileNames)
            AddFilesToPlaylist(currentPlaylist, song);
        return true;
    }

    /// <summary>
    /// Overload method : Add single file to playlist with using filepath instead of dialog
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="currentPlaylist"></param>
    /// <returns></returns>
    public static void AddFileToPlaylist(string filePath, IPlaylist? currentPlaylist)
    {
        var song = BmpSong.OpenFile(filePath).Result;

        // Check if the song title is not empty or null
        if (currentPlaylist != null && !string.IsNullOrEmpty(song.Title) && currentPlaylist.SingleOrDefault(x => x.FilePath.Equals(song.FilePath)) == null)
        {
            currentPlaylist.Add(song);
            BmpCoffer.Instance.SaveSong(song);
        }

        BmpCoffer.Instance.SavePlaylist(currentPlaylist);
    }
    /// <summary>
    /// Add a folder + subfolders to the playlist
    /// </summary>
    /// <param name="currentPlaylist"></param>
    /// <returns>true if success</returns>
    public static bool AddFolderToPlaylist(IPlaylist? currentPlaylist)
    {
        var dlg = new FolderPicker
        {
            InputPath = Directory.Exists(BmpPigeonhole.Instance.SongDirectory) ? Path.GetFullPath(BmpPigeonhole.Instance.SongDirectory) : Path.GetDirectoryName(AppContext.BaseDirectory)
        };

        if (dlg.ShowDialog() == true)
        {
            var path = dlg.ResultPath;

            if (!Directory.Exists(path))
                return false;

            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mid") || s.EndsWith(".mml") || s.EndsWith(".mmsong")).ToArray();
            foreach (var d in files)
            {
                var song = BmpSong.OpenFile(d).Result;
                if (currentPlaylist != null && currentPlaylist.SingleOrDefault(x => x.Title.Equals(song.Title)) == null)
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
    public static IPlaylist? GetFirstPlaylist()
    {
        return BmpCoffer.Instance.GetPlaylistNames().Count > 0 ? BmpCoffer.Instance.GetPlaylist(BmpCoffer.Instance.GetPlaylistNames()[0]) : null;
    }

    /// <summary>
    /// Creates and return a new playlist or return the existing one with the given name
    /// </summary>
    /// <param name="playlistName"></param>
    public static IPlaylist? CreatePlaylist(string playlistName)
    {
        return BmpCoffer.Instance.GetPlaylistNames().Contains(playlistName) ? BmpCoffer.Instance.GetPlaylist(playlistName) : BmpCoffer.CreatePlaylist(playlistName);
    }

    /// <summary>
    /// Get a song from the playlist
    /// </summary>
    /// <param name="playlist"></param>
    /// <param name="songName"></param>
    public static BmpSong? GetSongFromPlaylist(IPlaylist? playlist, string? songName)
    {
        return playlist?.FirstOrDefault(item => item.Title == songName);
    }

    /// <summary>
    /// get the song names as list
    /// </summary>
    /// <param name="playlist"></param>
    /// used: classic view

    // public static List<string> GetCurrentPlaylistItems(IPlaylist playlist)
    // {
    //     var data = new List<string>();
    //     if (playlist == null)
    //         return data;
    //
    //     data.AddRange(playlist.Select(item => item.Title));
    //     return data;
    // }

    public static IEnumerable<string> GetCurrentPlaylistItems(IPlaylist? playlist)
    {
        var data = new List<string>();
        if (playlist == null)
            return data;
        data.AddRange(playlist.Select(item => item.Title));
        return data;
    }

    public static IEnumerable<string> GetAllSongsInDb()
    {
        var data = new List<string>();
        data.AddRange(BmpCoffer.Instance.GetSongTitles().Select(item => item));
        return data;
    }

    /// <summary>
    /// Get the total time of all items in the playlist
    /// </summary>
    /// <param name="playlist"></param>
    /// <returns><see cref="TimeSpan"/></returns>
    public static TimeSpan GetTotalTime(IPlaylist? playlist)
    {
        var totalTime = new TimeSpan(0);

        var result = totalTime;
        if (playlist != null) result = playlist.Aggregate(result, (current, song) => current + song.Duration);
        return result;
    }

    /// <summary>
    /// Export a song to Midi
    /// </summary>
    /// <param name="song"></param>
    public static bool ExportSong(BmpSong? song)
    {
        if (song == null)
            return false;

        var saveFileDialog = new SaveFileDialog
        {
            Filter           = "MIDI file (*.mid)|*.mid",
            FilterIndex      = 2,
            RestoreDirectory = true,
            OverwritePrompt  = true
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            Stream myStream;
            if ((myStream = saveFileDialog.OpenFile()) != null)
            {
                song.GetExportMidi().WriteTo(myStream);
                myStream.Close();
                return true;
            }
        }
        return false;
    }
}