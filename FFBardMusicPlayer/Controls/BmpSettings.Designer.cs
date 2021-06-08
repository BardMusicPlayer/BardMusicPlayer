namespace FFBardMusicPlayer.Controls
{
    partial class BmpSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components          = new System.ComponentModel.Container();
            this.GeneralSettings     = new System.Windows.Forms.GroupBox();
            this.KeyboardTest        = new System.Windows.Forms.Button();
            this.SignatureFolder     = new System.Windows.Forms.Button();
            this.SettingsScrollPanel = new System.Windows.Forms.Panel();
            this.verboseToggle       = new System.Windows.Forms.CheckBox();
            this.sigCheckbox         = new System.Windows.Forms.CheckBox();
            this.SettingChatSave     = new System.Windows.Forms.CheckBox();
            this.SettingsTable       = new System.Windows.Forms.TableLayoutPanel();
            this.guitarkeygroup      = new System.Windows.Forms.GroupBox();
            this.g_Overdriven        = new System.Windows.Forms.Button();
            this.g_Special           = new System.Windows.Forms.Button();
            this.g_Clean             = new System.Windows.Forms.Button();
            this.g_PowerChords       = new System.Windows.Forms.Button();
            this.g_Muted             = new System.Windows.Forms.Button();
            this.ChatSettings        = new System.Windows.Forms.GroupBox();
            this.SettingBringGame    = new System.Windows.Forms.CheckBox();
            this.UnequipPause        = new System.Windows.Forms.CheckBox();
            this.SettingBringBmp     = new System.Windows.Forms.CheckBox();
            this.PlaybackSettings    = new System.Windows.Forms.GroupBox();
            this.SettingHoldNotes    = new System.Windows.Forms.CheckBox();
            this.ForceOpenToggle     = new System.Windows.Forms.CheckBox();
            this.MidiInputLabel      = new System.Windows.Forms.Label();
            this.SettingMidiInput    = new System.Windows.Forms.ComboBox();
            this.HelpTip             = new System.Windows.Forms.ToolTip(this.components);
            this.GeneralSettings.SuspendLayout();
            this.SettingsScrollPanel.SuspendLayout();
            this.SettingsTable.SuspendLayout();
            this.guitarkeygroup.SuspendLayout();
            this.ChatSettings.SuspendLayout();
            this.PlaybackSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // GeneralSettings
            // 
            this.GeneralSettings.AutoSize  = true;
            this.GeneralSettings.BackColor = System.Drawing.Color.Transparent;
            this.GeneralSettings.Controls.Add(this.KeyboardTest);
            this.GeneralSettings.Controls.Add(this.SignatureFolder);
            this.GeneralSettings.Controls.Add(this.SettingsScrollPanel);
            this.GeneralSettings.Dock     = System.Windows.Forms.DockStyle.Fill;
            this.GeneralSettings.Location = new System.Drawing.Point(0, 0);
            this.GeneralSettings.Margin   = new System.Windows.Forms.Padding(0);
            this.GeneralSettings.Name     = "GeneralSettings";
            this.GeneralSettings.Size     = new System.Drawing.Size(546, 325);
            this.GeneralSettings.TabIndex = 9;
            this.GeneralSettings.TabStop  = false;
            this.GeneralSettings.Text     = "Settings";
            // 
            // KeyboardTest
            // 
            this.KeyboardTest.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.KeyboardTest.Location                = new System.Drawing.Point(313, 3);
            this.KeyboardTest.Name                    = "KeyboardTest";
            this.KeyboardTest.Size                    = new System.Drawing.Size(97, 23);
            this.KeyboardTest.TabIndex                = 0;
            this.KeyboardTest.Text                    = "Test keyboard";
            this.KeyboardTest.UseVisualStyleBackColor = true;
            // 
            // SignatureFolder
            // 
            this.SignatureFolder.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.SignatureFolder.Location                = new System.Drawing.Point(416, 3);
            this.SignatureFolder.Name                    = "SignatureFolder";
            this.SignatureFolder.Size                    = new System.Drawing.Size(116, 23);
            this.SignatureFolder.TabIndex                = 13;
            this.SignatureFolder.Text                    = "Open Data folder";
            this.SignatureFolder.UseVisualStyleBackColor = true;
            // 
            // SettingsScrollPanel
            // 
            this.SettingsScrollPanel.AutoScroll = true;
            this.SettingsScrollPanel.Controls.Add(this.verboseToggle);
            this.SettingsScrollPanel.Controls.Add(this.sigCheckbox);
            this.SettingsScrollPanel.Controls.Add(this.SettingChatSave);
            this.SettingsScrollPanel.Controls.Add(this.SettingsTable);
            this.SettingsScrollPanel.Dock     = System.Windows.Forms.DockStyle.Fill;
            this.SettingsScrollPanel.Location = new System.Drawing.Point(3, 18);
            this.SettingsScrollPanel.Margin   = new System.Windows.Forms.Padding(0);
            this.SettingsScrollPanel.Name     = "SettingsScrollPanel";
            this.SettingsScrollPanel.Padding  = new System.Windows.Forms.Padding(8);
            this.SettingsScrollPanel.Size     = new System.Drawing.Size(540, 304);
            this.SettingsScrollPanel.TabIndex = 12;
            // 
            // verboseToggle
            // 
            this.verboseToggle.AutoSize = true;
            this.verboseToggle.Location = new System.Drawing.Point(26, 169);
            this.verboseToggle.Name     = "verboseToggle";
            this.verboseToggle.Size     = new System.Drawing.Size(136, 17);
            this.verboseToggle.TabIndex = 20;
            this.verboseToggle.Text     = "Enable verbose mode";
            this.HelpTip.SetToolTip(this.verboseToggle, "Print various kinds of information to the log window.");
            this.verboseToggle.UseVisualStyleBackColor =  true;
            this.verboseToggle.CheckedChanged          += new System.EventHandler(this.verboseToggle_CheckedChanged);
            // 
            // sigCheckbox
            // 
            this.sigCheckbox.AutoSize                =  true;
            this.sigCheckbox.Location                =  new System.Drawing.Point(26, 123);
            this.sigCheckbox.Name                    =  "sigCheckbox";
            this.sigCheckbox.Size                    =  new System.Drawing.Size(152, 17);
            this.sigCheckbox.TabIndex                =  15;
            this.sigCheckbox.Text                    =  "Ignore signature update";
            this.sigCheckbox.UseVisualStyleBackColor =  true;
            this.sigCheckbox.CheckedChanged          += new System.EventHandler(this.sigCheckbox_CheckedChanged);
            // 
            // SettingChatSave
            // 
            this.SettingChatSave.AutoSize = true;
            this.SettingChatSave.Location = new System.Drawing.Point(26, 146);
            this.SettingChatSave.Name     = "SettingChatSave";
            this.SettingChatSave.Size     = new System.Drawing.Size(177, 17);
            this.SettingChatSave.TabIndex = 5;
            this.SettingChatSave.Text     = "Save chatlogs to \"logs\" folder";
            this.HelpTip.SetToolTip(this.SettingChatSave, "Toggling this on requires a program restart.");
            this.SettingChatSave.UseVisualStyleBackColor = true;
            this.SettingChatSave.CheckedChanged += new System.EventHandler(this.SettingChatSave_CheckedChanged);
            // 
            // SettingsTable
            // 
            this.SettingsTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingsTable.ColumnCount = 2;
            this.SettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SettingsTable.Controls.Add(this.guitarkeygroup, 1, 1);
            this.SettingsTable.Controls.Add(this.ChatSettings, 0, 0);
            this.SettingsTable.Controls.Add(this.PlaybackSettings, 1, 0);
            this.SettingsTable.Location = new System.Drawing.Point(11, 11);
            this.SettingsTable.Name = "SettingsTable";
            this.SettingsTable.RowCount = 2;
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 153F));
            this.SettingsTable.Size = new System.Drawing.Size(518, 253);
            this.SettingsTable.TabIndex = 18;
            // 
            // guitarkeygroup
            // 
            this.guitarkeygroup.Controls.Add(this.g_Overdriven);
            this.guitarkeygroup.Controls.Add(this.g_Special);
            this.guitarkeygroup.Controls.Add(this.g_Clean);
            this.guitarkeygroup.Controls.Add(this.g_PowerChords);
            this.guitarkeygroup.Controls.Add(this.g_Muted);
            this.guitarkeygroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guitarkeygroup.Location = new System.Drawing.Point(260, 100);
            this.guitarkeygroup.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.guitarkeygroup.Name = "guitarkeygroup";
            this.guitarkeygroup.Padding = new System.Windows.Forms.Padding(0);
            this.guitarkeygroup.Size = new System.Drawing.Size(258, 153);
            this.guitarkeygroup.TabIndex = 26;
            this.guitarkeygroup.TabStop = false;
            this.guitarkeygroup.Text = "Guitar Keybinds";
            // 
            // g_Overdriven
            // 
            this.g_Overdriven.Location = new System.Drawing.Point(31, 18);
            this.g_Overdriven.Name = "g_Overdriven";
            this.g_Overdriven.Size = new System.Drawing.Size(75, 23);
            this.g_Overdriven.TabIndex = 21;
            this.g_Overdriven.Text = "Overdriven";
            this.g_Overdriven.UseVisualStyleBackColor = true;
            this.g_Overdriven.Click += new System.EventHandler(this.g_Overdriven_Click);
            // 
            // g_Special
            // 
            this.g_Special.Location = new System.Drawing.Point(31, 109);
            this.g_Special.Name = "g_Special";
            this.g_Special.Size = new System.Drawing.Size(75, 23);
            this.g_Special.TabIndex = 25;
            this.g_Special.Text = "Special";
            this.g_Special.UseVisualStyleBackColor = true;
            this.g_Special.Click += new System.EventHandler(this.g_Special_Click);
            // 
            // g_Clean
            // 
            this.g_Clean.Location = new System.Drawing.Point(31, 41);
            this.g_Clean.Name = "g_Clean";
            this.g_Clean.Size = new System.Drawing.Size(75, 23);
            this.g_Clean.TabIndex = 22;
            this.g_Clean.Text = "Clean";
            this.g_Clean.UseVisualStyleBackColor = true;
            this.g_Clean.Click += new System.EventHandler(this.g_Clean_Click);
            // 
            // g_PowerChords
            // 
            this.g_PowerChords.Location = new System.Drawing.Point(31, 84);
            this.g_PowerChords.Name = "g_PowerChords";
            this.g_PowerChords.Size = new System.Drawing.Size(75, 23);
            this.g_PowerChords.TabIndex = 24;
            this.g_PowerChords.Text = "PowerChords";
            this.g_PowerChords.UseVisualStyleBackColor = true;
            this.g_PowerChords.Click += new System.EventHandler(this.g_PowerChords_Click);
            // 
            // g_Muted
            // 
            this.g_Muted.Location = new System.Drawing.Point(31, 64);
            this.g_Muted.Name = "g_Muted";
            this.g_Muted.Size = new System.Drawing.Size(75, 23);
            this.g_Muted.TabIndex = 23;
            this.g_Muted.Text = "Muted";
            this.g_Muted.UseVisualStyleBackColor = true;
            this.g_Muted.Click += new System.EventHandler(this.g_Muted_Click);
            // 
            // ChatSettings
            // 
            this.ChatSettings.Controls.Add(this.SettingBringGame);
            this.ChatSettings.Controls.Add(this.UnequipPause);
            this.ChatSettings.Controls.Add(this.SettingBringBmp);
            this.ChatSettings.Dock     = System.Windows.Forms.DockStyle.Fill;
            this.ChatSettings.Location = new System.Drawing.Point(0, 0);
            this.ChatSettings.Margin   = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.ChatSettings.Name     = "ChatSettings";
            this.ChatSettings.Padding  = new System.Windows.Forms.Padding(0);
            this.ChatSettings.Size     = new System.Drawing.Size(258, 96);
            this.ChatSettings.TabIndex = 11;
            this.ChatSettings.TabStop  = false;
            this.ChatSettings.Text     = "Game";
            // 
            // SettingBringGame
            // 
            this.SettingBringGame.AutoSize   = true;
            this.SettingBringGame.Checked    = true;
            this.SettingBringGame.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SettingBringGame.Location   = new System.Drawing.Point(15, 18);
            this.SettingBringGame.Name       = "SettingBringGame";
            this.SettingBringGame.Size       = new System.Drawing.Size(127, 17);
            this.SettingBringGame.TabIndex   = 3;
            this.SettingBringGame.Text       = "Bring FFXIV to front";
            this.HelpTip.SetToolTip(this.SettingBringGame, "When playing the song, bring FFXIV to front.");
            this.SettingBringGame.UseVisualStyleBackColor = true;
            this.SettingBringGame.CheckedChanged += new System.EventHandler(this.SettingBringGame_CheckedChanged);
            // 
            // UnequipPause
            // 
            this.UnequipPause.AutoSize   = true;
            this.UnequipPause.Checked    = true;
            this.UnequipPause.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UnequipPause.Location   = new System.Drawing.Point(15, 63);
            this.UnequipPause.Name       = "UnequipPause";
            this.UnequipPause.Size       = new System.Drawing.Size(149, 17);
            this.UnequipPause.TabIndex   = 19;
            this.UnequipPause.Text       = "Pause song on unequip";
            this.HelpTip.SetToolTip(this.UnequipPause,
                "Pause the playing song when unequipping the instrument.\r\nUseful for switching ins" +
                "trument mid-performance.");
            this.UnequipPause.UseVisualStyleBackColor =  true;
            this.UnequipPause.CheckedChanged          += new System.EventHandler(this.UnequipPause_CheckedChanged);
            // 
            // SettingBringBmp
            // 
            this.SettingBringBmp.AutoSize   = true;
            this.SettingBringBmp.Checked    = true;
            this.SettingBringBmp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SettingBringBmp.Location   = new System.Drawing.Point(15, 40);
            this.SettingBringBmp.Name       = "SettingBringBmp";
            this.SettingBringBmp.Size       = new System.Drawing.Size(121, 17);
            this.SettingBringBmp.TabIndex   = 4;
            this.SettingBringBmp.Text       = "Bring BMP to front";
            this.HelpTip.SetToolTip(this.SettingBringBmp, "When Performance is opened in FFXIV, bring BMP to front.");
            this.SettingBringBmp.UseVisualStyleBackColor = true;
            this.SettingBringBmp.CheckedChanged += new System.EventHandler(this.SettingBringBmp_CheckedChanged);
            // 
            // PlaybackSettings
            // 
            this.PlaybackSettings.Controls.Add(this.SettingHoldNotes);
            this.PlaybackSettings.Controls.Add(this.ForceOpenToggle);
            this.PlaybackSettings.Controls.Add(this.MidiInputLabel);
            this.PlaybackSettings.Controls.Add(this.SettingMidiInput);
            this.PlaybackSettings.Dock     = System.Windows.Forms.DockStyle.Fill;
            this.PlaybackSettings.Location = new System.Drawing.Point(260, 0);
            this.PlaybackSettings.Margin   = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.PlaybackSettings.Name     = "PlaybackSettings";
            this.PlaybackSettings.Padding  = new System.Windows.Forms.Padding(0);
            this.PlaybackSettings.Size     = new System.Drawing.Size(258, 96);
            this.PlaybackSettings.TabIndex = 12;
            this.PlaybackSettings.TabStop  = false;
            this.PlaybackSettings.Text     = "Playback";
            // 
            // SettingHoldNotes
            // 
            this.SettingHoldNotes.AutoSize = true;
            this.SettingHoldNotes.Location = new System.Drawing.Point(13, 20);
            this.SettingHoldNotes.Name     = "SettingHoldNotes";
            this.SettingHoldNotes.Size     = new System.Drawing.Size(83, 17);
            this.SettingHoldNotes.TabIndex = 1;
            this.SettingHoldNotes.Text     = "Hold notes";
            this.HelpTip.SetToolTip(this.SettingHoldNotes, "Enables held notes.");
            this.SettingHoldNotes.UseVisualStyleBackColor = true;
            this.SettingHoldNotes.CheckedChanged += new System.EventHandler(this.SettingHoldNotes_CheckedChanged);
            // 
            // ForceOpenToggle
            // 
            this.ForceOpenToggle.AutoSize = true;
            this.ForceOpenToggle.Location = new System.Drawing.Point(13, 43);
            this.ForceOpenToggle.Name     = "ForceOpenToggle";
            this.ForceOpenToggle.Size     = new System.Drawing.Size(102, 17);
            this.ForceOpenToggle.TabIndex = 16;
            this.ForceOpenToggle.Text     = "Force playback";
            this.HelpTip.SetToolTip(this.ForceOpenToggle,
                "Ignores the current performance status and plays anyways.\r\n* Recommended to only " +
                "be used when patches break playback.\r\n* Ignored when hooked to non-FFXIV applica" +
                "tions.");
            this.ForceOpenToggle.UseVisualStyleBackColor = true;
            this.ForceOpenToggle.CheckedChanged += new System.EventHandler(this.ForceOpenToggle_CheckedChanged);
            // 
            // MidiInputLabel
            // 
            this.MidiInputLabel.Location  = new System.Drawing.Point(11, 65);
            this.MidiInputLabel.Name      = "MidiInputLabel";
            this.MidiInputLabel.Size      = new System.Drawing.Size(104, 21);
            this.MidiInputLabel.TabIndex  = 14;
            this.MidiInputLabel.Text      = "MIDI Input device:";
            this.MidiInputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SettingMidiInput
            // 
            this.SettingMidiInput.Anchor =
                ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
                                                        System.Windows.Forms.AnchorStyles.Left)
                                                       | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingMidiInput.DropDownStyle     = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SettingMidiInput.FormattingEnabled = true;
            this.SettingMidiInput.Items.AddRange(new object[]
            {
                "None"
            });
            this.SettingMidiInput.Location = new System.Drawing.Point(121, 66);
            this.SettingMidiInput.Name     = "SettingMidiInput";
            this.SettingMidiInput.Size     = new System.Drawing.Size(124, 21);
            this.SettingMidiInput.TabIndex = 13;
            // 
            // HelpTip
            // 
            this.HelpTip.AutoPopDelay = 5000;
            this.HelpTip.BackColor    = System.Drawing.Color.White;
            this.HelpTip.ForeColor    = System.Drawing.Color.Black;
            this.HelpTip.InitialDelay = 100;
            this.HelpTip.ReshowDelay  = 100;
            this.HelpTip.UseAnimation = false;
            this.HelpTip.UseFading    = false;
            // 
            // BmpSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GeneralSettings);
            this.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.Name = "BmpSettings";
            this.Size = new System.Drawing.Size(546, 325);
            this.GeneralSettings.ResumeLayout(false);
            this.SettingsScrollPanel.ResumeLayout(false);
            this.SettingsScrollPanel.PerformLayout();
            this.SettingsTable.ResumeLayout(false);
            this.guitarkeygroup.ResumeLayout(false);
            this.ChatSettings.ResumeLayout(false);
            this.ChatSettings.PerformLayout();
            this.PlaybackSettings.ResumeLayout(false);
            this.PlaybackSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox GeneralSettings;
        private System.Windows.Forms.CheckBox SettingHoldNotes;
        private System.Windows.Forms.CheckBox SettingBringGame;
        private System.Windows.Forms.CheckBox SettingBringBmp;
        private System.Windows.Forms.CheckBox SettingChatSave;
        private System.Windows.Forms.Button KeyboardTest;
        private System.Windows.Forms.ToolTip HelpTip;
        private System.Windows.Forms.GroupBox ChatSettings;
        private System.Windows.Forms.Panel SettingsScrollPanel;
        private System.Windows.Forms.GroupBox PlaybackSettings;
        private System.Windows.Forms.Label MidiInputLabel;
        private System.Windows.Forms.ComboBox SettingMidiInput;
        private System.Windows.Forms.Button SignatureFolder;
        private System.Windows.Forms.CheckBox sigCheckbox;
        private System.Windows.Forms.CheckBox ForceOpenToggle;
        private System.Windows.Forms.TableLayoutPanel SettingsTable;
        private System.Windows.Forms.CheckBox UnequipPause;
        private System.Windows.Forms.CheckBox verboseToggle;
        private System.Windows.Forms.NumericUpDown DelaySongsChange;
        private System.Windows.Forms.CheckBox WaitBetweenSongsToggle;
        private System.Windows.Forms.Button g_Overdriven;
        private System.Windows.Forms.Button g_Clean;
        private System.Windows.Forms.Button g_Muted;
        private System.Windows.Forms.Button g_PowerChords;
        private System.Windows.Forms.Button g_Special;
        private System.Windows.Forms.GroupBox guitarkeygroup;
    }
}