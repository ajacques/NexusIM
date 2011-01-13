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
using NexusIMWPF;

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
		}

		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			
		}
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Shutdown();
		}

		private void AddAccount_Click(object sender, SelectionChangedEventArgs e)
		{
			switch (addAccountCombo.SelectedIndex)
			{
				case 1: // Yahoo! IM
					
					break;
			}
		}
	}
}