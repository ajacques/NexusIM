using System.Windows.Controls;
using System;
using InstantMessage;
using NexusIM.Managers;
using System.Linq;
using System.Collections.Generic;
using InstantMessage.Protocols.Irc;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for AccountSelectorControl.xaml
	/// </summary>
	public partial class AccountSelectorControl : UserControl
	{
		public AccountSelectorControl()
		{
			this.InitializeComponent();

			mQueryClause = (e) => true;
		}

		public Func<IMProtocolWrapper, bool> QueryClause
		{
			get	{
				return mQueryClause;
			}
			set	{
				mQueryClause = value;

				PopulateDropdownList();
			}
		}

		public void PopulateDropdownList()
		{
			IEnumerable<IMProtocolWrapper> accounts = AccountManager.Accounts.Where(mQueryClause);

			foreach (var account in accounts)
			{
				ListViewItem viewitem = new ListViewItem();
				if (account.Protocol is IRCProtocol)
				{
					IRCProtocol ircProtocol = (IRCProtocol)account.Protocol;
					viewitem.Content = string.Format("{0}@{1} - IRC", ircProtocol.Nickname, ircProtocol.Server);
				} else
					viewitem.Content = account.Protocol.Username + " - " + account.Protocol.Protocol;
				viewitem.Tag = account;
				Selector.Items.Add(viewitem);
			}
		}

		public IMProtocolWrapper SelectedProtocol
		{
			get	{
				return (IMProtocolWrapper)((ListViewItem)Selector.SelectedItem).Tag;
			}
		}

		private Func<IMProtocolWrapper, bool> mQueryClause;
	}
}