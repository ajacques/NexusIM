using System;
using System.Windows.Forms;

namespace InstantMessage
{
	public class frmEmoticon : Form
	{
		public frmEmoticon()
		{
			InitializeComponent();
		}

		private Button button1;
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
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button1.Location = new System.Drawing.Point(13, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(32, 32);
			this.button1.TabIndex = 0;
			this.button1.Text = ":-)";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// frmEmoticon
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.button1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmEmoticon";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Emoticons";
			this.Load += new System.EventHandler(this.frmEmoticon_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private void frmEmoticon_Load(object sender, EventArgs e)
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				/*MARGINS margin = new MARGINS();
				margin.bottomHeight = 301;
				margin.topHeight = 301;
				margin.leftWidth = 301;
				margin.rightWidth = 301;
				DwmExtendFrameIntoClientArea(this.Handle, ref margin);*/
				Win32.DWM_BLURBEHIND blur = new Win32.DWM_BLURBEHIND();
				blur.dwFlags = Win32.DWM_BB.Enable;
				blur.fEnable = true;
				Win32.DwmEnableBlurBehindWindow(this.Handle, ref blur);
				this.TransparencyKey = System.Drawing.SystemColors.Control;
			}
		}
	}
}
