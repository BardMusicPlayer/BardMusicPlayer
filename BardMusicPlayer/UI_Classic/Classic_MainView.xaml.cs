/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Maestro.Old.Events;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;
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

        SongName.Text                             =  PlaybackFunctions.GetSongName();
        BmpMaestro.Instance.OnPlaybackTimeChanged += Instance_PlaybackTimeChanged;
        BmpMaestro.Instance.OnSongMaxTime         += Instance_PlaybackMaxTime;
        BmpMaestro.Instance.OnSongLoaded          += Instance_OnSongLoaded;
        BmpMaestro.Instance.OnPlaybackStarted     += Instance_PlaybackStarted;
        BmpMaestro.Instance.OnPlaybackStopped     += Instance_PlaybackStopped;
        BmpMaestro.Instance.OnTrackNumberChanged  += Instance_TrackNumberChanged;
        BmpMaestro.Instance.OnOctaveShiftChanged  += Instance_OctaveShiftChanged;
        BmpMaestro.Instance.OnSpeedChanged        += Instance_OnSpeedChange;
        //SirenVolume.Value                          =  BmpSiren.Instance.GetVolume();
        BmpSiren.Instance.SynthTimePositionChanged += Instance_SynthTimePositionChanged;
        BmpSiren.Instance.SongLoaded               += Instance_SongLoaded;
        SongBrowser.OnLoadSongFromBrowser          += Instance_SongBrowserLoadedSong;
        SongBrowser.OnAddSongFromBrowser           += Instance_SongBrowserAddSongToPlaylist;
        SongBrowser.OnLoadSongFromBrowserToPreview += Instance_SongBrowserLoadSongToPreview;
        BmpCoffer.Instance.OnPlaylistDataUpdated   += OnPlaylistDataUpdated;
        BmpSeer.Instance.InstrumentHeldChanged     += InstrumentOpenCloseState;

        PreviewKeyDown += PlaybackToggle_PreviewKeyDown;
        LoadConfig();
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

    private void Instance_OnSpeedChange(object? sender, SpeedShiftEvent e)
    {
        Dispatcher.BeginInvoke(new Action(() => SpeedShiftChange(e)));
    }

    private void Instance_SynthTimePositionChanged(string songTitle, double currentTime, double endTime, int activeVoices)
    {
        Dispatcher.BeginInvoke(new Action(() => Siren_PlaybackTimeChanged(currentTime, endTime, activeVoices)));
    }

    private void Instance_SongLoaded(string songTitle)
    {
        Dispatcher.BeginInvoke(new Action(() => SirenSongName.Content = songTitle));
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
        //Song title update
        SongName.Text = PlaybackFunctions.GetSongName();

        //Statistics update
        UpdateStats(e);

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

        speed_cmdReset_Click(null, e: null);
    }

    private void PlaybackStarted()
    {
        PlaybackFunctions.PlaybackState = PlaybackFunctions.PlaybackStateEnum.PlaybackStatePlaying;
        Play_Button_State(true);
    }

    private async void PlaybackStopped()
    {
        PlaybackFunctions.StopSong();
        Play_Button_State();

        //if this wasn't a song from the playlist, do nothing
        if (DirectLoaded)
            return;

        if (_autoPlay)
        {
            var rnd = new Random();
            await Task.Delay(4000); // Wait for last note sent before loading next song
            if (PlaylistContainer.SelectedIndex == PlaylistContainer.Items.Count - 1)
            {
                if (_playlistRepeat)
                {
                    PlaylistContainer.SelectedIndex = 0;
                    PlaybackFunctions.LoadSongFromPlaylist(PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
                    InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();

                    // Wait for instruments before playing
                    if (BmpPigeonhole.Instance.AutoEquipBards)
                    {
                        await Task.Delay(2000);
                    }

                    PlaybackFunctions.PlaySong(2000);
                    Play_Button_State(true);
                }
            }
            else
            {
                PlayNextSong();

                // Wait for instruments before playing
                if (BmpPigeonhole.Instance.AutoEquipBards)
                {
                    await Task.Delay(2000);
                }

                PlaybackFunctions.PlaySong(2000);
                Play_Button_State(true);
            }
        }
    }

    private void PlaybackToggle_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            // Check if the keyboard focus is on a search field
            if (Keyboard.FocusedElement is TextBox)
            {
                // Don't toggle playback state when in a search field
                return;
            }

            if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackStateEnum.PlaybackStatePlaying)
            {
                PlaybackFunctions.PauseSong();
                Play_Button_State();
            }
            else
            {
                Play_Button_State(true);
                PlaybackFunctions.PlaySong(0);
            }

            e.Handled = true;
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

    private void SpeedShiftChange(SpeedShiftEvent e)
    {
        if (e.IsHost)
            SpeedNumValue = e.SpeedShift;
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

        if (_numValue <= 0 || _numValue > _maxTracks)
        {
            if (BmpPigeonhole.Instance.EnsembleKeepTrackSetting)
                return;

            if (BmpPigeonhole.Instance.PlayAllTracks)
            {
                TrackTxtNum.Text = "t" + 0;
                BmpMaestro.Instance.SetTracknumberOnHost(0);
                return;
            }
            
            TrackTxtNum.Text = "t" + 1;
            BmpMaestro.Instance.SetTracknumberOnHost(1);
        }
        
        if (int.TryParse(TrackTxtNum.Text.Replace("t", ""), out _numValue))
        {
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
        }
    }
    private void octave_cmdUp_Click(object sender, RoutedEventArgs e)
    {
        if (OctaveNumValue >= 5)
            return;
        
        OctaveNumValue++;
        BmpMaestro.Instance.SetOctaveShiftOnHost(OctaveNumValue);
    }

    private void octave_cmdDown_Click(object sender, RoutedEventArgs e)
    {
        if (OctaveNumValue <= -5)
            return;
        
        OctaveNumValue--;
        BmpMaestro.Instance.SetOctaveShiftOnHost(OctaveNumValue);
    }

    private void octave_txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (OctaveTxtNum == null)
            return;

        if (OctaveNumValue is < -5 or > 5)
        {
            OctaveTxtNum.Text = @"ø" + 0;
            OctaveNumValue    = 0;
        }

        if (int.TryParse(OctaveTxtNum.Text.Replace(@"ø", ""), out _octaveNumValue))
        {
            OctaveTxtNum.Text = @"ø" + _octaveNumValue;
            BmpMaestro.Instance.SetOctaveShiftOnHost(_octaveNumValue);
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

    #region Speed shift
    private float _speedNumValue = 1.0f;

    private float SpeedNumValue
    {
        get => _speedNumValue;
        set
        {
            _speedNumValue = value;
            var roundedPercentage = (int)Math.Round(value * 100); // Round to the nearest whole number
            SpeedTxtNum.Text = $"{roundedPercentage}%";           // Use string interpolation for formatting
        }
    }

    private void speed_txtNum_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (SpeedTxtNum == null)
            return;

        if (int.TryParse(SpeedTxtNum.Text.Replace(@"%", ""), out var t))
        {
            var speedShift = (Convert.ToDouble(t) / 100).Clamp(0.20f, 2.00f);
            BmpMaestro.Instance.SetSpeedShiftAll((float)speedShift);
        }
    }

    private void speed_cmdUp_Click(object sender, RoutedEventArgs e)
    {
        var increment = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? 0.10f : 0.01f;
        var newSpeedShift = SpeedNumValue + increment;
        if (newSpeedShift < 2.01f) // Ensure the value is not increase above 200% to fix visual bouncing
        {
            BmpMaestro.Instance.SetSpeedShiftAll(newSpeedShift);
        }
    }

    private void speed_cmdDown_Click(object sender, RoutedEventArgs e)
    {
        var increment = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ? 0.10f : 0.01f;
        var newSpeedShift = SpeedNumValue - increment;
        if (newSpeedShift > 0.19f) // Ensure the value is not decreased below 20% to prevent crash
        {
            BmpMaestro.Instance.SetSpeedShiftAll(newSpeedShift);
        }
    }

    private void speed_cmdReset_Click(object? sender, RoutedEventArgs? e)
    {
        const float speedShift = 1.0f;
        BmpMaestro.Instance.SetSpeedShiftAll(speedShift);
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
        _ = BmpSiren.Instance.Load(currentSong);

        //Fill the lyrics editor
        _lyricsData.Clear();
        foreach (var line in currentSong.LyricsContainer)
            _lyricsData.Add(new LyricsContainer(line.Key, line.Value));
        SirenLyrics.DataContext = _lyricsData;
        SirenLyrics.Items.Refresh();
    }
    
    private void RdyCheck_Click(object sender, RoutedEventArgs e)
    {
        BmpMaestro.Instance.StartEnsCheck();
    }

    private void OpenInstrumentButton_Click(object sender, RoutedEventArgs e)
    {
        BmpMaestro.Instance.EquipInstruments();
    }

    private void CloseInstrumentButton_Click(object sender, RoutedEventArgs e)
    {
        if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackStateEnum.PlaybackStatePlaying)
        {
            PlaybackFunctions.PauseSong();
            CurrentInstance?.Play_Button_State();
        }

        BmpMaestro.Instance.StopLocalPerformer();
        BmpMaestro.Instance.UnEquipInstruments();
    }

    private void InstrumentOpenCloseState(InstrumentHeldChanged e)
    {
        if (e.InstrumentHeld.Index == 0)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (CloseInstrumentButton.Visibility != Visibility.Visible)
                        return;

                    OpenInstrumentButton.Visibility  = Visibility.Visible;
                    CloseInstrumentButton.Visibility = Visibility.Hidden;
                }
            ));
        }
        else
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (OpenInstrumentButton.Visibility != Visibility.Visible)
                        return;

                    OpenInstrumentButton.Visibility  = Visibility.Hidden;
                    CloseInstrumentButton.Visibility = Visibility.Visible;
                }
            ));
        }
    }
}