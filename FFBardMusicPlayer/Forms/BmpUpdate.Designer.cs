namespace FFBardMusicPlayer.Forms
{
    partial class BmpUpdate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UpdateLabel = new System.Windows.Forms.Label();
            this.ButtonSkip  = new System.Windows.Forms.Button();
            this.PanelUpdate = new System.Windows.Forms.Panel();
            this.IconPicture = new System.Windows.Forms.PictureBox();
            this.PanelUpdate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.IconPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // UpdateLabel
            // 
            this.UpdateLabel.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Left |
                                                       System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateLabel.Location  = new System.Drawing.Point(0, 23);
            this.UpdateLabel.Name      = "UpdateLabel";
            this.UpdateLabel.Size      = new System.Drawing.Size(156, 27);
            this.UpdateLabel.TabIndex  = 0;
            this.UpdateLabel.Text      = "Updating...";
            this.UpdateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ButtonSkip
            // 
            this.ButtonSkip.Anchor                  =  System.Windows.Forms.AnchorStyles.Bottom;
            this.ButtonSkip.Location                =  new System.Drawing.Point(39, 71);
            this.ButtonSkip.Name                    =  "ButtonSkip";
            this.ButtonSkip.Size                    =  new System.Drawing.Size(75, 23);
            this.ButtonSkip.TabIndex                =  1;
            this.ButtonSkip.Text                    =  "Skip";
            this.ButtonSkip.UseVisualStyleBackColor =  true;
            this.ButtonSkip.Click                   += new System.EventHandler(this.ButtonSkip_Click);
            // 
            // PanelUpdate
            // 
            this.PanelUpdate.Anchor =
                ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
                                                         System.Windows.Forms.AnchorStyles.Bottom)
                                                        | System.Windows.Forms.AnchorStyles.Left)
                                                       | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelUpdate.Controls.Add(this.ButtonSkip);
            this.PanelUpdate.Controls.Add(this.UpdateLabel);
            this.PanelUpdate.Location = new System.Drawing.Point(132, 4);
            this.PanelUpdate.Name     = "PanelUpdate";
            this.PanelUpdate.Size     = new System.Drawing.Size(156, 102);
            this.PanelUpdate.TabIndex = 2;
            // 
            // IconPicture
            // 
            this.IconPicture.Anchor                = System.Windows.Forms.AnchorStyles.Left;
            this.IconPicture.BackgroundImage       = global::FFBardMusicPlayer.Properties.Resources.bmp_icon_3;
            this.IconPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.IconPicture.Location              = new System.Drawing.Point(18, 13);
            this.IconPicture.Margin                = new System.Windows.Forms.Padding(0);
            this.IconPicture.Name                  = "IconPicture";
            this.IconPicture.Size                  = new System.Drawing.Size(91, 88);
            this.IconPicture.TabIndex              = 3;
            this.IconPicture.TabStop               = false;
            // 
            // AppUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(300, 110);
            this.Controls.Add(this.IconPicture);
            this.Controls.Add(this.PanelUpdate);
            this.Font            = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name            = "AppUpdate";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text            = "AppUpdateSignatures";
            this.PanelUpdate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.IconPicture)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label UpdateLabel;
        private System.Windows.Forms.Button ButtonSkip;
        private System.Windows.Forms.Panel PanelUpdate;
        private System.Windows.Forms.PictureBox IconPicture;
    }
}