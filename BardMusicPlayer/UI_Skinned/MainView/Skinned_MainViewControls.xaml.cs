using BardMusicPlayer.Ui.Globals.SkinContainer;
using System.Windows;
using System.Windows.Input;
using BardMusicPlayer.Ui.Functions;
using System.Threading;
using BardMusicPlayer.Maestro;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System;
using BardMusicPlayer.Pigeonhole;

namespace BardMusicPlayer.Ui.Skinned
{
    public partial class Skinned_MainView : System.Windows.Controls.UserControl
    {
        /// <summary>
        ///     load the prev song in the playlist
        /// </summary>
        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Prev_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON];
            _PlaylistView.PlayPrevSong();
        }
        private void Prev_Button_Down(object sender, MouseButtonEventArgs e)
        { this.Prev_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON_ACTIVE]; }
        private void Prev_Button_Up(object sender, MouseButtonEventArgs e)
        { this.Prev_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PREVIOUS_BUTTON]; }

        /// <summary>
        ///     play a loaded song
        /// </summary>
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Play_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON];
            PlaybackFunctions.PlaySong(0);
        }
        private void Play_Button_Down(object sender, MouseButtonEventArgs e)
        { this.Play_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON_ACTIVE];}
        private void Play_Button_Up(object sender, MouseButtonEventArgs e)
        { this.Play_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PLAY_BUTTON];}

        /// <summary>
        ///     pause the song playback
        /// </summary>
        private void Pause_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Pause_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON];
            PlaybackFunctions.PauseSong();
        }
        private void Pause_Button_Down(object sender, MouseButtonEventArgs e)
        { this.Pause_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON_ACTIVE]; }
        private void Pause_Button_Up(object sender, MouseButtonEventArgs e)
        { this.Pause_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_PAUSE_BUTTON]; }

        /// <summary>
        ///     stop song playback
        /// </summary>
        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Stop_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON];
            PlaybackFunctions.StopSong();
        }
        private void Stop_Button_Down(object sender, MouseButtonEventArgs e)
        { this.Stop_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON_ACTIVE]; }
        private void Stop_Button_Up(object sender, MouseButtonEventArgs e)
        { this.Stop_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_STOP_BUTTON]; }

        /// <summary>
        ///     Plays the next song in the playlist
        /// </summary>
        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Next_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON];
            _PlaylistView.PlayNextSong();
        }
        private void Next_Button_Down(object sender, MouseButtonEventArgs e)
        { this.Next_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON_ACTIVE]; }
        private void Next_Button_Up(object sender, MouseButtonEventArgs e)
        { this.Next_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_NEXT_BUTTON]; }

        /// <summary>
        ///     opens a song for single playback
        /// </summary>
        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Load_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON];
            if (PlaybackFunctions.LoadSong())
            {
                Scroller.Cancel();
                Scroller = new CancellationTokenSource();
                UpdateScroller(Scroller.Token, PlaybackFunctions.GetSongName()).ConfigureAwait(false);
                WriteInstrumentDigitField(PlaybackFunctions.GetInstrumentNameForHostPlayer());
            }
        }
        private void Load_Button_Down(object sender, MouseButtonEventArgs e)
        { this.Load_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON_ACTIVE]; }
        private void Load_Button_Up(object sender, MouseButtonEventArgs e)
        { this.Load_Button.Background = SkinContainer.CBUTTONS[SkinContainer.CBUTTON_TYPES.MAIN_EJECT_BUTTON]; }

        /// <summary>
        /// The track selection
        /// </summary>
        private void Trackbar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Trackbar_Slider.Value > MaxTracks)
                return;
            this.Trackbar_Background.Fill = SkinContainer.VOLUME[(SkinContainer.VOLUME_TYPES)Trackbar_Slider.Value];
            WriteSmallDigitField(Trackbar_Slider.Value.ToString());
        }

        private void Trackbar_Slider_DragStarted(object sender, DragStartedEventArgs e)
        { this._Trackbar_dragStarted = true; }

        private void Trackbar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (Trackbar_Slider.Value > MaxTracks)
                Trackbar_Slider.Value = MaxTracks;
            if (Trackbar_Slider.Value == 0)
                BmpPigeonhole.Instance.PlayAllTracks = true;
            else
                BmpPigeonhole.Instance.PlayAllTracks = false;

            BmpMaestro.Instance.SetTracknumberOnHost((int)Trackbar_Slider.Value);
            this._Trackbar_dragStarted = false;
        }

        /// <summary>
        /// The octave shifting
        /// </summary>
        private void Octavebar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Octavebar_Background.Fill = SkinContainer.BALANCE[(SkinContainer.BALANCE_TYPES)Octavebar_Slider.Value];
            WriteSmallOctaveDigitField((Octavebar_Slider.Value - 4).ToString());
        }

        private void Octavebar_Slider_DragStarted(object sender, DragStartedEventArgs e)
        { this._Octavebar_dragStarted = true; }

        private void Octavebar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this._Octavebar_dragStarted = false;
            BmpMaestro.Instance.SetOctaveshiftOnHost((int)Octavebar_Slider.Value - 4);
        }

        /// <summary>
        ///     open the settings
        /// </summary>
        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Settings_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON];
            SettingsView _settings = new SettingsView();
            _settings.Show();
        }
        private void Settings_Button_Down(object sender, MouseButtonEventArgs e)
        { this.Settings_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON_DEPRESSED]; }
        private void Settings_Button_Up(object sender, MouseButtonEventArgs e)
        { this.Settings_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_OPTIONS_BUTTON]; }

        /// <summary>
        ///     close the player
        /// </summary>
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON];
            Scroller.Cancel();
            Application.Current.Shutdown();
        }
        private void Close_Button_Down(object sender, MouseButtonEventArgs e)
        { this.Close_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON_DEPRESSED]; }
        private void Close_Button_Up(object sender, MouseButtonEventArgs e)
        { this.Close_Button.Background = SkinContainer.TITLEBAR[SkinContainer.TITLEBAR_TYPES.MAIN_CLOSE_BUTTON]; }

        /// <summary>
        ///     Show/Hide Playlist
        /// </summary>
        private void Playlist_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_PlaylistView.Visibility == Visibility.Visible)
            {
                _PlaylistView.Visibility = Visibility.Hidden;
                this.Playlist_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON];
            }
            else
            {
                _PlaylistView.Visibility = Visibility.Visible;
                this.Playlist_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_SELECTED];
            }
        }
        private void Playlist_Button_Down(object sender, MouseButtonEventArgs e)
        {
           if (_PlaylistView.Visibility == Visibility.Visible)
                this.Playlist_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_DEPRESSED];
            else
                this.Playlist_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_DEPRESSED_SELECTED];
        }
        private void Playlist_Button_Up(object sender, MouseButtonEventArgs e)
        {
            if (_PlaylistView.Visibility == Visibility.Visible)
                this.Playlist_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON];
            else
                this.Playlist_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_PLAYLIST_BUTTON_SELECTED];
        }

        /// <summary>
        /// sets the playlist to random or normal play
        /// </summary>
        private void Random_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_PlaylistView.NormalPlay)
            {
                _PlaylistView.NormalPlay = false;
                this.Random_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_SELECTED];
            }
            else
            {
                _PlaylistView.NormalPlay = true;
                this.Random_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON];
            }
        }
        private void Random_Button_Down(object sender, MouseButtonEventArgs e)
        {
            if (_PlaylistView.NormalPlay)
                this.Random_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_DEPRESSED];
            else
                this.Random_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_SELECTED_DEPRESSED];
        }
        private void Random_Button_Up(object sender, MouseButtonEventArgs e)
        {
            if (_PlaylistView.NormalPlay)
                this.Random_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON_SELECTED];
            else
                this.Random_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_SHUFFLE_BUTTON];
        }

        /// <summary>
        /// Enables the playlist load next after song stopped
        /// </summary>
        private void Loop_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_PlaylistView.LoopPlay)
            {
                _PlaylistView.LoopPlay = false;
                this.Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON];
            }
            else
            {
                _PlaylistView.LoopPlay = true;
                this.Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_SELECTED];
            }
        }
        private void Loop_Button_Down(object sender, MouseButtonEventArgs e)
        {
            if (_PlaylistView.LoopPlay)
                this.Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_DEPRESSED];
            else
                this.Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_SELECTED_DEPRESSED];
        }
        private void Loop_Button_Up(object sender, MouseButtonEventArgs e)
        {
            if (_PlaylistView.LoopPlay)
                this.Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON];
            else
                this.Loop_Button.Background = SkinContainer.SHUFREP[SkinContainer.SHUFREP_TYPES.MAIN_REPEAT_BUTTON_SELECTED];
        }
    }
}
