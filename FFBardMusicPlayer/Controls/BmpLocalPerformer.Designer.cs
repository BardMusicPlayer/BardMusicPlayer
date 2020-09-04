namespace FFBardMusicPlayer.Controls {
	partial class BmpLocalPerformer {
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
			this.CharacterName = new System.Windows.Forms.Label();
			this.PerformerTable = new System.Windows.Forms.TableLayoutPanel();
			this.EnableCheck = new System.Windows.Forms.CheckBox();
			this.InstrumentName = new System.Windows.Forms.Label();
			this.Keyboard = new FFBardMusicPlayer.Controls.BmpKeyboard();
			this.ControlTable = new System.Windows.Forms.TableLayoutPanel();
			this.Scroller = new FFBardMusicPlayer.Controls.BmpMiniScroller();
			this.TrackShift = new FFBardMusicPlayer.Components.BmpTrackShift();
			this.OctaveShift = new FFBardMusicPlayer.Components.BmpOctaveShift();
			this.PerformerTable.SuspendLayout();
			this.ControlTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.TrackShift)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.OctaveShift)).BeginInit();
			this.SuspendLayout();
			// 
			// CharacterName
			// 
			this.CharacterName.AutoEllipsis = true;
			this.CharacterName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CharacterName.Location = new System.Drawing.Point(83, 0);
			this.CharacterName.Name = "CharacterName";
			this.CharacterName.Size = new System.Drawing.Size(180, 25);
			this.CharacterName.TabIndex = 0;
			this.CharacterName.Text = "Firstname Lastname";
			this.CharacterName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// PerformerTable
			// 
			this.PerformerTable.ColumnCount = 5;
			this.PerformerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.PerformerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.PerformerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.PerformerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
			this.PerformerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.PerformerTable.Controls.Add(this.EnableCheck, 0, 0);
			this.PerformerTable.Controls.Add(this.CharacterName, 2, 0);
			this.PerformerTable.Controls.Add(this.InstrumentName, 1, 0);
			this.PerformerTable.Controls.Add(this.Keyboard, 4, 0);
			this.PerformerTable.Controls.Add(this.ControlTable, 3, 0);
			this.PerformerTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PerformerTable.Location = new System.Drawing.Point(0, 0);
			this.PerformerTable.Margin = new System.Windows.Forms.Padding(0);
			this.PerformerTable.Name = "PerformerTable";
			this.PerformerTable.RowCount = 1;
			this.PerformerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.PerformerTable.Size = new System.Drawing.Size(616, 25);
			this.PerformerTable.TabIndex = 1;
			// 
			// EnableCheck
			// 
			this.EnableCheck.Checked = true;
			this.EnableCheck.CheckState = System.Windows.Forms.CheckState.Checked;
			this.EnableCheck.Dock = System.Windows.Forms.DockStyle.Fill;
			this.EnableCheck.Location = new System.Drawing.Point(3, 3);
			this.EnableCheck.Name = "EnableCheck";
			this.EnableCheck.Size = new System.Drawing.Size(14, 19);
			this.EnableCheck.TabIndex = 3;
			this.EnableCheck.UseVisualStyleBackColor = true;
			// 
			// InstrumentName
			// 
			this.InstrumentName.AutoEllipsis = true;
			this.InstrumentName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InstrumentName.Location = new System.Drawing.Point(23, 0);
			this.InstrumentName.Name = "InstrumentName";
			this.InstrumentName.Size = new System.Drawing.Size(54, 25);
			this.InstrumentName.TabIndex = 6;
			this.InstrumentName.Text = "[Piano]";
			this.InstrumentName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Keyboard
			// 
			this.Keyboard.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Keyboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
			this.Keyboard.Location = new System.Drawing.Point(468, 0);
			this.Keyboard.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.Keyboard.Name = "Keyboard";
			this.Keyboard.OverrideText = null;
			this.Keyboard.Size = new System.Drawing.Size(146, 25);
			this.Keyboard.TabIndex = 4;
			// 
			// ControlTable
			// 
			this.ControlTable.ColumnCount = 3;
			this.ControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ControlTable.Controls.Add(this.Scroller, 0, 0);
			this.ControlTable.Controls.Add(this.TrackShift, 1, 0);
			this.ControlTable.Controls.Add(this.OctaveShift, 2, 0);
			this.ControlTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ControlTable.Location = new System.Drawing.Point(266, 0);
			this.ControlTable.Margin = new System.Windows.Forms.Padding(0);
			this.ControlTable.Name = "ControlTable";
			this.ControlTable.RowCount = 1;
			this.ControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ControlTable.Size = new System.Drawing.Size(200, 25);
			this.ControlTable.TabIndex = 5;
			// 
			// Scroller
			// 
			this.Scroller.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Scroller.Location = new System.Drawing.Point(3, 3);
			this.Scroller.Name = "Scroller";
			this.Scroller.Size = new System.Drawing.Size(94, 19);
			this.Scroller.TabIndex = 3;
			// 
			// TrackShift
			// 
			this.TrackShift.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.TrackShift.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
			this.TrackShift.Location = new System.Drawing.Point(103, 3);
			this.TrackShift.Name = "TrackShift";
			this.TrackShift.Size = new System.Drawing.Size(44, 20);
			this.TrackShift.TabIndex = 1;
			this.TrackShift.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TrackShift.ValueChanged += new System.EventHandler(this.TrackShift_ValueChanged);
			// 
			// OctaveShift
			// 
			this.OctaveShift.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.OctaveShift.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
			this.OctaveShift.Location = new System.Drawing.Point(153, 3);
			this.OctaveShift.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
			this.OctaveShift.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            -2147483648});
			this.OctaveShift.Name = "OctaveShift";
			this.OctaveShift.Size = new System.Drawing.Size(44, 20);
			this.OctaveShift.TabIndex = 2;
			this.OctaveShift.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.OctaveShift.ValueChanged += new System.EventHandler(this.OctaveShift_ValueChanged);
			// 
			// BmpLocalPerformer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.PerformerTable);
			this.Name = "BmpLocalPerformer";
			this.Size = new System.Drawing.Size(616, 25);
			this.PerformerTable.ResumeLayout(false);
			this.ControlTable.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.TrackShift)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.OctaveShift)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label CharacterName;
		private System.Windows.Forms.TableLayoutPanel PerformerTable;
		private System.Windows.Forms.CheckBox EnableCheck;
		private BmpKeyboard Keyboard;
		private System.Windows.Forms.TableLayoutPanel ControlTable;
		private Components.BmpTrackShift TrackShift;
		private Components.BmpOctaveShift OctaveShift;
		private System.Windows.Forms.Label InstrumentName;
		private BmpMiniScroller Scroller;
	}
}
