using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace NexusIM.Controls
{
	public class ChatAreaHost : TabItem
	{
		public ChatAreaHost(ContactChatArea area, object dataContext)
		{
			Content = area;
			DataContext = dataContext;
			Binding displayNameBinding = new Binding("DisplayName");
			displayNameBinding.Source = dataContext;
			SetBinding(TabItem.HeaderProperty, displayNameBinding);
		}
	}
}
