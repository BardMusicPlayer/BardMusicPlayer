namespace FFBardMusicPlayer.Controls {
	partial class BmpOrchestraPerformer {
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
			this.MainTable = new System.Windows.Forms.TableLayoutPanel();
			this.InfoTable = new System.Windows.Forms.TableLayoutPanel();
			this.PerformerInfo = new System.Windows.Forms.Label();
			this.ControlTable = new System.Windows.Forms.TableLayoutPanel();
			this.KeyboardPreview = new FFBardMusicPlayer.Controls.BmpKeyboard();
			this.OctaveShift = new FFBardMusicPlayer.Components.BmpOctaveShift();
			this.SpeedShift = new FFBardMusicPlayer.Controls.SpeedShiftComponent();
			this.TrackSelect = new FFBardMusicPlayer.Components.BmpTrackShift();
			this.MainTable.SuspendLayout();
			this.InfoTable.SuspendLayout();
			this.ControlTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) (this.OctaveShift)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.SpeedShift)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.TrackSelect)).BeginInit();
			this.SuspendLayout();
			// 
			// MainTable
			// 
			this.MainTable.ColumnCount = 2;
			this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.MainTable.Controls.Add(this.InfoTable, 0, 0);
			this.MainTable.Controls.Add(this.ControlTable, 1, 0);
			this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainTable.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.MainTable.Location = new System.Drawing.Point(2, 2);
			this.MainTable.Margin = new System.Windows.Forms.Padding(0);
			this.MainTable.Name = "MainTable";
			this.MainTable.RowCount = 1;
			this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.MainTable.Size = new System.Drawing.Size(350, 64);
			this.MainTable.TabIndex = 2;
			// 
			// InfoTable
			// 
			this.InfoTable.ColumnCount = 1;
			this.InfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.InfoTable.Controls.Add(this.PerformerInfo, 0, 0);
			this.InfoTable.Controls.Add(this.KeyboardPreview, 0, 1);
			this.InfoTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.InfoTable.Location = new System.Drawing.Point(0, 0);
			this.InfoTable.Margin = new System.Windows.Forms.Padding(0);
			this.InfoTable.Name = "InfoTable";
			this.InfoTable.RowCount = 2;
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
			this.InfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.InfoTable.Size = new System.Drawing.Size(290, 64);
			this.InfoTable.TabIndex = 4;
			// 
			// PerformerInfo
			// 
			this.PerformerInfo.AutoSize = true;
			this.PerformerInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PerformerInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.PerformerInfo.ForeColor = System.Drawing.Color.White;
			this.PerformerInfo.Location = new System.Drawing.Point(3, 0);
			this.PerformerInfo.Name = "PerformerInfo";
			this.PerformerInfo.Size = new System.Drawing.Size(284, 16);
			this.PerformerInfo.TabIndex = 1;
			this.PerformerInfo.Text = "Nare Katol";
			this.PerformerInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// ControlTable
			// 
			this.ControlTable.ColumnCount = 1;
			this.ControlTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ControlTable.Controls.Add(this.OctaveShift, 0, 2);
			this.ControlTable.Controls.Add(this.SpeedShift, 0, 1);
			this.ControlTable.Controls.Add(this.TrackSelect, 0, 0);
			this.ControlTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ControlTable.Location = new System.Drawing.Point(290, 0);
			this.ControlTable.Margin = new System.Windows.Forms.Padding(0);
			this.ControlTable.Name = "ControlTable";
			this.ControlTable.RowCount = 3;
			this.ControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.ControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.ControlTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.ControlTable.Size = new System.Drawing.Size(60, 64);
			this.ControlTable.TabIndex = 3;
			// 
			// KeyboardPreview
			// 
			this.KeyboardPreview.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.KeyboardPreview.BackColor = System.Drawing.Color.White;
			this.KeyboardPreview.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (20)))), ((int) (((byte) (20)))), ((int) (((byte) (20)))));
			this.KeyboardPreview.Location = new System.Drawing.Point(0, 16);
			this.KeyboardPreview.Margin = new System.Windows.Forms.Padding(0);
			this.KeyboardPreview.Name = "KeyboardPreview";
			this.KeyboardPreview.Size = new System.Drawing.Size(290, 48);
			this.KeyboardPreview.TabIndex = 0;
			this.KeyboardPreview.Text = null;
			// 
			// OctaveShift
			// 
			this.OctaveShift.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (50)))), ((int) (((byte) (50)))), ((int) (((byte) (50)))));
			this.OctaveShift.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OctaveShift.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (250)))), ((int) (((byte) (250)))), ((int) (((byte) (250)))));
			this.OctaveShift.Location = new System.Drawing.Point(0, 42);
			this.OctaveShift.Margin = new System.Windows.Forms.Padding(0);
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
			this.OctaveShift.Size = new System.Drawing.Size(60, 23);
			this.OctaveShift.TabIndex = 2;
			this.OctaveShift.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// SpeedShift
			// 
			this.SpeedShift.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (120)))), ((int) (((byte) (120)))), ((int) (((byte) (120)))));
			this.SpeedShift.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SpeedShift.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (250)))), ((int) (((byte) (250)))), ((int) (((byte) (250)))));
			this.SpeedShift.Increment = new decimal(new int[] {
			5,
			0,
			0,
			0});
			this.SpeedShift.Location = new System.Drawing.Point(0, 21);
			this.SpeedShift.Margin = new System.Windows.Forms.Padding(0);
			this.SpeedShift.Maximum = new decimal(new int[] {
			200,
			0,
			0,
			0});
			this.SpeedShift.Minimum = new decimal(new int[] {
			10,
			0,
			0,
			0});
			this.SpeedShift.Name = "SpeedShift";
			this.SpeedShift.Size = new System.Drawing.Size(60, 23);
			this.SpeedShift.TabIndex = 1;
			this.SpeedShift.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.SpeedShift.Value = new decimal(new int[] {
			100,
			0,
			0,
			0});
			// 
			// TrackSelect
			// 
			this.TrackSelect.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (200)))), ((int) (((byte) (200)))), ((int) (((byte) (200)))));
			this.TrackSelect.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TrackSelect.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (20)))), ((int) (((byte) (20)))), ((int) (((byte) (20)))));
			this.TrackSelect.Location = new System.Drawing.Point(0, 0);
			this.TrackSelect.Margin = new System.Windows.Forms.Padding(0);
			this.TrackSelect.Name = "TrackSelect";
			this.TrackSelect.Size = new System.Drawing.Size(60, 23);
			this.TrackSelect.TabIndex = 0;
			this.TrackSelect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// PerformerSetControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.MainTable);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "PerformerSetControl";
			this.Padding = new System.Windows.Forms.Padding(2);
			this.Size = new System.Drawing.Size(354, 68);
			this.Leave += new System.EventHandler(this.ConductPerformerControl_Leave);
			this.MainTable.ResumeLayout(false);
			this.InfoTable.ResumeLayout(false);
			this.InfoTable.PerformLayout();
			this.ControlTable.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) (this.OctaveShift)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.SpeedShift)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.TrackSelect)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel MainTable;
		private System.Windows.Forms.TableLayoutPanel ControlTable;
		private System.Windows.Forms.TableLayoutPanel InfoTable;
		private System.Windows.Forms.Label PerformerInfo;
		private BmpKeyboard KeyboardPreview;
		private Components.BmpOctaveShift OctaveShift;
		private SpeedShiftComponent SpeedShift;
		private Components.BmpTrackShift TrackSelect;
	}
}
