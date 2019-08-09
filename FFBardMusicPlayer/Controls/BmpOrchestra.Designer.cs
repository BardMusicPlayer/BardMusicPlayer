namespace FFBardMusicPlayer.Controls {
	partial class BmpOrchestra {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BmpOrchestra));
			this.OrchestraGroup = new System.Windows.Forms.GroupBox();
			this.RefreshPartyButton = new System.Windows.Forms.Button();
			this.AddMemberButton = new System.Windows.Forms.Button();
			this.PartyPanel = new System.Windows.Forms.Panel();
			this.ConductorPanel = new System.Windows.Forms.Panel();
			this.ConductorLabel = new System.Windows.Forms.Label();
			this.ConductorNameLabel = new System.Windows.Forms.Label();
			this.OrchestraTable = new System.Windows.Forms.TableLayoutPanel();
			this.Bhelp = new System.Windows.Forms.Label();
			this.ConductorList = new System.Windows.Forms.GroupBox();
			this.Commandlayout = new System.Windows.Forms.FlowLayoutPanel();
			this.Bcon = new System.Windows.Forms.Label();
			this.Bld = new System.Windows.Forms.Label();
			this.Btr = new System.Windows.Forms.Label();
			this.BplBst = new System.Windows.Forms.Label();
			this.BdlBsk = new System.Windows.Forms.Label();
			this.OrchestraGroup.SuspendLayout();
			this.ConductorPanel.SuspendLayout();
			this.OrchestraTable.SuspendLayout();
			this.ConductorList.SuspendLayout();
			this.Commandlayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// OrchestraGroup
			// 
			this.OrchestraGroup.BackColor = System.Drawing.Color.Transparent;
			this.OrchestraGroup.Controls.Add(this.RefreshPartyButton);
			this.OrchestraGroup.Controls.Add(this.AddMemberButton);
			this.OrchestraGroup.Controls.Add(this.PartyPanel);
			this.OrchestraGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OrchestraGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.OrchestraGroup.ForeColor = System.Drawing.Color.Orange;
			this.OrchestraGroup.Location = new System.Drawing.Point(0, 0);
			this.OrchestraGroup.Margin = new System.Windows.Forms.Padding(4);
			this.OrchestraGroup.Name = "OrchestraGroup";
			this.OrchestraGroup.Padding = new System.Windows.Forms.Padding(2);
			this.OrchestraGroup.Size = new System.Drawing.Size(203, 291);
			this.OrchestraGroup.TabIndex = 6;
			this.OrchestraGroup.TabStop = false;
			this.OrchestraGroup.Text = "ORCHESTRA";
			// 
			// RefreshPartyButton
			// 
			this.RefreshPartyButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RefreshPartyButton.BackColor = System.Drawing.Color.Black;
			this.RefreshPartyButton.FlatAppearance.BorderSize = 0;
			this.RefreshPartyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.RefreshPartyButton.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.RefreshPartyButton.ForeColor = System.Drawing.Color.White;
			this.RefreshPartyButton.Location = new System.Drawing.Point(130, 0);
			this.RefreshPartyButton.Margin = new System.Windows.Forms.Padding(0);
			this.RefreshPartyButton.Name = "RefreshPartyButton";
			this.RefreshPartyButton.Size = new System.Drawing.Size(15, 20);
			this.RefreshPartyButton.TabIndex = 3;
			this.RefreshPartyButton.Text = "r";
			this.RefreshPartyButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.RefreshPartyButton.UseVisualStyleBackColor = false;
			// 
			// AddMemberButton
			// 
			this.AddMemberButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AddMemberButton.BackColor = System.Drawing.Color.Black;
			this.AddMemberButton.FlatAppearance.BorderSize = 0;
			this.AddMemberButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.AddMemberButton.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.AddMemberButton.ForeColor = System.Drawing.Color.White;
			this.AddMemberButton.Location = new System.Drawing.Point(115, 0);
			this.AddMemberButton.Margin = new System.Windows.Forms.Padding(0);
			this.AddMemberButton.Name = "AddMemberButton";
			this.AddMemberButton.Size = new System.Drawing.Size(15, 20);
			this.AddMemberButton.TabIndex = 2;
			this.AddMemberButton.Text = "+";
			this.AddMemberButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.AddMemberButton.UseVisualStyleBackColor = false;
			// 
			// PartyPanel
			// 
			this.PartyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PartyPanel.Location = new System.Drawing.Point(2, 24);
			this.PartyPanel.Margin = new System.Windows.Forms.Padding(0);
			this.PartyPanel.Name = "PartyPanel";
			this.PartyPanel.Size = new System.Drawing.Size(199, 265);
			this.PartyPanel.TabIndex = 1;
			// 
			// ConductorPanel
			// 
			this.ConductorPanel.Controls.Add(this.ConductorLabel);
			this.ConductorPanel.Controls.Add(this.ConductorNameLabel);
			this.ConductorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConductorPanel.Location = new System.Drawing.Point(3, 3);
			this.ConductorPanel.Name = "ConductorPanel";
			this.ConductorPanel.Size = new System.Drawing.Size(197, 34);
			this.ConductorPanel.TabIndex = 9;
			// 
			// ConductorLabel
			// 
			this.ConductorLabel.BackColor = System.Drawing.Color.Transparent;
			this.ConductorLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.ConductorLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.ConductorLabel.ForeColor = System.Drawing.Color.Orange;
			this.ConductorLabel.Location = new System.Drawing.Point(0, 0);
			this.ConductorLabel.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
			this.ConductorLabel.Name = "ConductorLabel";
			this.ConductorLabel.Size = new System.Drawing.Size(197, 16);
			this.ConductorLabel.TabIndex = 7;
			this.ConductorLabel.Text = "CONDUCTOR";
			this.ConductorLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// ConductorNameLabel
			// 
			this.ConductorNameLabel.BackColor = System.Drawing.Color.Transparent;
			this.ConductorNameLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ConductorNameLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.ConductorNameLabel.ForeColor = System.Drawing.Color.Linen;
			this.ConductorNameLabel.Location = new System.Drawing.Point(0, 6);
			this.ConductorNameLabel.Name = "ConductorNameLabel";
			this.ConductorNameLabel.Size = new System.Drawing.Size(197, 28);
			this.ConductorNameLabel.TabIndex = 9;
			this.ConductorNameLabel.Text = "Conductor Name";
			this.ConductorNameLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// OrchestraTable
			// 
			this.OrchestraTable.ColumnCount = 1;
			this.OrchestraTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.OrchestraTable.Controls.Add(this.Bhelp, 0, 2);
			this.OrchestraTable.Controls.Add(this.ConductorList, 0, 1);
			this.OrchestraTable.Controls.Add(this.ConductorPanel, 0, 0);
			this.OrchestraTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OrchestraTable.Location = new System.Drawing.Point(0, 0);
			this.OrchestraTable.Margin = new System.Windows.Forms.Padding(0);
			this.OrchestraTable.Name = "OrchestraTable";
			this.OrchestraTable.RowCount = 2;
			this.OrchestraTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.OrchestraTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 125F));
			this.OrchestraTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.OrchestraTable.Size = new System.Drawing.Size(203, 291);
			this.OrchestraTable.TabIndex = 9;
			// 
			// Bhelp
			// 
			this.Bhelp.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)));
			this.Bhelp.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.Bhelp.ForeColor = System.Drawing.Color.Linen;
			this.Bhelp.Location = new System.Drawing.Point(3, 165);
			this.Bhelp.Name = "Bhelp";
			this.Bhelp.Size = new System.Drawing.Size(194, 126);
			this.Bhelp.TabIndex = 3;
			this.Bhelp.Text = resources.GetString("Bhelp.Text");
			// 
			// ConductorList
			// 
			this.ConductorList.BackColor = System.Drawing.Color.Transparent;
			this.ConductorList.Controls.Add(this.Commandlayout);
			this.ConductorList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConductorList.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.ConductorList.ForeColor = System.Drawing.Color.Orange;
			this.ConductorList.Location = new System.Drawing.Point(4, 44);
			this.ConductorList.Margin = new System.Windows.Forms.Padding(4);
			this.ConductorList.Name = "ConductorList";
			this.ConductorList.Padding = new System.Windows.Forms.Padding(2);
			this.ConductorList.Size = new System.Drawing.Size(195, 117);
			this.ConductorList.TabIndex = 10;
			this.ConductorList.TabStop = false;
			this.ConductorList.Text = "COMMAND LIST";
			// 
			// Commandlayout
			// 
			this.Commandlayout.Controls.Add(this.Bcon);
			this.Commandlayout.Controls.Add(this.Bld);
			this.Commandlayout.Controls.Add(this.Btr);
			this.Commandlayout.Controls.Add(this.BplBst);
			this.Commandlayout.Controls.Add(this.BdlBsk);
			this.Commandlayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Commandlayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.Commandlayout.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.Commandlayout.Location = new System.Drawing.Point(2, 24);
			this.Commandlayout.Margin = new System.Windows.Forms.Padding(0);
			this.Commandlayout.Name = "Commandlayout";
			this.Commandlayout.Size = new System.Drawing.Size(191, 91);
			this.Commandlayout.TabIndex = 2;
			// 
			// Bcon
			// 
			this.Bcon.Location = new System.Drawing.Point(3, 0);
			this.Bcon.Name = "Bcon";
			this.Bcon.Size = new System.Drawing.Size(188, 16);
			this.Bcon.TabIndex = 0;
			this.Bcon.Text = "<b.con> (off) - Ask to conduct";
			// 
			// Bld
			// 
			this.Bld.Location = new System.Drawing.Point(3, 16);
			this.Bld.Name = "Bld";
			this.Bld.Size = new System.Drawing.Size(188, 16);
			this.Bld.TabIndex = 4;
			this.Bld.Text = "<b.ld> [filename] - Load a MIDI";
			// 
			// Btr
			// 
			this.Btr.Location = new System.Drawing.Point(3, 32);
			this.Btr.Name = "Btr";
			this.Btr.Size = new System.Drawing.Size(188, 16);
			this.Btr.TabIndex = 6;
			this.Btr.Text = "<b.tr> [track] - Select MIDI track";
			// 
			// BplBst
			// 
			this.BplBst.Location = new System.Drawing.Point(3, 48);
			this.BplBst.Name = "BplBst";
			this.BplBst.Size = new System.Drawing.Size(188, 16);
			this.BplBst.TabIndex = 1;
			this.BplBst.Text = "<b.pl>/<b.st> - Play/Stop";
			// 
			// BdlBsk
			// 
			this.BdlBsk.Location = new System.Drawing.Point(3, 64);
			this.BdlBsk.Name = "BdlBsk";
			this.BdlBsk.Size = new System.Drawing.Size(188, 16);
			this.BdlBsk.TabIndex = 5;
			this.BdlBsk.Text = "<b.dl>/<b.sk> (ms) - Delay/Seek";
			// 
			// BmpOrchestra
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.OrchestraTable);
			this.Controls.Add(this.OrchestraGroup);
			this.DoubleBuffered = true;
			this.Name = "BmpOrchestra";
			this.Size = new System.Drawing.Size(200, 291);
			this.OrchestraGroup.ResumeLayout(false);
			this.ConductorPanel.ResumeLayout(false);
			this.OrchestraTable.ResumeLayout(false);
			this.ConductorList.ResumeLayout(false);
			this.Commandlayout.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.GroupBox OrchestraGroup;
		private System.Windows.Forms.Button RefreshPartyButton;
		private System.Windows.Forms.Button AddMemberButton;
		private System.Windows.Forms.Panel PartyPanel;
		private System.Windows.Forms.Panel ConductorPanel;
		private System.Windows.Forms.Label ConductorLabel;
		private System.Windows.Forms.Label ConductorNameLabel;
		private System.Windows.Forms.TableLayoutPanel OrchestraTable;
		private System.Windows.Forms.GroupBox ConductorList;
		private System.Windows.Forms.FlowLayoutPanel Commandlayout;
		private System.Windows.Forms.Label Bhelp;
		private System.Windows.Forms.Label Bcon;
		private System.Windows.Forms.Label Bld;
		private System.Windows.Forms.Label BplBst;
		private System.Windows.Forms.Label BdlBsk;
		private System.Windows.Forms.Label Btr;
	}
}
