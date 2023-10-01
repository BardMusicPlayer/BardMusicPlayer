/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Controls;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Resources;
using BardMusicPlayer.Transmogrify.Song;
using Microsoft.Win32;

namespace BardMusicPlayer.UI_Classic;

/// <summary>
/// Interaction logic for ClassicMainView.xaml
/// </summary>
public partial class ClassicMainView
{
    private bool _playlistRepeat;
    private bool _playlistShuffle;
    private bool _showingPlaylists; //are we displaying the playlist or the songs
    private bool _autoPlay = BmpPigeonhole.Instance.PlaylistAutoPlay;
    private IPlaylist? _currentPlaylist; //the current selected playlist
    private int _currentShuffleIndex;
    private List<string>? _shuffledPlaylist = new();

    /// <summary>
    /// Plays the next song from the playlist
    /// </summary>
    private async void PlayNextSong()
    {
        if (_currentPlaylist == null)
            return;

        if (PlaylistContainer.Items.Count == 0)
            return;

        if (_playlistShuffle)
        {
            if (_shuffledPlaylist != null && _currentShuffleIndex == _shuffledPlaylist.Count - 1)
            {
                PlaybackFunctions.StopSong();
                Play_Button_State();
                return;
            }

            _currentShuffleIndex++;
            PlaylistContainer.SelectedIndex = _currentShuffleIndex;
        }
        else
        {
            var nextIndex = PlaylistContainer.SelectedIndex + 1;

            if (nextIndex >= PlaylistContainer.Items.Count)
            {
                if (_playlistRepeat && (_autoPlay || !_autoPlay && PlaylistContainer.SelectedIndex == PlaylistContainer.Items.Count - 1))
                {
                    nextIndex = 0;
                }
                else
                {
                    PlaybackFunctions.StopSong();
                    Play_Button_State();
                    return;
                }
            }

            PlaylistContainer.SelectedIndex = nextIndex;
        }

        PlaybackFunctions.LoadSongFromPlaylist(PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
        InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();

        // Repeat the playlist if the last song is playing and the repeat button is enabled
        if (_playlistRepeat && PlaylistContainer.SelectedIndex == 0)
        {
            PlaylistContainer.SelectedIndex = PlaylistContainer.Items.Count - 1;
            PlaybackFunctions.LoadSongFromPlaylist(PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
            InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
        }
    
        // Send ensemble if there are 2 or more performers
        // TODO: Check enabled performers instead to not send ensemble for solo songs in a playlist.
        // TODO: Check if ensemble is already active before sending again. Auto-Equip stops ensemble on song load (instrument change) but if that isn't being used, loading next song may not start a new ensemble.
        if (BmpMaestro.Instance.GetAllPerformers().Count() >= 2)
        {
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
    }

    private void PlaylistContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_playlistShuffle)
        {
            if (_shuffledPlaylist != null)
                _currentShuffleIndex = _shuffledPlaylist.IndexOf((string)PlaylistContainer.SelectedItem);
        }
    }

    #region upper playlist button functions
    /// <summary>
    /// Create a new playlist but don't save it
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Playlist_New_Button_Click(object? sender, RoutedEventArgs? e)
    {
        var inputBox = new TextInputWindow("Playlist Name", windowTitle: "Create new playlist");
        if (inputBox.ShowDialog() == true)
        {
            if (inputBox.ResponseText.Length < 1)
                return;

            _currentPlaylist              = PlaylistFunctions.CreatePlaylist(inputBox.ResponseText);
            PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);
            _showingPlaylists             = false;

            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);

            UpdatePlaylistHeader();
        }
    }

    /// <summary>
    /// Add file(s) to the selected playlist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Playlist_Add_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        if (!PlaylistFunctions.AddFilesToPlaylist(_currentPlaylist))
            return;

        PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);

        UpdatePlaylistHeader();
    }

    /// <summary>
    /// Add file(s) to the selected playlist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Playlist_Add_Button_RightClick(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        if (!PlaylistFunctions.AddFolderToPlaylist(_currentPlaylist))
            return;

        PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);

        UpdatePlaylistHeader();
    }

    /// <summary>
    /// remove a song from the playlist but don't save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Playlist_Remove_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        if (_showingPlaylists)
            return;

        foreach (string? s in PlaylistContainer.SelectedItems)
        {
            var song = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, s);
            _currentPlaylist.Remove(song);
            BmpCoffer.Instance.DeleteSong(song);
        }
        BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
        PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);

        UpdatePlaylistHeader();
    }

    private void UpdatePlaylistHeader()
    {
        if (_currentPlaylist == null) 
            return;

        var icon = "↩".PadRight(2);
        var timeString = new DateTime(PlaylistFunctions.GetTotalTime(_currentPlaylist).Ticks).ToString("HH:mm:ss -").PadRight(4);
        var name = _currentPlaylist.GetName();
        var headerText = $"{icon} {timeString} {name}";

        PlaylistHeader.Header = headerText;
    }
    /// <summary>
    /// Delete a playlist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Playlist_Delete_Button_Click(object sender, RoutedEventArgs e)
    {
        //Showing the playlists?
        if (_showingPlaylists)
        {
            if ((string)PlaylistContainer.SelectedItem == null)
                return;

            var pls = BmpCoffer.Instance.GetPlaylist((string)PlaylistContainer.SelectedItem);
            if (pls == null)
                return;

            DeleteWithConfirmation(pls);
            PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
            return;
        }

        if (_currentPlaylist == null)
            return;

        _showingPlaylists = true;
        DeleteWithConfirmation(_currentPlaylist);
        PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
        PlaylistHeader.Header         = "Playlists";
        _currentPlaylist              = null;
    }

    /// <summary>
    /// Delete playlist with showing the confirmation window
    /// </summary>
    /// <param name="playlist"></param>
    private static void DeleteWithConfirmation(IPlaylist playlist)
    {
        var confirmDelete = MessageBox.Show(
            $"Are you sure you want to delete this playlist?\n\nPlaylist name : {playlist.GetName()}",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        if (confirmDelete == MessageBoxResult.Yes)
        {
            BmpCoffer.Instance.DeletePlaylist(playlist);
        }
    }
    #endregion

    #region PlaylistContainer actions
    /// <summary>
    /// Click on the head of the DataGrid brings you back to the playlists
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlaylistContainer_HeaderClick(object sender, RoutedEventArgs e)
    {
        if (sender is DataGridColumnHeader)
        {
            _showingPlaylists             = true;
            PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
            PlaylistHeader.Header         = "Playlists";
            _currentPlaylist              = null;
            if (_playlistShuffle)
            {
                _playlistShuffle              = false;
                PlaylistShuffleButton.Opacity = 0.5f;
            }
        }
    }

    /// <summary>
    /// if a song or playlist in the list was double clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlaylistContainer_PreviewMouseDoubleClick(object sender, RoutedEventArgs e)
    {
        if ((string)PlaylistContainer.SelectedItem == null)
            return;

        // Check if the double-click event occurred inside the playlist
        if (ItemsControl.ContainerFromElement(PlaylistContainer, (e.OriginalSource as DependencyObject)!) is DataGridRow { IsSelected: true } item)
        {
            if (_showingPlaylists)
            {
                _currentPlaylist              = BmpCoffer.Instance.GetPlaylist((string)PlaylistContainer.SelectedItem);
                _showingPlaylists             = false;
                PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);

                UpdatePlaylistHeader();
                return;
            }

            //If no playlist is active assume we are in the song selection
            PlaybackFunctions.LoadSongFromPlaylist (_currentPlaylist == null
                ? BmpCoffer.Instance.GetSong((string)PlaylistContainer.SelectedItem)
                : PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
            InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
            DirectLoaded           = false;
        }
    }

    /// <summary>
    /// Refresh song list whenever playlist updated, <see cref="BmpCoffer.OnPlaylistDataUpdated"/>
    /// </summary>
    private void OnPlaylistDataUpdated()
    {
        if (_showingPlaylists || _currentPlaylist == null)
            return;

        _currentPlaylist              = BmpCoffer.Instance.GetPlaylist(_currentPlaylist.GetName());
        PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);
        UpdatePlaylistHeader();
    }
    /// <summary>
    /// Opens the edit window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlaylistContainer_RightButton(object sender, MouseButtonEventArgs e)
    {
        var cellText = sender as TextBlock;
        if (cellText is { Text: "" })
            return;

        var song = _currentPlaylist == null ? BmpCoffer.Instance.GetSong(cellText?.Text) : PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, cellText?.Text);
        var _ = new SongEditWindow(song);
    }

    /// <summary>
    /// Drag start function to move songs in the playlist
    /// </summary>
    private void PlaylistContainer_MouseMove(object sender, MouseEventArgs e)
    {
        if (_playlistShuffle)
        {
            return;
        }

        if (sender is TextBlock cellText && e.LeftButton == MouseButtonState.Pressed && !_showingPlaylists)
        {
            DragDrop.DoDragDrop(PlaylistContainer, cellText, DragDropEffects.Move);
        }
    }

    /// <summary>
    /// And the drop
    /// </summary>
    private void Playlist_Drop(object sender, DragEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        var droppedDataTb = e.Data.GetData(typeof(TextBlock)) as TextBlock;

        if (((TextBlock)sender).DataContext is not string target || droppedDataTb?.DataContext is not string droppedDataStr)
        {
            return;
        }

        var removedIdx = PlaylistContainer.Items.IndexOf(droppedDataStr);
        var targetIdx = PlaylistContainer.Items.IndexOf(target);

        if (removedIdx < 0 || removedIdx >= PlaylistContainer.Items.Count)
        {
            // The source index is out of bounds, so we return without doing anything
            return;
        }

        if (targetIdx < 0 || targetIdx >= PlaylistContainer.Items.Count)
        {
            // The target index is out of bounds, so we return without doing anything
            return;
        }

        if (removedIdx < targetIdx)
        {
            _currentPlaylist?.Move(removedIdx, targetIdx);
            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
            PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);

        }
        else if (removedIdx == targetIdx)
        {
            PlaylistContainer.SelectedIndex = targetIdx;
        }
        else
        {
            _currentPlaylist?.Move(removedIdx, targetIdx);
            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
            PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);
        }
    }

    #endregion

    #region lower playlist button functions
    /// <summary>
    /// The playlist repeat toggle button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlaylistRepeat_Button_Click(object sender, RoutedEventArgs e)
    {
        _playlistRepeat              = !_playlistRepeat;
        PlaylistRepeatButton.Opacity = _playlistRepeat ? 1 : 0.5f;
    }

    /// <summary>
    /// The playlist shuffle toggle button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlaylistShuffle_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        _playlistShuffle              = !_playlistShuffle;
        PlaylistShuffleButton.Opacity = _playlistShuffle ? 1 : 0.5f;

        var playlist = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);

        if (_playlistShuffle)
        {
            // shuffle the playlist items
            _shuffledPlaylist = playlist.OrderBy(_ => Guid.NewGuid()).ToList();

            // update the data source for the DataGrid
            PlaylistContainer.ItemsSource = _shuffledPlaylist;
        }
        else
        {
            // use the original playlist as the data source
            _currentShuffleIndex          = PlaylistContainer.SelectedIndex;
            PlaylistContainer.ItemsSource = playlist;
            _shuffledPlaylist             = null;
        }

        // refresh the UI to display the shuffled or original playlist items
        _currentShuffleIndex = PlaylistContainer.SelectedIndex;
        PlaylistContainer.Items.Refresh();
    }

    /// <summary>
    /// Skips the current song
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SkipSong_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        if (PlaylistContainer.SelectedIndex == PlaylistContainer.Items.Count - 1)
        {
            if (!_playlistRepeat)
            {
                PlaybackFunctions.StopSong();
                Play_Button_State();
                return;
            }

            PlaylistContainer.SelectedIndex = 0;
            PlaybackFunctions.LoadSongFromPlaylist(PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
            InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
        }

        else
        {
            PlayNextSong();
        }

        if (!_autoPlay)
            return;

        // Wait for instruments before playing
        if (BmpPigeonhole.Instance.AutoEquipBards)
        {
            await Task.Delay(2000);
        }

        //var rnd = new Random();
        PlaybackFunctions.PlaySong(2000);
        Play_Button_State(true);
    }

    /// <summary>
    /// The Auto-Play button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AutoPlay_Checked(object sender, RoutedEventArgs e)
    {
        _autoPlay = AutoPlayCheckBox.IsChecked ?? false;
    }
    #endregion

    #region other "..." playlist menu function
    /// <summary>
    /// Search for a song in the playlist
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Search_Click(object sender, RoutedEventArgs e)
    {
        var inputBox = new TextInputWindow("Search for...", 80);
        inputBox.Focus();
        if (inputBox.ShowDialog() == true)
        {
            try
            {
                //Showing all songs
                if (_currentPlaylist == null && !_showingPlaylists)
                {
                    _currentPlaylist = null;
                    _showingPlaylists = false;
                    PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetSongTitles().Where(x => x.ToLower().Contains(inputBox.ResponseText.ToLower())).ToList();
                    var icon = "↩".PadRight(2);
                    const string name = "Search Result";
                    var headerText = $"{icon} {name}";
                    PlaylistHeader.Header = headerText;
                }
                else
                {
                    if (_currentPlaylist != null)
                    {
                        var song = _currentPlaylist.First(x => x.Title.ToLower().Contains(inputBox.ResponseText.ToLower()));
                        PlaylistContainer.SelectedIndex = PlaylistContainer.Items.IndexOf(song.Title);
                    }

                    PlaylistContainer.ScrollIntoView(PlaylistContainer.Items[PlaylistContainer.SelectedIndex]);
                    PlaylistContainer.UpdateLayout();
                }
            }
            catch
            {
                MessageBox.Show("Nothing found.", "Search", MessageBoxButton.OK);
            }
        }
    }

    /// <summary>
    /// Creates a new music catalog, loads it and refreshes the listed items
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Playlist_New_Cat_Button(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new SaveFileDialog
        {
            Filter           = Globals.Globals.MusicCatalogFilters,
            InitialDirectory = Path.GetDirectoryName(AppContext.BaseDirectory) + "\\"+ Globals.Globals.DataPath
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        BmpCoffer.Instance.LoadNew(openFileDialog.FileName);
        BmpPigeonhole.Instance.LastLoadedCatalog = openFileDialog.FileName;
        PlaylistContainer.ItemsSource            = BmpCoffer.Instance.GetPlaylistNames();
        _showingPlaylists                        = true;
    }

    /// <summary>
    /// Loads a MusicCatalog, loads it and refreshes the listed items
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Playlist_Open_Cat_Button(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter           = Globals.Globals.MusicCatalogFilters,
            Multiselect      = false,
            InitialDirectory = Path.GetDirectoryName(AppContext.BaseDirectory) + "\\" + Globals.Globals.DataPath
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        if (!openFileDialog.CheckFileExists)
            return;

        BmpCoffer.Instance.LoadNew(openFileDialog.FileName);
        BmpPigeonhole.Instance.LastLoadedCatalog = openFileDialog.FileName;
        PlaylistContainer.ItemsSource            = BmpCoffer.Instance.GetPlaylistNames();
        _showingPlaylists                        = true;
    }

    /// <summary>
    /// the export function, triggered from the Ui
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Playlist_Export_Cat_Button(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new SaveFileDialog
        {
            Filter = Globals.Globals.MusicCatalogFilters
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        BmpCoffer.Instance.Export(openFileDialog.FileName);
    }

    /// <summary>
    /// triggers the rebase function from Coffer
    /// </summary>
    private void Playlist_Cleanup_Cat_Button(object sender, RoutedEventArgs e)
    {
        BmpCoffer.Instance.CleanUpDb();
    }

    /// <summary>
    /// triggers the rebase function from Coffer
    /// </summary>
    private void Playlist_Import_JSon_Button(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter      = "Playlist file | *.plz",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        if (!openFileDialog.FileName.ToLower().EndsWith(".plz"))
            return;

        if (_currentPlaylist == null)
            Playlist_New_Button_Click(null, null);

        if (_currentPlaylist == null)
            return;

        var list = JsonPlaylist.Load(openFileDialog.FileName);
        if (list != null)
        {
            foreach (var rawData in list)
            {
                var song = BmpSong.ImportMidiFromByte(rawData.Data, "dummy").Result;
                song.Title = rawData.Name;
                _currentPlaylist.Add(song);
                BmpCoffer.Instance.SaveSong(song);
            }
        }

        BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
        PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist);
    }

    /// <summary>
    /// triggers the rebase function from Coffer
    /// </summary>
    private void Playlist_Export_JSon_Button(object sender, RoutedEventArgs e)
    {
        if (_currentPlaylist == null)
            return;

        var openFileDialog = new SaveFileDialog
        {
            Filter = "Playlist file | *.plz"
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var songs = _currentPlaylist.Select(song => new SongContainer { Name = song.Title, Data = song.GetExportMidi().ToArray() }).ToList();
        JsonPlaylist.Save(openFileDialog.FileName, songs);
    }

    private void Playlist_ShowSongs_Click(object sender, RoutedEventArgs e)
    {
        _currentPlaylist              = null;
        _showingPlaylists             = false;
        PlaylistContainer.ItemsSource = PlaylistFunctions.GetAllSongsInDb();

        var icon = "↩".PadRight(2);
        const string name = "All Songs";
        var headerText = $"{icon} {name}";
        PlaylistHeader.Header = headerText;
    }

    /// <summary>
    /// Button context menu routine
    /// </summary>
    private void MenuButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
    {
        var rectangle = sender as Button;
        var contextMenu = rectangle?.ContextMenu;
        if (contextMenu != null)
        {
            contextMenu.PlacementTarget = rectangle;
            contextMenu.Placement       = PlacementMode.Bottom;
            contextMenu.IsOpen          = true;
        }
    }
    #endregion
}