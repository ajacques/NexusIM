using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InstantMessage;
using NexusIM.Controls;
using NexusIM.Managers;
using System.Diagnostics;
using System.Threading;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for AccountsEdit.xaml
	/// </summary>
	sealed partial class AccountsEdit : Window
	{
		public AccountsEdit()
		{
			this.InitializeComponent();
		}
	
		private void DeselectAllExcept(UIElementCollection source, SetupAccountItem exception)
		{
			foreach (SetupAccountItem item in source)
			{
				if (item.Selected && item != exception)
					item.Deselect();
			}
		}

		// Event Handlers
		private void AddAccount_Select(object sender, SelectionChangedEventArgs e)
		{
			ComboBox addAccount = sender as ComboBox;
			IMProtocol protocol;

			switch (addAccount.SelectedIndex)
			{
				case 1:
					protocol = new IMYahooProtocol();
					break;
				default:
					return;
			}

			SetupAccountItem item = new SetupAccountItem();

			item.DataContext = new IMProtocolExtraData() { Protocol = protocol, Enabled = true };
			item.Margin = (Thickness)FindResource("ItemMargin");

			AccountsListBox.Children.Add(item);
			DeselectAllExcept(AccountsListBox.Children, item);
			item.Select();
		}
		private void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => IMSettings.Save() ));
		}
		private void AccountItem_Click(object sender, MouseButtonEventArgs e)
		{
			SetupAccountItem item = sender as SetupAccountItem;
			item.Select();
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (IMProtocolExtraData protocol in AccountManager.Accounts)
			{
				SetupAccountItem accItem = new SetupAccountItem();
				accItem.DataContext = protocol;
				AccountsListBox.Children.Add(accItem);
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.Source is SetupAccountItem)
			{
				SetupAccountItem acc = e.Source as SetupAccountItem;
				acc.Select();
				DeselectAllExcept(AccountsListBox.Children, acc);
			} else {
				DeselectAllExcept(AccountsListBox.Children, null);
			}
		}	
	}
}