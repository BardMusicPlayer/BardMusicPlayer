namespace FFBardMusicPlayer {
	partial class PerformerControl {
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
			this.components = new System.ComponentModel.Container();
			this.MemberNameLabel = new System.Windows.Forms.Label();
			this.MemberJobLabel = new System.Windows.Forms.Label();
			this.TrackNumLabel = new System.Windows.Forms.Label();
			this.PartyMemberTable = new System.Windows.Forms.TableLayoutPanel();
			this.AddActorMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.NearMeMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.PartyMemberMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.otherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PartyMemberTable.SuspendLayout();
			this.AddActorMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// MemberNameLabel
			// 
			this.MemberNameLabel.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (20)))), ((int) (((byte) (20)))), ((int) (((byte) (20)))));
			this.MemberNameLabel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.MemberNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MemberNameLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
			this.MemberNameLabel.ForeColor = System.Drawing.Color.Cornsilk;
			this.MemberNameLabel.Location = new System.Drawing.Point(30, 0);
			this.MemberNameLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.MemberNameLabel.Name = "MemberNameLabel";
			this.MemberNameLabel.Size = new System.Drawing.Size(204, 21);
			this.MemberNameLabel.TabIndex = 0;
			this.MemberNameLabel.Text = "Party Member";
			this.MemberNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// MemberJobLabel
			// 
			this.MemberJobLabel.BackColor = System.Drawing.Color.Transparent;
			this.MemberJobLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MemberJobLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.MemberJobLabel.ForeColor = System.Drawing.Color.Gainsboro;
			this.MemberJobLabel.Location = new System.Drawing.Point(0, 0);
			this.MemberJobLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.MemberJobLabel.Name = "MemberJobLabel";
			this.MemberJobLabel.Size = new System.Drawing.Size(30, 20);
			this.MemberJobLabel.TabIndex = 1;
			this.MemberJobLabel.Text = "BRD";
			this.MemberJobLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// TrackNumLabel
			// 
			this.TrackNumLabel.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (40)))), ((int) (((byte) (40)))), ((int) (((byte) (40)))));
			this.TrackNumLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TrackNumLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.TrackNumLabel.ForeColor = System.Drawing.Color.White;
			this.TrackNumLabel.Location = new System.Drawing.Point(237, 0);
			this.TrackNumLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.TrackNumLabel.Name = "TrackNumLabel";
			this.TrackNumLabel.Size = new System.Drawing.Size(20, 21);
			this.TrackNumLabel.TabIndex = 2;
			this.TrackNumLabel.Text = "12";
			this.TrackNumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// PartyMemberTable
			// 
			this.PartyMemberTable.ColumnCount = 3;
			this.PartyMemberTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.PartyMemberTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.PartyMemberTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.PartyMemberTable.Controls.Add(this.MemberJobLabel, 0, 0);
			this.PartyMemberTable.Controls.Add(this.MemberNameLabel, 1, 0);
			this.PartyMemberTable.Controls.Add(this.TrackNumLabel, 2, 0);
			this.PartyMemberTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PartyMemberTable.Location = new System.Drawing.Point(0, 0);
			this.PartyMemberTable.Margin = new System.Windows.Forms.Padding(0);
			this.PartyMemberTable.Name = "PartyMemberTable";
			this.PartyMemberTable.RowCount = 1;
			this.PartyMemberTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.PartyMemberTable.Size = new System.Drawing.Size(260, 24);
			this.PartyMemberTable.TabIndex = 3;
			// 
			// AddActorMenu
			// 
			this.AddActorMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.NearMeMenu,
			this.toolStripMenuItem1,
			this.PartyMemberMenu,
			this.toolStripMenuItem2,
			this.otherToolStripMenuItem});
			this.AddActorMenu.MaximumSize = new System.Drawing.Size(300, 500);
			this.AddActorMenu.Name = "AddActorMenu";
			this.AddActorMenu.Size = new System.Drawing.Size(181, 104);
			// 
			// NearMeMenu
			// 
			this.NearMeMenu.Name = "NearMeMenu";
			this.NearMeMenu.Size = new System.Drawing.Size(180, 22);
			this.NearMeMenu.Text = "Near me";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
			// 
			// PartyMemberMenu
			// 
			this.PartyMemberMenu.Name = "PartyMemberMenu";
			this.PartyMemberMenu.Size = new System.Drawing.Size(180, 22);
			this.PartyMemberMenu.Text = "Party";
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
			// 
			// otherToolStripMenuItem
			// 
			this.otherToolStripMenuItem.Name = "otherToolStripMenuItem";
			this.otherToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.otherToolStripMenuItem.Text = "Other...";
			// 
			// PerformerControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.PartyMemberTable);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.MinimumSize = new System.Drawing.Size(0, 20);
			this.Name = "PerformerControl";
			this.Size = new System.Drawing.Size(260, 24);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PerformerControl_Paint);
			this.PartyMemberTable.ResumeLayout(false);
			this.AddActorMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label MemberNameLabel;
		private System.Windows.Forms.Label MemberJobLabel;
		private System.Windows.Forms.Label TrackNumLabel;
		private System.Windows.Forms.TableLayoutPanel PartyMemberTable;
		private System.Windows.Forms.ContextMenuStrip AddActorMenu;
		private System.Windows.Forms.ToolStripMenuItem NearMeMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem PartyMemberMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem otherToolStripMenuItem;
	}
}
