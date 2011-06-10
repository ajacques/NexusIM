using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NexusIM.Windows;

namespace NexusIM.Misc
{
	class ChatWindowCollection : Dictionary<int, ChatWindow>
	{
		public ChatWindowCollection()
		{
			mUnboundWindows = new LinkedList<Tuple<int, ChatWindow>>();
		}

		public new void Add(int key, ChatWindow value)
		{
			if (key >= 0)
				base.Add(key, value);
			else
				mUnboundWindows.AddFirst(Tuple.Create(key, value));
		}

		private LinkedList<Tuple<int, ChatWindow>> mUnboundWindows;
	}
}