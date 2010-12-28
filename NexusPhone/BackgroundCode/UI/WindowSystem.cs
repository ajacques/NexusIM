using System;
using NexusPhone.UserInterface;
using System.Windows.Navigation;
using System.Windows;

namespace NexusPhone
{
	/// <summary>
	/// Wraps 
	/// </summary>
	internal static class WindowSystem
	{
		public static void OpenLoginWindow()
		{
			CurrentApp.RootFrame.Navigate(new Uri("/UserInterface/LoginWindow.xaml", UriKind.Relative));
		}
		public static void OpenMainWindow()
		{
			CurrentApp.RootFrame.Navigate(new Uri("/UserInterface/MainPage.xaml", UriKind.Relative));
		}
		public static void OpenAccountsWindow()
		{
			//CurrentApp.RootFrame.Content = new AccountWindow();
			CurrentApp.RootFrame.Navigate(new Uri("/UserInterface/Windows/AccountWindow.xaml", UriKind.Relative));
		}
		public static void OpenChatWindow(IMBuddy contact)
		{
			//CurrentApp.RootFrame.Content = new AccountWindow();
			mChatWindowBuddy = contact;

			//CurrentApp.RootFrame.Navigating += new NavigatingCancelEventHandler(NavigationService_Navigating);
			CurrentApp.RootFrame.Navigated += new NavigatedEventHandler(NavigationService_Navigated);
			CurrentApp.RootFrame.Navigate(new Uri("/UserInterface/Windows/ChatWindow.xaml", UriKind.Relative));
		}

		private static void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			CurrentApp.RootFrame.Navigating -= new NavigatingCancelEventHandler(NavigationService_Navigating);
		}
		private static void NavigationService_Navigated(object sender, NavigationEventArgs e)
		{
			CurrentApp.RootFrame.Navigated -= new NavigatedEventHandler(NavigationService_Navigated);
			ChatWindow window = e.Content as ChatWindow;
			window.Contact = mChatWindowBuddy;
			mChatWindowBuddy = null;
		}

		// Properties
		public static App CurrentApp
		{
			get {
				return App.Current as App;
			}
		}
		public static Uri CurrentUri
		{
			get	{
				return CurrentApp.RootFrame.CurrentSource;
			}
		}

		// Variables
		private static IMBuddy mChatWindowBuddy;
	}
}