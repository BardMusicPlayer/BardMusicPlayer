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
			this.ensembleCheck = new System.Windows.Forms.Button();
			this.muteAll = new System.Windows.Forms.Button();
			this.closeInstruments = new System.Windows.Forms.Button();
			this.openInstruments = new System.Windows.Forms.Button();
			this.PerformerLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.OrchestraGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// OrchestraGroup
			// 
			this.OrchestraGroup.Controls.Add(this.ensembleCheck);
			this.OrchestraGroup.Controls.Add(this.muteAll);
			this.OrchestraGroup.Controls.Add(this.closeInstruments);
			this.OrchestraGroup.Controls.Add(this.openInstruments);
			this.OrchestraGroup.Controls.Add(this.PerformerLayout);
			this.OrchestraGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OrchestraGroup.Location = new System.Drawing.Point(0, 0);
			this.OrchestraGroup.Name = "OrchestraGroup";
			this.OrchestraGroup.Size = new System.Drawing.Size(414, 257);
			this.OrchestraGroup.TabIndex = 0;
			this.OrchestraGroup.TabStop = false;
			this.OrchestraGroup.Text = "Local orchestra";
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
			// PerformerLayout
			// 
			this.PerformerLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.PerformerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PerformerLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.PerformerLayout.Location = new System.Drawing.Point(3, 16);
			this.PerformerLayout.Name = "PerformerLayout";
			this.PerformerLayout.Padding = new System.Windows.Forms.Padding(1);
			this.PerformerLayout.Size = new System.Drawing.Size(408, 238);
			this.PerformerLayout.TabIndex = 0;
			this.PerformerLayout.WrapContents = false;
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
		private System.Windows.Forms.FlowLayoutPanel PerformerLayout;
		private System.Windows.Forms.Button closeInstruments;
		private System.Windows.Forms.Button openInstruments;
		private System.Windows.Forms.Button muteAll;
		private System.Windows.Forms.Button ensembleCheck;
	}
}
