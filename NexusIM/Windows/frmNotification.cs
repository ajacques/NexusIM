using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace InstantMessage
{
	public class frmNotification : Form
	{
		public frmNotification()
		{
			InitializeComponent();
		}

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
		private System.Windows.Forms.Timer scrollOutTimer;
		private string scrollDirection = "up";
		public Label label1;
		public int yend;
		private Button btnClose;
		private Thread scrollThread;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.scrollOutTimer = new System.Windows.Forms.Timer(this.components);
			this.btnClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label1.Location = new System.Drawing.Point(12, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(175, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Example Notification - Hi Everybody";
			// 
			// scrollOutTimer
			// 
			this.scrollOutTimer.Interval = 3000;
			this.scrollOutTimer.Tick += new System.EventHandler(this.scrollOutTimer_Tick);
			// 
			// btnClose
			// 
			this.btnClose.FlatAppearance.BorderSize = 0;
			this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnClose.Location = new System.Drawing.Point(193, 0);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(19, 19);
			this.btnClose.TabIndex = 1;
			this.btnClose.Text = "X";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// frmNotification
			// 
			this.ClientSize = new System.Drawing.Size(208, 30);
			this.ControlBox = false;
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmNotification";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "frmNotification";
			this.Load += new System.EventHandler(this.frmNotification_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private void frmNotification_Load(object sender, EventArgs e)
		{
			this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width, Screen.PrimaryScreen.Bounds.Height);
			this.TopLevel = true;

			scrollThread = new Thread(new ThreadStart(this.scroller));
			scrollThread.Start();
		}
		private void scroller()
		{
			int speed = 2;
			int ystart = Screen.PrimaryScreen.Bounds.Bottom;
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

			while (this.Location.Y >= yend && Thread.CurrentThread.ThreadState == ThreadState.Running)
			{
				int ycurrent = this.Location.Y;
				int x = Screen.PrimaryScreen.Bounds.Width - this.Width;

				if (scrollDirection == "up")
				{
					if (ycurrent > yend)
					{
						MethodInvoker invoker = new MethodInvoker(delegate() { this.Location = new Point(x, ycurrent - speed); });
						this.BeginInvoke(invoker);
						//this.Location = new Point(x, ycurrent - speed);
					} else {
						MethodInvoker invoker = new MethodInvoker(delegate() { this.TopMost = true; scrollOutTimer.Start(); });
						this.BeginInvoke(invoker);
						//scrollOutTimer.Start();
						//this.TopMost = true;
						break;
					}
				} else {
					if (ycurrent < ystart)
					{
						MethodInvoker invoker = new MethodInvoker(delegate() { this.Location = new Point(x, ycurrent + speed); });
						this.BeginInvoke(invoker);
						//this.Location = new Point(x, ycurrent + speed);
					} else {
						Notifications.HandleNotificationHide(this);
						MethodInvoker invoker = new MethodInvoker(delegate() { this.Close(); });
						this.BeginInvoke(invoker);
						//this.Close();
						break;
					}
				}
				Thread.Sleep(25);
			}
		}
		private void scrollOutTimer_Tick(object sender, EventArgs e)
		{
			MethodInvoker invoker = new MethodInvoker(delegate() { this.TopMost = false; });
			this.BeginInvoke(invoker);
			//this.TopMost = false;
			scrollDirection = "down";
			Thread scrollThread = new Thread(new ThreadStart(this.scroller));
			scrollThread.Start();
			scrollOutTimer.Stop();
		}
		private void btnClose_Click(object sender, EventArgs e)
		{
			scrollDirection = "down";
		}
	}
}