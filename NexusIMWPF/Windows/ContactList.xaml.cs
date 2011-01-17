using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

		public ListView ContactList
		{
			get	{
				return listView1;
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
	}
}
