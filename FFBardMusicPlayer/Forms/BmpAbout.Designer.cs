namespace FFBardMusicPlayer {
	partial class BmpAbout {
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TextBox SharlayanLicense;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BmpAbout));
			System.Windows.Forms.TextBox MultiMidiLicense;
			this.LicenseTabs = new System.Windows.Forms.TabControl();
			this.AboutTab = new System.Windows.Forms.TabPage();
			this.About = new System.Windows.Forms.RichTextBox();
			this.Selfie = new System.Windows.Forms.PictureBox();
			this.Sharlayan = new System.Windows.Forms.TabPage();
			this.MultiMidi = new System.Windows.Forms.TabPage();
			this.HeheTooltip = new System.Windows.Forms.ToolTip(this.components);
			this.CloseButton = new System.Windows.Forms.Button();
			this.AboutPanel = new System.Windows.Forms.Panel();
			this.Title = new System.Windows.Forms.RichTextBox();
			this.DonationButton = new System.Windows.Forms.Button();
			this.Pointer = new System.Windows.Forms.RichTextBox();
			SharlayanLicense = new System.Windows.Forms.TextBox();
			MultiMidiLicense = new System.Windows.Forms.TextBox();
			this.LicenseTabs.SuspendLayout();
			this.AboutTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Selfie)).BeginInit();
			this.Sharlayan.SuspendLayout();
			this.MultiMidi.SuspendLayout();
			this.AboutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// SharlayanLicense
			// 
			SharlayanLicense.BorderStyle = System.Windows.Forms.BorderStyle.None;
			SharlayanLicense.Cursor = System.Windows.Forms.Cursors.Arrow;
			SharlayanLicense.Dock = System.Windows.Forms.DockStyle.Fill;
			SharlayanLicense.Font = new System.Drawing.Font("Segoe UI", 8F);
			SharlayanLicense.Location = new System.Drawing.Point(3, 3);
			SharlayanLicense.Margin = new System.Windows.Forms.Padding(0);
			SharlayanLicense.Multiline = true;
			SharlayanLicense.Name = "SharlayanLicense";
			SharlayanLicense.ReadOnly = true;
			SharlayanLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			SharlayanLicense.ShortcutsEnabled = false;
			SharlayanLicense.Size = new System.Drawing.Size(354, 175);
			SharlayanLicense.TabIndex = 1;
			SharlayanLicense.Text = resources.GetString("SharlayanLicense.Text");
			// 
			// MultiMidiLicense
			// 
			MultiMidiLicense.BorderStyle = System.Windows.Forms.BorderStyle.None;
			MultiMidiLicense.Cursor = System.Windows.Forms.Cursors.Arrow;
			MultiMidiLicense.Dock = System.Windows.Forms.DockStyle.Fill;
			MultiMidiLicense.Font = new System.Drawing.Font("Segoe UI", 8F);
			MultiMidiLicense.Location = new System.Drawing.Point(3, 3);
			MultiMidiLicense.Margin = new System.Windows.Forms.Padding(0);
			MultiMidiLicense.Multiline = true;
			MultiMidiLicense.Name = "MultiMidiLicense";
			MultiMidiLicense.ReadOnly = true;
			MultiMidiLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			MultiMidiLicense.ShortcutsEnabled = false;
			MultiMidiLicense.Size = new System.Drawing.Size(354, 175);
			MultiMidiLicense.TabIndex = 2;
			MultiMidiLicense.Text = resources.GetString("MultiMidiLicense.Text");
			// 
			// LicenseTabs
			// 
			this.LicenseTabs.Controls.Add(this.AboutTab);
			this.LicenseTabs.Controls.Add(this.Sharlayan);
			this.LicenseTabs.Controls.Add(this.MultiMidi);
			this.LicenseTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LicenseTabs.Location = new System.Drawing.Point(2, 2);
			this.LicenseTabs.Margin = new System.Windows.Forms.Padding(0);
			this.LicenseTabs.Name = "LicenseTabs";
			this.LicenseTabs.SelectedIndex = 0;
			this.LicenseTabs.Size = new System.Drawing.Size(368, 209);
			this.LicenseTabs.TabIndex = 2;
			// 
			// AboutTab
			// 
			this.AboutTab.Controls.Add(this.DonationButton);
			this.AboutTab.Controls.Add(this.Pointer);
			this.AboutTab.Controls.Add(this.Title);
			this.AboutTab.Controls.Add(this.Selfie);
			this.AboutTab.Controls.Add(this.About);
			this.AboutTab.Location = new System.Drawing.Point(4, 24);
			this.AboutTab.Name = "AboutTab";
			this.AboutTab.Padding = new System.Windows.Forms.Padding(3);
			this.AboutTab.Size = new System.Drawing.Size(360, 181);
			this.AboutTab.TabIndex = 2;
			this.AboutTab.Text = "About";
			this.AboutTab.UseVisualStyleBackColor = true;
			// 
			// About
			// 
			this.About.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.About.BackColor = System.Drawing.SystemColors.Control;
			this.About.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.About.Location = new System.Drawing.Point(0, 44);
			this.About.Margin = new System.Windows.Forms.Padding(0);
			this.About.Name = "About";
			this.About.ReadOnly = true;
			this.About.ShortcutsEnabled = false;
			this.About.Size = new System.Drawing.Size(266, 138);
			this.About.TabIndex = 16;
			this.About.Text = resources.GetString("About.Text");
			// 
			// Selfie
			// 
			this.Selfie.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Selfie.BackgroundImage")));
			this.Selfie.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.Selfie.Location = new System.Drawing.Point(269, 44);
			this.Selfie.Name = "Selfie";
			this.Selfie.Size = new System.Drawing.Size(90, 138);
			this.Selfie.TabIndex = 0;
			this.Selfie.TabStop = false;
			// 
			// Sharlayan
			// 
			this.Sharlayan.Controls.Add(SharlayanLicense);
			this.Sharlayan.Location = new System.Drawing.Point(4, 24);
			this.Sharlayan.Name = "Sharlayan";
			this.Sharlayan.Padding = new System.Windows.Forms.Padding(3);
			this.Sharlayan.Size = new System.Drawing.Size(360, 181);
			this.Sharlayan.TabIndex = 0;
			this.Sharlayan.Text = "Sharlayan";
			this.Sharlayan.UseVisualStyleBackColor = true;
			// 
			// MultiMidi
			// 
			this.MultiMidi.Controls.Add(MultiMidiLicense);
			this.MultiMidi.Location = new System.Drawing.Point(4, 24);
			this.MultiMidi.Name = "MultiMidi";
			this.MultiMidi.Padding = new System.Windows.Forms.Padding(3);
			this.MultiMidi.Size = new System.Drawing.Size(360, 181);
			this.MultiMidi.TabIndex = 1;
			this.MultiMidi.Text = "Sanford.Multimedia.Midi";
			this.MultiMidi.UseVisualStyleBackColor = true;
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.BackColor = System.Drawing.Color.Red;
			this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseButton.ForeColor = System.Drawing.Color.White;
			this.CloseButton.Location = new System.Drawing.Point(347, 3);
			this.CloseButton.Margin = new System.Windows.Forms.Padding(0);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(18, 23);
			this.CloseButton.TabIndex = 3;
			this.CloseButton.Text = "x";
			this.CloseButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.CloseButton.UseVisualStyleBackColor = false;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// AboutPanel
			// 
			this.AboutPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.AboutPanel.Controls.Add(this.CloseButton);
			this.AboutPanel.Controls.Add(this.LicenseTabs);
			this.AboutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AboutPanel.Location = new System.Drawing.Point(0, 0);
			this.AboutPanel.Name = "AboutPanel";
			this.AboutPanel.Padding = new System.Windows.Forms.Padding(2);
			this.AboutPanel.Size = new System.Drawing.Size(374, 215);
			this.AboutPanel.TabIndex = 4;
			// 
			// Title
			// 
			this.Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.Title.BackColor = System.Drawing.SystemColors.Control;
			this.Title.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Title.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.Title.Location = new System.Drawing.Point(0, 0);
			this.Title.Margin = new System.Windows.Forms.Padding(0);
			this.Title.Name = "Title";
			this.Title.ReadOnly = true;
			this.Title.ShortcutsEnabled = false;
			this.Title.Size = new System.Drawing.Size(357, 44);
			this.Title.TabIndex = 17;
            this.Title.Text = "BardMusicPlayer (c) MoogleTroupe, Chipotle, Paru\nhttps://bardmusicplayer.com";
			this.Title.WordWrap = false;
			// 
			// DonationButton
			// 
			this.DonationButton.BackColor = System.Drawing.Color.Gold;
			this.DonationButton.Location = new System.Drawing.Point(204, 155);
			this.DonationButton.Margin = new System.Windows.Forms.Padding(0);
			this.DonationButton.Name = "DonationButton";
			this.DonationButton.Size = new System.Drawing.Size(57, 23);
			this.DonationButton.TabIndex = 18;
			this.DonationButton.Text = "Donate";
			this.DonationButton.UseVisualStyleBackColor = false;
			this.DonationButton.Click += new System.EventHandler(this.DonationButton_Click);
			// 
			// Pointer
			// 
			this.Pointer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.Pointer.BackColor = System.Drawing.SystemColors.Control;
			this.Pointer.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.Pointer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Pointer.Location = new System.Drawing.Point(8, 159);
			this.Pointer.Margin = new System.Windows.Forms.Padding(0);
			this.Pointer.Name = "Pointer";
			this.Pointer.ReadOnly = true;
			this.Pointer.ShortcutsEnabled = false;
			this.Pointer.Size = new System.Drawing.Size(258, 23);
			this.Pointer.TabIndex = 19;
			this.Pointer.Text = "Want to help? You\'ll get a reward! →";
			// 
			// BmpAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(374, 215);
			this.Controls.Add(this.AboutPanel);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "BmpAbout";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AppAbout";
			this.LicenseTabs.ResumeLayout(false);
			this.AboutTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Selfie)).EndInit();
			this.Sharlayan.ResumeLayout(false);
			this.Sharlayan.PerformLayout();
			this.MultiMidi.ResumeLayout(false);
			this.MultiMidi.PerformLayout();
			this.AboutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TabControl LicenseTabs;
		private System.Windows.Forms.TabPage Sharlayan;
		private System.Windows.Forms.TabPage MultiMidi;
		private System.Windows.Forms.ToolTip HeheTooltip;
		private System.Windows.Forms.TabPage AboutTab;
		private System.Windows.Forms.RichTextBox About;
		private System.Windows.Forms.Button CloseButton;
		private System.Windows.Forms.Panel AboutPanel;
		private System.Windows.Forms.PictureBox Selfie;
		private System.Windows.Forms.RichTextBox Title;
		private System.Windows.Forms.Button DonationButton;
		private System.Windows.Forms.RichTextBox Pointer;
	}
}