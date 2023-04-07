using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Maestro.Old.Events;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.UI_Classic;

/// <summary>
/// Interaction logic for ClassicMainView.xaml
/// </summary>
public partial class ClassicMainView
{
    private int _maxTracks = 1;
    private bool DirectLoaded { get; set; } //indicates if a song was loaded directly or from playlist
    //private NetworkPlayWindow _networkWindow = null;
    public static ClassicMainView? CurrentInstance { get; private set; }
    public ClassicMainView()
    {
        InitializeComponent();
        CurrentInstance = this;
        //Always start with the playlists
        _showingPlaylists = true;
        //Fill the list
        PlaylistHeader.Header              =  "Playlists";
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
        SirenVolume.Value                          =  BmpSiren.Instance.GetVolume();
        BmpSiren.Instance.SynthTimePositionChanged += Instance_SynthTimePositionChanged;
        SongBrowser.OnLoadSongFromBrowser          += Instance_SongBrowserLoadedSong;
        SongBrowser.OnAddSongFromBrowser           += Instance_SongBrowserAddSongToPlaylist;
        SongBrowser.OnLoadSongFromBrowserToPreview += Instance_SongBrowserLoadSongToPreview;
        BmpCoffer.Instance.OnPlaylistDataUpdated   += OnPlaylistDataUpdated;

        Globals.Globals.OnConfigReload += Globals_OnConfigReload;
        LoadConfig();
    }

    private void Globals_OnConfigReload(object? sender, EventArgs e)
    {
        LoadConfig(true);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        KeyHeat.InitUi();
    }

