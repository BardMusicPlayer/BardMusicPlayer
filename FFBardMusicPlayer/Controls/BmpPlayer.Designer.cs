namespace FFBardMusicPlayer.Controls {
	partial class BmpPlayer {
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
            this.PlayerGroup = new System.Windows.Forms.GroupBox();
            this.PlayTable = new System.Windows.Forms.TableLayoutPanel();
            this.TrackTable = new System.Windows.Forms.TableLayoutPanel();
            this.CurrentProgressInfo = new System.Windows.Forms.Label();
            this.TrackProgress = new System.Windows.Forms.TrackBar();
            this.TotalProgressInfo = new System.Windows.Forms.Label();
            this.KeyboardTable = new System.Windows.Forms.TableLayoutPanel();
            this.KeyboardCtl = new FFBardMusicPlayer.Controls.BmpKeyboard();
            this.ControlTable = new System.Windows.Forms.TableLayoutPanel();
            this.ShiftPanel = new System.Windows.Forms.Panel();
            this.SelectorOctave = new FFBardMusicPlayer.Components.BmpOctaveShift();
            this.SelectorSpeed = new FFBardMusicPlayer.Controls.SpeedShiftComponent();
            this.TrackPlay = new System.Windows.Forms.Button();
            this.SongCtlPanel = new System.Windows.Forms.Panel();
            this.TrackLoop = new FFBardMusicPlayer.Components.BmpCheckButton(this.components);
            this.TrackSkip = new System.Windows.Forms.Button();
            this.InfoTable = new System.Windows.Forms.TableLayoutPanel();
            this.InfoTrackName = new System.Windows.Forms.Label();
            this.HelpTip = new System.Windows.Forms.ToolTip(this.components);
            this.BrowserList = new System.Windows.Forms.BindingSource(this.components);
            this.PlayerGroup.SuspendLayout();
            this.PlayTable.SuspendLayout();
            this.TrackTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackProgress)).BeginInit();
            this.KeyboardTable.SuspendLayout();
            this.ControlTable.SuspendLayout();
            this.ShiftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectorOctave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectorSpeed)).BeginInit();
            this.SongCtlPanel.SuspendLayout();
            this.InfoTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrowserList)).BeginInit();
            this.SuspendLayout();
            // 
            // PlayerGroup
            // 
            this.PlayerGroup.Controls.Add(this.PlayTable);
            this.PlayerGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayerGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayerGroup.Location = new System.Drawing.Point(0, 0);
            this.PlayerGroup.Margin = new System.Windows.Forms.Padding(0);
            this.PlayerGroup.Name = "PlayerGroup";
            this.PlayerGroup.Padding = new System.Windows.Forms.Padding(0);
            this.PlayerGroup.Size = new System.Drawing.Size(611, 210);
            this.PlayerGroup.TabIndex = 8;
            this.PlayerGroup.TabStop = false;
            this.PlayerGroup.Text = "Bard Music Player by Nare Katol (Ultros)";
            // 
            // PlayTable
            // 
            this.PlayTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PlayTable.BackColor = System.Drawing.Color.Transparent;
            this.PlayTable.ColumnCount = 1;
            this.PlayTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.PlayTable.Controls.Add(this.TrackTable, 0, 0);
            this.PlayTable.Controls.Add(this.KeyboardTable, 0, 1);
            this.PlayTable.Controls.Add(this.InfoTable, 0, 2);
            this.PlayTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayTable.Location = new System.Drawing.Point(0, 18);
            this.PlayTable.Margin = new System.Windows.Forms.Padding(0);
            this.PlayTable.Name = "PlayTable";
            this.PlayTable.RowCount = 3;
            this.PlayTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.PlayTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.PlayTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.PlayTable.Size = new System.Drawing.Size(611, 192);
            this.PlayTable.TabIndex = 29;
            // 
            // TrackTable
            // 
            this.TrackTable.ColumnCount = 5;
            this.TrackTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.TrackTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TrackTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.TrackTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TrackTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TrackTable.Controls.Add(this.CurrentProgressInfo, 0, 0);
            this.TrackTable.Controls.Add(this.TrackProgress, 1, 0);
            this.TrackTable.Controls.Add(this.TotalProgressInfo, 2, 0);
            this.TrackTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrackTable.Location = new System.Drawing.Point(0, 0);
            this.TrackTable.Margin = new System.Windows.Forms.Padding(0);
            this.TrackTable.Name = "TrackTable";
            this.TrackTable.RowCount = 1;
            this.TrackTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TrackTable.Size = new System.Drawing.Size(611, 20);
            this.TrackTable.TabIndex = 27;
            // 
            // CurrentProgressInfo
            // 
            this.CurrentProgressInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CurrentProgressInfo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.CurrentProgressInfo.Location = new System.Drawing.Point(3, 0);
            this.CurrentProgressInfo.Name = "CurrentProgressInfo";
            this.CurrentProgressInfo.Size = new System.Drawing.Size(44, 20);
            this.CurrentProgressInfo.TabIndex = 24;
            this.CurrentProgressInfo.Text = "00:00";
            this.CurrentProgressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TrackProgress
            // 
            this.TrackProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackProgress.AutoSize = false;
            this.TrackProgress.LargeChange = 1;
            this.TrackProgress.Location = new System.Drawing.Point(50, 0);
            this.TrackProgress.Margin = new System.Windows.Forms.Padding(0);
            this.TrackProgress.Name = "TrackProgress";
            this.TrackProgress.Size = new System.Drawing.Size(511, 20);
            this.TrackProgress.TabIndex = 5;
            this.TrackProgress.TickStyle = System.Windows.Forms.TickStyle.None;
            this.TrackProgress.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrackProgress_MouseDown);
            this.TrackProgress.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TrackProgress_MouseMove);
            this.TrackProgress.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TrackProgress_MouseUp);
            // 
            // TotalProgressInfo
            // 
            this.TotalProgressInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TotalProgressInfo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.TotalProgressInfo.Location = new System.Drawing.Point(564, 0);
            this.TotalProgressInfo.Name = "TotalProgressInfo";
            this.TotalProgressInfo.Size = new System.Drawing.Size(44, 20);
            this.TotalProgressInfo.TabIndex = 25;
            this.TotalProgressInfo.Text = "00:00";
            this.TotalProgressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // KeyboardTable
            // 
            this.KeyboardTable.ColumnCount = 2;
            this.KeyboardTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.KeyboardTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.KeyboardTable.Controls.Add(this.KeyboardCtl, 0, 0);
            this.KeyboardTable.Controls.Add(this.ControlTable, 1, 0);
            this.KeyboardTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.KeyboardTable.Location = new System.Drawing.Point(4, 24);
            this.KeyboardTable.Margin = new System.Windows.Forms.Padding(4);
            this.KeyboardTable.Name = "KeyboardTable";
            this.KeyboardTable.RowCount = 1;
            this.KeyboardTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.KeyboardTable.Size = new System.Drawing.Size(603, 144);
            this.KeyboardTable.TabIndex = 28;
            // 
            // KeyboardCtl
            // 
            this.KeyboardCtl.BackColor = System.Drawing.Color.WhiteSmoke;
            this.KeyboardCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.KeyboardCtl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.KeyboardCtl.Location = new System.Drawing.Point(1, 1);
            this.KeyboardCtl.Margin = new System.Windows.Forms.Padding(1);
            this.KeyboardCtl.Name = "KeyboardCtl";
            this.KeyboardCtl.OverrideText = null;
            this.KeyboardCtl.Size = new System.Drawing.Size(487, 142);
            this.KeyboardCtl.TabIndex = 6;
            // 
            // ControlTable
            // 
            this.ControlTable.ColumnCount = 1;
            this.ControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ControlTable.Controls.Add(this.ShiftPanel, 0, 0);
            this.ControlTable.Controls.Add(this.TrackPlay, 0, 1);
            this.ControlTable.Controls.Add(this.SongCtlPanel, 0, 2);
            this.ControlTable.Dock = System.Windows.Forms.DockStyle.Right;
            this.ControlTable.Location = new System.Drawing.Point(493, 0);
            this.ControlTable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ControlTable.Name = "ControlTable";
            this.ControlTable.RowCount = 3;
            this.ControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ControlTable.Size = new System.Drawing.Size(106, 144);
            this.ControlTable.TabIndex = 7;
            // 
            // ShiftPanel
            // 
            this.ShiftPanel.Controls.Add(this.SelectorOctave);
            this.ShiftPanel.Controls.Add(this.SelectorSpeed);
            this.ShiftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShiftPanel.Location = new System.Drawing.Point(0, 0);
            this.ShiftPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ShiftPanel.MinimumSize = new System.Drawing.Size(0, 30);
            this.ShiftPanel.Name = "ShiftPanel";
            this.ShiftPanel.Size = new System.Drawing.Size(106, 30);
            this.ShiftPanel.TabIndex = 10;
            // 
            // SelectorOctave
            // 
            this.SelectorOctave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectorOctave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.SelectorOctave.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.SelectorOctave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.SelectorOctave.Location = new System.Drawing.Point(56, 3);
            this.SelectorOctave.Margin = new System.Windows.Forms.Padding(0);
            this.SelectorOctave.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.SelectorOctave.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            -2147483648});
            this.SelectorOctave.Name = "SelectorOctave";
            this.SelectorOctave.Size = new System.Drawing.Size(50, 23);
            this.SelectorOctave.TabIndex = 4;
            this.SelectorOctave.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.HelpTip.SetToolTip(this.SelectorOctave, "Octave shift");
            this.SelectorOctave.ValueChanged += new System.EventHandler(this.SelectorOctave_ValueChanged);
            // 
            // SelectorSpeed
            // 
            this.SelectorSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.SelectorSpeed.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.SelectorSpeed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.SelectorSpeed.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.SelectorSpeed.Location = new System.Drawing.Point(0, 3);
            this.SelectorSpeed.Margin = new System.Windows.Forms.Padding(0);
            this.SelectorSpeed.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.SelectorSpeed.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.SelectorSpeed.Name = "SelectorSpeed";
            this.SelectorSpeed.Size = new System.Drawing.Size(54, 23);
            this.SelectorSpeed.TabIndex = 3;
            this.SelectorSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.HelpTip.SetToolTip(this.SelectorSpeed, "Speed shift");
            this.SelectorSpeed.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.SelectorSpeed.ValueChanged += new System.EventHandler(this.SelectorSpeed_ValueChanged);
            // 
            // TrackPlay
            // 
            this.TrackPlay.BackColor = System.Drawing.Color.Transparent;
            this.TrackPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.TrackPlay.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TrackPlay.FlatAppearance.BorderSize = 0;
            this.TrackPlay.Image = global::FFBardMusicPlayer.Properties.Resources.Play;
            this.TrackPlay.Location = new System.Drawing.Point(0, 36);
            this.TrackPlay.Margin = new System.Windows.Forms.Padding(0);
            this.TrackPlay.Name = "TrackPlay";
            this.TrackPlay.Size = new System.Drawing.Size(106, 58);
            this.TrackPlay.TabIndex = 7;
            this.TrackPlay.UseVisualStyleBackColor = false;
            this.TrackPlay.Click += new System.EventHandler(this.TrackPlay_Click);
            // 
            // SongCtlPanel
            // 
            this.SongCtlPanel.Controls.Add(this.TrackLoop);
            this.SongCtlPanel.Controls.Add(this.TrackSkip);
            this.SongCtlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SongCtlPanel.Location = new System.Drawing.Point(0, 94);
            this.SongCtlPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SongCtlPanel.MinimumSize = new System.Drawing.Size(0, 50);
            this.SongCtlPanel.Name = "SongCtlPanel";
            this.SongCtlPanel.Size = new System.Drawing.Size(106, 50);
            this.SongCtlPanel.TabIndex = 11;
            // 
            // TrackLoop
            // 
            this.TrackLoop.Appearance = System.Windows.Forms.Appearance.Button;
            this.TrackLoop.Dock = System.Windows.Forms.DockStyle.Top;
            this.TrackLoop.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.TrackLoop.Location = new System.Drawing.Point(0, 0);
            this.TrackLoop.Margin = new System.Windows.Forms.Padding(0);
            this.TrackLoop.Name = "TrackLoop";
            this.TrackLoop.Size = new System.Drawing.Size(106, 25);
            this.TrackLoop.TabIndex = 9;
            this.TrackLoop.Text = "Loop";
            this.TrackLoop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TrackLoop.UseVisualStyleBackColor = true;
            this.TrackLoop.CheckedChanged += new System.EventHandler(this.TrackLoop_CheckedChanged);
            // 
            // TrackSkip
            // 
            this.TrackSkip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TrackSkip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.TrackSkip.Location = new System.Drawing.Point(0, 25);
            this.TrackSkip.Margin = new System.Windows.Forms.Padding(0);
            this.TrackSkip.Name = "TrackSkip";
            this.TrackSkip.Size = new System.Drawing.Size(106, 25);
            this.TrackSkip.TabIndex = 8;
            this.TrackSkip.Text = "Skip";
            this.TrackSkip.UseVisualStyleBackColor = true;
            // 
            // InfoTable
            // 
            this.InfoTable.ColumnCount = 4;
            this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.InfoTable.Controls.Add(this.InfoTrackName, 1, 0);
            this.InfoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InfoTable.Location = new System.Drawing.Point(2, 172);
            this.InfoTable.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.InfoTable.Name = "InfoTable";
            this.InfoTable.RowCount = 1;
            this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.InfoTable.Size = new System.Drawing.Size(607, 20);
            this.InfoTable.TabIndex = 22;
            // 
            // InfoTrackName
            // 
            this.InfoTrackName.AutoEllipsis = true;
            this.InfoTrackName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InfoTrackName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.InfoTrackName.Location = new System.Drawing.Point(10, 0);
            this.InfoTrackName.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.InfoTrackName.Name = "InfoTrackName";
            this.InfoTrackName.Size = new System.Drawing.Size(597, 20);
            this.InfoTrackName.TabIndex = 20;
            this.InfoTrackName.Text = "Track name";
            this.InfoTrackName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.InfoTrackName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PlayerControl_MouseClick);
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
            // BrowserList
            // 
            this.BrowserList.DataMember = "List";
            this.BrowserList.DataSource = typeof(FFBardMusicPlayer.Components.BmpBrowser);
            // 
            // BmpPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PlayerGroup);
            this.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.Name = "BmpPlayer";
            this.Size = new System.Drawing.Size(611, 210);
            this.PlayerGroup.ResumeLayout(false);
            this.PlayTable.ResumeLayout(false);
            this.TrackTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrackProgress)).EndInit();
            this.KeyboardTable.ResumeLayout(false);
            this.ControlTable.ResumeLayout(false);
            this.ShiftPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SelectorOctave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectorSpeed)).EndInit();
            this.SongCtlPanel.ResumeLayout(false);
            this.InfoTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BrowserList)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox PlayerGroup;
		private System.Windows.Forms.TableLayoutPanel PlayTable;
		private System.Windows.Forms.TableLayoutPanel TrackTable;
		private System.Windows.Forms.Label TotalProgressInfo;
		private System.Windows.Forms.TrackBar TrackProgress;
		private System.Windows.Forms.Label CurrentProgressInfo;
		private System.Windows.Forms.TableLayoutPanel KeyboardTable;
		private System.Windows.Forms.Button TrackPlay;
		private System.Windows.Forms.TableLayoutPanel InfoTable;
		private System.Windows.Forms.Label InfoTrackName;
		private BmpKeyboard KeyboardCtl;
		private SpeedShiftComponent SelectorSpeed;
		private Components.BmpOctaveShift SelectorOctave;
		private System.Windows.Forms.ToolTip HelpTip;
		private Components.BmpCheckButton TrackLoop;
		private System.Windows.Forms.Button TrackSkip;
		private System.Windows.Forms.BindingSource BrowserList;
		private System.Windows.Forms.TableLayoutPanel ControlTable;
		private System.Windows.Forms.Panel ShiftPanel;
		private System.Windows.Forms.Panel SongCtlPanel;
	}
}
