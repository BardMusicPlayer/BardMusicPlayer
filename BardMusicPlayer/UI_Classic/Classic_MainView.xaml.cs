using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Maestro.Old.Events;
using BardMusicPlayer.Siren;

namespace BardMusicPlayer.UI_Classic;

/// <summary>
/// Interaction logic for Classic_MainView.xaml
/// </summary>
public partial class Classic_MainView
{
    private int MaxTracks = 1;
    private bool _directLoaded { get; set; } //indicates if a song was loaded directly or from playlist
    //private NetworkPlayWindow _networkWindow = null;
    public static Classic_MainView CurrentInstance { get; private set; }
    public Classic_MainView()
    {
        InitializeComponent();
        CurrentInstance = this;
        //Always start with the playlists
        _showingPlaylists = true;
        //Fill the list
        Playlist_Header.Header             =  "Playlists";
        PlaylistContainer.ItemsSource      =  BmpCoffer.Instance.GetPlaylistNames();
        PlaylistContainer.SelectionChanged += PlaylistContainer_SelectionChanged;

        SongName.Text                              =  PlaybackFunctions.GetSongName();
        BmpMaestro.Instance.OnPlaybackTimeChanged  += Instance_PlaybackTimeChanged;
        BmpMaestro.Instance.OnSongMaxTime          += Instance_PlaybackMaxTime;
        BmpMaestro.Instance.OnSongLoaded           += Instance_OnSongLoaded;
        BmpMaestro.Instance.OnPlaybackStarted      += Instance_PlaybackStarted;
        BmpMaestro.Instance.OnPlaybackStopped      += Instance_PlaybackStopped;
        BmpMaestro.Instance.OnTrackNumberChanged   += Instance_TrackNumberChanged;
        BmpMaestro.Instance.OnOctaveShiftChanged   += Instance_OctaveShiftChanged;
        Siren_Volume.Value                         =  BmpSiren.Instance.GetVolume();
        BmpSiren.Instance.SynthTimePositionChanged += Instance_SynthTimePositionChanged;
        SongBrowser.OnLoadSongFromBrowser          += Instance_SongBrowserLoadedSong;

        Globals.Globals.OnConfigReload += Globals_OnConfigReload;
        LoadConfig();
    }

    private void Globals_OnConfigReload(object sender, EventArgs e)
    {
        LoadConfig(true);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        KeyHeat.InitUi();
    }

