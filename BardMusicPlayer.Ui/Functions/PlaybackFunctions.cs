using System.Collections.Generic;
using System.IO;
using System.Windows;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Transmogrify.Song.Config;
using Microsoft.Win32;

namespace BardMusicPlayer.Ui.Functions
{
    public static class PlaybackFunctions
    {
        /// <summary>
        /// The playback states
        /// </summary>
        public enum PlaybackState_Enum
        {
            PLAYBACK_STATE_STOPPED = 0,
            PLAYBACK_STATE_PLAYING,
            PLAYBACK_STATE_PAUSE,
            PLAYBACK_STATE_PLAYNEXT //indicates the next song should be played
        };
        public static PlaybackState_Enum PlaybackState;

        /// <summary>
        /// The currently loaded song
        /// </summary>
        public static BmpSong CurrentSong { get; set; } = null;

        /// <summary>
        /// Loads a midi file into the sequencer
        /// </summary>
        /// <returns>true if success</returns>
        public static bool LoadSong()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Globals.Globals.DirectoryPath,
                Filter = Globals.Globals.FileFilters,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
                return false;

            if (!openFileDialog.CheckFileExists)
                return false;

            PlaybackState = PlaybackState_Enum.PLAYBACK_STATE_STOPPED;

			Globals.Globals.DirectoryPath = Path.GetDirectoryName(openFileDialog.FileName);

            CurrentSong = BmpSong.OpenFile(openFileDialog.FileName).Result;
            BmpMaestro.Instance.SetSong(CurrentSong);
            return true;
        }

        /// <summary>
        /// Loads a midi file into the sequencer
        /// </summary>
        /// <returns>true if success</returns>
        public static bool LoadSong(string filename)
        {
            if (!File.Exists(filename) || filename == null)
                return false;

            PlaybackState = PlaybackState_Enum.PLAYBACK_STATE_STOPPED;

            CurrentSong = BmpSong.OpenFile(filename).Result;
            BmpMaestro.Instance.SetSong(CurrentSong);
            return true;
        }

        /// <summary>
        /// Load a song from the playlist into the sequencer
        /// </summary>
        /// <param name="item"></param>
        public static void LoadSongFromPlaylist(BmpSong item)
        {
            BringAmpToFront();
            PlaybackState = PlaybackState_Enum.PLAYBACK_STATE_STOPPED;
            CurrentSong = item;
            BmpMaestro.Instance.SetSong(CurrentSong);
        }

        /// <summary>
        /// Starts the performance
        /// </summary>
        public static void PlaySong(int delay)
        {
            PlaybackState = PlaybackState_Enum.PLAYBACK_STATE_PLAYING;
            BmpMaestro.Instance.StartLocalPerformer(delay);
        }

        /// <summary>
        /// Pause the performance
        /// </summary>
        public static void PauseSong()
        {
            PlaybackState = PlaybackState_Enum.PLAYBACK_STATE_PAUSE;
            BmpMaestro.Instance.PauseLocalPerformer();
        }

        /// <summary>
        /// Stops the performance
        /// </summary>
        public static void StopSong()
        {
            PlaybackState = PlaybackState_Enum.PLAYBACK_STATE_STOPPED;
            BmpMaestro.Instance.StopLocalPerformer();
        }

        /// <summary>
        /// Gets the song name from the current song
        /// </summary>
        /// <returns>song name as string</returns>
        public static string GetSongName()
        {
            if (CurrentSong == null)
                return "please load a song";
            return CurrentSong.Title;
        }

        /// <summary>
        /// Gets the instrument from the current song and track
        /// </summary>
        /// <returns>instrument name as string</returns>
        public static string GetInstrumentNameForHostPlayer()
        {
            int tracknumber = BmpMaestro.Instance.GetHostBardTrack();
            if (tracknumber == 0)
                return "All Tracks";
            else
            {
                if (CurrentSong == null)
                    return "No song loaded";
                if (tracknumber > CurrentSong.TrackContainers.Count)
                    return "None";
                try
                {
                    ClassicProcessorConfig classicConfig = (ClassicProcessorConfig)CurrentSong.TrackContainers[tracknumber -1].ConfigContainers[0].ProcessorConfig; // track -1 cuz track 0 isn't in this container
                    return classicConfig.Instrument.Name;
                }
                catch (KeyNotFoundException)
                {
                    return "Unknown";
                }
            }
        }

        /// <summary>
        /// Gets the instrument name from a given song and track
        /// </summary>
        /// <param name="song"></param>
        /// <param name="tracknumber"></param>
        /// <returns>instrument name as string</returns>
        public static string GetInstrumentName(BmpSong song, int tracknumber)
        {
            if (tracknumber == 0)
                return "All Tracks";
            else
            {
                if (song == null)
                    return "No song loaded";
                if (tracknumber > CurrentSong.TrackContainers.Count)
                    return "None";
                try
                {
                    ClassicProcessorConfig classicConfig = (ClassicProcessorConfig)song.TrackContainers[tracknumber-1].ConfigContainers[0].ProcessorConfig;
                    return classicConfig.Instrument.Name;
                }
                catch (KeyNotFoundException)
                {
                    return "Unknown";
                }
            }
        }

        /// <summary>
        /// Helper to bring the player to front
        /// </summary>
        public static void BringAmpToFront()
        {
            if (!BmpPigeonhole.Instance.BringBMPtoFront)
                return;
            try
            {
                System.Windows.Window mainWindow = Application.Current.MainWindow;
                if (!mainWindow.IsVisible)
                {
                    mainWindow.Show();
                }

                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.WindowState = WindowState.Normal;
                }

                mainWindow.Activate();
                mainWindow.Topmost = true;  // important
                mainWindow.Topmost = false; // important
                mainWindow.Focus();         // important
            }
            catch { }
        }
    }
}
