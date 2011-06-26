using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NexusIM.Controls
{
	class ChatHistoryBox : RichTextBox
	{
		public ChatHistoryBox()
		{
			FlowDocument doc = new FlowDocument();

			mInlines = new Paragraph();

			doc.Blocks.Add(mInlines);
		}

		public void AppendInline(Inline inline)
		{
		}

		private Paragraph mInlines;
	}
}