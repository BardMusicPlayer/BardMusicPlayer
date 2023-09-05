using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Pigeonhole;

namespace BardMusicPlayer.UI_Classic;

/// <summary>
/// Interaction logic for ClassicMainView.xaml
/// </summary>
public partial class ClassicMainView
{
    private bool _allTracks;
    private bool _playBarDragStarted;
    private bool _sirenPlayBarDragStarted;

    /* Play button state */
    public void Play_Button_State(bool playing = false) { PlayButton.Content = !playing ? @"▶" : @"⏸"; }

    /* Playback */
    private void Play_Button_Click(object sender, RoutedEventArgs e)
    {
        if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackStateEnum.PlaybackStatePlaying)
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
        if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackStateEnum.PlaybackStatePlaying)
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
                default:
                    throw new ArgumentException(null);
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
                InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
                DirectLoaded           = true;
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

        _allTracks             = !_allTracks;
        TrackCmdDown.IsEnabled = !_allTracks;
        TrackCmdUp.IsEnabled   = !_allTracks;
        TrackTxtNum.IsEnabled  = !_allTracks;

        if (_allTracks)
        {
            BmpMaestro.Instance.SetTracknumberOnHost(0);
            BmpPigeonhole.Instance.PlayAllTracks = true;
            AllTracksButton.Opacity              = 0.5;
        }
        else
        {
            BmpPigeonhole.Instance.PlayAllTracks = false;
            BmpMaestro.Instance.SetTracknumberOnHost(1);
            NumValue = BmpMaestro.Instance.GetHostBardTrack();
            AllTracksButton.ClearValue(OpacityProperty);
        }
    }

    private void Rewind_Click(object sender, RoutedEventArgs e)
    {
        PlaybackFunctions.StopSong();
        PlayButton.Content = @"▶";
    }

    private void PlayBar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
    }

    private void PlayBar_Slider_DragStarted(object sender, DragStartedEventArgs e)
    {
        _playBarDragStarted = true;
    }

    private void PlayBar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        BmpMaestro.Instance.SetPlaybackStart((int)((Slider)sender).Value);
        _playBarDragStarted = false;
    }

}