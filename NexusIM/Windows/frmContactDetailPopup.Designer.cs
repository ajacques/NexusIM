namespace InstantMessage
{
	partial class frmContactDetailPopup
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
			this.components = new System.ComponentModel.Container();
			this.delayShowTimer = new System.Windows.Forms.Timer(this.components);
			this.lblUsername = new System.Windows.Forms.Label();
			this.buddyImage = new System.Windows.Forms.PictureBox();
			this.chkVisibility = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.buddyImage)).BeginInit();
			this.SuspendLayout();
			// 
			// delayShowTimer
			// 
			this.delayShowTimer.Interval = 1000;
			this.delayShowTimer.Tick += new System.EventHandler(this.delayShowTimer_Tick);
			// 
			// lblUsername
			// 
			this.lblUsername.AutoSize = true;
			this.lblUsername.Location = new System.Drawing.Point(12, 77);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(35, 13);
			this.lblUsername.TabIndex = 3;
			this.lblUsername.Text = "label3";
			// 
			// buddyImage
			// 
			this.buddyImage.Location = new System.Drawing.Point(8, 10);
			this.buddyImage.Name = "buddyImage";
			this.buddyImage.Size = new System.Drawing.Size(64, 64);
			this.buddyImage.TabIndex = 4;
			this.buddyImage.TabStop = false;
			// 
			// chkVisibility
			// 
			this.chkVisibility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.chkVisibility.DropDownWidth = 170;
			this.chkVisibility.Enabled = false;
			this.chkVisibility.FormattingEnabled = true;
			this.chkVisibility.Location = new System.Drawing.Point(12, 95);
			this.chkVisibility.Name = "chkVisibility";
			this.chkVisibility.Size = new System.Drawing.Size(126, 21);
			this.chkVisibility.TabIndex = 5;
			// 
			// frmContactDetailPopup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(150, 128);
			this.Controls.Add(this.chkVisibility);
			this.Controls.Add(this.buddyImage);
			this.Controls.Add(this.lblUsername);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "frmContactDetailPopup";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Contact Details";
			this.Load += new System.EventHandler(this.frmContactDetailPopup_Load);
			((System.ComponentModel.ISupportInitialize)(this.buddyImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer delayShowTimer;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.PictureBox buddyImage;
		private System.Windows.Forms.ComboBox chkVisibility;
	}
}