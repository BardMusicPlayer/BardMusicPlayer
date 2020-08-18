using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Specialized;
using FFBardMusicCommon;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpPlaylist : UserControl {

        public class BmpPlaylistRequestAddEvent : EventArgs
        {
            public string filePath { get; set; }
            public int track { get; set; }
            public int dropIndex { get; set; }

            public BmpPlaylistRequestAddEvent(string filePath, int track, int dropIndex)
            {
                this.filePath = filePath;
                this.track = track;
                this.dropIndex = dropIndex;
            }
        }

		private bool loopMode;
		public bool LoopMode {
			get {
				return loopMode;
			}
			set {
				loopMode = value;
			}
		}
		private bool randomMode;
		public bool RandomMode {
			get {
				return randomMode;
			}
			set {
				randomMode = value;
			}
		}

		public bool AutoPlay {
			get {
				return AutoPlayToggle.Checked;
			}
		}

		public EventHandler<BmpMidiEntry> OnMidiSelect;
		public EventHandler OnPlaylistRequestAdd;
        public EventHandler<BmpPlaylistRequestAddEvent> OnPlaylistManualRequestAdd;

		BmpMidiList playlistEntries = new BmpMidiList();
		BindingList<BmpMidiEntry> playlistBinding = new BindingList<BmpMidiEntry>();

		public BmpPlaylist() {
			InitializeComponent();

			PlaylistView.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;

			Font font = PlaylistView.DefaultCellStyle.Font;
			PlaylistView.DefaultCellStyle.Font = new Font(font.FontFamily, 8);

			LoadSettings();
		}

		public void LoadSettings() {
			BmpMidiListSettings mls = new BmpMidiListSettings();

			playlistEntries = new BmpMidiList(mls.MidiList);
			playlistBinding = new BindingList<BmpMidiEntry>(playlistEntries);
			PlaylistView.DataSource = playlistBinding;

		}

		public void SaveSettings() {
			BmpMidiListSettings mls = new BmpMidiListSettings();
			mls.Reload();
			mls.MidiList = playlistEntries;
			mls.Save();
		}

		public void Deselect() {
			PlaylistView.ClearSelection();
		}
		public void Select(string filePath) {
			for(int i = 0; i < PlaylistView.Rows.Count; i++) {
				BmpMidiEntry entry = PlaylistView.Rows[i].DataBoundItem as BmpMidiEntry;
				if(entry != null) {
					if(entry.FilePath.FilePath == filePath) {
						this.Select(i);
						return;
					}
				}
			}
		}
		public void Select(int index) {
			if(PlaylistView.Rows.Count > 0 && index < PlaylistView.Rows.Count) {
				PlaylistView.Rows[index].Selected = true;
			}
		}

		public BmpMidiEntry AddPlaylistEntry(string filename, int track = 0, int dropIndex = -1)
        {
			BmpMidiEntry entry = new BmpMidiEntry(filename, track);

			int entryIndex = 0;
            if (dropIndex >= 0)
            {
                entryIndex = dropIndex;
                playlistEntries.Insert(entryIndex, entry);
            }
			else if (PlaylistView.SelectedRows.Count == 1)
            {
				DataGridViewRow row = PlaylistView.SelectedRows[0];
				row.Selected = false;

				entryIndex = row.Index + 1;
				playlistEntries.Insert(entryIndex, entry);
			}
            else
            {
				playlistEntries.Add(entry);
				entryIndex = playlistEntries.Count - 1;
			}

			playlistBinding = new BindingList<BmpMidiEntry>(playlistEntries);
			PlaylistView.DataSource = playlistBinding;

            // select the newly added track here
            Select(entryIndex);
            SaveSettings();

			return entry;
		}

        public void RemovePlaylistEntry(int rowIndex)
        {
            if (rowIndex >= PlaylistView.RowCount || rowIndex < 0)
                return;

            // remove the provided row from the cell
            DataGridViewRow theChosenRow = PlaylistView.Rows[rowIndex];
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
                    rowIndex--;

                DataGridViewRow newSelectedRow = PlaylistView.Rows[rowIndex];
                if (newSelectedRow != null)
                {
                    newSelectedRow.Selected = true;
                }
            }

            SaveSettings();
        }

		public bool AdvanceNext(out string filename, out int track) {

			filename = string.Empty;
			track = 0;
			if(PlaylistView.RowCount == 0) {
				return false;
			}

			if(PlaylistView.SelectedRows.Count == 0) {
				return false;
			}

			int index = PlaylistView.SelectedRows[0].Index;
			PlaylistView.ClearSelection();

			if(RandomMode) {
				Random rand = new Random();
				index = rand.Next(0, PlaylistView.RowCount);
			} else {
				index++;
				if(index == PlaylistView.RowCount) {
					if(LoopMode) {
						index = 0;
					} else {
						return false;
					}
				}
			}

			DataGridViewRow row = PlaylistView.Rows[index];
			if(row != null && row.DataBoundItem is BmpMidiEntry entry) {
				filename = entry.FilePath.FilePath;
				track = entry.Track.Track;
				row.Selected = true;
			}

			return true;
		}

		private void PlaylistControl_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
			e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Focus);
			e.Handled = true;
		}
		private void Playlist_Add_Click(object sender, EventArgs e) {
			OnPlaylistRequestAdd?.Invoke(this, e);

			SaveSettings();
		}

		private void Playlist_Remove_Click(object sender, EventArgs e)
        {
			if (PlaylistView.SelectedRows.Count == 1)
            {
                int rowIndexToRemove = PlaylistView.SelectedRows[0].Index;
                RemovePlaylistEntry(rowIndexToRemove);
            }
		}
		private void PlaylistView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
			PlaySelectedMidi();
            Select(e.RowIndex);
        }

		private void Playlist_Loop_CheckedChanged(object sender, EventArgs e) {
			LoopMode = (sender as CheckBox).Checked;
		}

		private void Playlist_Random_CheckedChanged(object sender, EventArgs e) {
			RandomMode = (sender as CheckBox).Checked;
		}

		// Funcs
		public void PlaySelectedMidi()
        {
			if (GetSelectedMidiEntry(out BmpMidiEntry entry))
            {
				OnMidiSelect?.Invoke(this, entry);
			}
		}

		public bool HasMidi() {
			return (playlistBinding.Count > 0);
		}

		// Helpers

		private bool GetSelectedMidiEntry(out BmpMidiEntry entry) {
			if(PlaylistView.SelectedRows.Count == 1) {
				entry = PlaylistView.SelectedRows[0].DataBoundItem as BmpMidiEntry;
				return true;
			}
			entry = new BmpMidiEntry();
			return false;
		}

        // // Drag and Drop and/or Reorder

        private bool actuallyMoving = false;
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
            if (e.Button == MouseButtons.Left)
            {
                // if we don't yet have an initial row index
                if (!actuallyMoving)
                {
                    // and we've moved the cursor with the button held down for
                    // a decently sufficient time, i.e. 10 pixels. seems good.
                    if (Math.Abs(mouseYCoordForMovement - e.Y) >= 10)
                    {
                        actuallyMoving = true;

                        Console.WriteLine("initialRow detected as {0}", initialRowIndexForMovement);

                        BmpMidiEntry data = PlaylistView.Rows[initialRowIndexForMovement].DataBoundItem as BmpMidiEntry;
                        this.DoDragDrop(data, DragDropEffects.Move);
                    }
                }
            }
        }

        private void BmpMidiEntryList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // store this now, since we could move to the next closest cell in MouseMove
                initialRowIndexForMovement = e.RowIndex;
                mouseYCoordForMovement = e.Y;
            }
        }

		private void BmpMidiEntryList_DragOver(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.Move;
		}

		private void BmpMidiEntryList_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = this.PointToClient(new Point(e.X, e.Y));
            DataGridView.HitTestInfo hit = PlaylistView.HitTest(clientPoint.X, clientPoint.Y);

            // for internal drag-and-drop
            if (actuallyMoving)
            {
                int targetRowIndex = hit.RowIndex - 1;
                if (hit.Type == DataGridViewHitTestType.None)
                {
                    // user has dnd onto empty space, simply move midi to the end of the playlist
                    targetRowIndex = PlaylistView.RowCount - 1;
                }

                BmpMidiEntry oldData = PlaylistView.Rows[initialRowIndexForMovement].DataBoundItem as BmpMidiEntry;
                RemovePlaylistEntry(initialRowIndexForMovement);
                AddPlaylistEntry(oldData.FilePath.FilePath, oldData.Track.Track, targetRowIndex);
                actuallyMoving = false;

                return;
            }

            int dropIndex = 0;
            if (hit.Type == DataGridViewHitTestType.Cell)
            {
                // user has dnd onto an existing cell, move above the hovered midi
                dropIndex = hit.RowIndex - 1;
            }
            else if (hit.Type == DataGridViewHitTestType.None)
            {
                // user has dnd onto empty space, simply add midi to the end of the playlist
                dropIndex = playlistEntries.Count;
            }

            if (e.Effect == DragDropEffects.Move)
            {
                // just in case the user gets fancy with it and dnd's multiple midis
                string[] midiList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string midiFilePath in midiList)
                {
                    string filename = Path.GetFileName(midiFilePath);
                    BmpPlaylistRequestAddEvent addEvent = new BmpPlaylistRequestAddEvent(filename, 0, dropIndex);
                    OnPlaylistManualRequestAdd?.Invoke(this, addEvent);
                }
            }
        }
    }
}
