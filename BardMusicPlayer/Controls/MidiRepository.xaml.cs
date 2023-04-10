using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Resources;
using HtmlAgilityPack;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Web scraper to scrape the song list from https://songs.bardmusicplayer.com and populate in the listview
/// </summary>
public partial class MidiRepository
{
    private const string MidiRepoUrl        = "https://songs.bardmusicplayer.com";

    private const string SongNodeXpath      = "//div[contains(@class, 'midi-entry')]";
    private const string TitleNodeXpath     = ".//a[contains(@class, 'mtitle')]";
    private const string AuthorNodeXpath    = ".//span[contains(@class, 'mauthor')]";
    private const string CommentNodeXpath   = ".//span[contains(@class, 'r4')]";
    
    private List<Song> _fullListSong         = new();
    private List<Song> _previewListSong      = new();
    
    private readonly HttpClient _httpClient;
    private Song? _selectedSong;
    private bool _isDownloading;

    public MidiRepository()
    {
        InitializeComponent();
        _httpClient                      = new HttpClient();
        LoadingProgressBar.Visibility    = Visibility.Hidden;
        DownloadPanel.Visibility         = Visibility.Hidden;
        DownloadPath.Text                = BmpPigeonhole.Instance.MidiDownloadPath;
        DownloadProgressLabel.Visibility = Visibility.Hidden;
        DownloadProgressBar.Visibility   = Visibility.Hidden;
        RefreshPlaylistSelector();
        BmpCoffer.Instance.OnPlaylistDataUpdated += RefreshPlaylistSelector;
    }
    private class Song
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Comment { get; set; } = "";
        public string Url { get; set; } = "";
    }

    /// <summary>
    /// Fetch the html from https://songs.bardmusicplayer.com
    /// </summary>
    /// <returns></returns>
    private async Task<string> FetchSongData()
    {
        var response = await _httpClient.GetStringAsync(MidiRepoUrl);
        return response;
    }

    /// <summary>
    /// Get midi meta data from html result using web scraper
    /// </summary>
    /// <param name="html"></param>
    private void RefreshSongList(string html)
    {
        _fullListSong.Clear();
        _previewListSong.Clear();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var songNodes = htmlDoc.DocumentNode.SelectNodes(SongNodeXpath);

        foreach (var songNode in songNodes)
        {
            var titleNode = songNode.SelectSingleNode(TitleNodeXpath);
            var authorNode = songNode.SelectSingleNode(AuthorNodeXpath);
            var commentNode = songNode.SelectSingleNode(CommentNodeXpath);

            if (titleNode != null && authorNode != null && commentNode != null)
            {
                _fullListSong.Add(new Song
                {
                    Title   = titleNode.GetAttributeValue("title", ""),
                    Author  = authorNode.InnerText,
                    Comment = commentNode.InnerText,
                    Url     = titleNode.GetAttributeValue("href", ""),
                });
            }
        }
    }

    /// <summary>
    /// Click button to scrape the web and put the result to listSong
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        BtnGetSongList.IsEnabled      = false;
        LoadingProgressBar.Visibility = Visibility.Visible;

        var songData = await FetchSongData();
        
        RefreshSongList(songData);
        _previewListSong              = _fullListSong;
        MidiRepoContainer.ItemsSource = _previewListSong.Select(song => song.Title).ToList();
        RefreshCountTextBox();

        BtnGetSongList.IsEnabled      = true;
        BtnGetSongList.Content        = "Refresh";
        LoadingProgressBar.Visibility = Visibility.Hidden;
        SongSearchTextBox.Text        = "";
    }

    /// <summary>
    /// Show midi details when clicking the listview
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MidiRepoContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MidiRepoContainer.SelectedIndex == -1)
            return;

        DownloadPanel.Visibility = Visibility.Visible;
        _selectedSong            = _previewListSong[MidiRepoContainer.SelectedIndex];
        SongTitle.Text           = $"({_selectedSong.Author}) {_selectedSong.Title}";
        SongComment.Text         = _selectedSong.Comment;
    }

    /// <summary>
    /// Select download path on click button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectPath_Button_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new FolderPicker
        {
            InputPath = Directory.Exists(BmpPigeonhole.Instance.MidiDownloadPath) ? Path.GetFullPath(BmpPigeonhole.Instance.MidiDownloadPath) : Path.GetDirectoryName(AppContext.BaseDirectory)
        };

        if (dlg.ShowDialog() == true)
        {
            var path = dlg.ResultPath;
            if (!Directory.Exists(path))
                return;

            path                                    += path.EndsWith("\\") ? "" : "\\";
            DownloadPath.Text                       =  path;
            BmpPigeonhole.Instance.MidiDownloadPath =  path;
        }
    }

    /// <summary>
    /// Start download process
    /// </summary>
    /// <param name="url"></param>
    /// <param name="fileName"></param>
    private async void DownloadFile(string url, string fileName)
    {
        _isDownloading = true;
        var client = new HttpClient();
        var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        var contentLength = response.Content.Headers.ContentLength;
        var tempFilePath = Path.GetTempFileName();

        await using (var tempFileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await using (var contentStream = await response.Content.ReadAsStreamAsync())
            {
                var buffer = new byte[4096];
                long totalBytesRead = 0;
                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await tempFileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalBytesRead += bytesRead;
                    var percentComplete = (double)totalBytesRead / (contentLength ?? totalBytesRead) * 100;
                    Dispatcher.Invoke(() => DownloadProgressBar.Value = percentComplete);
                }
            }
        }
        var downloadsPath = BmpPigeonhole.Instance.MidiDownloadPath;
        var finalFilePath = $"{downloadsPath}/{fileName}.mid";

        File.Move(tempFilePath, finalFilePath, true);
        DownloadPanel.IsEnabled          = true;
        DownloadProgressLabel.Visibility = Visibility.Visible;
        _isDownloading                   = false;

        // Add to selected playlist
        var addToPlaylist = AddToPlaylistCheckBox.IsChecked ?? false;

        if (addToPlaylist && PlaylistDropdown.SelectedIndex != -1)
        {
            AddSongToPlaylist(finalFilePath);
        }
    }

    /// <summary>
    /// Download selected midi in the listview by clicking download button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadButtonClick(object sender, RoutedEventArgs e)
    {
        DownloadSelectedMidi();
    }

    /// <summary>
    /// Download selected midi by double click the list item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MidiRepoContainer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _selectedSong = _previewListSong[MidiRepoContainer.SelectedIndex];
        DownloadSelectedMidi();
    }

    /// <summary>
    /// Download current selected midi in the listview
    /// </summary>
    private void DownloadSelectedMidi()
    {
        if (_isDownloading)
            return;

        if (!Directory.Exists(BmpPigeonhole.Instance.MidiDownloadPath))
        {
            MessageBox.Show("The downloads directory is not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (_selectedSong == null)
            return;

        DownloadPanel.IsEnabled        = false;
        DownloadProgressBar.Visibility = Visibility.Visible;
        DownloadProgressBar.Value      = 0;
        DownloadFile($"{MidiRepoUrl}/{_selectedSong.Url}", $"({_selectedSong.Author}) {_selectedSong.Title}");
    }
   
    /// <summary>
    /// Refresh result count textblock
    /// </summary>
    private void RefreshCountTextBox()
    {
        ResultsCountTextBox.Text = $"{_previewListSong.Count} Results";
    }

    #region Search Functions
    /// <summary>
    /// Filter the midi listview based on SongSearchTextBox
    /// </summary>
    private void SearchSong()
    {
        if (_fullListSong.Count == 0)
            return;

        var filteredList = new List<string>();
        if (SongSearchTextBox.Text != "")
        {
            _previewListSong = _fullListSong.FindAll(s => s.Title.ToLower().Contains(SongSearchTextBox.Text.ToLower()));
            filteredList     = _previewListSong.Select(s => s.Title).ToList();
        }
        else
        {
            _previewListSong = _fullListSong;
            filteredList     = _previewListSong.Select(s => s.Title).ToList();
        }

        MidiRepoContainer.ItemsSource = filteredList;
        RefreshCountTextBox();
    }
    /// <summary>
    /// Filter song when textbox value changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SongSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        SearchSong();
    }
    #endregion

    #region Import To Playlist Functions
    /// <summary>
    /// Refresh playlist dropdown
    /// </summary>
    private void RefreshPlaylistSelector()
    {
        PlaylistDropdown.DataContext = BmpCoffer.Instance.GetPlaylistNames();
    }

    /// <summary>
    /// Disable 'add to playlist' feature if checkbox is unchecked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddToPlaylistCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        RefreshAddToPlaylistMode();
    }

    /// <summary>
    /// Enable 'add to playlist' feature if checkbox is checked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddToPlaylistCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        RefreshAddToPlaylistMode();
    }

    /// <summary>
    /// Hide and show playlist selector while check/uncheck 'add to playlist' checkbox
    /// </summary>
    private void RefreshAddToPlaylistMode()
    {
        var isChecked = AddToPlaylistCheckBox.IsChecked ?? false;
        PlaylistDropdown.Visibility = isChecked ? Visibility.Visible : Visibility.Hidden;
    }

    /// <summary>
    /// Add song to playlist by song filepath
    /// </summary>
    private void AddSongToPlaylist(string filePath)
    {
        var playlist = BmpCoffer.Instance.GetPlaylist(PlaylistDropdown.SelectedItem as string);
        PlaylistFunctions.AddFileToPlaylist(filePath, playlist);
    }
    #endregion
}