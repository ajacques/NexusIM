namespace NexusIM.Windows
{
	partial class frmSelectContact
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
			this.contacts = new NexusIM.Controls.BasicContactList();
			this.SuspendLayout();
			// 
			// contacts
			// 
			this.contacts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.contacts.Location = new System.Drawing.Point(12, 12);
			this.contacts.Name = "contacts";
			this.contacts.Size = new System.Drawing.Size(260, 205);
			this.contacts.TabIndex = 0;
			this.contacts.UseCompatibleStateImageBehavior = false;
			// 
			// frmSelectContact
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.contacts);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSelectContact";
			this.Text = "Select Contact";
			this.Load += new System.EventHandler(this.frmSelectContact_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private NexusIM.Controls.BasicContactList contacts;
	}
}