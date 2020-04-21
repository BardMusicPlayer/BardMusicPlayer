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
			this.AboutMe = new System.Windows.Forms.RichTextBox();
			this.Selfie = new System.Windows.Forms.PictureBox();
			this.AboutPage2 = new System.Windows.Forms.TabPage();
			this.NameGroup = new System.Windows.Forms.GroupBox();
			this.Names = new System.Windows.Forms.Label();
			this.Names2 = new System.Windows.Forms.Label();
			this.ThanksMacro = new System.Windows.Forms.Label();
			this.ThanksHeader = new System.Windows.Forms.Label();
			this.Sharlayan = new System.Windows.Forms.TabPage();
			this.MultiMidi = new System.Windows.Forms.TabPage();
			this.HeheTooltip = new System.Windows.Forms.ToolTip(this.components);
			this.CloseButton = new System.Windows.Forms.Button();
			this.AboutPanel = new System.Windows.Forms.Panel();
			SharlayanLicense = new System.Windows.Forms.TextBox();
			MultiMidiLicense = new System.Windows.Forms.TextBox();
			this.LicenseTabs.SuspendLayout();
			this.AboutTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Selfie)).BeginInit();
			this.AboutPage2.SuspendLayout();
			this.NameGroup.SuspendLayout();
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
			this.LicenseTabs.Controls.Add(this.AboutPage2);
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
			this.AboutTab.Controls.Add(this.AboutMe);
			this.AboutTab.Controls.Add(this.Selfie);
			this.AboutTab.Location = new System.Drawing.Point(4, 24);
			this.AboutTab.Name = "AboutTab";
			this.AboutTab.Padding = new System.Windows.Forms.Padding(3);
			this.AboutTab.Size = new System.Drawing.Size(360, 181);
			this.AboutTab.TabIndex = 2;
			this.AboutTab.Text = "About";
			this.AboutTab.UseVisualStyleBackColor = true;
			// 
			// AboutMe
			// 
			this.AboutMe.BackColor = System.Drawing.SystemColors.Control;
			this.AboutMe.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.AboutMe.Dock = System.Windows.Forms.DockStyle.Right;
			this.AboutMe.Location = new System.Drawing.Point(128, 3);
			this.AboutMe.Margin = new System.Windows.Forms.Padding(0);
			this.AboutMe.Name = "AboutMe";
			this.AboutMe.ReadOnly = true;
			this.AboutMe.ShortcutsEnabled = false;
			this.AboutMe.Size = new System.Drawing.Size(229, 175);
			this.AboutMe.TabIndex = 16;
			this.AboutMe.Text = "Bard Music Player (c) Paru\n\nWritten in C# and uses modified code from the Sharlay" +
    "an and Sanford.Multimedia.Midi projects.\n\nThank you for using my program!";
			// 
			// Selfie
			// 
			this.Selfie.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Selfie.BackgroundImage")));
			this.Selfie.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.Selfie.Dock = System.Windows.Forms.DockStyle.Left;
			this.Selfie.Location = new System.Drawing.Point(3, 3);
			this.Selfie.Name = "Selfie";
			this.Selfie.Size = new System.Drawing.Size(122, 175);
			this.Selfie.TabIndex = 0;
			this.Selfie.TabStop = false;
			// 
			// AboutPage2
			// 
			this.AboutPage2.Controls.Add(this.NameGroup);
			this.AboutPage2.Controls.Add(this.ThanksMacro);
			this.AboutPage2.Controls.Add(this.ThanksHeader);
			this.AboutPage2.Location = new System.Drawing.Point(4, 24);
			this.AboutPage2.Name = "AboutPage2";
			this.AboutPage2.Padding = new System.Windows.Forms.Padding(3);
			this.AboutPage2.Size = new System.Drawing.Size(360, 181);
			this.AboutPage2.TabIndex = 3;
			this.AboutPage2.Text = "Thanks";
			// 
			// NameGroup
			// 
			this.NameGroup.Controls.Add(this.Names);
			this.NameGroup.Controls.Add(this.Names2);
			this.NameGroup.Location = new System.Drawing.Point(3, 41);
			this.NameGroup.Name = "NameGroup";
			this.NameGroup.Size = new System.Drawing.Size(354, 97);
			this.NameGroup.TabIndex = 5;
			this.NameGroup.TabStop = false;
			this.NameGroup.Text = "for their contribution and support";
			// 
			// Names
			// 
			this.Names.Dock = System.Windows.Forms.DockStyle.Left;
			this.Names.Location = new System.Drawing.Point(172, 19);
			this.Names.Margin = new System.Windows.Forms.Padding(0);
			this.Names.Name = "Names";
			this.Names.Size = new System.Drawing.Size(182, 75);
			this.Names.TabIndex = 0;
			this.Names.Text = "Kazumi\r\nCocoPomel\r\nLogue\r\nBuddycat";
			// 
			// Names2
			// 
			this.Names2.Dock = System.Windows.Forms.DockStyle.Left;
			this.Names2.Location = new System.Drawing.Point(3, 19);
			this.Names2.Margin = new System.Windows.Forms.Padding(0);
			this.Names2.Name = "Names2";
			this.Names2.Size = new System.Drawing.Size(169, 75);
			this.Names2.TabIndex = 1;
			this.Names2.Text = "Miriye Scarletide\r\nRaven Ambree\r\nPost Cards";
			// 
			// ThanksMacro
			// 
			this.ThanksMacro.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ThanksMacro.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.ThanksMacro.Location = new System.Drawing.Point(53, 154);
			this.ThanksMacro.Margin = new System.Windows.Forms.Padding(0);
			this.ThanksMacro.Name = "ThanksMacro";
			this.ThanksMacro.Size = new System.Drawing.Size(299, 19);
			this.ThanksMacro.TabIndex = 4;
			this.ThanksMacro.Text = ".. and to all of you who used Bard Macro Player! ^_^";
			this.ThanksMacro.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ThanksHeader
			// 
			this.ThanksHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ThanksHeader.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ThanksHeader.Location = new System.Drawing.Point(16, 7);
			this.ThanksHeader.Margin = new System.Windows.Forms.Padding(0);
			this.ThanksHeader.Name = "ThanksHeader";
			this.ThanksHeader.Size = new System.Drawing.Size(150, 31);
			this.ThanksHeader.TabIndex = 1;
			this.ThanksHeader.Text = "Special thanks:";
			this.ThanksHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
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
			this.AboutPage2.ResumeLayout(false);
			this.NameGroup.ResumeLayout(false);
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
		private System.Windows.Forms.RichTextBox AboutMe;
		private System.Windows.Forms.Button CloseButton;
		private System.Windows.Forms.Panel AboutPanel;
		private System.Windows.Forms.TabPage AboutPage2;
		private System.Windows.Forms.Label ThanksHeader;
		private System.Windows.Forms.Label Names;
		private System.Windows.Forms.GroupBox NameGroup;
		private System.Windows.Forms.Label ThanksMacro;
		private System.Windows.Forms.PictureBox Selfie;
		private System.Windows.Forms.Label Names2;
	}
}