using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public class ComplexChatMessage
	{
		public ComplexChatMessage()
		{
			Inlines = new List<ChatInline>();
		}

		public List<ChatInline> Inlines
		{
			get;
			private set;
		}

		public override string ToString()
		{
			string result = String.Empty;

			foreach (ChatInline inline in Inlines)
				result += inline.ToString();

			return result;
		}
	}
}