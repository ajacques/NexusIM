namespace InstantMessage
{
	partial class frmJoinChatRoom
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
			this.cmbProtocol = new System.Windows.Forms.ComboBox();
			this.lblProtocol = new System.Windows.Forms.Label();
			this.lblRoomName = new System.Windows.Forms.Label();
			this.txtRoomName = new System.Windows.Forms.TextBox();
			this.btnJoin = new System.Windows.Forms.Button();
			this.error = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnRoomList = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
			this.SuspendLayout();
			// 
			// cmbProtocol
			// 
			this.cmbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbProtocol.FormattingEnabled = true;
			this.cmbProtocol.Location = new System.Drawing.Point(90, 6);
			this.cmbProtocol.Name = "cmbProtocol";
			this.cmbProtocol.Size = new System.Drawing.Size(121, 21);
			this.cmbProtocol.TabIndex = 0;
			// 
			// lblProtocol
			// 
			this.lblProtocol.AutoSize = true;
			this.lblProtocol.Location = new System.Drawing.Point(35, 9);
			this.lblProtocol.Name = "lblProtocol";
			this.lblProtocol.Size = new System.Drawing.Size(49, 13);
			this.lblProtocol.TabIndex = 1;
			this.lblProtocol.Text = "Protocol:";
			// 
			// lblRoomName
			// 
			this.lblRoomName.AutoSize = true;
			this.lblRoomName.Location = new System.Drawing.Point(12, 38);
			this.lblRoomName.Name = "lblRoomName";
			this.lblRoomName.Size = new System.Drawing.Size(69, 13);
			this.lblRoomName.TabIndex = 2;
			this.lblRoomName.Text = "Room Name:";
			// 
			// txtRoomName
			// 
			this.txtRoomName.Location = new System.Drawing.Point(90, 35);
			this.txtRoomName.Name = "txtRoomName";
			this.txtRoomName.Size = new System.Drawing.Size(121, 20);
			this.txtRoomName.TabIndex = 3;
			// 
			// btnJoin
			// 
			this.btnJoin.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnJoin.Location = new System.Drawing.Point(197, 77);
			this.btnJoin.Name = "btnJoin";
			this.btnJoin.Size = new System.Drawing.Size(75, 23);
			this.btnJoin.TabIndex = 4;
			this.btnJoin.Text = "Join";
			this.btnJoin.UseVisualStyleBackColor = true;
			this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
			// 
			// error
			// 
			this.error.ContainerControl = this;
			// 
			// btnRoomList
			// 
			this.btnRoomList.Location = new System.Drawing.Point(116, 77);
			this.btnRoomList.Name = "btnRoomList";
			this.btnRoomList.Size = new System.Drawing.Size(75, 23);
			this.btnRoomList.TabIndex = 5;
			this.btnRoomList.Text = "Room List";
			this.btnRoomList.UseVisualStyleBackColor = true;
			// 
			// frmJoinChatRoom
			// 
			this.AcceptButton = this.btnJoin;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 112);
			this.Controls.Add(this.btnRoomList);
			this.Controls.Add(this.btnJoin);
			this.Controls.Add(this.txtRoomName);
			this.Controls.Add(this.lblRoomName);
			this.Controls.Add(this.lblProtocol);
			this.Controls.Add(this.cmbProtocol);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmJoinChatRoom";
			this.Text = "Join Chat Room";
			this.Load += new System.EventHandler(this.frmJoinChatRoom_Load);
			((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbProtocol;
		private System.Windows.Forms.Label lblProtocol;
		private System.Windows.Forms.Label lblRoomName;
		private System.Windows.Forms.TextBox txtRoomName;
		private System.Windows.Forms.Button btnJoin;
		private System.Windows.Forms.ErrorProvider error;
		private System.Windows.Forms.Button btnRoomList;
	}
}