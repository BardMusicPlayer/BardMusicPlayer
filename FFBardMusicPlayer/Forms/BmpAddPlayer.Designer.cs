namespace FFBardMusicPlayer {
	partial class BmpAddPlayer {
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
			this.ActorList = new System.Windows.Forms.ListBox();
			this.ActorTable = new System.Windows.Forms.TableLayoutPanel();
			this.ActorFilter = new System.Windows.Forms.MaskedTextBox();
			this.ActorTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// ActorList
			// 
			this.ActorList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ActorList.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
			this.ActorList.FormattingEnabled = true;
			this.ActorList.ItemHeight = 12;
			this.ActorList.Location = new System.Drawing.Point(0, 23);
			this.ActorList.Margin = new System.Windows.Forms.Padding(0);
			this.ActorList.Name = "ActorList";
			this.ActorList.Size = new System.Drawing.Size(363, 224);
			this.ActorList.TabIndex = 0;
			this.ActorList.DoubleClick += new System.EventHandler(this.ActorList_DoubleClick);
			// 
			// ActorTable
			// 
			this.ActorTable.ColumnCount = 1;
			this.ActorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ActorTable.Controls.Add(this.ActorFilter, 0, 0);
			this.ActorTable.Controls.Add(this.ActorList, 0, 1);
			this.ActorTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ActorTable.Location = new System.Drawing.Point(0, 0);
			this.ActorTable.Name = "ActorTable";
			this.ActorTable.RowCount = 2;
			this.ActorTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ActorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.ActorTable.Size = new System.Drawing.Size(363, 247);
			this.ActorTable.TabIndex = 1;
			// 
			// ActorFilter
			// 
			this.ActorFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ActorFilter.Location = new System.Drawing.Point(0, 0);
			this.ActorFilter.Margin = new System.Windows.Forms.Padding(0);
			this.ActorFilter.Name = "ActorFilter";
			this.ActorFilter.Size = new System.Drawing.Size(363, 23);
			this.ActorFilter.TabIndex = 0;
			this.ActorFilter.TextChanged += new System.EventHandler(this.ActorFilter_TextChanged);
			this.ActorFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ActorFilter_KeyDown);
			// 
			// AppAddPerformer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(363, 247);
			this.Controls.Add(this.ActorTable);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "AppAddPerformer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AppAddPerformer";
			this.Load += new System.EventHandler(this.AppAddPerformer_Load);
			this.ActorTable.ResumeLayout(false);
			this.ActorTable.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox ActorList;
		private System.Windows.Forms.TableLayoutPanel ActorTable;
		private System.Windows.Forms.MaskedTextBox ActorFilter;
	}
}