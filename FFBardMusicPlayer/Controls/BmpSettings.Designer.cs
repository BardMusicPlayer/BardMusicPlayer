namespace FFBardMusicPlayer.Controls {
	partial class BmpSettings {
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
			this.GeneralSettings = new System.Windows.Forms.GroupBox();
			this.KeyboardTest = new System.Windows.Forms.Button();
			this.SettingsScrollPanel = new System.Windows.Forms.Panel();
			this.verboseToggle = new System.Windows.Forms.CheckBox();
			this.UnequipPause = new System.Windows.Forms.CheckBox();
			this.ForceOpenToggle = new System.Windows.Forms.CheckBox();
			this.SignatureFolder = new System.Windows.Forms.Button();
			this.sigCheckbox = new System.Windows.Forms.CheckBox();
			this.MidiInputLabel = new System.Windows.Forms.Label();
			this.SettingMidiInput = new System.Windows.Forms.ComboBox();
			this.SettingChatSave = new System.Windows.Forms.CheckBox();
			this.SettingBringBmp = new System.Windows.Forms.CheckBox();
			this.SettingBringGame = new System.Windows.Forms.CheckBox();
			this.SettingsTable = new System.Windows.Forms.TableLayoutPanel();
			this.ChatSettings = new System.Windows.Forms.GroupBox();
			this.ListenChatList = new System.Windows.Forms.ComboBox();
			this.ForceListenToggle = new System.Windows.Forms.CheckBox();
			this.PlaybackSettings = new System.Windows.Forms.GroupBox();
			this.TooFastChange = new System.Windows.Forms.NumericUpDown();
			this.PlayHoldChange = new System.Windows.Forms.NumericUpDown();
			this.SlowPlayToggle = new System.Windows.Forms.CheckBox();
			this.ArpeggiateToggle = new System.Windows.Forms.CheckBox();
			this.SettingHoldNotes = new System.Windows.Forms.CheckBox();
			this.ChatSimToggle = new System.Windows.Forms.CheckBox();
			this.HelpTip = new System.Windows.Forms.ToolTip(this.components);
			this.GeneralSettings.SuspendLayout();
			this.SettingsScrollPanel.SuspendLayout();
			this.SettingsTable.SuspendLayout();
			this.ChatSettings.SuspendLayout();
			this.PlaybackSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.TooFastChange)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PlayHoldChange)).BeginInit();
			this.SuspendLayout();
			// 
			// GeneralSettings
			// 
			this.GeneralSettings.AutoSize = true;
			this.GeneralSettings.BackColor = System.Drawing.Color.Transparent;
			this.GeneralSettings.Controls.Add(this.KeyboardTest);
			this.GeneralSettings.Controls.Add(this.SettingsScrollPanel);
			this.GeneralSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GeneralSettings.Location = new System.Drawing.Point(0, 0);
			this.GeneralSettings.Margin = new System.Windows.Forms.Padding(0);
			this.GeneralSettings.Name = "GeneralSettings";
			this.GeneralSettings.Size = new System.Drawing.Size(329, 318);
			this.GeneralSettings.TabIndex = 9;
			this.GeneralSettings.TabStop = false;
			this.GeneralSettings.Text = "Settings";
			// 
			// KeyboardTest
			// 
			this.KeyboardTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.KeyboardTest.Location = new System.Drawing.Point(227, 0);
			this.KeyboardTest.Name = "KeyboardTest";
			this.KeyboardTest.Size = new System.Drawing.Size(99, 23);
			this.KeyboardTest.TabIndex = 0;
			this.KeyboardTest.Text = "Test keyboard";
			this.KeyboardTest.UseVisualStyleBackColor = true;
			// 
			// SettingsScrollPanel
			// 
			this.SettingsScrollPanel.AutoScroll = true;
			this.SettingsScrollPanel.Controls.Add(this.verboseToggle);
			this.SettingsScrollPanel.Controls.Add(this.UnequipPause);
			this.SettingsScrollPanel.Controls.Add(this.ForceOpenToggle);
			this.SettingsScrollPanel.Controls.Add(this.SignatureFolder);
			this.SettingsScrollPanel.Controls.Add(this.sigCheckbox);
			this.SettingsScrollPanel.Controls.Add(this.MidiInputLabel);
			this.SettingsScrollPanel.Controls.Add(this.SettingMidiInput);
			this.SettingsScrollPanel.Controls.Add(this.SettingChatSave);
			this.SettingsScrollPanel.Controls.Add(this.SettingBringBmp);
			this.SettingsScrollPanel.Controls.Add(this.SettingBringGame);
			this.SettingsScrollPanel.Controls.Add(this.SettingsTable);
			this.SettingsScrollPanel.Controls.Add(this.ChatSimToggle);
			this.SettingsScrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SettingsScrollPanel.Location = new System.Drawing.Point(3, 18);
			this.SettingsScrollPanel.Margin = new System.Windows.Forms.Padding(0);
			this.SettingsScrollPanel.Name = "SettingsScrollPanel";
			this.SettingsScrollPanel.Padding = new System.Windows.Forms.Padding(8);
			this.SettingsScrollPanel.Size = new System.Drawing.Size(323, 297);
			this.SettingsScrollPanel.TabIndex = 12;
			// 
			// verboseToggle
			// 
			this.verboseToggle.AutoSize = true;
			this.verboseToggle.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.Verbose;
			this.verboseToggle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "Verbose", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.verboseToggle.Location = new System.Drawing.Point(13, 265);
			this.verboseToggle.Name = "verboseToggle";
			this.verboseToggle.Size = new System.Drawing.Size(136, 17);
			this.verboseToggle.TabIndex = 20;
			this.verboseToggle.Text = "Enable verbose mode";
			this.HelpTip.SetToolTip(this.verboseToggle, "Print various kinds of information to the log window.");
			this.verboseToggle.UseVisualStyleBackColor = true;
			// 
			// UnequipPause
			// 
			this.UnequipPause.AutoSize = true;
			this.UnequipPause.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.UnequipPause;
			this.UnequipPause.CheckState = System.Windows.Forms.CheckState.Checked;
			this.UnequipPause.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "UnequipPause", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.UnequipPause.Location = new System.Drawing.Point(13, 242);
			this.UnequipPause.Name = "UnequipPause";
			this.UnequipPause.Size = new System.Drawing.Size(196, 17);
			this.UnequipPause.TabIndex = 19;
			this.UnequipPause.Text = "Pause song when unequipping *";
			this.HelpTip.SetToolTip(this.UnequipPause, "Pause the playing song when unequipping the instrument.\r\nUseful for resynchroniza" +
        "tion or switching instrument mid-performance.");
			this.UnequipPause.UseVisualStyleBackColor = true;
			// 
			// ForceOpenToggle
			// 
			this.ForceOpenToggle.AutoSize = true;
			this.ForceOpenToggle.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.ForcedOpen;
			this.ForceOpenToggle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "ForcedOpen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.ForceOpenToggle.Location = new System.Drawing.Point(13, 225);
			this.ForceOpenToggle.Name = "ForceOpenToggle";
			this.ForceOpenToggle.Size = new System.Drawing.Size(222, 17);
			this.ForceOpenToggle.TabIndex = 16;
			this.ForceOpenToggle.Text = "Force playback without performance *";
			this.HelpTip.SetToolTip(this.ForceOpenToggle, "Ignores the current performance status and plays anyways.\r\n* Recommended to only " +
        "be used when patches break playback.\r\n* Ignored when hooked to non-FFXIV applica" +
        "tions.");
			this.ForceOpenToggle.UseVisualStyleBackColor = true;
			this.ForceOpenToggle.CheckedChanged += new System.EventHandler(this.ForceOpenToggle_CheckedChanged);
			// 
			// SignatureFolder
			// 
			this.SignatureFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SignatureFolder.Location = new System.Drawing.Point(210, 203);
			this.SignatureFolder.Name = "SignatureFolder";
			this.SignatureFolder.Size = new System.Drawing.Size(102, 23);
			this.SignatureFolder.TabIndex = 13;
			this.SignatureFolder.Text = "Open sig folder";
			this.SignatureFolder.UseVisualStyleBackColor = true;
			// 
			// sigCheckbox
			// 
			this.sigCheckbox.AutoSize = true;
			this.sigCheckbox.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.SigIgnore;
			this.sigCheckbox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "SigIgnore", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.sigCheckbox.Location = new System.Drawing.Point(13, 207);
			this.sigCheckbox.Name = "sigCheckbox";
			this.sigCheckbox.Size = new System.Drawing.Size(152, 17);
			this.sigCheckbox.TabIndex = 15;
			this.sigCheckbox.Text = "Ignore signature update";
			this.sigCheckbox.UseVisualStyleBackColor = true;
			// 
			// MidiInputLabel
			// 
			this.MidiInputLabel.Location = new System.Drawing.Point(11, 91);
			this.MidiInputLabel.Name = "MidiInputLabel";
			this.MidiInputLabel.Size = new System.Drawing.Size(104, 21);
			this.MidiInputLabel.TabIndex = 14;
			this.MidiInputLabel.Text = "MIDI Input device:";
			this.MidiInputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SettingMidiInput
			// 
			this.SettingMidiInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.SettingMidiInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SettingMidiInput.FormattingEnabled = true;
			this.SettingMidiInput.Items.AddRange(new object[] {
            "None"});
			this.SettingMidiInput.Location = new System.Drawing.Point(121, 92);
			this.SettingMidiInput.Name = "SettingMidiInput";
			this.SettingMidiInput.Size = new System.Drawing.Size(191, 21);
			this.SettingMidiInput.TabIndex = 13;
			// 
			// SettingChatSave
			// 
			this.SettingChatSave.AutoSize = true;
			this.SettingChatSave.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.SaveLog;
			this.SettingChatSave.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "SaveLog", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.SettingChatSave.Location = new System.Drawing.Point(13, 66);
			this.SettingChatSave.Name = "SettingChatSave";
			this.SettingChatSave.Size = new System.Drawing.Size(185, 17);
			this.SettingChatSave.TabIndex = 5;
			this.SettingChatSave.Text = "Save chatlogs to \"logs\" folder *";
			this.HelpTip.SetToolTip(this.SettingChatSave, "* requires program restart.");
			this.SettingChatSave.UseVisualStyleBackColor = true;
			// 
			// SettingBringBmp
			// 
			this.SettingBringBmp.AutoSize = true;
			this.SettingBringBmp.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.OpenBMP;
			this.SettingBringBmp.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SettingBringBmp.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "OpenBMP", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.SettingBringBmp.Location = new System.Drawing.Point(13, 23);
			this.SettingBringBmp.Name = "SettingBringBmp";
			this.SettingBringBmp.Size = new System.Drawing.Size(237, 17);
			this.SettingBringBmp.TabIndex = 4;
			this.SettingBringBmp.Text = "Bring BMP to front on Performance open";
			this.SettingBringBmp.UseVisualStyleBackColor = true;
			// 
			// SettingBringGame
			// 
			this.SettingBringGame.AutoSize = true;
			this.SettingBringGame.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.OpenFFXIV;
			this.SettingBringGame.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SettingBringGame.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "OpenFFXIV", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.SettingBringGame.Location = new System.Drawing.Point(13, 5);
			this.SettingBringGame.Name = "SettingBringGame";
			this.SettingBringGame.Size = new System.Drawing.Size(168, 17);
			this.SettingBringGame.TabIndex = 3;
			this.SettingBringGame.Text = "Bring FFXIV to front on Play";
			this.SettingBringGame.UseVisualStyleBackColor = true;
			// 
			// SettingsTable
			// 
			this.SettingsTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.SettingsTable.ColumnCount = 2;
			this.SettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.SettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.SettingsTable.Controls.Add(this.ChatSettings, 0, 0);
			this.SettingsTable.Controls.Add(this.PlaybackSettings, 1, 0);
			this.SettingsTable.Location = new System.Drawing.Point(11, 119);
			this.SettingsTable.Name = "SettingsTable";
			this.SettingsTable.RowCount = 1;
			this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.SettingsTable.Size = new System.Drawing.Size(301, 82);
			this.SettingsTable.TabIndex = 18;
			// 
			// ChatSettings
			// 
			this.ChatSettings.Controls.Add(this.ListenChatList);
			this.ChatSettings.Controls.Add(this.ForceListenToggle);
			this.ChatSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChatSettings.Location = new System.Drawing.Point(0, 0);
			this.ChatSettings.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.ChatSettings.Name = "ChatSettings";
			this.ChatSettings.Padding = new System.Windows.Forms.Padding(0);
			this.ChatSettings.Size = new System.Drawing.Size(149, 82);
			this.ChatSettings.TabIndex = 11;
			this.ChatSettings.TabStop = false;
			this.ChatSettings.Text = "Chat listen channel";
			// 
			// ListenChatList
			// 
			this.ListenChatList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ListenChatList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ListenChatList.FormattingEnabled = true;
			this.ListenChatList.Location = new System.Drawing.Point(8, 18);
			this.ListenChatList.Name = "ListenChatList";
			this.ListenChatList.Size = new System.Drawing.Size(129, 21);
			this.ListenChatList.TabIndex = 7;
			this.HelpTip.SetToolTip(this.ListenChatList, "Use the selected channel as the main channel. (b.conduct)");
			// 
			// ForceListenToggle
			// 
			this.ForceListenToggle.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.ForceListen;
			this.ForceListenToggle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "ForceListen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.ForceListenToggle.Location = new System.Drawing.Point(8, 41);
			this.ForceListenToggle.Margin = new System.Windows.Forms.Padding(0);
			this.ForceListenToggle.Name = "ForceListenToggle";
			this.ForceListenToggle.Size = new System.Drawing.Size(88, 16);
			this.ForceListenToggle.TabIndex = 6;
			this.ForceListenToggle.Text = "Force listen";
			this.HelpTip.SetToolTip(this.ForceListenToggle, "Forces commands to work from everyone in the selected chat channel.");
			this.ForceListenToggle.UseVisualStyleBackColor = true;
			// 
			// PlaybackSettings
			// 
			this.PlaybackSettings.Controls.Add(this.TooFastChange);
			this.PlaybackSettings.Controls.Add(this.PlayHoldChange);
			this.PlaybackSettings.Controls.Add(this.SlowPlayToggle);
			this.PlaybackSettings.Controls.Add(this.ArpeggiateToggle);
			this.PlaybackSettings.Controls.Add(this.SettingHoldNotes);
			this.PlaybackSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PlaybackSettings.Location = new System.Drawing.Point(151, 0);
			this.PlaybackSettings.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.PlaybackSettings.Name = "PlaybackSettings";
			this.PlaybackSettings.Padding = new System.Windows.Forms.Padding(0);
			this.PlaybackSettings.Size = new System.Drawing.Size(150, 82);
			this.PlaybackSettings.TabIndex = 12;
			this.PlaybackSettings.TabStop = false;
			this.PlaybackSettings.Text = "Playback";
			// 
			// TooFastChange
			// 
			this.TooFastChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TooFastChange.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FFBardMusicPlayer.Properties.Settings.Default, "TooFastDelay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.TooFastChange.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::FFBardMusicPlayer.Properties.Settings.Default, "SlowPlay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.TooFastChange.Font = new System.Drawing.Font("Segoe UI", 7F);
			this.TooFastChange.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.TooFastChange.Location = new System.Drawing.Point(108, 0);
			this.TooFastChange.Margin = new System.Windows.Forms.Padding(0);
			this.TooFastChange.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.TooFastChange.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.TooFastChange.Name = "TooFastChange";
			this.TooFastChange.Size = new System.Drawing.Size(41, 20);
			this.TooFastChange.TabIndex = 5;
			this.TooFastChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TooFastChange.Value = global::FFBardMusicPlayer.Properties.Settings.Default.TooFastDelay;
			this.TooFastChange.Visible = global::FFBardMusicPlayer.Properties.Settings.Default.SlowPlay;
			// 
			// PlayHoldChange
			// 
			this.PlayHoldChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.PlayHoldChange.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::FFBardMusicPlayer.Properties.Settings.Default, "PlayHold", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.PlayHoldChange.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::FFBardMusicPlayer.Properties.Settings.Default, "SlowPlay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.PlayHoldChange.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.PlayHoldChange.Location = new System.Drawing.Point(106, 51);
			this.PlayHoldChange.Margin = new System.Windows.Forms.Padding(0);
			this.PlayHoldChange.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.PlayHoldChange.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.PlayHoldChange.Name = "PlayHoldChange";
			this.PlayHoldChange.Size = new System.Drawing.Size(41, 22);
			this.PlayHoldChange.TabIndex = 4;
			this.PlayHoldChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.HelpTip.SetToolTip(this.PlayHoldChange, "For how long should the notes be held?");
			this.PlayHoldChange.Value = global::FFBardMusicPlayer.Properties.Settings.Default.PlayHold;
			this.PlayHoldChange.Visible = global::FFBardMusicPlayer.Properties.Settings.Default.SlowPlay;
			// 
			// SlowPlayToggle
			// 
			this.SlowPlayToggle.AutoSize = true;
			this.SlowPlayToggle.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.SlowPlay;
			this.SlowPlayToggle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "SlowPlay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.SlowPlayToggle.Location = new System.Drawing.Point(6, 53);
			this.SlowPlayToggle.Name = "SlowPlayToggle";
			this.SlowPlayToggle.Size = new System.Drawing.Size(93, 17);
			this.SlowPlayToggle.TabIndex = 3;
			this.SlowPlayToggle.Text = "Play all notes";
			this.HelpTip.SetToolTip(this.SlowPlayToggle, "Plays all notes in the song in exchange for speed accuracy.\r\nDo not use in orches" +
        "tra mode!");
			this.SlowPlayToggle.UseVisualStyleBackColor = true;
			this.SlowPlayToggle.CheckedChanged += new System.EventHandler(this.SlowPlayToggle_CheckedChanged);
			// 
			// ArpeggiateToggle
			// 
			this.ArpeggiateToggle.AutoSize = true;
			this.ArpeggiateToggle.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.AutoArpeggiate;
			this.ArpeggiateToggle.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ArpeggiateToggle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "AutoArpeggiate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.ArpeggiateToggle.Location = new System.Drawing.Point(6, 15);
			this.ArpeggiateToggle.Name = "ArpeggiateToggle";
			this.ArpeggiateToggle.Size = new System.Drawing.Size(108, 17);
			this.ArpeggiateToggle.TabIndex = 2;
			this.ArpeggiateToggle.Text = "Simulate chords";
			this.HelpTip.SetToolTip(this.ArpeggiateToggle, "Detect and simulate chords.");
			this.ArpeggiateToggle.UseVisualStyleBackColor = true;
			// 
			// SettingHoldNotes
			// 
			this.SettingHoldNotes.AutoSize = true;
			this.SettingHoldNotes.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.HoldNotes;
			this.SettingHoldNotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.SettingHoldNotes.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "HoldNotes", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.SettingHoldNotes.Location = new System.Drawing.Point(6, 34);
			this.SettingHoldNotes.Name = "SettingHoldNotes";
			this.SettingHoldNotes.Size = new System.Drawing.Size(83, 17);
			this.SettingHoldNotes.TabIndex = 1;
			this.SettingHoldNotes.Text = "Hold notes";
			this.HelpTip.SetToolTip(this.SettingHoldNotes, "Enables held notes.");
			this.SettingHoldNotes.UseVisualStyleBackColor = true;
			// 
			// ChatSimToggle
			// 
			this.ChatSimToggle.AutoSize = true;
			this.ChatSimToggle.Checked = global::FFBardMusicPlayer.Properties.Settings.Default.PlayLyrics;
			this.ChatSimToggle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::FFBardMusicPlayer.Properties.Settings.Default, "PlayLyrics", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.ChatSimToggle.Location = new System.Drawing.Point(13, 48);
			this.ChatSimToggle.Name = "ChatSimToggle";
			this.ChatSimToggle.Size = new System.Drawing.Size(169, 17);
			this.ChatSimToggle.TabIndex = 6;
			this.ChatSimToggle.Text = "Allow in-game chat typing *";
			this.HelpTip.SetToolTip(this.ChatSimToggle, "Allows program to type in the in-game chat.\r\nAffects MIDI lyrics and <b.cmd> comm" +
        "and.");
			this.ChatSimToggle.UseVisualStyleBackColor = true;
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
			// BmpSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.GeneralSettings);
			this.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.Name = "BmpSettings";
			this.Size = new System.Drawing.Size(329, 318);
			this.GeneralSettings.ResumeLayout(false);
			this.SettingsScrollPanel.ResumeLayout(false);
			this.SettingsScrollPanel.PerformLayout();
			this.SettingsTable.ResumeLayout(false);
			this.ChatSettings.ResumeLayout(false);
			this.PlaybackSettings.ResumeLayout(false);
			this.PlaybackSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.TooFastChange)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PlayHoldChange)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.GroupBox GeneralSettings;
		private System.Windows.Forms.CheckBox SettingHoldNotes;
		private System.Windows.Forms.CheckBox SettingBringGame;
		private System.Windows.Forms.CheckBox SettingBringBmp;
		private System.Windows.Forms.CheckBox SettingChatSave;
		private System.Windows.Forms.CheckBox ForceListenToggle;
		private System.Windows.Forms.ComboBox ListenChatList;
		private System.Windows.Forms.Button KeyboardTest;
		private System.Windows.Forms.CheckBox ChatSimToggle;
		private System.Windows.Forms.ToolTip HelpTip;
		private System.Windows.Forms.CheckBox ArpeggiateToggle;
		private System.Windows.Forms.GroupBox ChatSettings;
		private System.Windows.Forms.Panel SettingsScrollPanel;
		private System.Windows.Forms.GroupBox PlaybackSettings;
		private System.Windows.Forms.Label MidiInputLabel;
		private System.Windows.Forms.ComboBox SettingMidiInput;
		private System.Windows.Forms.CheckBox SlowPlayToggle;
		private System.Windows.Forms.NumericUpDown PlayHoldChange;
		private System.Windows.Forms.Button SignatureFolder;
		private System.Windows.Forms.CheckBox sigCheckbox;
		private System.Windows.Forms.NumericUpDown TooFastChange;
		private System.Windows.Forms.CheckBox ForceOpenToggle;
		private System.Windows.Forms.TableLayoutPanel SettingsTable;
		private System.Windows.Forms.CheckBox UnequipPause;
		private System.Windows.Forms.CheckBox verboseToggle;
	}
}
