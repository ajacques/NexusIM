using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace NexusPhone.UserInterface
{
	partial class ContactListItem : UserControl
	{
		public ContactListItem()
		{
			InitializeComponent();
		}
		internal ContactListItem(IMBuddy contact)
		{
			mBuddy = contact;

			InitializeComponent();
			DataContext = contact;
			contact.Dispatcher = Dispatcher;
			//AnimIn.Begin();
		}

		internal IMBuddy Contact
		{
			get	{
				return mBuddy;
			}
		}

		private IMBuddy mBuddy;

		private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			mBuddy.ShowWindow();
		}
		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			//mBuddy.ShowWindow();
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			
		}
	}
}