namespace FFBardMusicPlayer {
	partial class BmpDx11Warning {
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
			this.DX11_1 = new System.Windows.Forms.Panel();
			this.DX11_2 = new System.Windows.Forms.Panel();
			this.Header = new System.Windows.Forms.Label();
			this.HelpText = new System.Windows.Forms.Label();
			this.CloseButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// DX11_1
			// 
			this.DX11_1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.DX11_1.BackgroundImage = global::FFBardMusicPlayer.Properties.Resources.dx11_1;
			this.DX11_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.DX11_1.Location = new System.Drawing.Point(386, 32);
			this.DX11_1.Name = "DX11_1";
			this.DX11_1.Size = new System.Drawing.Size(72, 45);
			this.DX11_1.TabIndex = 0;
			// 
			// DX11_2
			// 
			this.DX11_2.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.DX11_2.BackgroundImage = global::FFBardMusicPlayer.Properties.Resources.dx11_2;
			this.DX11_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.DX11_2.Location = new System.Drawing.Point(1, 80);
			this.DX11_2.Name = "DX11_2";
			this.DX11_2.Size = new System.Drawing.Size(457, 61);
			this.DX11_2.TabIndex = 1;
			// 
			// Header
			// 
			this.Header.AutoSize = true;
			this.Header.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.Header.Location = new System.Drawing.Point(12, 9);
			this.Header.Name = "Header";
			this.Header.Size = new System.Drawing.Size(121, 21);
			this.Header.TabIndex = 0;
			this.Header.Text = "DirectX11 is off!";
			// 
			// HelpText
			// 
			this.HelpText.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.HelpText.Location = new System.Drawing.Point(13, 39);
			this.HelpText.Name = "HelpText";
			this.HelpText.Size = new System.Drawing.Size(347, 38);
			this.HelpText.TabIndex = 2;
			this.HelpText.Text = "Bard Music Player is only compatible with the DirectX11 (x64 bit) version of the " +
	"game. Please enable it in the FFXIV launcher:";
			// 
			// CloseButton
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.BackColor = System.Drawing.Color.Red;
			this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseButton.ForeColor = System.Drawing.Color.White;
			this.CloseButton.Location = new System.Drawing.Point(430, 3);
			this.CloseButton.Margin = new System.Windows.Forms.Padding(0);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(18, 23);
			this.CloseButton.TabIndex = 4;
			this.CloseButton.TabStop = false;
			this.CloseButton.Text = "x";
			this.CloseButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.CloseButton.UseVisualStyleBackColor = false;
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// AppDx11Warning
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(460, 144);
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.HelpText);
			this.Controls.Add(this.Header);
			this.Controls.Add(this.DX11_1);
			this.Controls.Add(this.DX11_2);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "AppDx11Warning";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Dx11Warning";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel DX11_1;
		private System.Windows.Forms.Panel DX11_2;
		private System.Windows.Forms.Label Header;
		private System.Windows.Forms.Label HelpText;
		private System.Windows.Forms.Button CloseButton;
	}
}