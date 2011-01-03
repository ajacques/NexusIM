namespace InstantMessage
{
	partial class frmRequestPassword
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
			this.label1 = new System.Windows.Forms.Label();
			this.lblAccount = new System.Windows.Forms.Label();
			this.lblPassword = new System.Windows.Forms.Label();
			this.btnIgnore = new System.Windows.Forms.Button();
			this.btnLogin = new System.Windows.Forms.Button();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.chkRemember = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(259, 30);
			this.label1.TabIndex = 0;
			this.label1.Text = "You didn\'t choose to remember your password for an account, now you need to enter" +
				" it in again to login.";
			// 
			// lblAccount
			// 
			this.lblAccount.AutoSize = true;
			this.lblAccount.Location = new System.Drawing.Point(30, 39);
			this.lblAccount.Name = "lblAccount";
			this.lblAccount.Size = new System.Drawing.Size(44, 13);
			this.lblAccount.TabIndex = 1;
			this.lblAccount.Text = "%1 - %2";
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Location = new System.Drawing.Point(13, 64);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(56, 13);
			this.lblPassword.TabIndex = 2;
			this.lblPassword.Text = "Password:";
			// 
			// btnIgnore
			// 
			this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnIgnore.Location = new System.Drawing.Point(197, 87);
			this.btnIgnore.Name = "btnIgnore";
			this.btnIgnore.Size = new System.Drawing.Size(75, 23);
			this.btnIgnore.TabIndex = 3;
			this.btnIgnore.Text = "Ignore";
			this.btnIgnore.UseVisualStyleBackColor = true;
			this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
			// 
			// btnLogin
			// 
			this.btnLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnLogin.Location = new System.Drawing.Point(116, 87);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(75, 23);
			this.btnLogin.TabIndex = 4;
			this.btnLogin.Text = "Login";
			this.btnLogin.UseVisualStyleBackColor = true;
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(75, 61);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(100, 20);
			this.txtPassword.TabIndex = 5;
			// 
			// chkRemember
			// 
			this.chkRemember.AutoSize = true;
			this.chkRemember.Location = new System.Drawing.Point(181, 64);
			this.chkRemember.Name = "chkRemember";
			this.chkRemember.Size = new System.Drawing.Size(77, 17);
			this.chkRemember.TabIndex = 6;
			this.chkRemember.Text = "Remember";
			this.chkRemember.UseVisualStyleBackColor = true;
			// 
			// frmRequestPassword
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 122);
			this.Controls.Add(this.chkRemember);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.btnIgnore);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.lblAccount);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmRequestPassword";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Password Request";
			this.Shown += new System.EventHandler(this.frmRequestPassword_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblAccount;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Button btnIgnore;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.CheckBox chkRemember;
	}
}