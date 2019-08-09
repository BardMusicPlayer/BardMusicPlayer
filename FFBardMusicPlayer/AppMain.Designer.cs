namespace FFBardMusicPlayer {
	partial class AppMain {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppMain));
			this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.MenuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.HelpTip = new System.Windows.Forms.ToolTip(this.components);
			this.ContentPanel = new System.Windows.Forms.TableLayoutPanel();
			this.TextPanel = new System.Windows.Forms.TableLayoutPanel();
			this.LogView = new FFBardMusicPlayer.Controls.BmpSettings();
			this.SwitchablePanel = new System.Windows.Forms.Panel();
			this.Playlist = new FFBardMusicPlayer.Controls.BmpPlaylist();
			this.Orchestra = new FFBardMusicPlayer.Controls.OrchestraControl();
			this.Bmp = new FFBardMusicPlayer.Controls.BmpPlayerInterface();
			this.ForceOpen = new System.Windows.Forms.CheckBox();
			this.InfoStatus = new System.Windows.Forms.Label();
			this.PerformerSetControl = new FFBardMusicPlayer.Controls.PerformerSetControl();
			this.InfoTable = new System.Windows.Forms.TableLayoutPanel();
			this.TrayMenu.SuspendLayout();
			this.ContentPanel.SuspendLayout();
			this.TextPanel.SuspendLayout();
			this.SwitchablePanel.SuspendLayout();
			this.InfoTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// TrayIcon
			// 
			this.TrayIcon.ContextMenuStrip = this.TrayMenu;
			this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
			this.TrayIcon.Text = "Bard Music Player";
			this.TrayIcon.Visible = true;
			// 
			// TrayMenu
			// 
			this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuExit});
			this.TrayMenu.Name = "notifyMenu";
			this.TrayMenu.Size = new System.Drawing.Size(93, 26);
			// 
			// MenuExit
			// 
			this.MenuExit.Name = "MenuExit";
			this.MenuExit.Size = new System.Drawing.Size(92, 22);
			this.MenuExit.Text = "Exit";
			// 
			// HelpTip
			// 
			this.HelpTip.AutoPopDelay = 5000;
			this.HelpTip.BackColor = System.Drawing.Color.White;
			this.HelpTip.ForeColor = System.Drawing.Color.Black;
			this.HelpTip.InitialDelay = 100;
			this.HelpTip.ReshowDelay = 100;
			this.HelpTip.UseAnimation = false;
			this.HelpTip.UseFading = false;
			// 
			// ContentPanel
			// 
			this.ContentPanel.AutoSize = true;
			this.ContentPanel.ColumnCount = 1;
			this.ContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ContentPanel.Controls.Add(this.TextPanel, 0, 0);
			this.ContentPanel.Controls.Add(this.Bmp, 0, 1);
			this.ContentPanel.Controls.Add(this.InfoTable, 0, 2);
			this.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ContentPanel.Location = new System.Drawing.Point(0, 0);
			this.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
			this.ContentPanel.Name = "ContentPanel";
			this.ContentPanel.RowCount = 3;
			this.ContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ContentPanel.Size = new System.Drawing.Size(534, 381);
			this.ContentPanel.TabIndex = 6;
			// 
			// TextPanel
			// 
			this.TextPanel.ColumnCount = 2;
			this.TextPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TextPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.TextPanel.Controls.Add(this.LogView, 1, 0);
			this.TextPanel.Controls.Add(this.SwitchablePanel, 0, 0);
			this.TextPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TextPanel.Location = new System.Drawing.Point(0, 0);
			this.TextPanel.Margin = new System.Windows.Forms.Padding(0);
			this.TextPanel.Name = "TextPanel";
			this.TextPanel.RowCount = 1;
			this.TextPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TextPanel.Size = new System.Drawing.Size(534, 189);
			this.TextPanel.TabIndex = 14;
			// 
			// LogView
			// 
			this.LogView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LogView.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.LogView.Location = new System.Drawing.Point(200, 0);
			this.LogView.Margin = new System.Windows.Forms.Padding(0);
			this.LogView.Name = "LogView";
			this.LogView.Size = new System.Drawing.Size(334, 189);
			this.LogView.TabIndex = 13;
			// 
			// SwitchablePanel
			// 
			this.SwitchablePanel.Controls.Add(this.Playlist);
			this.SwitchablePanel.Controls.Add(this.Orchestra);
			this.SwitchablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SwitchablePanel.Location = new System.Drawing.Point(3, 3);
			this.SwitchablePanel.Name = "SwitchablePanel";
			this.SwitchablePanel.Size = new System.Drawing.Size(194, 183);
			this.SwitchablePanel.TabIndex = 15;
			// 
			// Playlist
			// 
			this.Playlist.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Playlist.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Playlist.Location = new System.Drawing.Point(0, 0);
			this.Playlist.LoopMode = false;
			this.Playlist.Name = "Playlist";
			this.Playlist.RandomMode = false;
			this.Playlist.Size = new System.Drawing.Size(194, 183);
			this.Playlist.TabIndex = 15;
			// 
			// Orchestra
			// 
			this.Orchestra.BackColor = System.Drawing.Color.Black;
			this.Orchestra.ConductorName = "test";
			this.Orchestra.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Orchestra.Location = new System.Drawing.Point(0, 0);
			this.Orchestra.Margin = new System.Windows.Forms.Padding(0);
			this.Orchestra.Name = "Orchestra";
			this.Orchestra.Size = new System.Drawing.Size(194, 183);
			this.Orchestra.TabIndex = 14;
			// 
			// Bmp
			// 
			this.Bmp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Bmp.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.Bmp.Interactable = true;
			this.Bmp.Location = new System.Drawing.Point(0, 189);
			this.Bmp.Loop = false;
			this.Bmp.Margin = new System.Windows.Forms.Padding(0);
			this.Bmp.Name = "Bmp";
			this.Bmp.OctaveShift = 0;
			this.Bmp.PreferredInstrument = Sharlayan.Core.Enums.Performance.Instrument.Piano;
			this.Bmp.SelectorSongFocus = false;
			this.Bmp.Size = new System.Drawing.Size(534, 172);
			this.Bmp.SongBrowserVisible = false;
			this.Bmp.SpeedShift = 1F;
			this.Bmp.Status = FFBardMusicPlayer.Controls.BmpPlayerInterface.BmpStatus.PerformerSolo;
			this.Bmp.TabIndex = 12;
			this.Bmp.Tempo = 0;
			this.Bmp.TrackName = "Track name";
			// 
			// ForceOpen
			// 
			this.ForceOpen.AutoSize = true;
			this.ForceOpen.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.ForcedOpen;
			this.ForceOpen.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "ForcedOpen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.ForceOpen.Location = new System.Drawing.Point(517, 3);
			this.ForceOpen.Name = "ForceOpen";
			this.ForceOpen.Size = new System.Drawing.Size(14, 14);
			this.ForceOpen.TabIndex = 10;
			this.ForceOpen.UseVisualStyleBackColor = true;
			// 
			// InfoStatus
			// 
			this.InfoStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InfoStatus.Location = new System.Drawing.Point(0, 0);
			this.InfoStatus.Margin = new System.Windows.Forms.Padding(0);
			this.InfoStatus.MaximumSize = new System.Drawing.Size(430, 20);
			this.InfoStatus.MinimumSize = new System.Drawing.Size(430, 20);
			this.InfoStatus.Name = "InfoStatus";
			this.InfoStatus.Size = new System.Drawing.Size(430, 20);
			this.InfoStatus.TabIndex = 9;
			this.InfoStatus.Text = "Loading...";
			this.InfoStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PerformerSetControl
			// 
			this.PerformerSetControl.BackColor = System.Drawing.Color.Black;
			this.PerformerSetControl.Location = new System.Drawing.Point(169, 52);
			this.PerformerSetControl.Margin = new System.Windows.Forms.Padding(0);
			this.PerformerSetControl.Name = "PerformerSetControl";
			this.PerformerSetControl.Padding = new System.Windows.Forms.Padding(2);
			this.PerformerSetControl.Player = this.Bmp;
			this.PerformerSetControl.Size = new System.Drawing.Size(273, 61);
			this.PerformerSetControl.TabIndex = 13;
			this.PerformerSetControl.Visible = false;
			// 
			// InfoTable
			// 
			this.InfoTable.ColumnCount = 2;
			this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.Controls.Add(this.ForceOpen, 1, 0);
			this.InfoTable.Controls.Add(this.InfoStatus, 0, 0);
			this.InfoTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InfoTable.Location = new System.Drawing.Point(0, 361);
			this.InfoTable.Margin = new System.Windows.Forms.Padding(0);
			this.InfoTable.Name = "InfoTable";
			this.InfoTable.RowCount = 1;
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.Size = new System.Drawing.Size(534, 20);
			this.InfoTable.TabIndex = 11;
			// 
			// AppMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(534, 381);
			this.Controls.Add(this.ContentPanel);
			this.Controls.Add(this.PerformerSetControl);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(550, 560);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(550, 420);
			this.Name = "AppMain";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "BMP";
			this.TrayMenu.ResumeLayout(false);
			this.ContentPanel.ResumeLayout(false);
			this.TextPanel.ResumeLayout(false);
			this.SwitchablePanel.ResumeLayout(false);
			this.InfoTable.ResumeLayout(false);
			this.InfoTable.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ToolTip HelpTip;
		private System.Windows.Forms.NotifyIcon TrayIcon;
		private System.Windows.Forms.ContextMenuStrip TrayMenu;
		private System.Windows.Forms.ToolStripMenuItem MenuExit;
		private Controls.PerformerSetControl PerformerSetControl;
		private System.Windows.Forms.TableLayoutPanel ContentPanel;
		private Controls.BmpSettings LogView;
		private System.Windows.Forms.CheckBox ForceOpen;
		private System.Windows.Forms.Label InfoStatus;
		private Controls.BmpPlayerInterface Bmp;
		private System.Windows.Forms.TableLayoutPanel TextPanel;
		private Controls.OrchestraControl Orchestra;
		private System.Windows.Forms.Panel SwitchablePanel;
		private Controls.BmpPlaylist Playlist;
		private System.Windows.Forms.TableLayoutPanel InfoTable;
	}
}