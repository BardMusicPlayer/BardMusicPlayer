using BardMusicPlayer.Siren;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Functions;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BardMusicPlayer.Ui.Classic
{
    /// <summary>
    /// Interaktionslogik für Classic_MainView.xaml
    /// </summary>
    public sealed partial class Classic_MainView : UserControl
    {
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
    }
}
