namespace FFBardMusicPlayer.Forms {
	partial class BmpMain {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BmpMain));
			this.MainTable = new System.Windows.Forms.TableLayoutPanel();
			this.ChatPlaylistTable = new System.Windows.Forms.TableLayoutPanel();
			this.InfoTabs = new System.Windows.Forms.TabControl();
			this.ChatAllTab = new System.Windows.Forms.TabPage();
			this.CommandTab = new System.Windows.Forms.TabPage();
			this.localOrchestraTab = new System.Windows.Forms.TabPage();
			this.SettingsTab = new System.Windows.Forms.TabPage();
			this.StatisticTab = new System.Windows.Forms.TabPage();
			this.BottomTable = new System.Windows.Forms.TableLayoutPanel();
			this.DonationButton = new System.Windows.Forms.Button();
			this.Playlist = new FFBardMusicPlayer.Controls.BmpPlaylist();
			this.ChatLogAll = new FFBardMusicPlayer.Components.BmpChatLog(this.components);
			this.ChatLogCmd = new FFBardMusicPlayer.Components.BmpChatLog(this.components);
			this.LocalOrchestra = new FFBardMusicPlayer.Controls.BmpLocalOrchestra();
			this.Settings = new FFBardMusicPlayer.Controls.BmpSettings();
			this.Statistics = new FFBardMusicPlayer.Controls.BmpStatistics();
			this.Explorer = new FFBardMusicPlayer.Controls.BmpExplorer();
			this.Player = new FFBardMusicPlayer.Controls.BmpPlayer();
			this.FFXIV = new FFBardMusicPlayer.Controls.BmpHook();
			this.MainTable.SuspendLayout();
			this.ChatPlaylistTable.SuspendLayout();
			this.InfoTabs.SuspendLayout();
			this.ChatAllTab.SuspendLayout();
			this.CommandTab.SuspendLayout();
			this.localOrchestraTab.SuspendLayout();
			this.SettingsTab.SuspendLayout();
			this.StatisticTab.SuspendLayout();
			this.BottomTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// MainTable
			// 
			this.MainTable.ColumnCount = 1;
			this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.MainTable.Controls.Add(this.ChatPlaylistTable, 0, 1);
			this.MainTable.Controls.Add(this.Explorer, 0, 2);
			this.MainTable.Controls.Add(this.Player, 0, 3);
			this.MainTable.Controls.Add(this.BottomTable, 0, 4);
			this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainTable.Location = new System.Drawing.Point(0, 0);
			this.MainTable.Margin = new System.Windows.Forms.Padding(0);
			this.MainTable.Name = "MainTable";
			this.MainTable.RowCount = 5;
			this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
			this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this.MainTable.Size = new System.Drawing.Size(584, 461);
			this.MainTable.TabIndex = 1;
			// 
			// ChatPlaylistTable
			// 
			this.ChatPlaylistTable.ColumnCount = 3;
			this.ChatPlaylistTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ChatPlaylistTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ChatPlaylistTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ChatPlaylistTable.Controls.Add(this.Playlist, 0, 0);
			this.ChatPlaylistTable.Controls.Add(this.InfoTabs, 1, 0);
			this.ChatPlaylistTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChatPlaylistTable.Location = new System.Drawing.Point(0, 0);
			this.ChatPlaylistTable.Margin = new System.Windows.Forms.Padding(0);
			this.ChatPlaylistTable.Name = "ChatPlaylistTable";
			this.ChatPlaylistTable.RowCount = 1;
			this.ChatPlaylistTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ChatPlaylistTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 187F));
			this.ChatPlaylistTable.Size = new System.Drawing.Size(584, 187);
			this.ChatPlaylistTable.TabIndex = 1;
			// 
			// InfoTabs
			// 
			this.InfoTabs.Controls.Add(this.ChatAllTab);
			this.InfoTabs.Controls.Add(this.CommandTab);
			this.InfoTabs.Controls.Add(this.localOrchestraTab);
			this.InfoTabs.Controls.Add(this.SettingsTab);
			this.InfoTabs.Controls.Add(this.StatisticTab);
			this.InfoTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InfoTabs.Location = new System.Drawing.Point(200, 0);
			this.InfoTabs.Margin = new System.Windows.Forms.Padding(0);
			this.InfoTabs.Name = "InfoTabs";
			this.InfoTabs.SelectedIndex = 0;
			this.InfoTabs.Size = new System.Drawing.Size(384, 187);
			this.InfoTabs.TabIndex = 1;
			// 
			// ChatAllTab
			// 
			this.ChatAllTab.Controls.Add(this.ChatLogAll);
			this.ChatAllTab.Location = new System.Drawing.Point(4, 22);
			this.ChatAllTab.Name = "ChatAllTab";
			this.ChatAllTab.Size = new System.Drawing.Size(376, 161);
			this.ChatAllTab.TabIndex = 0;
			this.ChatAllTab.Text = "[Chat] All";
			this.ChatAllTab.UseVisualStyleBackColor = true;
			// 
			// CommandTab
			// 
			this.CommandTab.Controls.Add(this.ChatLogCmd);
			this.CommandTab.Location = new System.Drawing.Point(4, 22);
			this.CommandTab.Name = "CommandTab";
			this.CommandTab.Size = new System.Drawing.Size(376, 161);
			this.CommandTab.TabIndex = 1;
			this.CommandTab.Text = "[Chat] Commands";
			this.CommandTab.UseVisualStyleBackColor = true;
			// 
			// localOrchestraTab
			// 
			this.localOrchestraTab.Controls.Add(this.LocalOrchestra);
			this.localOrchestraTab.Location = new System.Drawing.Point(4, 22);
			this.localOrchestraTab.Name = "localOrchestraTab";
			this.localOrchestraTab.Padding = new System.Windows.Forms.Padding(3);
			this.localOrchestraTab.Size = new System.Drawing.Size(376, 161);
			this.localOrchestraTab.TabIndex = 4;
			this.localOrchestraTab.Text = "Local orchestra";
			this.localOrchestraTab.UseVisualStyleBackColor = true;
			// 
			// SettingsTab
			// 
			this.SettingsTab.Controls.Add(this.Settings);
			this.SettingsTab.Location = new System.Drawing.Point(4, 22);
			this.SettingsTab.Name = "SettingsTab";
			this.SettingsTab.Size = new System.Drawing.Size(376, 161);
			this.SettingsTab.TabIndex = 2;
			this.SettingsTab.Text = "Settings";
			this.SettingsTab.UseVisualStyleBackColor = true;
			// 
			// StatisticTab
			// 
			this.StatisticTab.Controls.Add(this.Statistics);
			this.StatisticTab.Location = new System.Drawing.Point(4, 22);
			this.StatisticTab.Name = "StatisticTab";
			this.StatisticTab.Padding = new System.Windows.Forms.Padding(3);
			this.StatisticTab.Size = new System.Drawing.Size(376, 161);
			this.StatisticTab.TabIndex = 3;
			this.StatisticTab.Text = "Statistics";
			this.StatisticTab.UseVisualStyleBackColor = true;
			// 
			// BottomTable
			// 
			this.BottomTable.ColumnCount = 3;
			this.BottomTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.BottomTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.BottomTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.BottomTable.Controls.Add(this.FFXIV, 0, 0);
			this.BottomTable.Controls.Add(this.DonationButton, 1, 0);
			this.BottomTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BottomTable.Location = new System.Drawing.Point(0, 437);
			this.BottomTable.Margin = new System.Windows.Forms.Padding(0);
			this.BottomTable.Name = "BottomTable";
			this.BottomTable.RowCount = 1;
			this.BottomTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.BottomTable.Size = new System.Drawing.Size(584, 24);
			this.BottomTable.TabIndex = 6;
			// 
			// DonationButton
			// 
			this.DonationButton.BackColor = System.Drawing.Color.Khaki;
			this.DonationButton.Location = new System.Drawing.Point(527, 0);
			this.DonationButton.Margin = new System.Windows.Forms.Padding(0);
			this.DonationButton.Name = "DonationButton";
			this.DonationButton.Size = new System.Drawing.Size(57, 23);
			this.DonationButton.TabIndex = 6;
			this.DonationButton.Text = "About";
			this.DonationButton.UseVisualStyleBackColor = false;
			this.DonationButton.Click += new System.EventHandler(this.DonationButton_Click);
			// 
			// Playlist
			// 
			this.Playlist.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Playlist.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Playlist.Location = new System.Drawing.Point(0, 0);
			this.Playlist.LoopMode = false;
			this.Playlist.Margin = new System.Windows.Forms.Padding(0);
			this.Playlist.Name = "Playlist";
			this.Playlist.RandomMode = false;
			this.Playlist.Size = new System.Drawing.Size(200, 187);
			this.Playlist.TabIndex = 0;
			// 
			// ChatLogAll
			// 
			this.ChatLogAll.BackColor = System.Drawing.Color.Gray;
			this.ChatLogAll.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ChatLogAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChatLogAll.Location = new System.Drawing.Point(0, 0);
			this.ChatLogAll.Margin = new System.Windows.Forms.Padding(0);
			this.ChatLogAll.Name = "ChatLogAll";
			this.ChatLogAll.ReadOnly = true;
			this.ChatLogAll.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.ChatLogAll.Size = new System.Drawing.Size(376, 161);
			this.ChatLogAll.TabIndex = 0;
			this.ChatLogAll.Text = "";
			// 
			// ChatLogCmd
			// 
			this.ChatLogCmd.BackColor = System.Drawing.Color.Gray;
			this.ChatLogCmd.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ChatLogCmd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChatLogCmd.Location = new System.Drawing.Point(0, 0);
			this.ChatLogCmd.Margin = new System.Windows.Forms.Padding(0);
			this.ChatLogCmd.Name = "ChatLogCmd";
			this.ChatLogCmd.ReadOnly = true;
			this.ChatLogCmd.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.ChatLogCmd.Size = new System.Drawing.Size(376, 161);
			this.ChatLogCmd.TabIndex = 0;
			this.ChatLogCmd.Text = "";
			// 
			// LocalOrchestra
			// 
			this.LocalOrchestra.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LocalOrchestra.Location = new System.Drawing.Point(3, 3);
			this.LocalOrchestra.Name = "LocalOrchestra";
			this.LocalOrchestra.OrchestraEnabled = false;
			this.LocalOrchestra.Sequencer = null;
			this.LocalOrchestra.Size = new System.Drawing.Size(370, 155);
			this.LocalOrchestra.TabIndex = 0;
			// 
			// Settings
			// 
			this.Settings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Settings.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.Settings.Location = new System.Drawing.Point(0, 0);
			this.Settings.Margin = new System.Windows.Forms.Padding(0);
			this.Settings.Name = "Settings";
			this.Settings.Size = new System.Drawing.Size(376, 161);
			this.Settings.TabIndex = 0;
			// 
			// Statistics
			// 
			this.Statistics.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Statistics.Location = new System.Drawing.Point(3, 3);
			this.Statistics.Name = "Statistics";
			this.Statistics.Size = new System.Drawing.Size(370, 155);
			this.Statistics.TabIndex = 0;
			// 
			// Explorer
			// 
			this.Explorer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Explorer.Location = new System.Drawing.Point(3, 190);
			this.Explorer.Name = "Explorer";
			this.Explorer.Size = new System.Drawing.Size(578, 24);
			this.Explorer.SongBrowserVisible = false;
			this.Explorer.TabIndex = 3;
			// 
			// Player
			// 
			this.Player.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Player.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.Player.Interactable = true;
			this.Player.Location = new System.Drawing.Point(3, 220);
			this.Player.Loop = false;
			this.Player.Name = "Player";
			this.Player.OctaveShift = 0;
			this.Player.Size = new System.Drawing.Size(578, 214);
			this.Player.SpeedShift = 1F;
			this.Player.Status = FFBardMusicPlayer.Controls.BmpPlayer.PlayerStatus.Performing;
			this.Player.TabIndex = 4;
			this.Player.Tempo = 0;
			this.Player.TrackName = null;
			// 
			// FFXIV
			// 
			this.FFXIV.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::FFBardMusicPlayer.Properties.Settings.Default, "Location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.FFXIV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FFXIV.Location = global::FFBardMusicPlayer.Properties.Settings.Default.Location;
			this.FFXIV.Margin = new System.Windows.Forms.Padding(0);
			this.FFXIV.Name = "FFXIV";
			this.FFXIV.Size = new System.Drawing.Size(527, 24);
			this.FFXIV.TabIndex = 5;
			// 
			// BmpMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 461);
			this.Controls.Add(this.MainTable);
			this.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(500, 500);
			this.Name = "BmpMain";
			this.Text = "BmpMain";
			this.MainTable.ResumeLayout(false);
			this.ChatPlaylistTable.ResumeLayout(false);
			this.InfoTabs.ResumeLayout(false);
			this.ChatAllTab.ResumeLayout(false);
			this.CommandTab.ResumeLayout(false);
			this.localOrchestraTab.ResumeLayout(false);
			this.SettingsTab.ResumeLayout(false);
			this.StatisticTab.ResumeLayout(false);
			this.BottomTable.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel MainTable;
		private System.Windows.Forms.TableLayoutPanel ChatPlaylistTable;
		private Controls.BmpPlaylist Playlist;
		private System.Windows.Forms.TabControl InfoTabs;
		private System.Windows.Forms.TabPage ChatAllTab;
		private System.Windows.Forms.TabPage CommandTab;
		private System.Windows.Forms.TabPage SettingsTab;
		private Components.BmpChatLog ChatLogCmd;
		private Controls.BmpSettings Settings;
		private Components.BmpChatLog ChatLogAll;
		private Controls.BmpPlayer Player;
		private Controls.BmpExplorer Explorer;
		private System.Windows.Forms.TabPage StatisticTab;
		private Controls.BmpStatistics Statistics;
		private System.Windows.Forms.TabPage localOrchestraTab;
		private Controls.BmpLocalOrchestra LocalOrchestra;
		private Controls.BmpHook FFXIV;
		private System.Windows.Forms.TableLayoutPanel BottomTable;
		private System.Windows.Forms.Button DonationButton;
	}
}