namespace FFBardMusicPlayer {
	partial class BmpConfirmConductor {
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
			this.ConductorNameLabel = new System.Windows.Forms.Label();
			this.ConductorConfirmText = new System.Windows.Forms.Label();
			this.YesButton = new System.Windows.Forms.Button();
			this.NoButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ConductorNameLabel
			// 
			this.ConductorNameLabel.BackColor = System.Drawing.Color.Transparent;
			this.ConductorNameLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.ConductorNameLabel.ForeColor = System.Drawing.Color.Black;
			this.ConductorNameLabel.Location = new System.Drawing.Point(1, 2);
			this.ConductorNameLabel.Name = "ConductorNameLabel";
			this.ConductorNameLabel.Size = new System.Drawing.Size(197, 28);
			this.ConductorNameLabel.TabIndex = 10;
			this.ConductorNameLabel.Text = "Conductor Name";
			this.ConductorNameLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// ConductorConfirmText
			// 
			this.ConductorConfirmText.Location = new System.Drawing.Point(10, 35);
			this.ConductorConfirmText.Name = "ConductorConfirmText";
			this.ConductorConfirmText.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.ConductorConfirmText.Size = new System.Drawing.Size(175, 33);
			this.ConductorConfirmText.TabIndex = 11;
			this.ConductorConfirmText.Text = "Let this character control\r\nthe music player?";
			this.ConductorConfirmText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// YesButton
			// 
			this.YesButton.Location = new System.Drawing.Point(9, 75);
			this.YesButton.Margin = new System.Windows.Forms.Padding(0);
			this.YesButton.Name = "YesButton";
			this.YesButton.Size = new System.Drawing.Size(91, 23);
			this.YesButton.TabIndex = 12;
			this.YesButton.Text = "Yes";
			this.YesButton.UseVisualStyleBackColor = true;
			this.YesButton.Click += new System.EventHandler(this.YesButton_Click);
			// 
			// NoButton
			// 
			this.NoButton.Location = new System.Drawing.Point(100, 75);
			this.NoButton.Margin = new System.Windows.Forms.Padding(0);
			this.NoButton.Name = "NoButton";
			this.NoButton.Size = new System.Drawing.Size(91, 23);
			this.NoButton.TabIndex = 13;
			this.NoButton.Text = "No";
			this.NoButton.UseVisualStyleBackColor = true;
			this.NoButton.Click += new System.EventHandler(this.NoButton_Click);
			// 
			// AppConfirmConductor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(200, 100);
			this.Controls.Add(this.NoButton);
			this.Controls.Add(this.YesButton);
			this.Controls.Add(this.ConductorConfirmText);
			this.Controls.Add(this.ConductorNameLabel);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "AppConfirmConductor";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label ConductorNameLabel;
		private System.Windows.Forms.Label ConductorConfirmText;
		private System.Windows.Forms.Button YesButton;
		private System.Windows.Forms.Button NoButton;
	}
}