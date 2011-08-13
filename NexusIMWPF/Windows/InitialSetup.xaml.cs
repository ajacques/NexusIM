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
using NexusIM;
using NexusIM.Managers;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for InitialSetup.xaml
	/// </summary>
	partial class InitialSetupWindow : Window
	{
		public InitialSetupWindow()
		{
			this.InitializeComponent();

			NexusCoreManager.OnStateChange += new EventHandler<NexusCoreStateEventArgs>(NexusCoreManager_onLogin);
		}

		private void NexusCoreManager_onLogin(object sender, NexusCoreStateEventArgs e)
		{
			switch (e.CurrentState)
			{
				case NexusCoreState.Synchronizing:
					Storyboard Step2AnimIn = FindResource("Step2AnimIn") as Storyboard;
					Step2AnimIn.Dispatcher.BeginInvoke(new Action(() => Step2AnimIn.Begin()), DispatcherPriority.Normal);
					break;
				case NexusCoreState.Online:
					WindowSystem.ShowSysTrayIcon();
					WindowSystem.OpenContactListWindow();
					Dispatcher.BeginInvoke(new Action(() => this.Close()));
					break;
			}
		}
		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			string username = UsernameBox.Text;
			string password = PasswordBox.Password;

			// We invoke Login() using an async delegate because without it, there is a delay of about 3 seconds somewhere in the Login() method that causes the UI to stop responding until it's over
			GenericEvent d = new GenericEvent(() => NexusCoreManager.Login(username, password));
			d.BeginInvoke(null, null);
		}
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Shutdown();
		}
		private void SkipLogin_Click(object sender, RoutedEventArgs e)
		{
			WindowSystem.ShowSysTrayIcon();
			WindowSystem.OpenContactListWindow();
			
			AccountsEdit accountEdit = new AccountsEdit();
			accountEdit.Show();

			this.Close();
		}
		private void SignUpLink_Click(object sender, RoutedEventArgs e)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = "http://dev.nexus-im.com/";
			Process.Start(startInfo);
		}
	}
}