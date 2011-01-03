using System;
using System.Windows.Forms;

namespace InstantMessage
{
	public class frmMUCChatWindow : frmChatWindowBase
	{
		public frmMUCChatWindow()
		{
			InitializeComponent();
		}

		private IChatRoom chatroom;
		//private RichTextBox txtToSend;
		private ListView listView1;
		public IChatRoom ChatRoomContainer
		{
			get {
				return chatroom;
			}
			set {
				chatroom = value;
				UpdateRoom();
			}
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listView1 = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// txtToSend
			// 
			this.txtToSend.Location = new System.Drawing.Point(13, 256);
			this.txtToSend.Size = new System.Drawing.Size(355, 96);
			this.txtToSend.TabIndex = 5;
			this.txtToSend.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtToSend_KeyPress);
			// 
			// listView1
			// 
			this.listView1.Location = new System.Drawing.Point(252, 28);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(121, 196);
			this.listView1.TabIndex = 1;
			this.listView1.UseCompatibleStateImageBehavior = false;
			// 
			// frmMUCChatWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(380, 362);
			this.Controls.Add(this.txtToSend);
			this.Controls.Add(this.listView1);
			this.Name = "frmMUCChatWindow";
			this.Text = "frmMUCChatWindow";
			this.Controls.SetChildIndex(this.listView1, 0);
			this.Controls.SetChildIndex(this.txtToSend, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		public void UpdateRoom()
		{
			if (frmMain.Instance.InvokeRequired)
			{
				MethodInvoker invoker = new MethodInvoker(delegate() { this.UpdateRoom(); });
				frmMain.Instance.Invoke(invoker);
			} else {
				foreach (string user in chatroom.Participants)
				{
					listView1.Items.Add(user);
				}
				this.Text = chatroom.Name;
			}
		}
		private void txtToSend_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				e.Handled = true;
				if (this.txtToSend.Text.Length >= 2)
				{
					AppendChatMessage(mProtocol.Username, this.txtToSend.Text.Substring(0, this.txtToSend.Text.Length - 1));
					mProtocol.SendMessageToRoom(chatroom.Name, txtToSend.Text.Substring(0, this.txtToSend.Text.Length - 1));
					typingsent = false;
				}
				txtToSend.Text = String.Empty;
			}
		}
	}
}