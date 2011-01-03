using System.ComponentModel;
using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmAccounts
	{
		public AccountsListView lstAccounts;
		private Button btnAccept;
		private Button btnNew;
		private Button btnEdit;
		private Button btnDelete;
		private ImageList protocols;
		private IContainer components;

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		public void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Yahoo", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("AIM", System.Windows.Forms.HorizontalAlignment.Left);
			this.btnAccept = new System.Windows.Forms.Button();
			this.lstAccounts = new InstantMessage.AccountsListView();
			this.protocols = new System.Windows.Forms.ImageList(this.components);
			this.btnNew = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnAccept
			// 
			this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnAccept.Location = new System.Drawing.Point(297, 229);
			this.btnAccept.Name = "btnAccept";
			this.btnAccept.Size = new System.Drawing.Size(75, 23);
			this.btnAccept.TabIndex = 1;
			this.btnAccept.Text = "Accept";
			this.btnAccept.UseVisualStyleBackColor = true;
			this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
			// 
			// lstAccounts
			// 
			this.lstAccounts.FullRowSelect = true;
			this.lstAccounts.GridLines = true;
			listViewGroup1.Header = "Yahoo";
			listViewGroup1.Name = "grpYahoo";
			listViewGroup2.Header = "AIM";
			listViewGroup2.Name = "grpAIM";
			this.lstAccounts.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
			this.lstAccounts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstAccounts.HideSelection = false;
			this.lstAccounts.LargeImageList = this.protocols;
			this.lstAccounts.Location = new System.Drawing.Point(12, 12);
			this.lstAccounts.Name = "lstAccounts";
			this.lstAccounts.OwnerDraw = true;
			this.lstAccounts.Size = new System.Drawing.Size(360, 211);
			this.lstAccounts.SmallImageList = this.protocols;
			this.lstAccounts.TabIndex = 0;
			this.lstAccounts.UseCompatibleStateImageBehavior = false;
			this.lstAccounts.View = System.Windows.Forms.View.Tile;
			this.lstAccounts.SelectedIndexChanged += new System.EventHandler(this.lstAccounts_SelectedIndexChanged);
			this.lstAccounts.DoubleClick += new System.EventHandler(this.lstAccounts_DoubleClick);
			// 
			// protocols
			// 
			this.protocols.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.protocols.ImageSize = new System.Drawing.Size(16, 16);
			this.protocols.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// btnNew
			// 
			this.btnNew.Location = new System.Drawing.Point(216, 229);
			this.btnNew.Name = "btnNew";
			this.btnNew.Size = new System.Drawing.Size(75, 23);
			this.btnNew.TabIndex = 3;
			this.btnNew.Text = "New";
			this.btnNew.UseVisualStyleBackColor = true;
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Enabled = false;
			this.btnEdit.Location = new System.Drawing.Point(135, 229);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(75, 23);
			this.btnEdit.TabIndex = 4;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Enabled = false;
			this.btnDelete.Location = new System.Drawing.Point(54, 229);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(75, 23);
			this.btnDelete.TabIndex = 5;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// frmAccounts
			// 
			this.AcceptButton = this.btnAccept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 264);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnNew);
			this.Controls.Add(this.lstAccounts);
			this.Controls.Add(this.btnAccept);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAccounts";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Accounts";
			this.Load += new System.EventHandler(this.frmAccounts_Load);
			this.ResumeLayout(false);

		}

		#endregion
	}
}