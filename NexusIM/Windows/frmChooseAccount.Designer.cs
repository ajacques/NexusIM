namespace InstantMessage
{
	partial class frmChooseAccount
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
			this.btnSelect = new System.Windows.Forms.Button();
			this.cmbAccount = new System.Windows.Forms.ComboBox();
			this.lblHelp = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnSelect
			// 
			this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSelect.Location = new System.Drawing.Point(197, 68);
			this.btnSelect.Name = "btnSelect";
			this.btnSelect.Size = new System.Drawing.Size(75, 23);
			this.btnSelect.TabIndex = 0;
			this.btnSelect.Text = "Select";
			this.btnSelect.UseVisualStyleBackColor = true;
			this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
			// 
			// cmbAccount
			// 
			this.cmbAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbAccount.FormattingEnabled = true;
			this.cmbAccount.Location = new System.Drawing.Point(40, 41);
			this.cmbAccount.Name = "cmbAccount";
			this.cmbAccount.Size = new System.Drawing.Size(160, 21);
			this.cmbAccount.TabIndex = 1;
			// 
			// lblHelp
			// 
			this.lblHelp.AutoSize = true;
			this.lblHelp.Location = new System.Drawing.Point(13, 13);
			this.lblHelp.Name = "lblHelp";
			this.lblHelp.Size = new System.Drawing.Size(268, 13);
			this.lblHelp.TabIndex = 2;
			this.lblHelp.Text = "Please select the account you want to use for this task.";
			// 
			// frmChooseAccount
			// 
			this.AcceptButton = this.btnSelect;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 102);
			this.Controls.Add(this.lblHelp);
			this.Controls.Add(this.cmbAccount);
			this.Controls.Add(this.btnSelect);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseAccount";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Choose Account";
			this.Shown += new System.EventHandler(this.frmChooseAccount_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSelect;
		private System.Windows.Forms.ComboBox cmbAccount;
		private System.Windows.Forms.Label lblHelp;
	}
}