using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using InstantMessage.Protocols;

namespace NexusIM.Controls
{
	class ChatRoomAreaHost : TabItem
	{
		public ChatRoomAreaHost(IChatRoom chatRoom)
		{
			Content = mArea = new MUCChatArea();
			mArea.PopulateUIControls(chatRoom);

			Grid g = new Grid();
			Header = g;
			g.Children.Add(new TextBlock() { Text = "Text" });
			//Header = chatRoom.Name;
		}

		private MUCChatArea mArea;
	}
}