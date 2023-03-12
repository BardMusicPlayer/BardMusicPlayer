using System.Windows;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Controls;

/// <summary>
/// Interaction logic for SongEditWindow.xaml
/// </summary>
public partial class SongEditWindow
{
    private readonly BmpSong? _song;
    public SongEditWindow(BmpSong? song)
    {
        InitializeComponent();
        if (song == null)
        {
            Close();
            return;
        }
        Visibility = Visibility.Visible;

        _song                   = song;
        InternalTrackName.Text  = _song.Title;
        DisplayedTrackName.Text = _song.DisplayedTitle;
    }

    private void CopyI_D_Click(object sender, RoutedEventArgs e)
    {
        DisplayedTrackName.Text = InternalTrackName.Text;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_song != null)
        {
            _song.DisplayedTitle = DisplayedTrackName.Text;
            BmpCoffer.Instance.SaveSong(_song);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}