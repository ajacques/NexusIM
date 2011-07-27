using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using NexusIM.Controls;
using NexusIM.Misc;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for AppOptionsWindow.xaml
	/// </summary>
	public partial class AppOptionsWindow : Window
	{
		public AppOptionsWindow()
		{
			dateTimeFormats = new string[] { "h:mm", "t", "T", "h:mm:ss.ff tt", "h:mm:ss.ffff tt" };

			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			string curFormat = SettingCache.GetValue("ChatMsgTimestampFormat");

			DateTime now = DateTime.Now;

			int index = 0;
			foreach (string format in dateTimeFormats)
			{
				ChatMsgStampFormat.Items.Add(now.ToString(format, CultureInfo.InstalledUICulture));

				if (curFormat == format)
					ChatMsgStampFormat.SelectedIndex = index + 1;
				index++;
			}

			if (ChatMsgStampFormat.SelectedIndex == -1)
			{
				CustomMsgStamp.Text = curFormat;
				ChatMsgStampFormat.SelectedIndex = 0;
			}

			Placeholder.SetText(CustomMsgStamp, "Custom format");
		}

		private void CustomMsgStampHelp_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx");
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (ChatMsgStampFormat.SelectedIndex == 0) // Custom format
				SettingCache.SetValue("ChatMsgTimestampFormat", CustomMsgStamp.Text);
			else
				SettingCache.SetValue("ChatMsgTimestampFormat", dateTimeFormats[ChatMsgStampFormat.SelectedIndex - 1]);

			this.Close();
		}
		private void CustomMsgStamp_GotFocus(object sender, RoutedEventArgs e)
		{
			ChatMsgStampFormat.SelectedIndex = 0;
		}

		private string[] dateTimeFormats;
	}
}