using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using InstantMessage;
using InstantMessage.Protocols.Irc;
using System.Threading;

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
		}

		public void PopulateUIControls(IMProtocolWrapper extraData)
		{
			mExtraData = extraData;
			IRCProtocol protocol = (IRCProtocol)extraData.Protocol;

			IDictionary<string, string> configTable = protocol.ConfigurationSettings;

			string autoexecute;
			configTable.TryGetValue("autoexecute", out autoexecute);

			Dispatcher.BeginInvoke(new GenericEvent(() => {
				HeaderAccountUsername.Text = extraData.Protocol.Username;
				AutoExecuteBox.Text = autoexecute;

				if (mExtraData.Enabled)
					ConnectedWarning.Visibility = Visibility.Visible;

				Hostname.Text = protocol.Server;
				Port.Text = protocol.Port.ToString();
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