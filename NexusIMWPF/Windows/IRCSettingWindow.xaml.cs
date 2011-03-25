using System;
using System.Windows;
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

		public void PopulateUIControls(IMProtocolExtraData extraData)
		{
			HeaderAccountUsername.Text = extraData.Protocol.Username;
		}
	}
}