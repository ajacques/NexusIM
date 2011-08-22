using System.Windows;
using System;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			CloseButton.Focus();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
