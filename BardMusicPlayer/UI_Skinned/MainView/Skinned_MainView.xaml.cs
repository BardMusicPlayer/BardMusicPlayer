using BardMusicPlayer.Seer;
using BardMusicPlayer.Ui.Globals.SkinContainer;
using Melanchall.DryWetMidi.Interaction;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.Ui.Functions;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Jamboree;

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    /// Interaktionslogik für Skinned_MainView.xaml
    /// </summary>
    public partial class Skinned_MainView : UserControl
    {
        private int MaxTracks { get; set; } = 1;
        private bool _Playbar_dragStarted = false;
        private bool _Trackbar_dragStarted { get; set; } = false;
        private bool _Octavebar_dragStarted { get; set; } = false;

        private TimeSpan _maxTime;
        private bool _showLapTime { get; set; } = true;

        public Skinned_PlaylistView _PlaylistView;
        public SongbrowserWindow _SongbrowserView;
        public BardsWindow _BardListView;
        public NetworkPlayWindow _Networkplaywindow;
        public Skinned_MainView_Ex _MainView_Ex;
        public InfoBox _infoBox = null;

        private CancellationTokenSource Scroller = new CancellationTokenSource();
        public Skinned_MainView()
        {
            InitializeComponent();
            LoadSkin(BmpPigeonhole.Instance.LastSkin);

            _MainView_Ex = new Skinned_MainView_Ex();
            if (BmpPigeonhole.Instance.SkinnedUi_UseExtendedView)
                _MainView_Ex.Visibility = Visibility.Visible;

            //init the songbrowser
            _SongbrowserView = new SongbrowserWindow();
            _SongbrowserView.Show();
            this._SongbrowserView.Visibility = Visibility.Hidden;
            this._SongbrowserView.OnLoadSongFromBrowser += OnLoadSongFromSongbrowser;


            //open the bards window
            _BardListView = new BardsWindow();
            _BardListView.Show();
            this._BardListView.Visibility = Visibility.Hidden;

            _Networkplaywindow = new NetworkPlayWindow();
            _Networkplaywindow.Show();
            this._Networkplaywindow.Visibility = Visibility.Hidden;

            //open the playlist and bind the event
            _PlaylistView = new Skinned_PlaylistView();
            _PlaylistView.Show();
            _PlaylistView.Top = ((MainWindow)Application.Current.MainWindow).Top + ((MainWindow)Application.Current.MainWindow).ActualHeight;
            _PlaylistView.Left = ((MainWindow)Application.Current.MainWindow).Left;
            _PlaylistView.OnLoadSongFromPlaylist += OnLoadSongFromPlaylist;

            //bind events from maestro
            BmpMaestro.Instance.OnSongLoaded += Instance_OnSongLoaded;
            BmpMaestro.Instance.OnSongMaxTime += Instance_PlaybackMaxTime;
            BmpMaestro.Instance.OnPlaybackTimeChanged += Instance_PlaybackTimeChanged;
            BmpMaestro.Instance.OnTrackNumberChanged += Instance_TrackNumberChanged;
            BmpMaestro.Instance.OnOctaveShiftChanged += Instance_OctaveShiftChanged;
            BmpMaestro.Instance.OnPlaybackStarted += Instance_PlaybackStarted;
            BmpMaestro.Instance.OnPlaybackStopped += Instance_PlaybackStopped;

            //same for seer
            BmpSeer.Instance.ChatLog += Instance_ChatLog;

            //Set the *bar params
            this.Trackbar_Slider.Maximum = 8;
            this.Trackbar_Slider.Value = 1;
            this.Octavebar_Slider.Maximum = 8;
            this.Octavebar_Slider.Minimum = 0;
            this.Octavebar_Slider.Value = 4;

            //if we have selected all tracks in the config use it
            if (BmpPigeonhole.Instance.PlayAllTracks)
                BmpMaestro.Instance.SetTracknumberOnHost(0);

            int track = BmpMaestro.Instance.GetHostBardTrack();
            WriteSmallDigitField(track.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowPositions();
        }

        #region EventCallbacks

        /// <summary>
        /// triggered by the songbrowser if a file should be loaded
        /// </summary>
        private void OnLoadSongFromSongbrowser(object sender, string filename)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (PlaybackFunctions.LoadSong(filename))
                {
                    Scroller.Cancel();
                    Scroller = new CancellationTokenSource();
                    UpdateScroller(Scroller.Token, PlaybackFunctions.GetSongName()).ConfigureAwait(false);
                    WriteInstrumentDigitField(PlaybackFunctions.GetInstrumentNameForHostPlayer());
                }
            }));
        }

        /// <summary>
        /// called from playlist if a song should be loaded
        /// </summary>
        private void OnLoadSongFromPlaylist(object sender, BmpSong e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                //Cancel and rebuild the scroller
                Scroller.Cancel();
                Scroller = new CancellationTokenSource();
                UpdateScroller(Scroller.Token, PlaybackFunctions.GetSongName()).ConfigureAwait(false);
                WriteInstrumentDigitField(PlaybackFunctions.GetInstrumentNameForHostPlayer());

                //if playlist is on autoplay, play next song
                if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYNEXT)
                    PlaybackFunctions.PlaySong(0);
            }));
        }
        private void Instance_OnSongLoaded(object sender, Maestro.Events.SongLoadedEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.OnSongLoaded(e)));
        }
        private void Instance_PlaybackMaxTime(object sender, Maestro.Events.MaxPlayTimeEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.PlaybackMaxTime(e)));
        }
        private void Instance_PlaybackTimeChanged(object sender, Maestro.Events.CurrentPlayPositionEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.PlaybackTimeChanged(e)));
        }
        private void Instance_TrackNumberChanged(object sender, Maestro.Events.TrackNumberChangedEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.UpdateTrackNumberAndInstrument(e)));
        }
        private void Instance_OctaveShiftChanged(object sender, Maestro.Events.OctaveShiftChangedEvent e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.UpdateOctaveShift(e)));
        }

        private void Instance_PlaybackStarted(object sender, bool e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.OnSongStarted()));
        }

        private void Instance_PlaybackStopped(object sender, bool e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.OnSongStopped()));
        }

        private void Instance_ChatLog(Seer.Events.ChatLog seerEvent)
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.AppendChatLog(seerEvent.ChatLogCode, seerEvent.ChatLogLine)));
        }

        /// <summary>
        /// triggered if a song was loaded into maestro
        /// </summary>
        private void OnSongLoaded(Maestro.Events.SongLoadedEvent e)
        {
            MaxTracks = e.MaxTracks;
            if (Trackbar_Slider.Value > MaxTracks)
                Trackbar_Slider.Value = MaxTracks;

            if (BmpMaestro.Instance.GetHostBardTrack() <= MaxTracks)
                return;
            BmpMaestro.Instance.SetTracknumberOnHost(MaxTracks);
        }

        /// <summary>
        /// triggered if we know the max time from meastro
        /// </summary>
        private void PlaybackMaxTime(Maestro.Events.MaxPlayTimeEvent e)
        {
            DisplayPlayTime(e.timeSpan);
            Playbar_Slider.Dispatcher.BeginInvoke(new Action(() => Playbar_Slider.Maximum = e.tick));
            _maxTime = e.timeSpan;
        }

        /// <summary>
        /// triggered via maestro
        /// </summary>
        private void PlaybackTimeChanged(Maestro.Events.CurrentPlayPositionEvent e)
        {
            if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
                DisplayPlayTime(e.timeSpan);
            if (!_Playbar_dragStarted)
                Playbar_Slider.Dispatcher.BeginInvoke(new Action(() => Playbar_Slider.Value = e.tick));
        }

        /// <summary>
        /// update the track and instrument if a track was changed
        /// </summary>
        private void UpdateTrackNumberAndInstrument(Maestro.Events.TrackNumberChangedEvent e)
        {
            if (!e.IsHost)
                return;
            int track = BmpMaestro.Instance.GetHostBardTrack();
            this.Trackbar_Slider.Value = track;
            WriteSmallDigitField(e.TrackNumber.ToString());
            WriteInstrumentDigitField(PlaybackFunctions.GetInstrumentNameForHostPlayer());
        }

        /// <summary>
        /// update the octaveshift slider and displayed value
        /// </summary>
        private void UpdateOctaveShift(Maestro.Events.OctaveShiftChangedEvent e)
        {
            if (!e.IsHost)
                return;
            this.Octavebar_Slider.Value = e.OctaveShift+4;
        }

        /// <summary>
        /// triggered if playback was started
        /// </summary>
        private void OnSongStarted()
        {
            PlaybackFunctions.PlaybackState = PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING;
        }

        /// <summary>
        /// triggered if playback was stopped
        /// </summary>
        private void OnSongStopped()
        {
            if (_PlaylistView.LoopPlay)
                _PlaylistView.PlayNextSong();
            else
                PlaybackFunctions.StopSong();
        }

        /// <summary>
        /// triggered if a chatmsg is comming
        /// </summary>
        /// <param name="code"></param>
        /// <param name="line"></param>
        public void AppendChatLog(string code, string line)
        {
            //The old autostart method with the chat
            if (code == "0039")
            {
                if (line.Contains(@"Anzählen beginnt") ||
                    line.Contains("The count-in will now commence.") ||
                    line.Contains("orchestre est pr"))
                {
                    if (BmpPigeonhole.Instance.AutostartMethod != (int)Globals.Globals.Autostart_Types.VIA_CHAT)
                        return;

                    if (PlaybackFunctions.PlaybackState == PlaybackFunctions.PlaybackState_Enum.PLAYBACK_STATE_PLAYING)
                        return;
                    PlaybackFunctions.PlaySong(3000);
                }
            }
        }
        #endregion


        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                double oLeft = Application.Current.MainWindow.Left;
                double oTop = Application.Current.MainWindow.Top;

                ((MainWindow)System.Windows.Application.Current.MainWindow).DragMove();
                oLeft = (Application.Current.MainWindow.Left - oLeft);
                oTop = (Application.Current.MainWindow.Top - oTop);


                if (this._MainView_Ex.Visibility == Visibility.Visible)
                {
                    Window mainWindow = Application.Current.MainWindow;
                    _MainView_Ex.Width = mainWindow.Width;
                    _MainView_Ex.Top = (mainWindow.Top + mainWindow.Height);
                    _MainView_Ex.Left = mainWindow.Left;

                    _PlaylistView.Width = mainWindow.Width;
                    _PlaylistView.Left = _PlaylistView.Left + oLeft;
                    _PlaylistView.Top = _PlaylistView.Top + oTop;
                    mainWindow = null;
                }
                else
                {
                    Window mainWindow = Application.Current.MainWindow;
                    _PlaylistView.Left = _PlaylistView.Left + oLeft;
                    _PlaylistView.Top = _PlaylistView.Top + oTop;
                }
            }
        }

        private void MainLostFocus(object sender, System.EventArgs e)
        {
            //TitleBar.Fill = _titlebar_image[1];
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
            BmpMaestro.Instance.SetPlaybackStart((int)Playbar_Slider.Value);
            this._Playbar_dragStarted = false;
        }

        private void DisplayPlayTime(TimeSpan t)
        {
            //Display the lap time or the remaining time
            if (!_showLapTime)
                t = _maxTime - t;

            string Seconds = t.ToString().Split(':')[2];
            this.Second_Last.Dispatcher.BeginInvoke(new Action(() => Second_Last.Fill = SkinContainer.NUMBERS[(SkinContainer.NUMBER_TYPES)((Seconds.Length == 1) ? Convert.ToInt32(Seconds) : Convert.ToInt32(Seconds.Substring(1, 1)))]));
            this.Second_First.Dispatcher.BeginInvoke(new Action(() => Second_First.Fill = SkinContainer.NUMBERS[(SkinContainer.NUMBER_TYPES)((Seconds.Length == 1) ? 0 : Convert.ToInt32(Seconds.Substring(0, 1)))]));

            string Minutes = t.ToString().Split(':')[1];
            this.Minutes_Last.Dispatcher.BeginInvoke(new Action(() => Minutes_Last.Fill = SkinContainer.NUMBERS[(SkinContainer.NUMBER_TYPES)((Minutes.Length == 1) ? Convert.ToInt32(Minutes) : Convert.ToInt32(Minutes.Substring(1, 1)))]));
            this.Minutes_First.Dispatcher.BeginInvoke(new Action(() => Minutes_First.Fill = SkinContainer.NUMBERS[(SkinContainer.NUMBER_TYPES)((Minutes.Length == 1) ? 0 : Convert.ToInt32(Minutes.Substring(0, 1)))]));
        }

        private void ShowSongBrowserWindow_Click(object sender, RoutedEventArgs e)
        {
            this._SongbrowserView.Visibility = Visibility.Visible;
        }

        private void ShowBardsWindow_Click(object sender, RoutedEventArgs e)
        {
            this._BardListView.Visibility = Visibility.Visible;
        }

        private void ShowNetworkWindow_Click(object sender, RoutedEventArgs e)
        {
            this._Networkplaywindow.Visibility = Visibility.Visible;
        }

        private void ShowPlaylistWindow_Click(object sender, RoutedEventArgs e)
        {
            this._PlaylistView.Visibility = Visibility.Visible;
        }

        private void ShowExtendedView_Click(object sender, RoutedEventArgs e)
        {
            if (_MainView_Ex.IsVisible)
                this._MainView_Ex.Visibility = Visibility.Hidden;
            else
            {
                this._MainView_Ex.Visibility = Visibility.Visible;
                Window mainWindow = Application.Current.MainWindow;
                _MainView_Ex.Width = mainWindow.Width;
                _MainView_Ex.Top = (mainWindow.Top + mainWindow.Height);
                _MainView_Ex.Left = mainWindow.Left;
            }
            BmpPigeonhole.Instance.SkinnedUi_UseExtendedView = this._MainView_Ex.Visibility == Visibility.Visible ? true : false;
        }

        /// <summary>
        /// Set the exview and playlist position to the main window
        /// </summary>
        private void SetWindowPositions()
        {
            if (this._MainView_Ex.Visibility == Visibility.Visible)
            {
                Window mainWindow = Application.Current.MainWindow;
                _MainView_Ex.Top = (mainWindow.Top + mainWindow.Height);
                _MainView_Ex.Left = mainWindow.Left;

                _PlaylistView.Top = (mainWindow.Top + mainWindow.Height) + 172;
                _PlaylistView.Left = mainWindow.Left;
                mainWindow = null;
            }
            else
            {
                Window mainWindow = Application.Current.MainWindow;
                _PlaylistView.Top = (mainWindow.Top + mainWindow.Height);
                _PlaylistView.Left = mainWindow.Left;
            }
        }

        /// <summary>
        /// Set the exview and playlist position to the main window
        /// </summary>
        private void ResethWindowPositions_Click(object sender, RoutedEventArgs e)
        {
            if (this._MainView_Ex.Visibility == Visibility.Visible)
            {
                Window mainWindow = Application.Current.MainWindow;
                _MainView_Ex.Width = mainWindow.Width;
                _MainView_Ex.Top = (mainWindow.Top + mainWindow.Height);
                _MainView_Ex.Left = mainWindow.Left;

                _PlaylistView.Width = mainWindow.Width;
                _PlaylistView.Top = (mainWindow.Top + mainWindow.Height) + 172;
                _PlaylistView.Left = mainWindow.Left;
                mainWindow = null;
            }
            else
            {
                Window mainWindow = Application.Current.MainWindow;
                _PlaylistView.Width = mainWindow.Width;
                _PlaylistView.Top = (mainWindow.Top + mainWindow.Height);
                _PlaylistView.Left = mainWindow.Left;
            }
        }

        /// <summary>
        /// when clicked on the time digits, toggle between lap and remaining time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Time_Display_Clicked(object sender, MouseButtonEventArgs e)
        {
            _showLapTime = !_showLapTime;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_infoBox == null)
                _infoBox = new InfoBox();
            _infoBox.Visibility = Visibility.Visible;
        }
    }
}
