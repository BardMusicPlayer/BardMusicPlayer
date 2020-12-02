namespace FFBardMusicPlayer.Controls {
	partial class BmpExplorer {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.SeekerTable = new System.Windows.Forms.TableLayoutPanel();
            this.PlayAllTracks = new FFBardMusicPlayer.Components.BmpCheckButton(this.components);
            this.MusicReload = new System.Windows.Forms.Button();
            this.SelectorSong = new FFBardMusicPlayer.Controls.SongSearcher();
            this.SelectorTrack = new FFBardMusicPlayer.Components.BmpTrackShift();
            this.BrowserTable = new System.Windows.Forms.TableLayoutPanel();
            this.SongBrowser = new FFBardMusicPlayer.Components.BmpBrowser(this.components);
            this.SeekerTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectorTrack)).BeginInit();
            this.BrowserTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // SeekerTable
            // 
            this.SeekerTable.ColumnCount = 4;
            this.SeekerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SeekerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SeekerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SeekerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SeekerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SeekerTable.Controls.Add(this.PlayAllTracks, 3, 0);
            this.SeekerTable.Controls.Add(this.MusicReload, 0, 0);
            this.SeekerTable.Controls.Add(this.SelectorSong, 1, 0);
            this.SeekerTable.Controls.Add(this.SelectorTrack, 2, 0);
            this.SeekerTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SeekerTable.Location = new System.Drawing.Point(0, 0);
            this.SeekerTable.Margin = new System.Windows.Forms.Padding(0);
            this.SeekerTable.Name = "SeekerTable";
            this.SeekerTable.RowCount = 1;
            this.SeekerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SeekerTable.Size = new System.Drawing.Size(813, 24);
            this.SeekerTable.TabIndex = 29;
            // 
            // PlayAllTracks
            // 
            this.PlayAllTracks.Appearance = System.Windows.Forms.Appearance.Button;
            this.PlayAllTracks.AutoSize = true;
            this.PlayAllTracks.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.PlayAllTracks;
            this.PlayAllTracks.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "PlayAllTracks", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.PlayAllTracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayAllTracks.Location = new System.Drawing.Point(753, 0);
            this.PlayAllTracks.Margin = new System.Windows.Forms.Padding(0);
            this.PlayAllTracks.Name = "PlayAllTracks";
            this.PlayAllTracks.Size = new System.Drawing.Size(60, 24);
            this.PlayAllTracks.TabIndex = 3;
            this.PlayAllTracks.Text = "All tracks";
            this.PlayAllTracks.UseVisualStyleBackColor = true;
            this.PlayAllTracks.CheckedChanged += new System.EventHandler(this.PlayAllTracks_CheckedChanged);
            // 
            // MusicReload
            // 
            this.MusicReload.AutoSize = true;
            this.MusicReload.FlatAppearance.BorderSize = 0;
            this.MusicReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MusicReload.Image = global::FFBardMusicPlayer.Properties.Resources.Refresh;
            this.MusicReload.Location = new System.Drawing.Point(0, 0);
            this.MusicReload.Margin = new System.Windows.Forms.Padding(0);
            this.MusicReload.Name = "MusicReload";
            this.MusicReload.Size = new System.Drawing.Size(26, 24);
            this.MusicReload.TabIndex = 0;
            this.MusicReload.TabStop = false;
            this.MusicReload.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.MusicReload.UseVisualStyleBackColor = true;
            // 
            // SelectorSong
            // 
            this.SelectorSong.BackColor = System.Drawing.Color.White;
            this.SelectorSong.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SelectorSong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectorSong.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.SelectorSong.Location = new System.Drawing.Point(28, 0);
            this.SelectorSong.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SelectorSong.Name = "SelectorSong";
            this.SelectorSong.Size = new System.Drawing.Size(673, 22);
            this.SelectorSong.TabIndex = 1;
            this.SelectorSong.Text = "Click here to load Midi file...";
            this.SelectorSong.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SelectorTrack
            // 
            this.SelectorTrack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.SelectorTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectorTrack.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.SelectorTrack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.SelectorTrack.Location = new System.Drawing.Point(703, 0);
            this.SelectorTrack.Margin = new System.Windows.Forms.Padding(0);
            this.SelectorTrack.Name = "SelectorTrack";
            this.SelectorTrack.Size = new System.Drawing.Size(50, 25);
            this.SelectorTrack.TabIndex = 2;
            this.SelectorTrack.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BrowserTable
            // 
            this.BrowserTable.ColumnCount = 1;
            this.BrowserTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.BrowserTable.Controls.Add(this.SeekerTable, 0, 0);
            this.BrowserTable.Controls.Add(this.SongBrowser, 0, 1);
            this.BrowserTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BrowserTable.Location = new System.Drawing.Point(0, 0);
            this.BrowserTable.Name = "BrowserTable";
            this.BrowserTable.RowCount = 1;
            this.BrowserTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.BrowserTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.BrowserTable.Size = new System.Drawing.Size(813, 149);
            this.BrowserTable.TabIndex = 30;
            // 
            // SongBrowser
            // 
            this.SongBrowser.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SongBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SongBrowser.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.SongBrowser.FilenameFilter = "";
            this.SongBrowser.Font = new System.Drawing.Font("Lucida Console", 9F);
            this.SongBrowser.FormattingEnabled = true;
            this.SongBrowser.IntegralHeight = false;
            this.SongBrowser.ItemHeight = 16;
            this.SongBrowser.Location = new System.Drawing.Point(0, 24);
            this.SongBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.SongBrowser.Name = "SongBrowser";
            this.SongBrowser.Size = new System.Drawing.Size(813, 125);
            this.SongBrowser.TabIndex = 33;
            // 
            // BmpExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BrowserTable);
            this.Name = "BmpExplorer";
            this.Size = new System.Drawing.Size(813, 149);
            this.SeekerTable.ResumeLayout(false);
            this.SeekerTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectorTrack)).EndInit();
            this.BrowserTable.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel SeekerTable;
		private System.Windows.Forms.Button MusicReload;
		private SongSearcher SelectorSong;
		private Components.BmpTrackShift SelectorTrack;
		private System.Windows.Forms.TableLayoutPanel BrowserTable;
		private Components.BmpBrowser SongBrowser;
        private Components.BmpCheckButton PlayAllTracks;
    }
}
