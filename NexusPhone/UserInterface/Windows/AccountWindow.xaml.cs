using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace NexusPhone.UserInterface
{
	public partial class AccountWindow : PhoneApplicationPage
	{
		public AccountWindow()
		{
			InitializeComponent();
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (var account in AccountManager.Accounts)
			{
				AccountListItem item = new AccountListItem(account.AccountInfo);
				
				AccountList.Children.Add(item);
			}
		}
	}
}