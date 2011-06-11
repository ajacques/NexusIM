using System.Windows;
using System.Windows.Input;
using InstantMessage;
using InstantMessage.Protocols;
using NexusIM.Controls;
using NexusIM.Managers;

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
			AccountSelector.QueryClause = (extraData) => extraData.Enabled && extraData.Protocol is IHasMUCRooms;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			IMProtocolWrapper extraData = AccountSelector.SelectedProtocol;

			IHasMUCRooms muc = (IHasMUCRooms)extraData.Protocol;
			IChatRoom chatRoom = muc.JoinChatRoom(RoomName.Text);

			WindowSystem.OpenGroupChatWindow(chatRoom);
		}

		private void RoomName_KeyUp(object sender, KeyEventArgs e)
		{
			if (RoomName.Text.Length >= 1)
				JoinButton.IsEnabled = true;
			else
				JoinButton.IsEnabled = false;
		}
	}
}