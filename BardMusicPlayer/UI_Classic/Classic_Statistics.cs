using System.IO;
using System.Windows;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old.Events;
using Microsoft.Win32;

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
        var song = PlaybackFunctions.CurrentSong;
        var saveFileDialog = new SaveFileDialog
        {
            Filter           = "MIDI file (*.mid)|*.mid",
            FilterIndex      = 2,
            RestoreDirectory = true,
            OverwritePrompt  = true
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            Stream myStream;
            if ((myStream = saveFileDialog.OpenFile()) != null)
            {
                song?.GetExportMidi().WriteTo(myStream);
                myStream.Close();
            }
        }
    }
}