using System.Windows;
using Microsoft.Win32;
using BardMusicPlayer.Controls;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old.Events;

namespace BardMusicPlayer.UI_Classic;

/// <summary>
/// only here cuz someone would like to have it back
/// </summary>
public partial class ClassicMainView
{
    private List<int> _notesCountForTracks = new();
    private void UpdateStats(SongLoadedEvent e)
    {
        StatisticsTotalTracksLabel.Content    = e.MaxTracks.ToString();
        StatisticsTotalNoteCountLabel.Content = e.TotalNoteCount.ToString();

        _notesCountForTracks.Clear();
        _notesCountForTracks = e.CurrentNoteCountForTracks;

        if (NumValue >= _notesCountForTracks.Count)
        {
            StatisticsTrackNoteCountLabel.Content = "Invalid track";
            return;
        }
        StatisticsTrackNoteCountLabel.Content = _notesCountForTracks[NumValue];
    }

    private void UpdateNoteCountForTrack()
    {
        if (PlaybackFunctions.CurrentSong == null)
            return;

        if (NumValue >= _notesCountForTracks.Count)
        {
            StatisticsTrackNoteCountLabel.Content = "Invalid track";
            return;
        }

        StatisticsTrackNoteCountLabel.Content = _notesCountForTracks[NumValue];
    }

    private void ExportAsMidi(object sender, RoutedEventArgs routedEventArgs)
    {
        PlaylistFunctions.ExportSong(PlaybackFunctions.CurrentSong);
    }
    
    private void QuickMidiProcessing_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter      = Globals.Globals.FileFilters,
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        if (!openFileDialog.CheckFileExists)
            return;

        _ = new MidiBardConverterWindow(openFileDialog.FileName)
        {
            Visibility = Visibility.Visible
        };
    }
}