namespace FFBardMusicPlayer.Controls {
	partial class BmpMiniScroller {
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
			this.ScrollerTable = new System.Windows.Forms.TableLayoutPanel();
			this.Status = new System.Windows.Forms.Label();
			this.LeftButton = new System.Windows.Forms.Button();
			this.RightButton = new System.Windows.Forms.Button();
			this.ScrollerTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// ScrollerTable
			// 
			this.ScrollerTable.ColumnCount = 3;
			this.ScrollerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ScrollerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ScrollerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ScrollerTable.Controls.Add(this.Status, 1, 0);
			this.ScrollerTable.Controls.Add(this.LeftButton, 0, 0);
			this.ScrollerTable.Controls.Add(this.RightButton, 2, 0);
			this.ScrollerTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ScrollerTable.Location = new System.Drawing.Point(0, 0);
			this.ScrollerTable.Name = "ScrollerTable";
			this.ScrollerTable.RowCount = 1;
			this.ScrollerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ScrollerTable.Size = new System.Drawing.Size(241, 26);
			this.ScrollerTable.TabIndex = 0;
			// 
			// Status
			// 
			this.Status.AutoSize = true;
			this.Status.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Status.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Status.Location = new System.Drawing.Point(23, 0);
			this.Status.Name = "Status";
			this.Status.Size = new System.Drawing.Size(195, 26);
			this.Status.TabIndex = 0;
			this.Status.Text = "0";
			this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LeftButton
			// 
			this.LeftButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LeftButton.FlatAppearance.BorderSize = 0;
			this.LeftButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.LeftButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.LeftButton.Location = new System.Drawing.Point(0, 0);
			this.LeftButton.Margin = new System.Windows.Forms.Padding(0);
			this.LeftButton.Name = "LeftButton";
			this.LeftButton.Size = new System.Drawing.Size(20, 26);
			this.LeftButton.TabIndex = 1;
			this.LeftButton.Text = "←";
			this.LeftButton.UseVisualStyleBackColor = true;
			// 
			// RightButton
			// 
			this.RightButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RightButton.FlatAppearance.BorderSize = 0;
			this.RightButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RightButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.RightButton.Location = new System.Drawing.Point(221, 0);
			this.RightButton.Margin = new System.Windows.Forms.Padding(0);
			this.RightButton.Name = "RightButton";
			this.RightButton.Size = new System.Drawing.Size(20, 26);
			this.RightButton.TabIndex = 2;
			this.RightButton.Text = "→";
			this.RightButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.RightButton.UseVisualStyleBackColor = true;
			// 
			// BmpMiniScroller
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ScrollerTable);
			this.Name = "BmpMiniScroller";
			this.Size = new System.Drawing.Size(241, 26);
			this.ScrollerTable.ResumeLayout(false);
			this.ScrollerTable.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel ScrollerTable;
		private System.Windows.Forms.Label Status;
		private System.Windows.Forms.Button LeftButton;
		private System.Windows.Forms.Button RightButton;
	}
}
