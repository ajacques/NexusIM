namespace InstantMessage
{
	partial class frmContactGroups
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
			if (disposing && (components != null))
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblName = new System.Windows.Forms.Label();
			this.txtNickname = new System.Windows.Forms.TextBox();
			this.chkMain = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(197, 227);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(116, 227);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.Location = new System.Drawing.Point(13, 13);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(58, 13);
			this.lblName.TabIndex = 3;
			this.lblName.Text = "Nickname:";
			// 
			// txtNickname
			// 
			this.txtNickname.Location = new System.Drawing.Point(77, 10);
			this.txtNickname.Name = "txtNickname";
			this.txtNickname.Size = new System.Drawing.Size(100, 20);
			this.txtNickname.TabIndex = 4;
			// 
			// chkMain
			// 
			this.chkMain.CheckBoxes = true;
			this.chkMain.Location = new System.Drawing.Point(13, 30);
			this.chkMain.Name = "chkMain";
			this.chkMain.Size = new System.Drawing.Size(259, 191);
			this.chkMain.TabIndex = 5;
			this.chkMain.UseCompatibleStateImageBehavior = false;
			this.chkMain.View = System.Windows.Forms.View.List;
			// 
			// frmContactGroups
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.chkMain);
			this.Controls.Add(this.txtNickname);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmContactGroups";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "frmContactGroups";
			this.Load += new System.EventHandler(this.frmContactGroups_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.TextBox txtNickname;
		private System.Windows.Forms.ListView chkMain;
	}
}