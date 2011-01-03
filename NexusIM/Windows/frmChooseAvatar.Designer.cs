namespace NexusIM.Windows
{
	partial class frmChooseAvatar
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
			this.radDisable = new System.Windows.Forms.RadioButton();
			this.radWindows = new System.Windows.Forms.RadioButton();
			this.picWindows = new System.Windows.Forms.PictureBox();
			this.radCustom = new System.Windows.Forms.RadioButton();
			this.picCustom = new System.Windows.Forms.PictureBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.chooseImageDialog = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.picWindows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picCustom)).BeginInit();
			this.SuspendLayout();
			// 
			// radDisable
			// 
			this.radDisable.AutoSize = true;
			this.radDisable.Location = new System.Drawing.Point(13, 13);
			this.radDisable.Name = "radDisable";
			this.radDisable.Size = new System.Drawing.Size(108, 17);
			this.radDisable.TabIndex = 0;
			this.radDisable.TabStop = true;
			this.radDisable.Text = "No Display Image";
			this.radDisable.UseVisualStyleBackColor = true;
			// 
			// radWindows
			// 
			this.radWindows.AutoSize = true;
			this.radWindows.Location = new System.Drawing.Point(13, 46);
			this.radWindows.Name = "radWindows";
			this.radWindows.Size = new System.Drawing.Size(91, 17);
			this.radWindows.TabIndex = 1;
			this.radWindows.TabStop = true;
			this.radWindows.Text = "Use Windows";
			this.radWindows.UseVisualStyleBackColor = true;
			// 
			// picWindows
			// 
			this.picWindows.Location = new System.Drawing.Point(59, 69);
			this.picWindows.Name = "picWindows";
			this.picWindows.Size = new System.Drawing.Size(64, 64);
			this.picWindows.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picWindows.TabIndex = 2;
			this.picWindows.TabStop = false;
			// 
			// radCustom
			// 
			this.radCustom.AutoSize = true;
			this.radCustom.Location = new System.Drawing.Point(13, 152);
			this.radCustom.Name = "radCustom";
			this.radCustom.Size = new System.Drawing.Size(82, 17);
			this.radCustom.TabIndex = 3;
			this.radCustom.TabStop = true;
			this.radCustom.Text = "Use Custom";
			this.radCustom.UseVisualStyleBackColor = true;
			// 
			// picCustom
			// 
			this.picCustom.Location = new System.Drawing.Point(59, 175);
			this.picCustom.Name = "picCustom";
			this.picCustom.Size = new System.Drawing.Size(64, 64);
			this.picCustom.TabIndex = 4;
			this.picCustom.TabStop = false;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(129, 175);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 5;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// btnEdit
			// 
			this.btnEdit.Location = new System.Drawing.Point(130, 205);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Size = new System.Drawing.Size(75, 23);
			this.btnEdit.TabIndex = 6;
			this.btnEdit.Text = "Edit";
			this.btnEdit.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(168, 258);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(87, 258);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 8;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			// 
			// chooseImageDialog
			// 
			this.chooseImageDialog.Filter = "Images|*.jpg,*.bmp";
			// 
			// frmChooseAvatar
			// 
			this.AcceptButton = this.btnSave;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(255, 293);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.picCustom);
			this.Controls.Add(this.radCustom);
			this.Controls.Add(this.picWindows);
			this.Controls.Add(this.radWindows);
			this.Controls.Add(this.radDisable);
			this.DoubleBuffered = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmChooseAvatar";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Edit Display Image";
			this.Load += new System.EventHandler(this.frmChooseAvatar_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmChooseAvatar_Paint);
			((System.ComponentModel.ISupportInitialize)(this.picWindows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picCustom)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radDisable;
		private System.Windows.Forms.RadioButton radWindows;
		private System.Windows.Forms.PictureBox picWindows;
		private System.Windows.Forms.RadioButton radCustom;
		private System.Windows.Forms.PictureBox picCustom;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.OpenFileDialog chooseImageDialog;
	}
}