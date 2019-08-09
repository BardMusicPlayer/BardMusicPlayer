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

		public EventHandler<BmpMidiEntry> OnMidiSelect;
		public EventHandler OnPlaylistRequestAdd;

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

		public void Select(int index) {
			if(PlaylistView.Rows.Count > 0 && index < PlaylistView.Rows.Count) {
				PlaylistView.Rows[index].Selected = true;
			}
		}

		public BmpMidiEntry AddPlaylistEntry(string filename, int track = 0) {
			BmpMidiEntry entry = new BmpMidiEntry(filename, track);

			int entryIndex = 0;
			if(PlaylistView.SelectedRows.Count == 1) {
				DataGridViewRow row = PlaylistView.SelectedRows[0];
				row.Selected = false;

				entryIndex = row.Index + 1;
				playlistEntries.Insert(entryIndex, entry);
			} else {
				playlistEntries.Add(entry);
				entryIndex = playlistEntries.Count - 1;
			}

			if(entryIndex < PlaylistView.Rows.Count) {
				PlaylistView.Rows[entryIndex].Selected = true;
			}

			playlistBinding = new BindingList<BmpMidiEntry>(playlistEntries);
			PlaylistView.DataSource = playlistBinding;
			return entry;
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

		private void Playlist_Remove_Click(object sender, EventArgs e) {
			// Remove
			if(PlaylistView.SelectedRows.Count == 1) {
				DataGridViewRow row1 = PlaylistView.SelectedRows[0];
				int row1index = row1.Index;
				if(row1.DataBoundItem is BmpMidiEntry entry) {
					row1.Selected = false;
					playlistBinding.Remove(entry);
				}
				if(PlaylistView.Rows.Count > 0) {
					int r = row1index.Clamp(0, PlaylistView.Rows.Count - 1);
					DataGridViewRow row2 = PlaylistView.Rows[r];
					if(row2 != null) {
						row2.Selected = true;
					}
				}
				SaveSettings();
			}
		}
		private void PlaylistView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
			PlaySelectedMidi();
		}

		private void Playlist_Loop_CheckedChanged(object sender, EventArgs e) {
			LoopMode = (sender as CheckBox).Checked;
		}

		private void Playlist_Random_CheckedChanged(object sender, EventArgs e) {
			RandomMode = (sender as CheckBox).Checked;
		}
		// Funcs
		public void PlaySelectedMidi() {
			if(GetSelectedMidiEntry(out BmpMidiEntry entry)) {
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

		// Reorder

		private Rectangle dragBox;
		private int rowIndex;
		private int dropIndex;

		private void BmpMidiEntryList_MouseMove(object sender, MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) == MouseButtons.Left) {
				if(dragBox != Rectangle.Empty && !dragBox.Contains(e.X, e.Y)) {
					BmpMidiEntry data = PlaylistView.Rows[rowIndex].DataBoundItem as BmpMidiEntry;
					DragDropEffects dropEffect = this.DoDragDrop(data, DragDropEffects.Move);
				}
			}
		}

		private void BmpMidiEntryList_MouseDown(object sender, MouseEventArgs e) {
			rowIndex = PlaylistView.HitTest(e.X, e.Y).RowIndex;
			if(rowIndex != -1) {
				Size dragSize = SystemInformation.DragSize;
				dragBox = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
			} else {
				dragBox = Rectangle.Empty;
			}
		}

		private void BmpMidiEntryList_DragOver(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.Move;
		}

		private void BmpMidiEntryList_DragDrop(object sender, DragEventArgs e) {

			Point clientPoint = this.PointToClient(new Point(e.X, e.Y));
			dropIndex = PlaylistView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

			if(dropIndex >= playlistEntries.Count || dropIndex == -1) {
				dropIndex = playlistEntries.Count - 1;
			}

			if(e.Effect == DragDropEffects.Move) {
				//DataGridViewRow data = e.Data.GetData(typeof(DataGridViewRow)) as DataGridViewRow;
				var data = e.Data.GetData(typeof(BmpMidiEntry)) as BmpMidiEntry;
				playlistEntries.RemoveAt(rowIndex);
				playlistEntries.Insert(dropIndex, new BmpMidiEntry(data.FilePath.FilePath, data.Track.Track));
				Select(dropIndex);
			}
			playlistBinding = new BindingList<BmpMidiEntry>(playlistEntries);
			PlaylistView.DataSource = playlistBinding;
		}
	}
}
