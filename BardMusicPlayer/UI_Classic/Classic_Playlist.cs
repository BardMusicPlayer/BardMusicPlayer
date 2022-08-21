using BardMusicPlayer.Coffer;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Ui.Controls;
using BardMusicPlayer.Ui.Functions;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UI.Resources;

namespace BardMusicPlayer.Ui.Classic
{
    /// <summary>
    /// Interaktionslogik für Classic_MainView.xaml
    /// </summary>
    public partial class Classic_MainView : UserControl
    {
        private bool _playlistRepeat = false;
        private bool _playlistShuffle = false;
        private bool _showingPlaylists = false;     //are we displaying the playlist or the songs
        private IPlaylist _currentPlaylist = null;  //the current selected playlist

        /// <summary>
        /// Plays the next song from the playlist
        /// </summary>
        private void playNextSong()
        {
            if (PlaylistContainer.Items.Count == 0)
                return;

            if (_playlistShuffle)
            {
                Random rnd = new Random();
                int random = rnd.Next(1, PlaylistContainer.Items.Count);

                if (random == PlaylistContainer.SelectedIndex) 
                    random = (random + 1) % PlaylistContainer.Items.Count;
                if (random == 0)
                    random = 1;
                PlaylistContainer.SelectedIndex = random;
            }
            else
            {
                if ((PlaylistContainer.SelectedIndex == -1) || (PlaylistContainer.SelectedIndex == 0))
                {
                    PlaylistContainer.SelectedIndex = 1;
                }
                else
                {
                    if (PlaylistContainer.SelectedIndex == PlaylistContainer.Items.Count - 1)
                    {
                        PlaylistContainer.SelectedIndex = 1;
                    }
                    else
                        PlaylistContainer.SelectedIndex = PlaylistContainer.SelectedIndex + 1;
                }
            }
            PlaybackFunctions.LoadSongFromPlaylist(PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
            this.SongName.Text = PlaybackFunctions.GetSongName();
            this.InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
        }

        private void SelectSongByIndex(int idx)
        {
            //are we at MB compat?
            if (!BmpPigeonhole.Instance.MidiBardCompatMode)
                return;

            if (PlaylistContainer.Items.Count == 0)
                return;

            PlaylistContainer.SelectedIndex = idx;

            PlaybackFunctions.LoadSongFromPlaylist(PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
            this.SongName.Text = PlaybackFunctions.GetSongName();
            this.InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
        }

        #region upper playlist button functions
        /// <summary>
        /// Create a new playlist but don't save it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Playlist_New_Button_Click(object sender, RoutedEventArgs e)
        {
            var inputbox = new TextInputWindow("Playlistname");
            if (inputbox.ShowDialog() == true)
            {
                _currentPlaylist = PlaylistFunctions.CreatePlaylist(inputbox.ResponseText);
                PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist, true);
                _showingPlaylists = false;
                Playlist_Header.Header = _currentPlaylist.GetName().PadRight(75 - _currentPlaylist.GetName().Length, ' ') + new DateTime(PlaylistFunctions.GetTotalTime(_currentPlaylist).Ticks).ToString("HH:mm:ss");
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

            PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist, true);
            Playlist_Header.Header = _currentPlaylist.GetName().PadRight(75 - _currentPlaylist.GetName().Length, ' ') + new DateTime(PlaylistFunctions.GetTotalTime(_currentPlaylist).Ticks).ToString("HH:mm:ss");
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

            PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist, true);
            Playlist_Header.Header = _currentPlaylist.GetName().PadRight(75 - _currentPlaylist.GetName().Length, ' ') + new DateTime(PlaylistFunctions.GetTotalTime(_currentPlaylist).Ticks).ToString("HH:mm:ss");
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

            foreach (string s in PlaylistContainer.SelectedItems)
            {
                BmpSong song = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, s);
                if (song == null)
                    continue;
                _currentPlaylist.Remove(song);
                BmpCoffer.Instance.DeleteSong(song);
            }
            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);

            PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist, true);
            Playlist_Header.Header = _currentPlaylist.GetName().PadRight(75 - _currentPlaylist.GetName().Length, ' ') + new DateTime(PlaylistFunctions.GetTotalTime(_currentPlaylist).Ticks).ToString("HH:mm:ss");
        }

        /// <summary>
        /// Delete a playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Playlist_Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPlaylist == null)
                return;

            _showingPlaylists = true;
            BmpCoffer.Instance.DeletePlaylist(_currentPlaylist);
            PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
            Playlist_Header.Header = "Playlists";
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
            var columnHeader = sender as System.Windows.Controls.Primitives.DataGridColumnHeader;
            if (columnHeader != null)
            {
                _showingPlaylists = true;
                PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
                Playlist_Header.Header = "Playlists";
            }
        }

        /// <summary>
        /// if a song or playlist in the list was doubleclicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistContainer_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((string)PlaylistContainer.SelectedItem == null)
                return;

            if (_showingPlaylists)
            {
                if ((string)PlaylistContainer.SelectedItem == "..")
                    return;

                _currentPlaylist = BmpCoffer.Instance.GetPlaylist((string)PlaylistContainer.SelectedItem);
                _showingPlaylists = false;
                PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist, true);
                Playlist_Header.Header = _currentPlaylist.GetName().PadRight(75- _currentPlaylist.GetName().Length, ' ') + new DateTime(PlaylistFunctions.GetTotalTime(_currentPlaylist).Ticks).ToString("HH:mm:ss");
                return;
            }
            else
            {
                if((string)PlaylistContainer.SelectedItem == (string)"..")
                {
                    _showingPlaylists = true;
                    PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
                    Playlist_Header.Header = "Playlists";
                    return;
                }
            }

            PlaybackFunctions.LoadSongFromPlaylist(PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, (string)PlaylistContainer.SelectedItem));
            this.SongName.Text = PlaybackFunctions.GetSongName();
            this.InstrumentInfo.Content = PlaybackFunctions.GetInstrumentNameForHostPlayer();
            _directLoaded = false;
            return;
        }

        /// <summary>
        /// Opens the edit window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistContainer_RightButton(object sender, MouseButtonEventArgs e)
        {
            TextBlock celltext = sender as TextBlock;
            if (celltext.Text.Equals(""))
                return;

            BmpSong song = PlaylistFunctions.GetSongFromPlaylist(_currentPlaylist, celltext.Text);
            SongEditWindow sew = new SongEditWindow(song);
        }

        /// <summary>
        /// Drag start function to move songs in the playlist
        /// </summary>
        private void PlaylistContainer_MouseMove(object sender, MouseEventArgs e)
        {
            TextBlock celltext = sender as TextBlock;
            if (celltext != null && e.LeftButton == MouseButtonState.Pressed && !_showingPlaylists)
            {
                DragDrop.DoDragDrop(PlaylistContainer, celltext, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// And the drop
        /// </summary>
        private void Playlist_Drop(object sender, DragEventArgs e)
        {
            TextBlock droppedDataTB = e.Data.GetData(typeof(TextBlock)) as TextBlock;
            string droppedDataStr = droppedDataTB.DataContext as string;
            string target = ((TextBlock)(sender)).DataContext as string;

            if ((droppedDataStr.Equals("..")) || (target.Equals("..")))
                return;


            int removedIdx = PlaylistContainer.Items.IndexOf(droppedDataStr);
            int targetIdx = PlaylistContainer.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                _currentPlaylist.Move(removedIdx-1, targetIdx-1);
                BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
                PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist, true);

            }
            else if (removedIdx == targetIdx)
            {
                PlaylistContainer.SelectedIndex = targetIdx;
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (PlaylistContainer.Items.Count + 1 > remIdx)
                {
                    _currentPlaylist.Move(removedIdx-1, targetIdx-1);
                    BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
                    PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist, true);
                }
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
            _playlistRepeat = !_playlistRepeat;
            this.PlaylistRepeat_Button.Opacity = _playlistRepeat ? 1 : 0.5f;
        }

        /// <summary>
        /// The playlist shuffle toggle button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistShuffle_Button_Click(object sender, RoutedEventArgs e)
        {
            _playlistShuffle = !_playlistShuffle;
            this.PlaylistShuffle_Button.Opacity = _playlistShuffle ? 1 : 0.5f;
        }

        /// <summary>
        /// Skips the current song (only works on autoplay)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkipSong_Button_Click(object sender, RoutedEventArgs e)
        {
            playNextSong();

            if (!BmpPigeonhole.Instance.PlaylistAutoPlay)
                return;

            Random rnd = new Random();
            PlaybackFunctions.PlaySong(rnd.Next(15, 35) * 100);
            Play_Button_State(true);
        }

        /// <summary>
        /// The Auto-Play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoPlay_Checked(object sender, RoutedEventArgs e)
        {
            BmpPigeonhole.Instance.PlaylistAutoPlay = AutoPlay_CheckBox.IsChecked ?? false;
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
            var inputbox = new TextInputWindow("Search for...");
            inputbox.Focus();
            if (inputbox.ShowDialog() == true)
            {
                try
                {
                    var song = _currentPlaylist.Where(x => x.Title.ToLower().Contains(inputbox.ResponseText.ToLower())).First();
                    PlaylistContainer.SelectedIndex = PlaylistContainer.Items.IndexOf(song.Title);
                    PlaylistContainer.ScrollIntoView(PlaylistContainer.Items[PlaylistContainer.SelectedIndex]);
                    PlaylistContainer.UpdateLayout();
                }
                catch
                {
                    MessageBox.Show("Nothing found", "Nope", MessageBoxButton.OK);
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
                Filter = Globals.Globals.MusicCatalogFilters
            };
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\"+ Globals.Globals.DataPath;

            if (openFileDialog.ShowDialog() != true)
                return;

            BmpCoffer.Instance.LoadNew(openFileDialog.FileName);
            Pigeonhole.BmpPigeonhole.Instance.LastLoadedCatalog = openFileDialog.FileName;
            PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
            _showingPlaylists = true;
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
                Filter = Globals.Globals.MusicCatalogFilters,
                Multiselect = false
            };
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + Globals.Globals.DataPath;

            if (openFileDialog.ShowDialog() != true)
                return;

            if (!openFileDialog.CheckFileExists)
                return;

            BmpCoffer.Instance.LoadNew(openFileDialog.FileName);
            Pigeonhole.BmpPigeonhole.Instance.LastLoadedCatalog = openFileDialog.FileName;
            PlaylistContainer.ItemsSource = BmpCoffer.Instance.GetPlaylistNames();
            _showingPlaylists = true;
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
        /// triggeres the reabase function from Coffer
        /// </summary>
        private void Playlist_Cleanup_Cat_Button(object sender, RoutedEventArgs e)
        {
            BmpCoffer.Instance.CleanUpDB();
        }

        /// <summary>
        /// triggeres the reabase function from Coffer
        /// </summary>
        private void Playlist_Import_JSon_Button(object sender, RoutedEventArgs e)
        {
            if (_currentPlaylist == null)
                return;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Playlist file | *.plz",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            var list = JsonPlaylist.Load(openFileDialog.FileName);
            foreach (var rawdata in list)
            {
                var song = BmpSong.ImportMidiFromByte(rawdata.Data, "dummy").Result;
                song.Title = rawdata.Name;
                _currentPlaylist.Add(song);
                BmpCoffer.Instance.SaveSong(song);
            }
            BmpCoffer.Instance.SavePlaylist(_currentPlaylist);
            PlaylistContainer.ItemsSource = PlaylistFunctions.GetCurrentPlaylistItems(_currentPlaylist, true);
        }

        /// <summary>
        /// triggeres the reabase function from Coffer
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

            System.Collections.Generic.List<SongContainer> songs = new System.Collections.Generic.List<SongContainer>();
            foreach (var song in _currentPlaylist)
            {
                SongContainer sc = new SongContainer();
                sc.Name = song.Title;
                sc.Data = song.GetExportMidi().ToArray();
                songs.Add(sc);
            }
            JsonPlaylist.Save(openFileDialog.FileName, songs);
        }

        /// <summary>
        /// Button context menu routine
        /// </summary>
        private void MenuButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Button rectangle = sender as Button;
            ContextMenu contextMenu = rectangle.ContextMenu;
            contextMenu.PlacementTarget = rectangle;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }
        #endregion
    }
}
