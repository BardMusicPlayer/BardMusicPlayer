namespace FFBardMusicPlayer.Controls {
	partial class BmpHook {
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
            this.HookTable = new System.Windows.Forms.TableLayoutPanel();
            this.HookButton = new System.Windows.Forms.Button();
            this.CharIdSelector = new System.Windows.Forms.ComboBox();
            this.HookGlobalMessageLabel = new System.Windows.Forms.Label();
            this.HookTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // HookTable
            // 
            this.HookTable.ColumnCount = 3;
            this.HookTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.HookTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.HookTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.HookTable.Controls.Add(this.HookButton, 0, 0);
            this.HookTable.Controls.Add(this.CharIdSelector, 2, 0);
            this.HookTable.Controls.Add(this.HookGlobalMessageLabel, 1, 0);
            this.HookTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HookTable.Location = new System.Drawing.Point(0, 0);
            this.HookTable.Margin = new System.Windows.Forms.Padding(0);
            this.HookTable.Name = "HookTable";
            this.HookTable.RowCount = 1;
            this.HookTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.HookTable.Size = new System.Drawing.Size(598, 24);
            this.HookTable.TabIndex = 0;
            // 
            // HookButton
            // 
            this.HookButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HookButton.FlatAppearance.BorderSize = 0;
            this.HookButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HookButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HookButton.Location = new System.Drawing.Point(2, 0);
            this.HookButton.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HookButton.Name = "HookButton";
            this.HookButton.Size = new System.Drawing.Size(156, 24);
            this.HookButton.TabIndex = 0;
            this.HookButton.Text = "Hook process...";
            this.HookButton.UseVisualStyleBackColor = true;
            this.HookButton.Click += new System.EventHandler(this.HookButton_Click);
            // 
            // CharIdSelector
            // 
            this.CharIdSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CharIdSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CharIdSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CharIdSelector.IntegralHeight = false;
            this.CharIdSelector.Location = new System.Drawing.Point(398, 0);
            this.CharIdSelector.Margin = new System.Windows.Forms.Padding(2, 0, 20, 0);
            this.CharIdSelector.Name = "CharIdSelector";
            this.CharIdSelector.Size = new System.Drawing.Size(180, 21);
            this.CharIdSelector.TabIndex = 2;
            this.CharIdSelector.SelectedIndexChanged += new System.EventHandler(this.CharIdSelector_SelectedIndexChanged);
            // 
            // HookGlobalMessageLabel
            // 
            this.HookGlobalMessageLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.HookGlobalMessageLabel.AutoSize = true;
            this.HookGlobalMessageLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HookGlobalMessageLabel.ForeColor = System.Drawing.Color.Crimson;
            this.HookGlobalMessageLabel.Location = new System.Drawing.Point(163, 4);
            this.HookGlobalMessageLabel.Name = "HookGlobalMessageLabel";
            this.HookGlobalMessageLabel.Size = new System.Drawing.Size(0, 15);
            this.HookGlobalMessageLabel.TabIndex = 3;
            this.HookGlobalMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BmpHook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HookTable);
            this.Name = "BmpHook";
            this.Size = new System.Drawing.Size(598, 24);
            this.HookTable.ResumeLayout(false);
            this.HookTable.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel HookTable;
		private System.Windows.Forms.Button HookButton;
		private System.Windows.Forms.ComboBox CharIdSelector;
        private System.Windows.Forms.Label HookGlobalMessageLabel;
    }
}
