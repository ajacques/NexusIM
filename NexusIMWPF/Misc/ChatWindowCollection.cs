using System;
using System.Collections.Generic;
using NexusIM.Windows;
using System.Linq;

namespace NexusIM.Misc
{
	class ChatWindowCollection : SortedDictionary<int, ChatWindow>, IEnumerable<KeyValuePair<int, ChatWindow>>
	{
		public ChatWindowCollection()
		{
			mUnboundWindows = new LinkedList<KeyValuePair<int, ChatWindow>>();
		}

		public new void Add(int key, ChatWindow value)
		{
			if (key >= 0)
				base.Add(key, value);
			else
				mUnboundWindows.AddFirst(new KeyValuePair<int, ChatWindow>(key, value));
		}

		public new IEnumerator<KeyValuePair<int, ChatWindow>> GetEnumerator()
		{
			return Enumerable.Concat(this, mUnboundWindows).GetEnumerator();
		}

		private LinkedList<KeyValuePair<int, ChatWindow>> mUnboundWindows;
	}
}