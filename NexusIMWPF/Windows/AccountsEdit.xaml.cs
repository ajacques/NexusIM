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
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media.Animation;

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
				default:
					return;
			}

			Trace.WriteLine("User is adding new account of type " + protocol.Protocol);

			SetupAccountItem item = new SetupAccountItem();

			item.DataContext = new IMProtocolExtraData() { Protocol = protocol, Enabled = true };
			item.Margin = (Thickness)FindResource("ItemMargin");

			AccountsListBox.Children.Add(item);
			DeselectAllExcept(AccountsListBox.Children, item);
			item.Select();
		}
		private void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
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
					SetupAccountItem accItem = new SetupAccountItem();
					accItem.DataContext = extraData;
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
				AccountsListBox.Children.Add(accItem);
			}

			AccountManager.Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);
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
	}
}