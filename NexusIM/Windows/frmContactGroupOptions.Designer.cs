namespace NexusIM.Windows
{
	partial class frmContactGroupOptions
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
			this.lblName = new System.Windows.Forms.Label();
			this.txtDisplayName = new System.Windows.Forms.TextBox();
			this.orderedListBox1 = new NexusIM.Controls.OrderedListBox(this.components);
			this.btnAccept = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnAddContact = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.Location = new System.Drawing.Point(13, 13);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(75, 13);
			this.lblName.TabIndex = 0;
			this.lblName.Text = "Display Name:";
			// 
			// txtDisplayName
			// 
			this.txtDisplayName.Location = new System.Drawing.Point(94, 10);
			this.txtDisplayName.Name = "txtDisplayName";
			this.txtDisplayName.Size = new System.Drawing.Size(106, 20);
			this.txtDisplayName.TabIndex = 1;
			// 
			// orderedListBox1
			// 
			this.orderedListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.orderedListBox1.FormattingEnabled = true;
			this.orderedListBox1.Items.AddRange(new object[] {
            "test1",
            "test2",
            "test3"});
			this.orderedListBox1.Location = new System.Drawing.Point(18, 48);
			this.orderedListBox1.Name = "orderedListBox1";
			this.orderedListBox1.Size = new System.Drawing.Size(182, 95);
			this.orderedListBox1.TabIndex = 2;
			// 
			// btnAccept
			// 
			this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnAccept.Location = new System.Drawing.Point(197, 227);
			this.btnAccept.Name = "btnAccept";
			this.btnAccept.Size = new System.Drawing.Size(75, 23);
			this.btnAccept.TabIndex = 3;
			this.btnAccept.Text = "Accept";
			this.btnAccept.UseVisualStyleBackColor = true;
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.Location = new System.Drawing.Point(205, 77);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(67, 23);
			this.btnMoveUp.TabIndex = 4;
			this.btnMoveUp.Text = "Up";
			this.btnMoveUp.UseVisualStyleBackColor = true;
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.Location = new System.Drawing.Point(206, 107);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(66, 23);
			this.btnMoveDown.TabIndex = 5;
			this.btnMoveDown.Text = "Down";
			this.btnMoveDown.UseVisualStyleBackColor = true;
			// 
			// btnAddContact
			// 
			this.btnAddContact.Location = new System.Drawing.Point(205, 48);
			this.btnAddContact.Name = "btnAddContact";
			this.btnAddContact.Size = new System.Drawing.Size(67, 23);
			this.btnAddContact.TabIndex = 6;
			this.btnAddContact.Text = "Add";
			this.btnAddContact.UseVisualStyleBackColor = true;
			this.btnAddContact.Click += new System.EventHandler(this.btnAddContact_Click);
			// 
			// frmContactGroupOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.btnAddContact);
			this.Controls.Add(this.btnMoveDown);
			this.Controls.Add(this.btnMoveUp);
			this.Controls.Add(this.btnAccept);
			this.Controls.Add(this.orderedListBox1);
			this.Controls.Add(this.txtDisplayName);
			this.Controls.Add(this.lblName);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmContactGroupOptions";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Group Options";
			this.Load += new System.EventHandler(this.frmContactGroupOptions_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.TextBox txtDisplayName;
		private NexusIM.Controls.OrderedListBox orderedListBox1;
		private System.Windows.Forms.Button btnAccept;
		private System.Windows.Forms.Button btnMoveUp;
		private System.Windows.Forms.Button btnMoveDown;
		private System.Windows.Forms.Button btnAddContact;
	}
}