    #region EventHandler
    private void Instance_PlaybackTimeChanged(object? sender, CurrentPlayPositionEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => PlaybackTimeChanged(e)));
    }

    private void Instance_PlaybackMaxTime(object? sender, MaxPlayTimeEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => PlaybackMaxTime(e)));
    }

    private void Instance_OnSongLoaded(object? sender, SongLoadedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => OnSongLoaded(e)));
    }

    private void Instance_PlaybackStarted(object? sender, bool e)
    {
        Dispatcher.BeginInvoke(new Action(PlaybackStarted));
    }

    private void Instance_PlaybackStopped(object? sender, bool e)
    {
        Dispatcher.BeginInvoke(new Action(PlaybackStopped));
    }

    private void Instance_TrackNumberChanged(object? sender, TrackNumberChangedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => TrackNumberChanged(e)));
    }

    private void Instance_OctaveShiftChanged(object? sender, OctaveShiftChangedEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => OctaveShiftChanged(e)));
    }

    private void Instance_SynthTimePositionChanged(string songTitle, double currentTime, double endTime, int activeVoices)
    {
        Dispatcher.BeginInvoke(new Action(() => Siren_PlaybackTimeChanged(currentTime, endTime, activeVoices)));
    }

    private void PlaybackTimeChanged(CurrentPlayPositionEvent e)
    {
        var seconds = e.timeSpan.Seconds.ToString();
        var minutes = e.timeSpan.Minutes.ToString();
        var time = (minutes.Length == 1 ? "0" + minutes : minutes) + ":" + (seconds.Length == 1 ? "0" + seconds : seconds);
        ElapsedTime.Content = time;

        if (!_playBarDragStarted)
            PlayBarSlider.Value = e.tick;
    }

    private void PlaybackMaxTime(MaxPlayTimeEvent e)
    {
        var seconds = e.timeSpan.Seconds.ToString();
        var minutes = e.timeSpan.Minutes.ToString();
        var time = (minutes.Length == 1 ? "0" + minutes : minutes) + ":" + (seconds.Length == 1 ? "0" + seconds : seconds);
        TotalTime.Content = time;

        PlayBarSlider.Maximum = e.tick;

    }

    private void OnSongLoaded(SongLoadedEvent e)
    {
        //Statistics update
        UpdateStats(e);
        //update heatmap
        KeyHeat.InitiateUi(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);

        if (PlaybackFunctions.PlaybackState != PlaybackFunctions.PlaybackStateEnum.PlaybackStatePlaying)
        {
            PlaybackFunctions.StopSong();
            Play_Button_State();
        }

        _maxTracks = e.MaxTracks;
        if (NumValue > _maxTracks)
            return;
        //NumValue = _maxTracks;

        if (BmpPigeonhole.Instance.PlayAllTracks && !BmpPigeonhole.Instance.EnsembleKeepTrackSetting)
        {
            BmpPigeonhole.Instance.PlayAllTracks = false;
            AllTracksButton.ClearValue(OpacityProperty);
            TrackCmdDown.IsEnabled = true;
            TrackCmdUp.IsEnabled   = true;
            TrackTxtNum.IsEnabled  = true;
            _allTracks             = false;
            //BmpMaestro.Instance.SetTracknumberOnHost(_maxTracks);
        }
    }

    private void PlaybackStarted()
    {
        PlaybackFunctions.PlaybackState = PlaybackFunctions.PlaybackStateEnum.PlaybackStatePlaying;
        Play_Button_State(true);
    }

    private void PlaybackStopped()
    {
        PlaybackFunctions.StopSong();
        Play_Button_State();

        //if this wasn't a song from the playlist, do nothing
        if (DirectLoaded)
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
                PlayNextSong();
                PlaybackFunctions.PlaySong(rnd.Next(15, 35) * 100);
                Play_Button_State(true);
            }
        }
    }

    private void TrackNumberChanged(TrackNumberChangedEvent e)
    {
        if (e.IsHost)
        {
            NumValue = e.TrackNumber;
            UpdateNoteCountForTrack();
        }
    }

    private void OctaveShiftChanged(OctaveShiftChangedEvent e)
    {
        if (e.IsHost)
            OctaveNumValue = e.OctaveShift;
    }
    #endregion

    #region Track UP/Down
    private int _numValue = 1;

    private int NumValue
    {
        get => _numValue;
        set
        {
            _numValue        = value;
            TrackTxtNum.Text = "t" + value;

            //update heatmap
            KeyHeat.InitiateUi(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);
            InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
        }
    }
    private void track_cmdUp_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue >= _maxTracks)
            return;
        NumValue++;
        BmpMaestro.Instance.SetTracknumberOnHost(NumValue);
    }

    private void track_cmdDown_Click(object sender, RoutedEventArgs e)
    {
        if (NumValue <= 1)
            return;
        NumValue--;
        BmpMaestro.Instance.SetTracknumberOnHost(NumValue);
    }

    private void track_txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (TrackTxtNum == null)
            return;

        if (int.TryParse(TrackTxtNum.Text.Replace("t", ""), out _numValue))
        {
            if (_numValue <= 1 || _numValue > _maxTracks)
                return;
            TrackTxtNum.Text = "t" + _numValue;
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
    private int _octaveNumValue = 1;

    private int OctaveNumValue
    {
        get => _octaveNumValue;
        set
        {
            _octaveNumValue   = value;
            OctaveTxtNum.Text = @"ø" + value;
            KeyHeat.InitiateUi(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);
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
        if (OctaveTxtNum == null)
            return;

        if (int.TryParse(OctaveTxtNum.Text.Replace(@"ø", ""), out _octaveNumValue))
        {
            OctaveTxtNum.Text = @"ø" + _octaveNumValue;
            BmpMaestro.Instance.SetOctaveshiftOnHost(_octaveNumValue);
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
        var _ = new MacroLaunchpad
        {
            Visibility = Visibility.Visible
        };
    }

    /// <summary>
    /// triggered by the song browser if a file should be loaded
    /// </summary>
    private void Instance_SongBrowserLoadedSong(object? sender, string? filename)
    {
        if (PlaybackFunctions.LoadSong(filename))
        {
            SongName.Text          = PlaybackFunctions.GetSongName();
            InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
            DirectLoaded           = true;
        }
    }

    /// <summary>
    /// triggered by the song browser if a file should be added to the playlist
    /// </summary>
    private void Instance_SongBrowserAddSongToPlaylist(object? sender, string filename)
    {
        if (_currentPlaylist == null)
            return;

        if (!PlaylistFunctions.AddFilesToPlaylist(_currentPlaylist, filename))
            return;

        PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);

        var icon = "↩".PadRight(2);
        var timeString = new DateTime(PlaylistFunctions.GetTotalTime(_currentPlaylist).Ticks).ToString("HH:mm:ss -").PadRight(4);
        var name = _currentPlaylist.GetName();
        var headerText = $"{icon} {timeString} {name}";

        PlaylistHeader.Header = headerText;
    }

    private void Instance_SongBrowserLoadSongToPreview(object? sender, string filename)
    {
        if (BmpSiren.Instance.IsReadyForPlayback)
            BmpSiren.Instance.Stop();
        IsPlaying              = false;
        SirenPlayPause.Content = "Play";

        var currentSong = BmpSong.OpenFile(filename).Result;
        _                     = BmpSiren.Instance.Load(currentSong);
        SirenSongName.Content = BmpSiren.Instance.CurrentSongTitle;

        //Fill the lyrics editor
        _lyricsData.Clear();
        foreach (var line in currentSong.LyricsContainer)
            _lyricsData.Add(new LyricsContainer(line.Key, line.Value));
        SirenLyrics.DataContext = _lyricsData;
        SirenLyrics.Items.Refresh();
    }
}