using BardMusicPlayer.Seer;
using System;
using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Ui.Functions;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Siren;

namespace BardMusicPlayer.Ui.Classic
{
    /// <summary>
    /// Interaktionslogik für Classic_MainView.xaml
    /// </summary>
    public partial class Classic_MainView : UserControl
    {
        private int MaxTracks = 1;
        private bool _directLoaded { get; set; } = false; //indicates if a song was loaded directly or from playlist
        //private NetworkPlayWindow _networkWindow = null;
        public static Classic_MainView CurrentInstance { get; private set; }
        public Classic_MainView()
        {
            InitializeComponent();
            CurrentInstance = this;
            //Always start with the playlists
            _showingPlaylists = true;
            //Fill the list
            PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
            Playlist_Header.Header = "Playlists";

            this.SongName.Text = PlaybackFunctions.GetSongName();
            BmpMaestro.Instance.OnPlaybackTimeChanged   += Instance_PlaybackTimeChanged;
            BmpMaestro.Instance.OnSongMaxTime           += Instance_PlaybackMaxTime;
            BmpMaestro.Instance.OnSongLoaded            += Instance_OnSongLoaded;
            BmpMaestro.Instance.OnPlaybackStarted       += Instance_PlaybackStarted;
            BmpMaestro.Instance.OnPlaybackStopped       += Instance_PlaybackStopped;
            BmpMaestro.Instance.OnTrackNumberChanged    += Instance_TrackNumberChanged;
            BmpMaestro.Instance.OnOctaveShiftChanged    += Instance_OctaveShiftChanged;
            BmpSeer.Instance.ChatLog                    += Instance_ChatLog;
            Siren_Volume.Value = BmpSiren.Instance.GetVolume();
            BmpSiren.Instance.SynthTimePositionChanged  += Instance_SynthTimePositionChanged;
            SongBrowser.OnLoadSongFromBrowser           += Instance_SongBrowserLoadedSong;

            BmpSeer.Instance.MidibardPlaylistEvent    += Instance_MidibardPlaylistEvent;

            Globals.Globals.OnConfigReload              += Globals_OnConfigReload;
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
        private void Instance_PlaybackTimeChanged(object sender, Maestro.Events.CurrentPlayPositionEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.PlaybackTimeChanged(e)));
        }

