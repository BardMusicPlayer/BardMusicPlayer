using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Resources;
using HtmlAgilityPack;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Web scraper to scrape the song list from https://songs.bardmusicplayer.com and populate in the listview
/// </summary>
public partial class MidiRepository : UserControl
{
    private const string songNodeXpath = "//div[contains(@class, 'midi-entry')]";
    private const string titleNodeXpath = ".//a[contains(@class, 'mtitle')]";
    private const string authorNodeXpath = ".//span[contains(@class, 'mauthor')]";
    private const string commentNodeXpath = ".//span[contains(@class, 'r4')]";
    private readonly HttpClient httpClient;
    private readonly string midiRepoUrl = "https://songs.bardmusicplayer.com";
    private List<Song> fullListSong = new List<Song>();
    private List<Song> previewListSong = new List<Song>();
    private Song? selectedSong;
    public MidiRepository()
    {
        InitializeComponent();
        httpClient = new HttpClient();
        LoadingProgressBar.Visibility = Visibility.Hidden;
        DownloadPanel.Visibility = Visibility.Hidden;
        DownloadPath.Text = BmpPigeonhole.Instance.MidiDownloadPath;
        DownloadProgressLabel.Visibility = Visibility.Hidden;
        DownloadProgressBar.Visibility = Visibility.Hidden;
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
        var response = await httpClient.GetStringAsync(midiRepoUrl);
        return response;
    }

    /// <summary>
    /// Get midi meta data from html result using web scraper
    /// </summary>
    /// <param name="html"></param>
    private void RefreshSongList(string html)
    {
        fullListSong.Clear();
        previewListSong.Clear();
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var songNodes = htmlDoc.DocumentNode.SelectNodes(songNodeXpath);

        foreach (var songNode in songNodes)
        {
            var titleNode = songNode.SelectSingleNode(titleNodeXpath);
            var authorNode = songNode.SelectSingleNode(authorNodeXpath);
            var commentNode = songNode.SelectSingleNode(commentNodeXpath);

            if (titleNode != null && authorNode != null && commentNode != null)
            {
                fullListSong.Add(new Song
                {
                    Title = titleNode.GetAttributeValue("title", ""),
                    Author = authorNode.InnerText,
                    Comment = commentNode.InnerText,
                    Url = titleNode.GetAttributeValue("href", ""),
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
        BtnGetSongList.IsEnabled = false;
        LoadingProgressBar.Visibility = Visibility.Visible;

        var songData = await FetchSongData();
        
        RefreshSongList(songData);
        previewListSong = fullListSong;
        MidiRepoContainer.ItemsSource = previewListSong.Select(song => song.Title).ToList();
        RefreshCountTextBox();

        BtnGetSongList.IsEnabled = true;
        BtnGetSongList.Content = "Refresh";
        LoadingProgressBar.Visibility = Visibility.Hidden;
        SongSearchTextBox.Text = "";
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
        selectedSong = previewListSong[MidiRepoContainer.SelectedIndex];
        SongTitle.Text = $"({selectedSong.Author}) {selectedSong.Title}";
        SongComment.Text = selectedSong.Comment;
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
            InputPath = Directory.Exists(BmpPigeonhole.Instance.MidiDownloadPath) ? Path.GetFullPath(BmpPigeonhole.Instance.MidiDownloadPath) : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        };

        if (dlg.ShowDialog() == true)
        {
            var path = dlg.ResultPath;
            if (!Directory.Exists(path))
                return;

            path += path.EndsWith("\\") ? "" : "\\";
            DownloadPath.Text = path;
            BmpPigeonhole.Instance.MidiDownloadPath = path;
        }
    }

    /// <summary>
    /// Start download process
    /// </summary>
    /// <param name="url"></param>
    /// <param name="fileName"></param>
    private async void DownloadFile(string url, string fileName)
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        long? contentLength = response.Content.Headers.ContentLength;
        string tempFilePath = Path.GetTempFileName();

        using (FileStream tempFileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
            {
                byte[] buffer = new byte[4096];
                long totalBytesRead = 0;
                int bytesRead = 0;
                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await tempFileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                    double percentComplete = (double)totalBytesRead / (contentLength ?? totalBytesRead) * 100;
                    Dispatcher.Invoke(() => DownloadProgressBar.Value = percentComplete);
                }
            }
        }
        string downloadsPath = BmpPigeonhole.Instance.MidiDownloadPath;
        string finalFilePath = $"{downloadsPath}/{fileName}.mid";

        File.Move(tempFilePath, finalFilePath, true);
        DownloadButton.IsEnabled = true;
        DownloadProgressLabel.Visibility = Visibility.Visible;
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
        selectedSong = previewListSong[MidiRepoContainer.SelectedIndex];
        DownloadSelectedMidi();
    }

    /// <summary>
    /// Download current selected midi in the listview
    /// </summary>
    private void DownloadSelectedMidi()
    {
        if (!Directory.Exists(BmpPigeonhole.Instance.MidiDownloadPath))
        {
            MessageBox.Show("The downloads directory is not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (selectedSong == null)
            return;

        DownloadButton.IsEnabled = false;
        DownloadProgressBar.Visibility = Visibility.Visible;
        DownloadProgressBar.Value = 0;
        DownloadFile($"{midiRepoUrl}/{selectedSong.Url}", $"({selectedSong.Author}) {selectedSong.Title}");
    }
    /// <summary>
    /// Filter the midi listview based on SongSearchTextBox
    /// </summary>
    private void SearchSong()
    {
        if (fullListSong.Count == 0)
            return;

        var filteredList = new List<string>();
        if (SongSearchTextBox.Text != "")
        {
            previewListSong = previewListSong.FindAll(s => s.Title.ToLower().Contains(SongSearchTextBox.Text.ToLower()));
            filteredList = previewListSong.Select(s => s.Title).ToList();
        }
        else
        {
            previewListSong = fullListSong;
            filteredList = previewListSong.Select(s => s.Title).ToList();
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
    /// <summary>
    /// Refresh result count textblock
    /// </summary>
    private void RefreshCountTextBox()
    {
        ResultsCountTextBox.Text = $"{previewListSong.Count} Results";
    }
}
