using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusIM.Windows;
using InstantMessage;
using NexusIM.Controls;

namespace NexusIM.Managers
{
	class ChatAreaCollection : Dictionary<IContact, ContactChatArea>
	{
		public ContactChatArea this[string username]
		{
			get {
				ContactChatArea area = this.FirstOrDefault(s => s.Key.Username == username).Value;
				if (area == null)
					throw new KeyNotFoundException();

				return area;
			}
		}
	}
}