    #region EventHandler
    private void Instance_PlaybackTimeChanged(object sender, CurrentPlayPositionEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => PlaybackTimeChanged(e)));
    }

    private void Instance_PlaybackMaxTime(object sender, MaxPlayTimeEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => PlaybackMaxTime(e)));
    }

    private void Instance_OnSongLoaded(object sender, SongLoadedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => OnSongLoaded(e)));
    }

    private void Instance_PlaybackStarted(object sender, bool e)
    {
        Dispatcher.BeginInvoke(new Action(PlaybackStarted));
    }

    private void Instance_PlaybackStopped(object sender, bool e)
    {
        Dispatcher.BeginInvoke(new Action(PlaybackStopped));
    }

    private void Instance_TrackNumberChanged(object sender, TrackNumberChangedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => TrackNumberChanged(e)));
    }

    private void Instance_OctaveShiftChanged(object sender, OctaveShiftChangedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => OctaveShiftChanged(e)));
    }

    private void Instance_SynthTimePositionChanged(string songTitle, double currentTime, double endTime, int activeVoices)
    {
        Dispatcher.BeginInvoke(new Action(() => Siren_PlaybackTimeChanged(currentTime, endTime, activeVoices)));
    }

    private void PlaybackTimeChanged(CurrentPlayPositionEvent e)
    {
        var Seconds = e.timeSpan.Seconds.ToString();
        var Minutes = e.timeSpan.Minutes.ToString();
        var time = (Minutes.Length == 1 ? "0" + Minutes : Minutes) + ":" + (Seconds.Length == 1 ? "0" + Seconds : Seconds);
        ElapsedTime.Content = time;

        if (!_Playbar_dragStarted)
            Playbar_Slider.Value = e.tick;
    }

    private void PlaybackMaxTime(MaxPlayTimeEvent e)
    {
        var Seconds = e.timeSpan.Seconds.ToString();
        var Minutes = e.timeSpan.Minutes.ToString();
        var time = (Minutes.Length == 1 ? "0" + Minutes : Minutes) + ":" + (Seconds.Length == 1 ? "0" + Seconds : Seconds);
        TotalTime.Content = time;

        Playbar_Slider.Maximum = e.tick;

    }

    private void OnSongLoaded(SongLoadedEvent e)
    {
        //Statistics update
        UpdateStats(e);
        //update heatmap
        KeyHeat.initUI(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);

        if (PlaybackFunctions.PlaybackState != PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
            Play_Button_State();

        MaxTracks = e.MaxTracks;
        if (NumValue <= MaxTracks)
            return;
        NumValue = MaxTracks;

        BmpMaestro.Instance.SetTracknumberOnHost(MaxTracks);
    }

    public void PlaybackStarted()
    {
        PlaybackFunctions.PlaybackState = PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING;
        Play_Button_State(true);
    }

    public void PlaybackStopped()
    {
        PlaybackFunctions.StopSong();
        Play_Button_State();

        //if this wasn't a song from the playlist, do nothing
        if (_directLoaded)
            return;

        if (_autoPlay)
        {
            var rnd = new Random();
            if (PlaylistContainer.SelectedIndex == PlaylistContainer.Items.Count - 1)
            {
                if (_playlistRepeat)
                {
                    PlaylistContainer.SelectedIndex = 0;
                    PlaybackFunctions.LoadSongFromPlaylist(PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
                    SongName.Text          = PlaybackFunctions.GetSongName();
                    InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
                    PlaybackFunctions.PlaySong(rnd.Next(15, 35) * 100);
                    Play_Button_State(true);
                }
            }
            else
            {
                playNextSong();
                PlaybackFunctions.PlaySong(rnd.Next(15, 35) * 100);
                Play_Button_State(true);
            }
        }
    }

    public void TrackNumberChanged(TrackNumberChangedEvent e)
    {
        if (e.IsHost)
        {
            NumValue = e.TrackNumber;
            UpdateNoteCountForTrack();
        }
    }

    public void OctaveShiftChanged(OctaveShiftChangedEvent e)
    {
        if (e.IsHost)
            OctaveNumValue = e.OctaveShift;
    }
    #endregion

    #region Track UP/Down
    private int _numValue = 1;
    public int NumValue
    {
        get => _numValue;
        set
        {
            _numValue         = value;
            track_txtNum.Text = "t" + value;

            //update heatmap
            KeyHeat.initUI(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);
            InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
        }
    }
    private void track_cmdUp_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue == MaxTracks)
            return;
        NumValue++;
        BmpMaestro.Instance.SetTracknumberOnHost(NumValue);
    }

    private void track_cmdDown_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue == 1)
            return;
        NumValue--;
        BmpMaestro.Instance.SetTracknumberOnHost(NumValue);
    }

    private void track_txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (track_txtNum == null)
            return;

        if (int.TryParse(track_txtNum.Text.Replace("t", ""), out _numValue))
        {
            if (_numValue < 0 || _numValue > MaxTracks)
                return;
            track_txtNum.Text = "t" + _numValue;
            BmpMaestro.Instance.SetTracknumberOnHost(_numValue);
        }
    }

    private void track_txtNum_KeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                track_cmdUp_Click(sender, e);
                break;
            case Key.Down:
                track_cmdDown_Click(sender, e);
                break;
        }
    }

    #endregion

    #region Octave UP/Down
    private int _octavenumValue = 1;
    public int OctaveNumValue
    {
        get => _octavenumValue;
        set
        {
            _octavenumValue    = value;
            octave_txtNum.Text = @"ø" + value;
            KeyHeat.initUI(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);
        }
    }
    private void octave_cmdUp_Click(object sender, RoutedEventArgs e)
    {
        OctaveNumValue++;
        BmpMaestro.Instance.SetOctaveshiftOnHost(OctaveNumValue);
    }

    private void octave_cmdDown_Click(object sender, RoutedEventArgs e)
    {
        OctaveNumValue--;
        BmpMaestro.Instance.SetOctaveshiftOnHost(OctaveNumValue);
    }

    private void octave_txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (octave_txtNum == null)
            return;

        if (int.TryParse(octave_txtNum.Text.Replace(@"ø", ""), out _octavenumValue))
        {
            octave_txtNum.Text = @"ø" + _octavenumValue;
            BmpMaestro.Instance.SetOctaveshiftOnHost(_octavenumValue);
        }
    }
    private void octave_txtNum_KeyUp(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                octave_cmdUp_Click(sender, e);
                break;
            case Key.Down:
                octave_cmdDown_Click(sender, e);
                break;
        }
    }
    #endregion


    private void Macro_Button_Click(object sender, RoutedEventArgs e)
    {
        var macroLaunchpad = new MacroLaunchpad
        {
            Visibility = Visibility.Visible
        };
    }

    /// <summary>
    /// triggered by the song browser if a file should be loaded
    /// </summary>
    private void Instance_SongBrowserLoadedSong(object sender, string filename)
    {
        if (PlaybackFunctions.LoadSong(filename))
        {
            SongName.Text          = PlaybackFunctions.GetSongName();
            InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
            _directLoaded          = true;
        }
    }
}
