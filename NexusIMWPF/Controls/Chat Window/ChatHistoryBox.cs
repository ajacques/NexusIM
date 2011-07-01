using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using InstantMessage;

namespace NexusIM.Controls
{
	class ChatHistoryBox : RichTextBox
	{
		public ChatHistoryBox()
		{
			this.IsReadOnly = true;
			this.IsDocumentEnabled = true;
			this.Background = null;
			this.BorderThickness = new Thickness(0);

			// Setup Document inlines
			FlowDocument doc = new FlowDocument();
			mInlines = new Paragraph();
			doc.Blocks.Add(mInlines);

			this.Document = doc;
		}

		public void AttachToProtocol(IMProtocolWrapper protocol)
		{
			
		}

		public void AppendInline(Inline inline)
		{
			if (mInlines.Inlines.Count >= 1)
				mInlines.Inlines.Add(new LineBreak());

			mInlines.Inlines.Add(inline);

			this.ScrollToEnd();
		}
		public void RemoveLast()
		{
			mInlines.Inlines.Remove(mInlines.Inlines.LastInline);
		}

		public void Clear()
		{
			mInlines.Inlines.Clear();
		}

		private Paragraph mInlines;
	}
}