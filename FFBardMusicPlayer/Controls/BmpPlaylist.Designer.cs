﻿namespace FFBardMusicPlayer.Controls
{
    partial class BmpPlaylist
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 =
                new System.Windows.Forms.DataGridViewCellStyle();
            this.Playlist                          = new System.Windows.Forms.GroupBox();
            this.PlaylistView                      = new System.Windows.Forms.DataGridView();
            this.filePathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5        = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MidiEntrySource                   = new System.Windows.Forms.BindingSource(this.components);
            this.Playlist_ClearAll                 = new System.Windows.Forms.Button();
            this.Playlist_Remove                   = new System.Windows.Forms.Button();
            this.Playlist_Add                      = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1        = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ButtonPanel                       = new System.Windows.Forms.Panel();
            this.Playlist_Delay                    = new System.Windows.Forms.NumericUpDown();
            this.AutoPlayToggle                    = new System.Windows.Forms.CheckBox();
            this.Playlist_Random                   = new FFBardMusicPlayer.Components.BmpCheckButton(this.components);
            this.ButtonBindSource                  = new System.Windows.Forms.BindingSource(this.components);
            this.Playlist_Loop                     = new FFBardMusicPlayer.Components.BmpCheckButton(this.components);
            this.dataGridViewTextBoxColumn3        = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4        = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2        = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HelpTip                           = new System.Windows.Forms.ToolTip(this.components);
            this.Playlist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.PlaylistView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.MidiEntrySource)).BeginInit();
            this.ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.Playlist_Delay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.ButtonBindSource)).BeginInit();
            this.SuspendLayout();
            // 
            // Playlist
            // 
            this.Playlist.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
                                                         System.Windows.Forms.AnchorStyles.Bottom)
                                                        | System.Windows.Forms.AnchorStyles.Left)
                                                       | System.Windows.Forms.AnchorStyles.Right)));
            this.Playlist.Controls.Add(this.PlaylistView);
            this.Playlist.Controls.Add(this.Playlist_ClearAll);
            this.Playlist.Controls.Add(this.Playlist_Remove);
            this.Playlist.Controls.Add(this.Playlist_Add);
            this.Playlist.Font     = new System.Drawing.Font("Segoe UI", 12F);
            this.Playlist.Location = new System.Drawing.Point(0, 0);
            this.Playlist.Name     = "Playlist";
            this.Playlist.Padding  = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Playlist.Size     = new System.Drawing.Size(205, 207);
            this.Playlist.TabIndex = 0;
            this.Playlist.TabStop  = false;
            this.Playlist.Text     = "PLAYLIST";
            // 
            // PlaylistView
            // 
            this.PlaylistView.AllowDrop                = true;
            this.PlaylistView.AllowUserToAddRows       = false;
            this.PlaylistView.AllowUserToDeleteRows    = false;
            this.PlaylistView.AllowUserToResizeColumns = false;
            this.PlaylistView.AllowUserToResizeRows    = false;
            this.PlaylistView.AutoGenerateColumns      = false;
            this.PlaylistView.AutoSizeColumnsMode      = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.PlaylistView.BorderStyle              = System.Windows.Forms.BorderStyle.None;
            this.PlaylistView.CellBorderStyle          = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.PlaylistView.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.PlaylistView.ColumnHeadersVisible = false;
            this.PlaylistView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
            {
                this.filePathDataGridViewTextBoxColumn,
                this.dataGridViewTextBoxColumn5
            });
            this.PlaylistView.DataSource        = this.MidiEntrySource;
            this.PlaylistView.Dock              = System.Windows.Forms.DockStyle.Fill;
            this.PlaylistView.EditMode          = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.PlaylistView.GridColor         = System.Drawing.SystemColors.AppWorkspace;
            this.PlaylistView.Location          = new System.Drawing.Point(3, 25);
            this.PlaylistView.Margin            = new System.Windows.Forms.Padding(0);
            this.PlaylistView.MultiSelect       = false;
            this.PlaylistView.Name              = "PlaylistView";
            this.PlaylistView.ReadOnly          = true;
            this.PlaylistView.RowHeadersVisible = false;
            this.PlaylistView.RowHeadersWidth   = 40;
            this.PlaylistView.RowHeadersWidthSizeMode =
                System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.BackColor          = System.Drawing.SystemColors.AppWorkspace;
            dataGridViewCellStyle2.ForeColor          = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode           = System.Windows.Forms.DataGridViewTriState.False;
            this.PlaylistView.RowsDefaultCellStyle    = dataGridViewCellStyle2;
            this.PlaylistView.ScrollBars              = System.Windows.Forms.ScrollBars.Vertical;
            this.PlaylistView.SelectionMode           = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.PlaylistView.Size                    = new System.Drawing.Size(199, 182);
            this.PlaylistView.TabIndex                = 0;
            this.PlaylistView.CellMouseDoubleClick +=
                new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.PlaylistView_CellMouseDoubleClick);
            this.PlaylistView.CellMouseDown +=
                new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.BmpMidiEntryList_CellMouseDown);
            this.PlaylistView.CellMouseMove +=
                new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.BmpMidiEntryList_CellMouseMove);
            this.PlaylistView.CellMouseUp +=
                new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.BmpMidiEntryList_CellMouseUp);
            this.PlaylistView.CellPainting +=
                new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.PlaylistControl_CellPainting);
            this.PlaylistView.DragDrop += new System.Windows.Forms.DragEventHandler(this.BmpMidiEntryList_DragDrop);
            this.PlaylistView.DragOver += new System.Windows.Forms.DragEventHandler(this.BmpMidiEntryList_DragOver);
            // 
            // filePathDataGridViewTextBoxColumn
            // 
            this.filePathDataGridViewTextBoxColumn.AutoSizeMode =
                System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.filePathDataGridViewTextBoxColumn.DataPropertyName = "FilePath";
            this.filePathDataGridViewTextBoxColumn.HeaderText       = "FilePath";
            this.filePathDataGridViewTextBoxColumn.Name             = "filePathDataGridViewTextBoxColumn";
            this.filePathDataGridViewTextBoxColumn.ReadOnly         = true;
            this.filePathDataGridViewTextBoxColumn.Resizable        = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "Track";
            this.dataGridViewTextBoxColumn5.HeaderText       = "Track";
            this.dataGridViewTextBoxColumn5.Name             = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly         = true;
            this.dataGridViewTextBoxColumn5.Resizable        = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn5.Width            = 5;
            // 
            // MidiEntrySource
            // 
            this.MidiEntrySource.DataSource = typeof(FFBardMusicCommon.BmpMidiEntry);
            // 
            // Playlist_ClearAll
            // 
            this.Playlist_ClearAll.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.Playlist_ClearAll.BackgroundImageLayout     = System.Windows.Forms.ImageLayout.None;
            this.Playlist_ClearAll.FlatAppearance.BorderSize = 0;
            this.Playlist_ClearAll.FlatStyle                 = System.Windows.Forms.FlatStyle.System;
            this.Playlist_ClearAll.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point, ((byte) (0)), true);
            this.Playlist_ClearAll.Location = new System.Drawing.Point(138, 3);
            this.Playlist_ClearAll.Name     = "Playlist_ClearAll";
            this.Playlist_ClearAll.Size     = new System.Drawing.Size(22, 22);
            this.Playlist_ClearAll.TabIndex = 9;
            this.Playlist_ClearAll.Text     = "×";
            this.HelpTip.SetToolTip(this.Playlist_ClearAll, "Clear the playlist.");
            this.Playlist_ClearAll.UseVisualStyleBackColor =  true;
            this.Playlist_ClearAll.Click                   += new System.EventHandler(this.Playlist_ClearAll_Click);
            // 
            // Playlist_Remove
            // 
            this.Playlist_Remove.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.Playlist_Remove.FlatAppearance.BorderSize = 0;
            this.Playlist_Remove.FlatStyle                 = System.Windows.Forms.FlatStyle.System;
            this.Playlist_Remove.Location                  = new System.Drawing.Point(159, 3);
            this.Playlist_Remove.Name                      = "Playlist_Remove";
            this.Playlist_Remove.Size                      = new System.Drawing.Size(22, 22);
            this.Playlist_Remove.TabIndex                  = 8;
            this.Playlist_Remove.Text                      = "-";
            this.HelpTip.SetToolTip(this.Playlist_Remove, "Remove entry from playlist.");
            this.Playlist_Remove.UseVisualStyleBackColor =  true;
            this.Playlist_Remove.Click                   += new System.EventHandler(this.Playlist_Remove_Click);
            // 
            // Playlist_Add
            // 
            this.Playlist_Add.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.Playlist_Add.FlatAppearance.BorderSize = 0;
            this.Playlist_Add.FlatStyle                 = System.Windows.Forms.FlatStyle.System;
            this.Playlist_Add.Location                  = new System.Drawing.Point(180, 3);
            this.Playlist_Add.Name                      = "Playlist_Add";
            this.Playlist_Add.Size                      = new System.Drawing.Size(22, 22);
            this.Playlist_Add.TabIndex                  = 3;
            this.Playlist_Add.Text                      = "+";
            this.HelpTip.SetToolTip(this.Playlist_Add, "Add Midi to playlist.");
            this.Playlist_Add.UseVisualStyleBackColor =  true;
            this.Playlist_Add.Click                   += new System.EventHandler(this.Playlist_Add_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Tag";
            this.dataGridViewTextBoxColumn1.HeaderText       = "Tag";
            this.dataGridViewTextBoxColumn1.Name             = "dataGridViewTextBoxColumn1";
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Controls.Add(this.Playlist_Delay);
            this.ButtonPanel.Controls.Add(this.AutoPlayToggle);
            this.ButtonPanel.Controls.Add(this.Playlist_Random);
            this.ButtonPanel.Controls.Add(this.Playlist_Loop);
            this.ButtonPanel.Dock     = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 210);
            this.ButtonPanel.Name     = "ButtonPanel";
            this.ButtonPanel.Padding  = new System.Windows.Forms.Padding(2);
            this.ButtonPanel.Size     = new System.Drawing.Size(205, 26);
            this.ButtonPanel.TabIndex = 10;
            // 
            // Playlist_Delay
            // 
            this.Playlist_Delay.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.Playlist_Delay.DecimalPlaces = 1;
            this.Playlist_Delay.Increment = new decimal(new int[]
            {
                1,
                0,
                0,
                65536
            });
            this.Playlist_Delay.Location = new System.Drawing.Point(152, 1);
            this.Playlist_Delay.Maximum = new decimal(new int[]
            {
                10,
                0,
                0,
                0
            });
            this.Playlist_Delay.Name     = "Playlist_Delay";
            this.Playlist_Delay.Size     = new System.Drawing.Size(48, 23);
            this.Playlist_Delay.TabIndex = 11;
            this.HelpTip.SetToolTip(this.Playlist_Delay, "Delay before playing the next song.");
            this.Playlist_Delay.ValueChanged += new System.EventHandler(this.Playlist_Delay_ValueChanged);
            // 
            // AutoPlayToggle
            // 
            this.AutoPlayToggle.AutoSize   = true;
            this.AutoPlayToggle.Checked    = global::FFBardMusicPlayer.Properties.Settings.Default.PlaylistAutoPlay;
            this.AutoPlayToggle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoPlayToggle.DataBindings.Add(new System.Windows.Forms.Binding("Checked",
                global::FFBardMusicPlayer.Properties.Settings.Default, "PlaylistAutoPlay", true,
                System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.AutoPlayToggle.Location = new System.Drawing.Point(57, 4);
            this.AutoPlayToggle.Name     = "AutoPlayToggle";
            this.AutoPlayToggle.Size     = new System.Drawing.Size(79, 19);
            this.AutoPlayToggle.TabIndex = 10;
            this.AutoPlayToggle.Text     = "Auto-play";
            this.HelpTip.SetToolTip(this.AutoPlayToggle, "Autoplays the next entry if it hasn\'t reached the end.");
            this.AutoPlayToggle.UseVisualStyleBackColor = true;
            // 
            // Playlist_Random
            // 
            this.Playlist_Random.Appearance = System.Windows.Forms.Appearance.Button;
            this.Playlist_Random.AutoSize   = true;
            this.Playlist_Random.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.ButtonBindSource,
                "RandomMode", true));
            this.Playlist_Random.Dock      = System.Windows.Forms.DockStyle.Left;
            this.Playlist_Random.Image     = global::FFBardMusicPlayer.Properties.Resources.Shuffle;
            this.Playlist_Random.Location  = new System.Drawing.Point(28, 2);
            this.Playlist_Random.Margin    = new System.Windows.Forms.Padding(0);
            this.Playlist_Random.Name      = "Playlist_Random";
            this.Playlist_Random.Size      = new System.Drawing.Size(26, 22);
            this.Playlist_Random.TabIndex  = 7;
            this.Playlist_Random.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.HelpTip.SetToolTip(this.Playlist_Random, "Randomize the next song.");
            this.Playlist_Random.UseVisualStyleBackColor = true;
            this.Playlist_Random.CheckedChanged += new System.EventHandler(this.Playlist_Random_CheckedChanged);
            // 
            // ButtonBindSource
            // 
            this.ButtonBindSource.DataSource = typeof(FFBardMusicPlayer.Controls.BmpPlaylist);
            // 
            // Playlist_Loop
            // 
            this.Playlist_Loop.Appearance = System.Windows.Forms.Appearance.Button;
            this.Playlist_Loop.AutoSize   = true;
            this.Playlist_Loop.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.ButtonBindSource,
                "LoopMode", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.Playlist_Loop.Dock      = System.Windows.Forms.DockStyle.Left;
            this.Playlist_Loop.Image     = global::FFBardMusicPlayer.Properties.Resources.Loop;
            this.Playlist_Loop.Location  = new System.Drawing.Point(2, 2);
            this.Playlist_Loop.Margin    = new System.Windows.Forms.Padding(0);
            this.Playlist_Loop.Name      = "Playlist_Loop";
            this.Playlist_Loop.Size      = new System.Drawing.Size(26, 22);
            this.Playlist_Loop.TabIndex  = 6;
            this.Playlist_Loop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.HelpTip.SetToolTip(this.Playlist_Loop, "Loop the entire playlist.");
            this.Playlist_Loop.UseVisualStyleBackColor =  true;
            this.Playlist_Loop.CheckedChanged          += new System.EventHandler(this.Playlist_Loop_CheckedChanged);
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "FilePath";
            this.dataGridViewTextBoxColumn3.HeaderText       = "FilePath";
            this.dataGridViewTextBoxColumn3.Name             = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly         = true;
            this.dataGridViewTextBoxColumn3.Width            = 5;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Track";
            this.dataGridViewTextBoxColumn4.HeaderText       = "Track";
            this.dataGridViewTextBoxColumn4.Name             = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly         = true;
            this.dataGridViewTextBoxColumn4.Width            = 5;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode     = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "FilePath";
            this.dataGridViewTextBoxColumn2.HeaderText       = "FilePath";
            this.dataGridViewTextBoxColumn2.Name             = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly         = true;
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
            // BmpPlaylist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.Playlist);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "BmpPlaylist";
            this.Size = new System.Drawing.Size(205, 236);
            this.Playlist.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.PlaylistView)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.MidiEntrySource)).EndInit();
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.Playlist_Delay)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.ButtonBindSource)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox Playlist;
        private System.Windows.Forms.DataGridView PlaylistView;
        private System.Windows.Forms.Button Playlist_Add;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.BindingSource ButtonBindSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn trackDataGridViewTextBoxColumn;
        private Components.BmpCheckButton Playlist_Loop;
        private Components.BmpCheckButton Playlist_Random;
        private System.Windows.Forms.Button Playlist_Remove;
        private System.Windows.Forms.Panel ButtonPanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.BindingSource MidiEntrySource;
        private System.Windows.Forms.DataGridViewTextBoxColumn filePathDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.CheckBox AutoPlayToggle;
        private System.Windows.Forms.NumericUpDown Playlist_Delay;
        private System.Windows.Forms.ToolTip HelpTip;
        private System.Windows.Forms.Button Playlist_ClearAll;
    }
}