using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using NexusIM.Controls;
using System.Windows.Input;
using InstantMessage;
using NexusIM.Managers;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for frmMain.xaml
	/// </summary>
	public partial class ContactListWindow : Window
	{
		public ContactListWindow()
		{
			InitializeComponent();
		}

		public IList ContactList
		{
			get	{
				return ContactListControl.Children;
			}
		}

		private void DeselectAllExcept(UIElementCollection source, UIElement exception)
		{
			foreach (ContactListItem item in source)
			{
				if (item.Selected && item != exception)
					item.Deselect();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Left = SystemParameters.PrimaryScreenWidth - (double)GetValue(WidthProperty);
			Top = SystemParameters.PrimaryScreenHeight / 2 - ((double)GetValue(HeightProperty) / 2);
		}
		private void EditAccounts_Click(object sender, RoutedEventArgs e)
		{
			AccountsEdit window = new AccountsEdit();
			window.Show();
		}
		private void ContactListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ContactListItem item = sender as ContactListItem;
			IMBuddy contact = item.DataContext as IMBuddy;

			WindowSystem.OpenContactWindow(contact);
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.Source is ContactListItem)
			{
				ContactListItem acc = e.Source as ContactListItem;
				acc.Select();
				DeselectAllExcept(ContactListControl.Children, acc);
			} else {
				DeselectAllExcept(ContactListControl.Children, null);
			}
		}
	}
}