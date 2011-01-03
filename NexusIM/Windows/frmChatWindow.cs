using System;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{
	public partial class frmChatWindow : frmChatWindowBase
	{
		public frmChatWindow()
		{
			InitializeComponent();
		}

		private void mnuIMVironDoodle_CheckedChanged(object sender, EventArgs e)
		{
			IMYahooProtocol protocol = (IMYahooProtocol)mProtocol;
			protocol.StartIMVironment(mBuddy.Username, IMYahooProtocol.YahooIMVironment.Doodle);
		}
		private void txtToSend_DragDrop(object sender, DragEventArgs e)
		{
			
		}
		protected void txtToSend_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				//txtToSend.Text = txtToSend.Text.TrimEnd(new char[] { '\r', '\n' });
				e.Handled = true;
				if (txtToSend.Text.Length >= 2)
				{
					AppendChatMessage(mProtocol.Username, txtToSend.Text.Substring(0, txtToSend.Text.Length - 1));
					mProtocol.SendMessage(this.Text, txtToSend.Text.Substring(0, txtToSend.Text.Length - 1));
					typingsent = false;
				}
				txtToSend.Text = String.Empty;
			} else {
				// Send Typing Notification
				if (txtToSend.Text.Length >= 2)
				{
					if (!typingsent)
					{
						try	{
							mProtocol.IsTyping(mBuddy.Username, true);
						} catch (Exception x) {
							MessageBox.Show(x.Message);
						}
						typingsent = true;
					}
				} else {
					mProtocol.IsTyping(mBuddy.Username, false);
					typingsent = false;
				}
			}
			if (mBuddy.MaxMessageLength != -1)
			{
				int remaining = mBuddy.MaxMessageLength - (txtToSend.TextLength + 1);
				charsRemaining.Text = "Characters Remaining: " + remaining.ToString();
				if (remaining < 0)
				{
					txtToSend.Text = txtToSend.Text.Substring(0, mBuddy.MaxMessageLength);
				}
			}
		}
		private void frmChatWindow_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;

			this.Text = mBuddy.DisplayName;
		}
		private void frmChatWindow_Shown(object sender, EventArgs e)
		{
			if (mProtocol.Protocol == "Yahoo")
			{
				mnuExtrasIMVironment.Enabled = true;
			}
		}
		private void frmChatWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			ProtocolManager.Instance.HandleContactWindowClose(Buddy);
		}
	}
}