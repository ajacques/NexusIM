using System.Windows;
using System.Windows.Input;
using InstantMessage;
using InstantMessage.Protocols;
using NexusIM.Controls;
using NexusIM.Managers;
using System.Linq;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for JoinChatRoom.xaml
	/// </summary>
	public partial class JoinChatRoom : Window
	{
		public JoinChatRoom()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
			AccountSelector.QueryClause = (extraData) => extraData.Enabled && extraData.Protocol is IHasMUCRooms && extraData.Protocol.ProtocolStatus == IMProtocolStatus.Online;

			if (AccountManager.Accounts.Any(AccountSelector.QueryClause))
			{
				NoAccountsMsg.Visibility = Visibility.Collapsed;
				MainGrid.Visibility = Visibility.Visible;
			} else {
				NoAccountsMsg.Visibility = Visibility.Visible;
				MainGrid.Visibility = Visibility.Collapsed;
			}
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			base.OnPreviewKeyUp(e);

			if (e.Key == Key.Enter)
				Button_Click(null, null);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			IMProtocolWrapper extraData = AccountSelector.SelectedProtocol;

			IHasMUCRooms muc = (IHasMUCRooms)extraData.Protocol;
			muc.JoinChatRoom(RoomName.Text);

			this.Close();
		}
		private void RoomName_KeyUp(object sender, KeyEventArgs e)
		{
			if (RoomName.Text.Length >= 1)
			{
				JoinButton.IsEnabled = true;

				IMProtocolWrapper wrapper = AccountSelector.SelectedProtocol;

				if (wrapper.Protocol is IHasMUCRooms)
				{

				}
			} else
				JoinButton.IsEnabled = false;
		}
	}
}