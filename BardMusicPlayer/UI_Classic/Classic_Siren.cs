using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Transmogrify.Song;
using Microsoft.Win32;

namespace BardMusicPlayer.UI_Classic;

public sealed class LyricsContainer
{
    public LyricsContainer(DateTime t, string l) { Time = t; Line = l; }
    public DateTime Time { get; set; }
    public string Line { get; set; }
}

/// <summary>
/// Interaction logic for ClassicMainView.xaml
/// </summary>
public sealed partial class ClassicMainView
{
    private readonly ObservableCollection<LyricsContainer> _lyricsData = new();

    /// <summary>
    /// load button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Siren_Load_Click(object sender, RoutedEventArgs e)
    {
        SirenVoiceCount.Content = 0;
        BmpSong? currentSong;
        if (PlaylistContainer.SelectedItem is not string song)
        {
            currentSong = Siren_LoadMidiFile();
            if (currentSong == null)
                return;
            IsPlaying              = false;
            SirenPlayPause.Content = "Play";
        }
        else if (_currentPlaylist != null)
        {
            currentSong            = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, song);
            IsPlaying              = false;
            SirenPlayPause.Content = "Play";
        }
        else
        {
            // Handle the case where _currentPlaylist is null
            return;
        }

        _                     = BmpSiren.Instance.Load(currentSong);
        SirenSongName.Content = BmpSiren.Instance.CurrentSongTitle;

        //Fill the lyrics editor
        _lyricsData.Clear();
        if (currentSong != null)
        {
            foreach (var line in currentSong.LyricsContainer)
                _lyricsData.Add(new LyricsContainer(line.Key, line.Value));
        }

