using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using InstantMessage;
using InstantMessage.Protocols.Irc;
using InstantMessage.Protocols.Yahoo;
using NexusIM.Controls;
using NexusIM.Managers;

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

			mNewAccounts = new List<IMProtocolExtraData>();
		}
	
		private void DeselectAllExcept(UIElementCollection source, SetupAccountItem exception)
		{
			foreach (SetupAccountItem item in source)
			{
				if (item.Selected && item != exception)
					item.Deselect();
			}
		}

		// Helper Utilities
		private void FadeOut(UIElement element, Delegate onComplete)
		{
			// Storyboard to fade out an object
			Storyboard storyboard = new Storyboard();
			DoubleAnimation anim = new DoubleAnimation();
			anim.To = 0;
			anim.SetValue(Storyboard.TargetProperty, element);
			anim.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(FrameworkElement.Opacity)"));
			anim.Duration = TimeSpan.FromMilliseconds(500); // Half a second
			storyboard.Children.Add(anim);
			EventHandler handler = null;
			handler = new EventHandler((object sender, EventArgs e) =>
			{
				Dispatcher.BeginInvoke(onComplete);
				storyboard.Completed -= handler;
			});
			storyboard.Completed += handler;
			storyboard.Begin();
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
				case 2:
					protocol = new IRCProtocol();
					break;
				default:
					return;
			}

			Trace.WriteLine("User is adding new account of type " + protocol.Protocol);

			SetupAccountItem item = new SetupAccountItem();

			IMProtocolExtraData extraData = new IMProtocolExtraData() { Protocol = protocol, Enabled = false };
			item.PopulateUIControls(extraData);

			item.Margin = (Thickness)FindResource("ItemMargin");

			AccountsListBox.Children.Add(item);
			DeselectAllExcept(AccountsListBox.Children, item);
			item.Select();

			mNewAccounts.Add(extraData);			
		}
		private void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();

			ThreadPool.QueueUserWorkItem(new WaitCallback(SaveNewAccounts), mNewAccounts);

			mNewAccounts = null;
		}
		private void AccountItem_Click(object sender, MouseButtonEventArgs e)
		{
			SetupAccountItem item = sender as SetupAccountItem;
			item.Select();
		}
		private void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (IMProtocolExtraData extraData in e.NewItems)
				{
					if (mNewAccounts.Contains(extraData))
						continue;

					SetupAccountItem accItem = new SetupAccountItem();
					accItem.DataContext = extraData;
					accItem.Margin = (Thickness)FindResource("ItemMargin");
					AccountsListBox.Children.Add(accItem);
				}
			}

			if (e.OldItems != null)
			{
				foreach (IMProtocolExtraData extraData in e.OldItems)
				{
					foreach (UserControl element in AccountsListBox.Children)
					{
						if (element.DataContext == extraData)
						{
							FadeOut(element, new GenericEvent(() => AccountsListBox.Children.Remove(element)));
							break;
						}
					}
				}
			}
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (IMProtocolExtraData protocol in AccountManager.Accounts)
			{
				SetupAccountItem accItem = new SetupAccountItem();
				accItem.DataContext = protocol;
				accItem.Margin = (Thickness)FindResource("ItemMargin");
				AccountsListBox.Children.Add(accItem);
			}

			AccountManager.Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);
		}

		private static void SaveNewAccounts(object accounts)
		{
			IList<IMProtocolExtraData> mNewAccounts = (IList<IMProtocolExtraData>)accounts;

			foreach (var extraData in mNewAccounts)
			{
				IMSettings.Accounts.Add(extraData);
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
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			AccountManager.Accounts.CollectionChanged -= new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);
		}

		private List<IMProtocolExtraData> mNewAccounts;
	}
}