        private void Instance_PlaybackMaxTime(object sender, Maestro.Events.MaxPlayTimeEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.PlaybackMaxTime(e)));
        }

        private void Instance_OnSongLoaded(object sender, Maestro.Events.SongLoadedEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.OnSongLoaded(e)));
        }

        private void Instance_PlaybackStarted(object sender, bool e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.PlaybackStarted()));
        }

        private void Instance_PlaybackStopped(object sender, bool e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.PlaybackStopped()));
        }

        private void Instance_TrackNumberChanged(object sender, Maestro.Events.TrackNumberChangedEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.TracknumberChanged(e)));
        }

        private void Instance_OctaveShiftChanged(object sender, Maestro.Events.OctaveShiftChangedEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.OctaveShiftChanged(e)));
        }

        private void Instance_ChatLog(Seer.Events.ChatLog seerEvent)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.AppendChatLog(seerEvent)));
        }

        private void Instance_MidibardPlaylistEvent(Seer.Events.MidibardPlaylistEvent seerEvent)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.SelectSongByIndex(seerEvent.Song)));
        }

        private void Instance_SynthTimePositionChanged(string songTitle, double currentTime, double endTime, int activeVoices)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.Siren_PlaybackTimeChanged(currentTime, endTime, activeVoices)));
        }

        private void PlaybackTimeChanged(Maestro.Events.CurrentPlayPositionEvent e)
        {
            string time;
            string Seconds = e.timeSpan.Seconds.ToString();
            string Minutes = e.timeSpan.Minutes.ToString();
            time = ((Minutes.Length == 1) ? "0" + Minutes : Minutes) + ":" + ((Seconds.Length == 1) ? "0" + Seconds : Seconds);
            ElapsedTime.Content = time;

            if (!_Playbar_dragStarted)
                Playbar_Slider.Value = e.tick;
        }

        private void PlaybackMaxTime(Maestro.Events.MaxPlayTimeEvent e)
        {
            string time;
            string Seconds = e.timeSpan.Seconds.ToString();
            string Minutes = e.timeSpan.Minutes.ToString();
            time = ((Minutes.Length == 1) ? "0" + Minutes : Minutes) + ":" + ((Seconds.Length == 1) ? "0" + Seconds : Seconds);
            TotalTime.Content = time;

            Playbar_Slider.Maximum = e.tick;

        }

        private void OnSongLoaded(Maestro.Events.SongLoadedEvent e)
        {
            //Statistics update
            UpdateStats(e);
            //update heatmap
            KeyHeat.initUI(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);

            if (PlaybackFunctions.PlaybackState != PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
                Play_Button_State(false);

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
            Play_Button_State(false);

            //if this wasn't a song from the playlist, do nothing
            if (_directLoaded)
                return;

            if (BmpPigeonhole.Instance.PlaylistAutoPlay)
            {
                playNextSong();
                Random rnd = new Random();
                PlaybackFunctions.PlaySong(rnd.Next(15, 35)*100);
                Play_Button_State(true);
            }
        }

        public void TracknumberChanged(Maestro.Events.TrackNumberChangedEvent e)
        {
            if (e.IsHost)
            {
                NumValue = e.TrackNumber;
                UpdateNoteCountForTrack();
            }
        }

        public void OctaveShiftChanged(Maestro.Events.OctaveShiftChangedEvent e)
        {
            if (e.IsHost)
                OctaveNumValue = e.OctaveShift;
        }

        public void AppendChatLog(Seer.Events.ChatLog ev)
        {
            if (BmpMaestro.Instance.GetHostPid() == ev.ChatLogGame.Pid)
            {
                BmpChatParser.AppendText(ChatBox, ev);
                this.ChatBox.ScrollToEnd();
            }

            if (ev.ChatLogCode == "0039")
            {
                if (ev.ChatLogLine.Contains(@"Anzählen beginnt") ||
                    ev.ChatLogLine.Contains("The count-in will now commence.") ||
                    ev.ChatLogLine.Contains("orchestre est pr"))
                {
                    if (BmpPigeonhole.Instance.AutostartMethod != (int)Globals.Globals.Autostart_Types.VIA_CHAT)
                        return;
                    if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
                        return;
                    PlaybackFunctions.PlaySong(3000);
                    Play_Button_State(true);
                }
            }
        }
        #endregion

        #region Track UP/Down
        private int _numValue = 1;
        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                track_txtNum.Text = "t" + value.ToString();

                //update heatmap
                KeyHeat.initUI(PlaybackFunctions.CurrentSong, NumValue, OctaveNumValue);
                this.InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
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
                track_txtNum.Text = "t" + _numValue.ToString();
                BmpMaestro.Instance.SetTracknumberOnHost(_numValue);
            }
        }

        private void track_txtNum_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
           switch (e.Key)
            {
                case System.Windows.Input.Key.Up:
                    track_cmdUp_Click(sender, e);
                    break;
                case System.Windows.Input.Key.Down:
                    track_cmdDown_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Octave UP/Down
        private int _octavenumValue = 1;
        public int OctaveNumValue
        {
            get { return _octavenumValue; }
            set
            {
                _octavenumValue = value;
                octave_txtNum.Text = @"ø" + value.ToString();
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
                octave_txtNum.Text = @"ø" + _octavenumValue.ToString();
                BmpMaestro.Instance.SetOctaveshiftOnHost(_octavenumValue);
            }
        }
        private void octave_txtNum_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Up:
                    octave_cmdUp_Click(sender, e);
                    break;
                case System.Windows.Input.Key.Down:
                    octave_cmdDown_Click(sender, e);
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void Info_Button_Click(object sender, RoutedEventArgs e)
        {
            InfoBox _infoBox = new InfoBox();
            _infoBox.Show();
        }

        private void Info_Button_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "BASIC file|*.bas",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            if (!openFileDialog.CheckFileExists)
                return;

            Script.BmpScript.Instance.LoadAndRun(openFileDialog.FileName);
            /*if (_networkWindow == null)
                _networkWindow = new NetworkPlayWindow();
            _networkWindow.Visibility = Visibility.Visible;*/
        }

        /// <summary>
        /// triggered by the songbrowser if a file should be loaded
        /// </summary>
        private void Instance_SongBrowserLoadedSong(object sender, string filename)
        {
            if (PlaybackFunctions.LoadSong(filename))
            {
                SongName.Text = PlaybackFunctions.GetSongName();
                InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
                _directLoaded = true;
            }
        }
    }
}