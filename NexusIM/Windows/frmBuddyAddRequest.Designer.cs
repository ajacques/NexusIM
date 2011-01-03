namespace InstantMessage
{
	partial class frmBuddyAddRequest
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
			this.lblMessage = new System.Windows.Forms.Label();
			this.lblIntroMsg = new System.Windows.Forms.Label();
			this.btnDeny = new System.Windows.Forms.Button();
			this.btnAccept = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblMessage
			// 
			this.lblMessage.AutoSize = true;
			this.lblMessage.Location = new System.Drawing.Point(13, 13);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(35, 13);
			this.lblMessage.TabIndex = 0;
			this.lblMessage.Text = "label1";
			// 
			// lblIntroMsg
			// 
			this.lblIntroMsg.Location = new System.Drawing.Point(12, 41);
			this.lblIntroMsg.Name = "lblIntroMsg";
			this.lblIntroMsg.Size = new System.Drawing.Size(260, 44);
			this.lblIntroMsg.TabIndex = 1;
			// 
			// btnDeny
			// 
			this.btnDeny.Location = new System.Drawing.Point(197, 127);
			this.btnDeny.Name = "btnDeny";
			this.btnDeny.Size = new System.Drawing.Size(75, 23);
			this.btnDeny.TabIndex = 2;
			this.btnDeny.Text = "Deny";
			this.btnDeny.UseVisualStyleBackColor = true;
			this.btnDeny.Click += new System.EventHandler(this.btnDeny_Click);
			// 
			// btnAccept
			// 
			this.btnAccept.Location = new System.Drawing.Point(116, 127);
			this.btnAccept.Name = "btnAccept";
			this.btnAccept.Size = new System.Drawing.Size(75, 23);
			this.btnAccept.TabIndex = 3;
			this.btnAccept.Text = "Accept";
			this.btnAccept.UseVisualStyleBackColor = true;
			this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
			// 
			// frmBuddyAddRequest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 162);
			this.Controls.Add(this.btnAccept);
			this.Controls.Add(this.btnDeny);
			this.Controls.Add(this.lblIntroMsg);
			this.Controls.Add(this.lblMessage);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmBuddyAddRequest";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add Buddy Request";
			this.Shown += new System.EventHandler(this.frmBuddyAddRequest_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.Label lblIntroMsg;
		private System.Windows.Forms.Button btnDeny;
		private System.Windows.Forms.Button btnAccept;
	}
}