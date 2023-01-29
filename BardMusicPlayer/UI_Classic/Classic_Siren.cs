using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Transmogrify.Song;
using Microsoft.Win32;

namespace BardMusicPlayer.UI_Classic
{
    public sealed class LyricsContainer
    {
        public LyricsContainer(DateTime t, string l) { time = t; line = l; }
        public DateTime time { get; set; }
        public string line { get; set; }
    }

    /// <summary>
    /// Interaction logic for Classic_MainView.xaml
    /// </summary>
    public sealed partial class Classic_MainView
    {
        ObservableCollection<LyricsContainer> lyricsData = new();

        /// <summary>
        /// load button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Load_Click(object sender, RoutedEventArgs e)
        {
            Siren_VoiceCount.Content = 0;
            BmpSong CurrentSong;
            if (PlaylistContainer.SelectedItem is not string song)
            {
                CurrentSong = Siren_LoadMidiFile();
                if (CurrentSong == null)
                    return;
            }
            else
            {
                if (song == "..")
                {
                    CurrentSong = Siren_LoadMidiFile();
                    if (CurrentSong == null)
                        return;
                }
                else
                    CurrentSong = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, song);
            }

            _ = BmpSiren.Instance.Load(CurrentSong);
            Siren_SongName.Content = BmpSiren.Instance.CurrentSongTitle;

            //Fill the lyrics editor
            lyricsData.Clear();
            foreach (var line in CurrentSong.LyricsContainer)
                lyricsData.Add(new LyricsContainer(line.Key, line.Value));
            Siren_Lyrics.DataContext = lyricsData;
            Siren_Lyrics.Items.Refresh();
        }

        /// <summary>
        /// playback start button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Play_Click(object sender, RoutedEventArgs e)
        {
            if (BmpSiren.Instance.IsReadyForPlayback)
                BmpSiren.Instance.Play();
        }