        SirenLyrics.DataContext = _lyricsData;
        SirenLyrics.Items.Refresh();
    }

    
    private bool IsPlaying { get; set; }

    /// <summary>
    /// playback start / pause button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Siren_PlayPause_Click(object sender, RoutedEventArgs e)
    {
        if (BmpSiren.Instance.IsReadyForPlayback)
        {
            if (IsPlaying)
            {
                BmpSiren.Instance.Pause();
                IsPlaying              = false;
                SirenPlayPause.Content = "Play";
            }
            else
            {
                BmpSiren.Instance.Play();
                IsPlaying              = true;
                SirenPlayPause.Content = "Pause";
            }
        }
    }

    private void Siren_Pause_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Right)
        {
            var curr = new DateTime(1, 1, 1).AddMilliseconds(SirenPosition.Value);
            if (SirenLyrics.SelectedIndex == -1)
                return;

            var idx = SirenLyrics.SelectedIndex;
            var t = _lyricsData[idx];
            _lyricsData.RemoveAt(idx);
            t.Time = curr;
            _lyricsData.Insert(idx, t);

            // Use the Dispatcher to schedule the refresh
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SirenLyrics.DataContext = _lyricsData;
                SirenLyrics.Items.Refresh();
            }));
        }
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
        IsPlaying              = false;
        SirenPlayPause.Content = "Play";
    }

    /// <summary>
    /// opens a file selector box and loads the selected song 
    /// </summary>
    /// <returns>BmpSong</returns>
    private static BmpSong? Siren_LoadMidiFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter      = Globals.Globals.FileFilters,
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return null;

        return !openFileDialog.CheckFileExists ? null : BmpSong.OpenFile(openFileDialog.FileName).Result;
    }

    /// <summary>
    /// Control, if user changed the volume
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Siren_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (e.OriginalSource is Slider slider)
        {
            slider.Minimum = 0;
            slider.Maximum = 30;
            BmpSiren.Instance.SetVolume((float)slider.Value, max:20);
        }
    }

    /// <summary>
    /// Triggered by Siren event, changes the max and lap time
    /// </summary>
    /// <param name="currentTime"></param>
    /// <param name="endTime"></param>
    /// <param name="activeVoices"></param>
    private void Siren_PlaybackTimeChanged(double currentTime, double endTime, int activeVoices)
    {
        //if we are finished, stop the playback
        if (currentTime >= endTime)
        {
            BmpSiren.Instance.Stop();
            IsPlaying              = false;
            SirenPlayPause.Content = "Play";
        }

        SirenVoiceCount.Content = activeVoices.ToString();

        TimeSpan t;
        const float tolerance = 0.0001f;                           // define a small tolerance value
        if (Math.Abs(SirenPosition.Maximum - endTime) > tolerance) // use tolerance to compare values
        {
            SirenPosition.Maximum   = endTime;
            t                       = TimeSpan.FromMilliseconds(endTime);
            SirenTimeLapsed.Content = $"{t.Minutes:D2}:{t.Seconds:D2}";
        }

        t                 = TimeSpan.FromMilliseconds(currentTime);
        SirenTime.Content = $"{t.Minutes:D2}:{t.Seconds:D2}";
        if (!_sirenPlayBarDragStarted)
            SirenPosition.Value = currentTime;

        //Set the lyrics progress
        if (SirenLyrics.Items.Count > 0)
        {
            var ret = SirenLyrics.Items.Cast<LyricsContainer>().ToList();
            var idx = -1 + ret
                .Select(dt => new TimeSpan(0, dt.Time.Hour, dt.Time.Minute, dt.Time.Second, dt.Time.Millisecond))
                .TakeWhile(ts => ts < t).Count();

            SirenLyrics.SelectedIndex = idx;
            if (SirenLyrics.SelectedItem != null)
                SirenLyrics.ScrollIntoView(SirenLyrics.SelectedItem);
        }
    }

    /// <summary>
    /// Does nothing atm
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Siren_PlayBar_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { }

    /// <summary>
    /// DragStarted, to indicate the slider has moved by user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Siren_PlayBar_Slider_DragStarted(object sender, DragStartedEventArgs e)
    {
        _sirenPlayBarDragStarted = true;
    }

    /// <summary>
    /// Drag action for the play bar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Siren_PlayBar_Slider_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        BmpSiren.Instance.SetPosition((int)SirenPosition.Value);
        _sirenPlayBarDragStarted = false;
    }

    private void Siren_Lyrics_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var curr = new DateTime(1, 1, 1).AddMilliseconds(SirenPosition.Value);
        switch (e.ChangedButton)
        {
            case MouseButton.Middle when SirenLyrics.SelectedIndex == -1:
                return;
            case MouseButton.Middle:
            {
                var idx = SirenLyrics.SelectedIndex;
                var t = _lyricsData[idx];
                _lyricsData.RemoveAt(idx);
                t.Time = curr;
                _lyricsData.Insert(idx, t);
                break;
            }
            case MouseButton.Right when SirenLyrics.SelectedIndex == -1:
                _lyricsData.Insert(0, new LyricsContainer(curr, ""));
                break;
            case MouseButton.Right:
                _lyricsData.Insert(SirenLyrics.SelectedIndex + 1, new LyricsContainer(curr, ""));
                break;
            default:
                return;
        }

        SirenLyrics.CommitEdit();
        SirenLyrics.DataContext = _lyricsData;
    }

    private void Siren_Lyrics_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) { }

    /// <summary>
    /// save the lrc to file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Siren_Save_LRC_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new SaveFileDialog
        {
            Filter = "Lyrics File | *.lrc"
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        if (BmpSiren.Instance.CurrentSong != null)
        {
            var file = new StreamWriter(File.Create(openFileDialog.FileName));
            file.WriteLine("[length:" + BmpSiren.Instance.CurrentSong.Duration.Minutes + ":"
                           + BmpSiren.Instance.CurrentSong.Duration.Seconds + "."
                           + BmpSiren.Instance.CurrentSong.Duration.Milliseconds + "]");

            if (BmpSiren.Instance.CurrentSong.DisplayedTitle?.Length > 0)
                file.WriteLine("[ti:" + BmpSiren.Instance.CurrentSong.DisplayedTitle + "]");
            else
                file.WriteLine("[ti:" + BmpSiren.Instance.CurrentSong.Title + "]");
            file.WriteLine("[re:BardMusicPlayer]");
            file.WriteLine("[ve:" + Assembly.GetExecutingAssembly().GetName().Version + "]");

            // show an error message or take some other appropriate action
            foreach (var l in _lyricsData)
            {
                file.WriteLine("[" + l.Time.Minute + ":"
                               + l.Time.Second + "."
                               + l.Time.Millisecond + "]"
                               + l.Line);
            }

            file.Close();

            BmpSiren.Instance.CurrentSong.LyricsContainer.Clear();
            foreach (var l in _lyricsData)
            {
                BmpSiren.Instance.CurrentSong.LyricsContainer.TryAdd(l.Time, l.Line);
            }
        }
    }
}