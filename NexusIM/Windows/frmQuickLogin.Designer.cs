namespace InstantMessage
{
	partial class frmQuickLogin
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
			this.btnContinue = new System.Windows.Forms.Button();
			this.lblQuickHelp = new System.Windows.Forms.Label();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.lblUsername = new System.Windows.Forms.Label();
			this.lblPassword = new System.Windows.Forms.Label();
			this.txtPassword = new NexusIM.Controls.HintTextBox();
			this.SuspendLayout();
			// 
			// btnContinue
			// 
			this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.btnContinue.Location = new System.Drawing.Point(197, 107);
			this.btnContinue.Name = "btnContinue";
			this.btnContinue.Size = new System.Drawing.Size(75, 23);
			this.btnContinue.TabIndex = 0;
			this.btnContinue.Text = "Continue";
			this.btnContinue.UseVisualStyleBackColor = true;
			this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
			// 
			// lblQuickHelp
			// 
			this.lblQuickHelp.Location = new System.Drawing.Point(13, 13);
			this.lblQuickHelp.Name = "lblQuickHelp";
			this.lblQuickHelp.Size = new System.Drawing.Size(259, 32);
			this.lblQuickHelp.TabIndex = 1;
			this.lblQuickHelp.Text = "Please enter your username/password to connect to this service";
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(77, 46);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(132, 20);
			this.txtUsername.TabIndex = 2;
			// 
			// lblUsername
			// 
			this.lblUsername.AutoSize = true;
			this.lblUsername.Location = new System.Drawing.Point(13, 49);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(58, 13);
			this.lblUsername.TabIndex = 3;
			this.lblUsername.Text = "Username:";
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Location = new System.Drawing.Point(13, 76);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(56, 13);
			this.lblPassword.TabIndex = 4;
			this.lblPassword.Text = "Password:";
			// 
			// txtPassword
			// 
			this.txtPassword.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.txtPassword.Hint = "   (optional)";
			this.txtPassword.HintEnabled = true;
			this.txtPassword.Location = new System.Drawing.Point(76, 73);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Size = new System.Drawing.Size(133, 20);
			this.txtPassword.TabIndex = 5;
			this.txtPassword.Text = "   (optional)";
			// 
			// frmQuickLogin
			// 
			this.AcceptButton = this.btnContinue;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 142);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.lblUsername);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.lblQuickHelp);
			this.Controls.Add(this.btnContinue);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmQuickLogin";
			this.ShowIcon = false;
			this.Text = "Login Credentials";
			this.Shown += new System.EventHandler(this.frmQuickLogin_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnContinue;
		private System.Windows.Forms.Label lblQuickHelp;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.Label lblPassword;
		private NexusIM.Controls.HintTextBox txtPassword;
	}
}