        /// <summary>
        /// playback pause button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (!BmpSiren.Instance.IsReadyForPlayback)
                return;
            BmpSiren.Instance.Pause();
        }

        private void Siren_Pause_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Right)
            {
                var curr = new DateTime(1, 1, 1).AddMilliseconds(Siren_Position.Value);
                if (Siren_Lyrics.SelectedIndex == -1)
                    return;

                var idx = Siren_Lyrics.SelectedIndex;
                var t = lyricsData[idx];
                lyricsData.RemoveAt(idx);
                t.time = curr;
                lyricsData.Insert(idx, t);

                Siren_Lyrics.DataContext = lyricsData;
                Siren_Lyrics.Items.Refresh();
            }
        }

        /// <summary>
        /// playback stop button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (!BmpSiren.Instance.IsReadyForPlayback)
                return;
            BmpSiren.Instance.Stop();
        }

        /// <summary>
        /// opens a file selector box and loads the selected song 
        /// </summary>
        /// <returns>BmpSong</returns>
        private static BmpSong Siren_LoadMidiFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Globals.Globals.FileFilters,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
                return null;

            return !openFileDialog.CheckFileExists ? null : BmpSong.OpenFile(openFileDialog.FileName).Result;
        }

        /// <summary>
        /// Control, if user changed the volume
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.OriginalSource is Slider slider) BmpSiren.Instance.SetVolume((float)slider.Value);
        }

        /// <summary>
        /// Triggered by Siren event, changes the max and lap time
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="endTime"></param>
        /// <param name="activeVoices"></param>
        private void Siren_PlaybackTimeChanged(double currentTime, double endTime, int activeVoices)
        {
            //if we are finished, stop the playback
            if (currentTime >= endTime)
                BmpSiren.Instance.Stop();

            Siren_VoiceCount.Content = activeVoices.ToString();

            TimeSpan t;
            if (Siren_Position.Maximum != endTime)
            {
                Siren_Position.Maximum   = endTime;
                t                        = TimeSpan.FromMilliseconds(endTime);
                Siren_TimeLapsed.Content = $"{t.Minutes:D2}:{t.Seconds:D2}";
            }

            t                  = TimeSpan.FromMilliseconds(currentTime);
            Siren_Time.Content = $"{t.Minutes:D2}:{t.Seconds:D2}";
            if (!_Siren_Playbar_dragStarted)
                Siren_Position.Value = currentTime;

            //Set the lyrics progress
            if (Siren_Lyrics.Items.Count >0)
            {
                var ret = Siren_Lyrics.Items.Cast<LyricsContainer>().ToList();
                var idx = -1 + ret.Select(dt => new TimeSpan(0, dt.time.Hour, dt.time.Minute, dt.time.Second, dt.time.Millisecond)).TakeWhile(ts => ts < t).Count();

                Siren_Lyrics.SelectedIndex = idx;
                if (Siren_Lyrics.SelectedItem != null)
                    Siren_Lyrics.ScrollIntoView(Siren_Lyrics.SelectedItem);
            }
        }

        /// <summary>
        /// Does nothing atm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Playbar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        /// <summary>
        /// DragStarted, to indicate the slider has moved by user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Playbar_Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            _Siren_Playbar_dragStarted = true;
        }

        /// <summary>
        /// Drag action for the playbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Playbar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            BmpSiren.Instance.SetPosition((int)Siren_Position.Value);
            _Siren_Playbar_dragStarted = false;
        }
        
        private void Siren_Lyrics_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var curr = new DateTime(1, 1, 1).AddMilliseconds(Siren_Position.Value);
            switch (e.ChangedButton)
            {
                case MouseButton.Middle when Siren_Lyrics.SelectedIndex == -1:
                    return;
                case MouseButton.Middle:
                {
                    var idx = Siren_Lyrics.SelectedIndex;
                    var t = lyricsData[idx];
                    lyricsData.RemoveAt(idx);
                    t.time = curr;
                    lyricsData.Insert(idx, t);
                    break;
                }
                case MouseButton.Right when Siren_Lyrics.SelectedIndex == -1:
                    lyricsData.Insert(0, new LyricsContainer(curr, ""));
                    break;
                case MouseButton.Right:
                    lyricsData.Insert(Siren_Lyrics.SelectedIndex + 1, new LyricsContainer(curr, ""));
                    break;
                default:
                    return;
            }
            Siren_Lyrics.DataContext = lyricsData;
            Siren_Lyrics.Items.Refresh();
        }

        private void Siren_Lyrics_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

        }

        /// <summary>
        /// save the lrc to file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Save_LRC_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new SaveFileDialog
            {
                Filter = "Lyrics File | *.lrc"
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            var file = new StreamWriter(File.Create(openFileDialog.FileName));
            file.WriteLine("[length:" + BmpSiren.Instance.CurrentSong.Duration.Minutes + ":"
                                      + BmpSiren.Instance.CurrentSong.Duration.Seconds + "."
                                      + BmpSiren.Instance.CurrentSong.Duration.Milliseconds + "]");

            if (BmpSiren.Instance.CurrentSong.DisplayedTitle.Length > 0)
                file.WriteLine("[ti:" + BmpSiren.Instance.CurrentSong.DisplayedTitle + "]");
            else
                file.WriteLine("[ti:" + BmpSiren.Instance.CurrentSong.Title + "]");
            file.WriteLine("[re:BardMusicPlayer]");
            file.WriteLine("[ve:" + Assembly.GetExecutingAssembly().GetName().Version + "]");

            foreach (var l in lyricsData)
            {
                file.WriteLine("[" + l.time.Minute + ":"
                                   + l.time.Second + "."
                                   + l.time.Millisecond + "]"
                                   + l.line);
            }
            file.Close();

            BmpSiren.Instance.CurrentSong.LyricsContainer.Clear();
            foreach (var l in lyricsData)
                BmpSiren.Instance.CurrentSong.LyricsContainer.Add(l.time, l.line);


        }
    }
}
