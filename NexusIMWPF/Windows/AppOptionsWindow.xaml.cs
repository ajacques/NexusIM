using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for AppOptionsWindow.xaml
	/// </summary>
	public partial class AppOptionsWindow : Window
	{
		public AppOptionsWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			DateTime now = DateTime.Now;
			string[] formats = new string[] { "h:mm", "t", "T", "h:mm:ss.ff tt", "h:mm:ss.ffff tt" };

			foreach (string format in formats)
				ChatMsgStampFormat.Items.Add(now.ToString(format, CultureInfo.InstalledUICulture));
		}
	}
}