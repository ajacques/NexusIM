using System;
using System.Collections;
using System.Windows.Forms;
using NexusIM.Managers;

namespace InstantMessage
{
	public partial class frmContactGroups : Form
	{
		public frmContactGroups()
		{
			InitializeComponent();
		}

		private void frmContactGroups_Load(object sender, EventArgs e)
		{
			IEnumerator i = AccountManager.Accounts.GetEnumerator();
			while (i.MoveNext())
			{
				IMProtocol protocol = (IMProtocol)i.Current;
				ListViewGroup group = new ListViewGroup(protocol.Username);
				chkMain.Groups.Add(group);
				IEnumerator o = protocol.ContactList.GetEnumerator();
				while (o.MoveNext())
				{
					IMBuddy buddy = (IMBuddy)o.Current;
					if (!buddy.IsInternalBuddy)
					{
						ListViewItem item = new ListViewItem();
						item.Text = buddy.Username;
						item.Group = group;
						item.Tag = (object)buddy;
						chkMain.Items.Add(item);
					}
				}
			}
		}
		private void btnOK_Click(object sender, EventArgs e)
		{
		}
	}
}
