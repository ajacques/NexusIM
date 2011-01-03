using System;
using System.Linq;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{
	public partial class frmJoinChatRoom : Form
	{
		public frmJoinChatRoom()
		{
			InitializeComponent();
		}

		private void frmJoinChatRoom_Load(object sender, EventArgs e)
		{
			var items = from IMProtocol p in AccountManager.Accounts where p.SupportsMultiUserChat && p.ProtocolStatus == IMProtocolStatus.ONLINE select new { p };

			foreach (var protocol in items)
			{
				cmbProtocol.Items.Add(protocol.p.ToString());
			}

			cmbProtocol.SelectedIndex = 0;
		}

		private void btnJoin_Click(object sender, EventArgs e)
		{
			var item = (from IMProtocol p in AccountManager.Accounts where p.SupportsMultiUserChat /*&& p.ToString() == cmbProtocol.SelectedItem*/ && p.ProtocolStatus == IMProtocolStatus.ONLINE select new { p }).FirstOrDefault();

			item.p.JoinChatRoom(txtRoomName.Text);
		}
	}
}
