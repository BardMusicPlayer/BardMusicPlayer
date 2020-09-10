namespace FFBardMusicPlayer.Controls {
	partial class BmpStatistics {
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
			this.midiStatsBox = new System.Windows.Forms.GroupBox();
			this.InfoTable = new System.Windows.Forms.TableLayoutPanel();
			this.ncText = new System.Windows.Forms.Label();
			this.bpmText = new System.Windows.Forms.Label();
			this.ncCount = new System.Windows.Forms.Label();
			this.bpmCount = new System.Windows.Forms.Label();
			this.trkText = new System.Windows.Forms.Label();
			this.trkCount = new System.Windows.Forms.Label();
			this.nscText = new System.Windows.Forms.Label();
			this.nscCount = new System.Windows.Forms.Label();
			this.npsText = new System.Windows.Forms.Label();
			this.npsCount = new System.Windows.Forms.Label();
			this.chordText = new System.Windows.Forms.Label();
			this.chordCount = new System.Windows.Forms.Label();
			this.midiStatsBox.SuspendLayout();
			this.InfoTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// midiStatsBox
			// 
			this.midiStatsBox.Controls.Add(this.InfoTable);
			this.midiStatsBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.midiStatsBox.Location = new System.Drawing.Point(0, 0);
			this.midiStatsBox.Name = "midiStatsBox";
			this.midiStatsBox.Size = new System.Drawing.Size(226, 246);
			this.midiStatsBox.TabIndex = 0;
			this.midiStatsBox.TabStop = false;
			this.midiStatsBox.Text = "Midi statistics";
			// 
			// InfoTable
			// 
			this.InfoTable.AutoSize = true;
			this.InfoTable.ColumnCount = 2;
			this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.21739F));
			this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.78261F));
			this.InfoTable.Controls.Add(this.ncText, 0, 2);
			this.InfoTable.Controls.Add(this.bpmText, 0, 0);
			this.InfoTable.Controls.Add(this.ncCount, 1, 2);
			this.InfoTable.Controls.Add(this.bpmCount, 1, 0);
			this.InfoTable.Controls.Add(this.trkText, 0, 1);
			this.InfoTable.Controls.Add(this.trkCount, 1, 1);
			this.InfoTable.Controls.Add(this.nscText, 0, 3);
			this.InfoTable.Controls.Add(this.nscCount, 1, 3);
			this.InfoTable.Controls.Add(this.npsText, 0, 4);
			this.InfoTable.Controls.Add(this.npsCount, 1, 4);
			this.InfoTable.Controls.Add(this.chordText, 0, 5);
			this.InfoTable.Controls.Add(this.chordCount, 1, 5);
			this.InfoTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InfoTable.Location = new System.Drawing.Point(3, 16);
			this.InfoTable.Name = "InfoTable";
			this.InfoTable.RowCount = 7;
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.InfoTable.Size = new System.Drawing.Size(220, 227);
			this.InfoTable.TabIndex = 0;
			// 
			// ncText
			// 
			this.ncText.Location = new System.Drawing.Point(3, 40);
			this.ncText.Name = "ncText";
			this.ncText.Size = new System.Drawing.Size(126, 20);
			this.ncText.TabIndex = 0;
			this.ncText.Text = "Total note count";
			this.ncText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// bpmText
			// 
			this.bpmText.Location = new System.Drawing.Point(3, 0);
			this.bpmText.Name = "bpmText";
			this.bpmText.Size = new System.Drawing.Size(126, 20);
			this.bpmText.TabIndex = 4;
			this.bpmText.Text = "Beats per minute";
			this.bpmText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ncCount
			// 
			this.ncCount.Location = new System.Drawing.Point(146, 40);
			this.ncCount.Name = "ncCount";
			this.ncCount.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.ncCount.Size = new System.Drawing.Size(71, 20);
			this.ncCount.TabIndex = 1;
			this.ncCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// bpmCount
			// 
			this.bpmCount.Location = new System.Drawing.Point(146, 0);
			this.bpmCount.Name = "bpmCount";
			this.bpmCount.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.bpmCount.Size = new System.Drawing.Size(71, 20);
			this.bpmCount.TabIndex = 5;
			this.bpmCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// trkText
			// 
			this.trkText.Location = new System.Drawing.Point(3, 20);
			this.trkText.Name = "trkText";
			this.trkText.Size = new System.Drawing.Size(126, 20);
			this.trkText.TabIndex = 6;
			this.trkText.Text = "Total tracks";
			this.trkText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// trkCount
			// 
			this.trkCount.Location = new System.Drawing.Point(146, 20);
			this.trkCount.Name = "trkCount";
			this.trkCount.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.trkCount.Size = new System.Drawing.Size(71, 20);
			this.trkCount.TabIndex = 7;
			this.trkCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// nscText
			// 
			this.nscText.Location = new System.Drawing.Point(3, 60);
			this.nscText.Name = "nscText";
			this.nscText.Size = new System.Drawing.Size(126, 20);
			this.nscText.TabIndex = 12;
			this.nscText.Text = "Track note count";
			this.nscText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// nscCount
			// 
			this.nscCount.Location = new System.Drawing.Point(146, 60);
			this.nscCount.Name = "nscCount";
			this.nscCount.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.nscCount.Size = new System.Drawing.Size(71, 20);
			this.nscCount.TabIndex = 13;
			this.nscCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// npsText
			// 
			this.npsText.Location = new System.Drawing.Point(3, 80);
			this.npsText.Name = "npsText";
			this.npsText.Size = new System.Drawing.Size(126, 20);
			this.npsText.TabIndex = 2;
			this.npsText.Text = "Note per seconds";
			this.npsText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// npsCount
			// 
			this.npsCount.Location = new System.Drawing.Point(146, 80);
			this.npsCount.Name = "npsCount";
			this.npsCount.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.npsCount.Size = new System.Drawing.Size(71, 20);
			this.npsCount.TabIndex = 3;
			this.npsCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chordText
			// 
			this.chordText.Location = new System.Drawing.Point(3, 100);
			this.chordText.Name = "chordText";
			this.chordText.Size = new System.Drawing.Size(126, 20);
			this.chordText.TabIndex = 8;
			this.chordText.Text = "Chords detected";
			this.chordText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chordCount
			// 
			this.chordCount.Location = new System.Drawing.Point(146, 100);
			this.chordCount.Name = "chordCount";
			this.chordCount.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.chordCount.Size = new System.Drawing.Size(71, 20);
			this.chordCount.TabIndex = 9;
			this.chordCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// BmpStatistics
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.midiStatsBox);
			this.Name = "BmpStatistics";
			this.Size = new System.Drawing.Size(226, 246);
			this.midiStatsBox.ResumeLayout(false);
			this.midiStatsBox.PerformLayout();
			this.InfoTable.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox midiStatsBox;
		private System.Windows.Forms.TableLayoutPanel InfoTable;
		private System.Windows.Forms.Label ncText;
		private System.Windows.Forms.Label ncCount;
		private System.Windows.Forms.Label npsText;
		private System.Windows.Forms.Label npsCount;
		private System.Windows.Forms.Label bpmText;
		private System.Windows.Forms.Label bpmCount;
		private System.Windows.Forms.Label trkText;
		private System.Windows.Forms.Label trkCount;
		private System.Windows.Forms.Label chordText;
		private System.Windows.Forms.Label chordCount;
		private System.Windows.Forms.Label nscText;
		private System.Windows.Forms.Label nscCount;
	}
}
