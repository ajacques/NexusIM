using System.Windows.Forms;

namespace InstantMessage
{
	public partial class frmChatWindow
	{
		private StatusStrip statusBar;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.SuspendLayout();
			// 
			// txtToSend
			// 
			this.txtToSend.Location = new System.Drawing.Point(12, 254);
			this.txtToSend.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtToSend_DragDrop);
			this.txtToSend.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtToSend_KeyPress);
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 319);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(307, 22);
			this.statusBar.TabIndex = 2;
			// 
			// frmChatWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(307, 341);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.txtToSend);
			this.Name = "frmChatWindow";
			this.Load += new System.EventHandler(this.frmChatWindow_Load);
			this.Shown += new System.EventHandler(this.frmChatWindow_Shown);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmChatWindow_FormClosed);
			this.Controls.SetChildIndex(this.txtToSend, 0);
			this.Controls.SetChildIndex(this.statusBar, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
	}
}