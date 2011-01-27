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

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for ChatWindow.xaml
	/// </summary>
	public partial class ChatWindow : Window
	{
		public ChatWindow()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

		private void MessageBody_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;

				
			}
		}
	}
}