using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Pigeonhole;

namespace BardMusicPlayer.UI_Classic;

/// <summary>
/// Interaction logic for Classic_MainView.xaml
/// </summary>
public partial class Classic_MainView
{
    private bool _alltracks;
    private bool _Playbar_dragStarted;
    private bool _Siren_Playbar_dragStarted;

    /* Play button state */
    public void Play_Button_State(bool playing = false) { Play_Button.Content = !playing ? @"▶" : @"⏸"; }

    /* Playback */
    private void Play_Button_Click(object sender, RoutedEventArgs e)
    {
        if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
        {
            PlaybackFunctions.PauseSong();
            Play_Button_State();
        }
        else
        {
            PlaybackFunctions.PlaySong(0);
            Play_Button_State(true);
        }
    }

    // TODO: Needs revision, move off play button into its own logic elsewhere
    private async void Play_Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
            return;

        if (!BmpPigeonhole.Instance.UsePluginForInstrumentOpen)
            return;
        
        var state = PlaybackState.EquipInstruments;

        while (true)
        {
            switch (state)
            {
                case PlaybackState.EquipInstruments:
                    BmpMaestro.Instance.EquipInstruments();
                    state = PlaybackState.Wait;
                    break;
                case PlaybackState.Wait:
                    await Task.Delay(2000);
                    state = PlaybackState.StartEnsCheck;
                    break;
                case PlaybackState.StartEnsCheck:
                    BmpMaestro.Instance.StartEnsCheck();
                    return;
            }
        }
    }

    private enum PlaybackState
    {
        EquipInstruments,
        Wait,
        StartEnsCheck
    }

    /* Song Select */
    private void SongName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (PlaybackFunctions.LoadSong())
            {
                SongName.Text          = PlaybackFunctions.GetSongName();
                InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
                _directLoaded          = true;
            }
        }

        if (e.RightButton == MouseButtonState.Pressed)
            SongName.SelectAll();
    }

    /* All tracks */
    private void all_tracks_button_Click(object sender, RoutedEventArgs e)
    {
        if (PlaybackFunctions.GetSongName() == "please load a song")
        {
            // A song is not loaded, so do nothing.
            return;
        }

        _alltracks              = !_alltracks;
        track_cmdDown.IsEnabled = !_alltracks;
        track_cmdUp.IsEnabled   = !_alltracks;
        track_txtNum.IsEnabled  = !_alltracks;

        if (_alltracks)
        {
            BmpMaestro.Instance.SetTracknumberOnHost(0);
            BmpPigeonhole.Instance.PlayAllTracks = true;
            all_tracks_button.Opacity            = 0.5;
        }
        else
        {
            BmpPigeonhole.Instance.PlayAllTracks = false;
            BmpMaestro.Instance.SetTracknumberOnHost(1);
            NumValue = BmpMaestro.Instance.GetHostBardTrack();
            all_tracks_button.ClearValue(OpacityProperty);
        }
    }

    private void Rewind_Click(object sender, RoutedEventArgs e)
    {
        PlaybackFunctions.StopSong();
        Play_Button.Content = @"▶";
    }

    private void Playbar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
    }

    private void Playbar_Slider_DragStarted(object sender, DragStartedEventArgs e)
    {
        _Playbar_dragStarted = true;
    }

    private void Playbar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        BmpMaestro.Instance.SetPlaybackStart((int)((Slider)sender).Value);
        _Playbar_dragStarted = false;
    }

}