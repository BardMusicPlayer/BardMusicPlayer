using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using BardMusicPlayer.Ui.Functions;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;

namespace BardMusicPlayer.Ui.Classic
{
    /// <summary>
    /// Interaktionslogik für Classic_MainView.xaml
    /// </summary>
    public partial class Classic_MainView : UserControl
    {
        private bool _alltracks = false;
        private bool _Playbar_dragStarted = false;
        private bool _Siren_Playbar_dragStarted = false;

        /* Playbuttonstate */
        public void Play_Button_State(bool playing = false)
        {
            if (!playing)
                Play_Button.Content = @"▶";
            else
                Play_Button.Content = @"⏸";
        }

        /* Playback */
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
            {
                PlaybackFunctions.PauseSong();
                Play_Button_State(false);
            }
            else
            {
                PlaybackFunctions.PlaySong(0);
                Play_Button_State(true);
            }
        }

        /* Song Select */
        private void SongName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (PlaybackFunctions.LoadSong())
                {
                    SongName.Text = PlaybackFunctions.GetSongName();
                    InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
                    _directLoaded = true;
                }
            }

            if (e.RightButton == MouseButtonState.Pressed)
                SongName.SelectAll();
        }

        /* All tracks */
        private void all_tracks_button_Click(object sender, RoutedEventArgs e)
        {
            _alltracks = !_alltracks;
            track_cmdDown.IsEnabled = !_alltracks;
            track_cmdUp.IsEnabled = !_alltracks;
            track_txtNum.IsEnabled = !_alltracks;

            if (_alltracks)
            {
                BmpMaestro.Instance.SetTracknumberOnHost(0);
                BmpPigeonhole.Instance.PlayAllTracks = true;
                all_tracks_button.Background = Brushes.LightSteelBlue;
            }
            else
            {
                BmpPigeonhole.Instance.PlayAllTracks = false;
                BmpMaestro.Instance.SetTracknumberOnHost(1);
                NumValue = BmpMaestro.Instance.GetHostBardTrack();
                all_tracks_button.ClearValue(Button.BackgroundProperty);
            }
        }

        private void Rewind_Click(object sender, RoutedEventArgs e)
        {
            PlaybackFunctions.StopSong();
            Play_Button.Content = @"▶";
        }

        private void Loop_Button_Click(object sender, RoutedEventArgs e)
        {
            _directLoaded = !_directLoaded;

            if (_directLoaded)
            {
                Loop_Button.Background = Brushes.LightSteelBlue;
            }
            else
            {
                Loop_Button.ClearValue(Button.BackgroundProperty);
            }
        }

        private void Playbar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void Playbar_Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this._Playbar_dragStarted = true;
        }

        private void Playbar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            BmpMaestro.Instance.SetPlaybackStart((int)((Slider)sender).Value);
            this._Playbar_dragStarted = false;
        }

    }

}
