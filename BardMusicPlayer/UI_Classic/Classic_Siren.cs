using BardMusicPlayer.Siren;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Functions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BardMusicPlayer.Ui.Classic
{
    public class LyricsContainer
    {
        public LyricsContainer(DateTime t, string l) { time = t; line = l; }
        public DateTime time { get; set; }
        public string line { get; set; }
    }
    
    /// <summary>
    /// Interaktionslogik für Classic_MainView.xaml
    /// </summary>
    public sealed partial class Classic_MainView : UserControl
    {
        ObservableCollection<LyricsContainer> lyricsData = new ObservableCollection<LyricsContainer>();

        /// <summary>
        /// load button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Load_Click(object sender, RoutedEventArgs e)
        {
            Siren_VoiceCount.Content = 0;

            BmpSong CurrentSong = null;
            string song = PlaylistContainer.SelectedItem as String;
            if (song == null)
            {
                CurrentSong = Siren_LoadMidiFile();
                if (CurrentSong == null)
                    return;
            }
            else
            {
                if ((string)PlaylistContainer.SelectedItem == "..")
                {
                    CurrentSong = Siren_LoadMidiFile();
                    if (CurrentSong == null)
                        return;
                }
                else
                    CurrentSong = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem);
            }

            _ = BmpSiren.Instance.Load(CurrentSong);
            this.Siren_SongName.Content = BmpSiren.Instance.CurrentSongTitle;
            
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
        /// opens a fileslector box and loads the selected song 
        /// </summary>
        /// <returns>BmpSong</returns>
        private BmpSong Siren_LoadMidiFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Globals.Globals.FileFilters,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
                return null;

            if (!openFileDialog.CheckFileExists)
                return null;

            return BmpSong.OpenFile(openFileDialog.FileName).Result;
        }

        /// <summary>
        /// Control, if user changed the volume
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = e.OriginalSource as Slider;
            BmpSiren.Instance.SetVolume((float)slider.Value);
        }

        /// <summary>
        /// Triggered by Siren event, changes the max and lap time
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="endTime"></param>
        private void Siren_PlaybackTimeChanged(double currentTime, double endTime, int activeVoices)
        {
            //if we are finished, stop the playback
            if (currentTime >= endTime)
                BmpSiren.Instance.Stop();

            Siren_VoiceCount.Content = activeVoices.ToString();

            TimeSpan t;
            if (Siren_Position.Maximum != endTime)
            {
                Siren_Position.Maximum = endTime;
                t = TimeSpan.FromMilliseconds(endTime);
                Siren_TimeLapsed.Content = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            }

            t = TimeSpan.FromMilliseconds(currentTime);
            Siren_Time.Content = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            if (!this._Siren_Playbar_dragStarted)
                Siren_Position.Value = currentTime;

            //Set the lyrics progress
            if (Siren_Lyrics.Items.Count >0)
            {
                List<LyricsContainer> ret = Siren_Lyrics.Items.Cast<LyricsContainer>().ToList();
                int idx = -1;
                foreach (var dt in ret)
                {
                    var ts = new TimeSpan(0, dt.time.Hour, dt.time.Minute, dt.time.Second, dt.time.Millisecond);
                    if (ts >= t)
                        break;
                    idx++;
                }

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
            this._Siren_Playbar_dragStarted = true;
        }

        /// <summary>
        /// Dragaction for the playbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Siren_Playbar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            BmpSiren.Instance.SetPosition((int)Siren_Position.Value);
            this._Siren_Playbar_dragStarted = false;
        }
        
        private void Siren_Lyrics_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var curr = new DateTime(1, 1, 1).AddMilliseconds(Siren_Position.Value);
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (Siren_Lyrics.SelectedIndex == -1)
                    return;

                var idx = Siren_Lyrics.SelectedIndex;
                var t = lyricsData[idx];
                lyricsData.RemoveAt(idx);
                t.time = curr;
                lyricsData.Insert(idx, t);
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (Siren_Lyrics.SelectedIndex == -1)
                    lyricsData.Insert(0, new LyricsContainer(curr, ""));
                else
                    lyricsData.Insert(Siren_Lyrics.SelectedIndex + 1, new LyricsContainer(curr, ""));
            }
            else
                return;
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
            var openFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Performerconfig | *.lrc"
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            var file = new StreamWriter(File.Create(openFileDialog.FileName));
            file.WriteLine("[length:" + BmpSiren.Instance.CurrentSong.Duration.Minutes.ToString() + ":"
                                      + BmpSiren.Instance.CurrentSong.Duration.Seconds.ToString() + "."
                                      + BmpSiren.Instance.CurrentSong.Duration.Milliseconds.ToString() + "]");

            if (BmpSiren.Instance.CurrentSong.DisplayedTitle.Length > 0)
                file.WriteLine("[ti:" + BmpSiren.Instance.CurrentSong.DisplayedTitle + "]");
            else
                file.WriteLine("[ti:" + BmpSiren.Instance.CurrentSong.Title + "]");
            file.WriteLine("[re:LightAmp]");
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
