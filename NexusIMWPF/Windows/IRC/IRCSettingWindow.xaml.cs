using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using InstantMessage;
using InstantMessage.Protocols.Irc;

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
			mProtocol = (IRCProtocol)extraData.Protocol;

			IDictionary<string, string> configTable = mProtocol.ConfigurationSettings;

			string autoexecute;
			configTable.TryGetValue("autoexecute", out autoexecute);

			Dispatcher.BeginInvoke(new GenericEvent(() => {
				HeaderAccountUsername.Text = mProtocol.Nickname;
				AutoExecuteBox.Text = autoexecute;

				if (mExtraData.Enabled)
				{
					ConnectedWarning.Visibility = Visibility.Visible;
					ContainerGrid.Margin = new Thickness(0, 35, 0, 0);
				}

				Hostname.Text = mProtocol.Server;
				Port.Text = mProtocol.Port.ToString();
				UseSsl.IsChecked = mProtocol.SslEnabled;

				UsernameBox.Text = mProtocol.Username;
				RealNameBox.Text = mProtocol.RealName;
			}));
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			Reconcile("autoexecute", AutoExecuteBox.Text);
			Reconcile("realname", RealNameBox.Text);

			mProtocol.Server = Hostname.Text;
			mProtocol.Username = UsernameBox.Text;
			mProtocol.RealName = RealNameBox.Text;
			mProtocol.SslEnabled = UseSsl.IsChecked.Value;
			
			int port;
			if (Int32.TryParse(Port.Text, out port))
				mProtocol.Port = port;

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
		private IRCProtocol mProtocol;
	}
}