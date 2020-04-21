namespace FFBardMusicPlayer.Controls {
	partial class BmpLocalOrchestra {
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
			this.OrchestraGroup = new System.Windows.Forms.GroupBox();
			this.testC = new System.Windows.Forms.Button();
			this.ensembleCheck = new System.Windows.Forms.Button();
			this.muteAll = new System.Windows.Forms.Button();
			this.closeInstruments = new System.Windows.Forms.Button();
			this.openInstruments = new System.Windows.Forms.Button();
			this.PerformerPanel = new System.Windows.Forms.Panel();
			this.OrchestraGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// OrchestraGroup
			// 
			this.OrchestraGroup.Controls.Add(this.testC);
			this.OrchestraGroup.Controls.Add(this.ensembleCheck);
			this.OrchestraGroup.Controls.Add(this.muteAll);
			this.OrchestraGroup.Controls.Add(this.closeInstruments);
			this.OrchestraGroup.Controls.Add(this.openInstruments);
			this.OrchestraGroup.Controls.Add(this.PerformerPanel);
			this.OrchestraGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OrchestraGroup.Location = new System.Drawing.Point(0, 0);
			this.OrchestraGroup.Name = "OrchestraGroup";
			this.OrchestraGroup.Size = new System.Drawing.Size(414, 257);
			this.OrchestraGroup.TabIndex = 0;
			this.OrchestraGroup.TabStop = false;
			this.OrchestraGroup.Text = "Local orchestra";
			// 
			// testC
			// 
			this.testC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.testC.BackColor = System.Drawing.Color.PaleGreen;
			this.testC.Location = new System.Drawing.Point(117, 0);
			this.testC.Name = "testC";
			this.testC.Size = new System.Drawing.Size(51, 21);
			this.testC.TabIndex = 5;
			this.testC.Text = "Sync C";
			this.testC.UseVisualStyleBackColor = false;
			this.testC.Click += new System.EventHandler(this.testC_Click);
			// 
			// ensembleCheck
			// 
			this.ensembleCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ensembleCheck.Location = new System.Drawing.Point(174, 0);
			this.ensembleCheck.Name = "ensembleCheck";
			this.ensembleCheck.Size = new System.Drawing.Size(65, 21);
			this.ensembleCheck.TabIndex = 4;
			this.ensembleCheck.Text = "Ensemble";
			this.ensembleCheck.UseVisualStyleBackColor = true;
			this.ensembleCheck.Click += new System.EventHandler(this.ensembleCheck_Click);
			// 
			// muteAll
			// 
			this.muteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.muteAll.Location = new System.Drawing.Point(245, 0);
			this.muteAll.Name = "muteAll";
			this.muteAll.Size = new System.Drawing.Size(60, 21);
			this.muteAll.TabIndex = 3;
			this.muteAll.Text = "Un/mute";
			this.muteAll.UseVisualStyleBackColor = true;
			this.muteAll.Click += new System.EventHandler(this.muteAll_Click);
			// 
			// closeInstruments
			// 
			this.closeInstruments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closeInstruments.Location = new System.Drawing.Point(357, 0);
			this.closeInstruments.Name = "closeInstruments";
			this.closeInstruments.Size = new System.Drawing.Size(51, 21);
			this.closeInstruments.TabIndex = 2;
			this.closeInstruments.Text = "Close";
			this.closeInstruments.UseVisualStyleBackColor = true;
			this.closeInstruments.Click += new System.EventHandler(this.closeInstruments_Click);
			// 
			// openInstruments
			// 
			this.openInstruments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.openInstruments.Location = new System.Drawing.Point(311, 0);
			this.openInstruments.Name = "openInstruments";
			this.openInstruments.Size = new System.Drawing.Size(45, 21);
			this.openInstruments.TabIndex = 1;
			this.openInstruments.Text = "Open";
			this.openInstruments.UseVisualStyleBackColor = true;
			this.openInstruments.Click += new System.EventHandler(this.openInstruments_Click);
			// 
			// PerformerPanel
			// 
			this.PerformerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PerformerPanel.Location = new System.Drawing.Point(3, 16);
			this.PerformerPanel.Name = "PerformerPanel";
			this.PerformerPanel.Size = new System.Drawing.Size(408, 238);
			this.PerformerPanel.TabIndex = 6;
			// 
			// BmpLocalOrchestra
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.OrchestraGroup);
			this.Name = "BmpLocalOrchestra";
			this.Size = new System.Drawing.Size(414, 257);
			this.OrchestraGroup.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox OrchestraGroup;
		private System.Windows.Forms.Button closeInstruments;
		private System.Windows.Forms.Button openInstruments;
		private System.Windows.Forms.Button muteAll;
		private System.Windows.Forms.Button ensembleCheck;
		private System.Windows.Forms.Button testC;
		private System.Windows.Forms.Panel PerformerPanel;
	}
}
