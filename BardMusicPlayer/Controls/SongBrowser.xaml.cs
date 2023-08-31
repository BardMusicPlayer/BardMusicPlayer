using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Resources;

namespace BardMusicPlayer.Controls;

/// <summary>
/// The song browser
/// </summary>
public partial class SongBrowser
{
    public EventHandler<string>? OnLoadSongFromBrowser;
    public EventHandler<string>? OnLoadSongFromBrowserToPreview;
    public EventHandler<string>? OnAddSongFromBrowser;

    public SongBrowser()
    {
        InitializeComponent();
        SongPath.Text = BmpPigeonhole.Instance.SongDirectory;
        SongSearch_PreviewTextInput(null, null);
        SongSearch.TextChanged += SongSearch_TextChanged;
    }

    /// <summary>
    /// Load the double clicked song into the sequencer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SongBrowserContainer_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SongBrowserContainer.SelectedItem is not string selectedFile)
            return;

        var fullPath = Path.Combine(SongPath.Text, selectedFile);
        if (File.Exists(fullPath))
        {
            OnLoadSongFromBrowser?.Invoke(this, fullPath);
        }
    }

    private static string GetRelativePath(string basePath, string fullPath)
    {
        var baseUri = new Uri(basePath);
        var fullUri = new Uri(fullPath);
        return baseUri.MakeRelativeUri(fullUri).ToString();
    }

    /// <summary>
    /// Sets the search parameter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SongSearch_PreviewTextInput(object? sender, TextCompositionEventArgs? e)
    {
        if (!Directory.Exists(SongPath.Text))
            return;

        var files = Directory.EnumerateFiles(SongPath.Text, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mid") || s.EndsWith(".mml") || s.EndsWith(".mmsong") || s.EndsWith(".gp*")).ToArray();
        var list = new List<string>(files);
        list = list.Select(file => 
        {
            var directory = Path.GetDirectoryName(file);
            if (directory != null && !directory.Equals(SongPath.Text, StringComparison.OrdinalIgnoreCase))
            {
                return Uri.UnescapeDataString(GetRelativePath(SongPath.Text, file));
            }

            return Path.GetFileName(file);
        }).ToList();
        if (SongSearch.Text != "")
            list = list.FindAll(s => s.ToLower().Contains(SongSearch.Text.ToLower()));
        SongBrowserContainer.ItemsSource = list;
    }

    /// <summary>
    /// Refresh list on any character change
    /// </summary>
    private void SongSearch_TextChanged(object? sender, TextChangedEventArgs e)
    {
        SongSearch_PreviewTextInput(sender, null);
    }

    /// <summary>
    /// Sets the songs folder path by typing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SongPath_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (!Directory.Exists(SongPath.Text))
            return;

        BmpPigeonhole.Instance.SongDirectory = SongPath.Text + (SongPath.Text.EndsWith("\\") ? "" : "\\");

        var files = Directory.EnumerateFiles(SongPath.Text, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mid") || s.EndsWith(".mml") || s.EndsWith(".mmsong")).ToArray();
        var list = new List<string>(files);
        SongBrowserContainer.ItemsSource = list;
    }

    /// <summary>
    /// Sets the songs folder path by folder selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new FolderPicker
        {
            InputPath = Directory.Exists(BmpPigeonhole.Instance.SongDirectory) ? Path.GetFullPath(BmpPigeonhole.Instance.SongDirectory) : Path.GetDirectoryName(AppContext.BaseDirectory)
        };

        if (dlg.ShowDialog() == true)
        {
            var path = dlg.ResultPath;
            if (!Directory.Exists(path))
                return;

            path                                 += path != null && path.EndsWith("\\") ? "" : "\\";
            SongPath.Text                        =  path;
            BmpPigeonhole.Instance.SongDirectory =  path;
            SongSearch_PreviewTextInput(null, null);
        }
    }

    /// <summary>
    /// Handle the right click on an item from ListView
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is FrameworkElement { DataContext: string selectedFile })
        {
            if (SongBrowserContainer.SelectedItem is not string currentSelectedFile || currentSelectedFile != selectedFile)
            {
                SongBrowserContainer.SelectedItem = selectedFile;
            }
            e.Handled = true;
        }
    }

    /// <summary>
    /// Handle add to playlist context menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
    {
        if (SongBrowserContainer.SelectedItem is not string selectedFile)
            return;

        var fullPath = Path.Combine(BmpPigeonhole.Instance.SongDirectory, selectedFile);
        OnAddSongFromBrowser?.Invoke(this, fullPath);
    }

    /// <summary>
    /// Handle the load to preview context menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadSongToPreview(object sender, RoutedEventArgs e)
    {
        if (SongBrowserContainer.SelectedItem is not string selectedFile)
            return;

        var fullPath = Path.Combine(BmpPigeonhole.Instance.SongDirectory, selectedFile);
        OnLoadSongFromBrowserToPreview?.Invoke(this, fullPath);
    }
}