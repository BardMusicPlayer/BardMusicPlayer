namespace FFBardMusicPlayer.Controls {
	partial class BmpConductor {
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
			this.ConductorNameLabel = new System.Windows.Forms.Label();
			this.ConductorLabel = new System.Windows.Forms.Label();
			this.ConductorTable = new System.Windows.Forms.TableLayoutPanel();
			this.ConductorTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// ConductorNameLabel
			// 
			this.ConductorNameLabel.BackColor = System.Drawing.Color.Transparent;
			this.ConductorNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConductorNameLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Underline);
			this.ConductorNameLabel.ForeColor = System.Drawing.Color.Linen;
			this.ConductorNameLabel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.ConductorNameLabel.Location = new System.Drawing.Point(167, 0);
			this.ConductorNameLabel.Name = "ConductorNameLabel";
			this.ConductorNameLabel.Size = new System.Drawing.Size(159, 22);
			this.ConductorNameLabel.TabIndex = 9;
			this.ConductorNameLabel.Text = "Conductor Name";
			this.ConductorNameLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// ConductorLabel
			// 
			this.ConductorLabel.BackColor = System.Drawing.Color.Transparent;
			this.ConductorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConductorLabel.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Italic);
			this.ConductorLabel.ForeColor = System.Drawing.Color.Orange;
			this.ConductorLabel.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.ConductorLabel.Location = new System.Drawing.Point(10, 0);
			this.ConductorLabel.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.ConductorLabel.Name = "ConductorLabel";
			this.ConductorLabel.Size = new System.Drawing.Size(144, 22);
			this.ConductorLabel.TabIndex = 7;
			this.ConductorLabel.Text = "CONDUCTOR";
			this.ConductorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ConductorTable
			// 
			this.ConductorTable.ColumnCount = 2;
			this.ConductorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.ConductorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.ConductorTable.Controls.Add(this.ConductorNameLabel, 1, 0);
			this.ConductorTable.Controls.Add(this.ConductorLabel, 0, 0);
			this.ConductorTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConductorTable.Location = new System.Drawing.Point(0, 0);
			this.ConductorTable.Name = "ConductorTable";
			this.ConductorTable.RowCount = 1;
			this.ConductorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.ConductorTable.Size = new System.Drawing.Size(329, 22);
			this.ConductorTable.TabIndex = 10;
			// 
			// BmpConductor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.ConductorTable);
			this.DoubleBuffered = true;
			this.Name = "BmpConductor";
			this.Size = new System.Drawing.Size(329, 22);
			this.ConductorTable.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label ConductorNameLabel;
		private System.Windows.Forms.Label ConductorLabel;
		private System.Windows.Forms.TableLayoutPanel ConductorTable;
	}
}
