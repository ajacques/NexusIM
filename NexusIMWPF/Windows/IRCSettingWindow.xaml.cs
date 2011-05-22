using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using InstantMessage;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for IRCSettingWindow.xaml
	/// </summary>
	public partial class IRCSettingWindow : Window
	{
		public IRCSettingWindow()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

		public void PopulateUIControls(IMProtocolWrapper extraData)
		{
			mExtraData = extraData;
			HeaderAccountUsername.Text = extraData.Protocol.Username;

			IDictionary<string, string> configTable = mExtraData.Protocol.ConfigurationSettings;

			string autoexecute;
			configTable.TryGetValue("autoexecute", out autoexecute);

			Dispatcher.BeginInvoke(new GenericEvent(() => {
				AutoExecuteBox.Text = autoexecute;

				if (mExtraData.Enabled)
					ConnectedWarning.Visibility = Visibility.Visible;
			}));
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			Reconcile("autoexecute", AutoExecuteBox.Text);

			this.Close();
		}

		private void Reconcile(string key, string value)
		{
			IDictionary<string, string> configTable = mExtraData.Protocol.ConfigurationSettings;

			if (String.IsNullOrEmpty(value))
				configTable.Remove(key);
			else
				configTable[key] = value;
		}

		private IMProtocolWrapper mExtraData;
	}
}