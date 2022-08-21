using BardMusicPlayer.Ui.Functions;
using System.Collections.Generic;
using System.Windows.Controls;

namespace BardMusicPlayer.Ui.Classic
{
    /// <summary>
    /// only here cuz someone would like to have it back
    /// </summary>
    public partial class Classic_MainView : UserControl
    {
        private List<int> _notesCountForTracks = new List<int>();
        private void UpdateStats(Maestro.Events.SongLoadedEvent e)
        {
            this.Statistics_Total_Tracks_Label.Content = e.MaxTracks.ToString();
            this.Statistics_Total_Note_Count_Label.Content = e.TotalNoteCount.ToString();

            this._notesCountForTracks.Clear();
            this._notesCountForTracks = e.CurrentNoteCountForTracks;

            if (NumValue >= _notesCountForTracks.Count)
            {
                this.Statistics_Track_Note_Count_Label.Content = "Invalid track";
                return;
            }
            this.Statistics_Track_Note_Count_Label.Content = _notesCountForTracks[NumValue];
        }

        private void UpdateNoteCountForTrack()
        {
            if (PlaybackFunctions.CurrentSong == null)
                return;

            if (NumValue >= _notesCountForTracks.Count)
            {
                this.Statistics_Track_Note_Count_Label.Content = "Invalid track";
                return;
            }

            this.Statistics_Track_Note_Count_Label.Content = _notesCountForTracks[NumValue];
        }

    }
}
