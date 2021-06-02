using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using FFBardMusicCommon;
using Timer = System.Timers.Timer;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpPlaylist : UserControl
    {
        public class BmpPlaylistRequestAddEvent : EventArgs
        {
            public string FilePath { get; set; }

            public int Track { get; set; }

            public int DropIndex { get; set; }

            public BmpPlaylistRequestAddEvent(string filePath, int track, int dropIndex)
            {
                FilePath  = filePath;
                Track     = track;
                DropIndex = dropIndex;
            }
        }

        public bool LoopMode { get; set; }

        public bool RandomMode { get; set; }

        public bool AutoPlay => AutoPlayToggle.Checked;

        public EventHandler<BmpMidiEntry> OnMidiSelect;
        public EventHandler OnPlaylistRequestAdd;
        public EventHandler<BmpPlaylistRequestAddEvent> OnPlaylistManualRequestAdd;
        private float playlistDelay;
        private BmpMidiList playlistEntries = new BmpMidiList();
        private BindingList<BmpMidiEntry> playlistBinding = new BindingList<BmpMidiEntry>();

        public BmpPlaylist()
        {
            InitializeComponent();

            PlaylistView.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

            var font = PlaylistView.DefaultCellStyle.Font;
            PlaylistView.DefaultCellStyle.Font = new Font(font.FontFamily, 8);

            playlistDelay = Properties.Settings.Default.PlaylistDelay;
            var min = decimal.ToSingle(Playlist_Delay.Minimum);
            var max = decimal.ToSingle(Playlist_Delay.Maximum);
            playlistDelay        = playlistDelay.Clamp(min, max);
            Playlist_Delay.Value = new decimal(playlistDelay);

            LoadSettings();
        }

        public void LoadSettings()
        {
            var mls = new BmpMidiListSettings();

            playlistEntries         = new BmpMidiList(mls.MidiList);
            playlistBinding         = new BindingList<BmpMidiEntry>(playlistEntries);
            PlaylistView.DataSource = playlistBinding;
        }

        public void SaveSettings()
        {
            var mls = new BmpMidiListSettings();
            mls.Reload();
            mls.MidiList = playlistEntries;
            mls.Save();
        }

        public void Deselect() { PlaylistView.ClearSelection(); }

        public void Select(string filePath)
        {
            for (var i = 0; i < PlaylistView.Rows.Count; i++)
            {
                if (PlaylistView.Rows[i].DataBoundItem is BmpMidiEntry entry)
                {
                    if (entry.FilePath.FilePath == filePath)
                    {
                        Select(i);
                        return;
                    }
                }
            }
        }

        public void Select(int index)
        {
            if (PlaylistView.Rows.Count > 0 && index < PlaylistView.Rows.Count)
            {
                PlaylistView.Rows[index].Selected = true;
            }
        }

        public BmpMidiEntry AddPlaylistEntry(string filename, int track = 0, int dropIndex = -1)
        {
            var entry = new BmpMidiEntry(filename, track);

            var entryIndex = 0;
            if (dropIndex >= 0)
            {
                entryIndex = dropIndex;
                playlistEntries.Insert(entryIndex, entry);
            }
            else if (PlaylistView.SelectedRows.Count == 1)
            {
                var row = PlaylistView.SelectedRows[0];
                row.Selected = false;

                entryIndex = row.Index + 1;
                playlistEntries.Insert(entryIndex, entry);
            }
            else
            {
                playlistEntries.Add(entry);
                entryIndex = playlistEntries.Count - 1;
            }

            playlistBinding         = new BindingList<BmpMidiEntry>(playlistEntries);
            PlaylistView.DataSource = playlistBinding;

            // select the newly added track here
            Select(entryIndex);
            SaveSettings();

            return entry;
        }

        public void RemovePlaylistEntry(int rowIndex)
        {
            if (rowIndex >= PlaylistView.RowCount || rowIndex < 0)
            {
                return;
            }

            // remove the provided row from the cell
            var theChosenRow = PlaylistView.Rows[rowIndex];
            if (theChosenRow.DataBoundItem is BmpMidiEntry entry)
            {
                theChosenRow.Selected = false;
                playlistBinding.Remove(entry);
            }

            // update the list
            PlaylistView.DataSource = playlistBinding;

            // if we have something else in the list, make sure to move the
            // 'currently selected' down by one to highlight the midi that took its place
            if (PlaylistView.Rows.Count > 0)
            {
                // edge case for if the last entry in the playlist was deleted
                if (rowIndex == PlaylistView.Rows.Count)
                {
                    rowIndex--;
                }

                PlaylistView.Rows[rowIndex].Selected = true;
            }

            SaveSettings();
        }

        public bool AdvanceNext(out string filename, out int track)
        {
            filename = string.Empty;
            track    = 0;
            if (PlaylistView.RowCount == 0)
            {
                return false;
            }

            if (PlaylistView.SelectedRows.Count == 0)
            {
                return false;
            }

            var index = PlaylistView.SelectedRows[0].Index;
            PlaylistView.ClearSelection();

            if (RandomMode)
            {
                var rand = new Random();
                var newRandomIndex = rand.Next(0, PlaylistView.RowCount);
                while (newRandomIndex == index)
                {
                    newRandomIndex = rand.Next(0, PlaylistView.RowCount);
                }

                index = newRandomIndex;
            }
            else
            {
                index++;
                if (index == PlaylistView.RowCount)
                {
                    if (LoopMode)
                    {
                        index = 0;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            var row = PlaylistView.Rows[index];
            if (row.DataBoundItem is BmpMidiEntry entry)
            {
                filename     = entry.FilePath.FilePath;
                track        = entry.Track.Track;
                row.Selected = true;
            }

            return true;
        }

        private void PlaylistControl_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Focus);
            e.Handled = true;
        }

        private void Playlist_Add_Click(object sender, EventArgs e)
        {
            OnPlaylistRequestAdd?.Invoke(this, e);

            SaveSettings();
        }

        private void Playlist_Remove_Click(object sender, EventArgs e)
        {
            if (PlaylistView.SelectedRows.Count == 1)
            {
                var rowIndexToRemove = PlaylistView.SelectedRows[0].Index;
                RemovePlaylistEntry(rowIndexToRemove);
            }
        }

        private void PlaylistView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            PlaySelectedMidi();
            Select(e.RowIndex);
        }

        private void Playlist_Loop_CheckedChanged(object sender, EventArgs e)
        {
            LoopMode = ((CheckBox) sender).Checked;
        }

        private void Playlist_Random_CheckedChanged(object sender, EventArgs e)
        {
            RandomMode = ((CheckBox) sender).Checked;
        }

        public void PlaySelectedMidi(bool delay = true)
        {
            var playlistTimer = new Timer
            {
                Interval = delay ? playlistDelay * 1000 : 100, AutoReset = false
            };

            playlistTimer.Elapsed += delegate
            {
                TimerPlayMidi();
                playlistTimer.Dispose();
            };

            playlistTimer.Start();
        }

        private void TimerPlayMidi()
        {
            if (GetSelectedMidiEntry(out var entry))
            {
                OnMidiSelect?.Invoke(this, entry);
            }
        }

        public bool HasMidi() => playlistBinding.Count > 0;

        // Helpers

        private bool GetSelectedMidiEntry(out BmpMidiEntry entry)
        {
            if (PlaylistView.SelectedRows.Count == 1)
            {
                entry = PlaylistView.SelectedRows[0].DataBoundItem as BmpMidiEntry;
                return true;
            }

            entry = new BmpMidiEntry();
            return false;
        }

        // // Drag and Drop and/or Reorder
        private bool actuallyMoving;
        private int mouseYCoordForMovement = -1;
        private int initialRowIndexForMovement = -1;

        private void BmpMidiEntryList_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // remove entry if right click
            if (e.Button == MouseButtons.Right)
            {
                Select(e.RowIndex);
                RemovePlaylistEntry(e.RowIndex);
            }
        }

        private void BmpMidiEntryList_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !actuallyMoving)
            {
                // if we don't yet have an initial row index
                // and we've moved the cursor with the button held down for
                // a decently sufficient time, i.e. 10 pixels. seems good.
                if (Math.Abs(mouseYCoordForMovement - e.Y) >= 10)
                {
                    actuallyMoving = true;

                    Console.WriteLine($"initialRow detected as {initialRowIndexForMovement}");

                    var data = PlaylistView.Rows[initialRowIndexForMovement].DataBoundItem as BmpMidiEntry;
                    DoDragDrop(data, DragDropEffects.Move);
                }
            }
        }

        private void BmpMidiEntryList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // store this now, since we could move to the next closest cell in MouseMove
                initialRowIndexForMovement = e.RowIndex;
                mouseYCoordForMovement     = e.Y;
            }
        }

        private void BmpMidiEntryList_DragOver(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Move; }

        private void Playlist_Delay_ValueChanged(object sender, EventArgs e)
        {
            playlistDelay                             = decimal.ToSingle(Playlist_Delay.Value);
            Properties.Settings.Default.PlaylistDelay = playlistDelay;
        }

        private void BmpMidiEntryList_DragDrop(object sender, DragEventArgs e)
        {
            var clientPoint = PointToClient(new Point(e.X, e.Y));
            var hit = PlaylistView.HitTest(clientPoint.X, clientPoint.Y);

            // for internal drag-and-drop
            if (actuallyMoving)
            {
                var targetRowIndex = hit.RowIndex - 1;
                if (hit.Type == DataGridViewHitTestType.None)
                {
                    // user has dnd onto empty space, simply move midi to the end of the playlist
                    targetRowIndex = PlaylistView.RowCount - 1;
                }

                var oldData = PlaylistView.Rows[initialRowIndexForMovement].DataBoundItem as BmpMidiEntry;
                RemovePlaylistEntry(initialRowIndexForMovement);
                AddPlaylistEntry(oldData.FilePath.FilePath, oldData.Track.Track, targetRowIndex);
                actuallyMoving = false;

                return;
            }

            var dropIndex = 0;
            switch (hit.Type)
            {
                case DataGridViewHitTestType.Cell:
                    // user has dnd onto an existing cell, move above the hovered midi
                    dropIndex = hit.RowIndex - 1;
                    break;
                case DataGridViewHitTestType.None:
                    // user has dnd onto empty space, simply add midi to the end of the playlist
                    dropIndex = playlistEntries.Count;
                    break;
            }

            if (e.Effect == DragDropEffects.Move)
            {
                // just in case the user gets fancy with it and dnd's multiple midis
                var midiList = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
                foreach (var midiFilePath in midiList)
                {
                    var filename = Path.GetFileName(midiFilePath);
                    var addEvent = new BmpPlaylistRequestAddEvent(filename, 0, dropIndex);
                    OnPlaylistManualRequestAdd?.Invoke(this, addEvent);
                }
            }
        }

        private void Playlist_ClearAll_Click(object sender, EventArgs e)
        {
            while (PlaylistView.RowCount != 0)
            {
                RemovePlaylistEntry(0);
            }
        }
    }
}