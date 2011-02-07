using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace InstantMessage
{
	public interface ChatInline {}

	public class TextChatInline : ChatInline
	{
		public double FontSize
		{
			get;
			set;
		}
		public string FontFamily
		{
			get;
			set;
		}
		public Color Foreground
		{
			get;
			set;
		}
	}

	public class LineBreak : TextChatInline
	{
		public override string ToString()
		{
			return Environment.NewLine;
		}
	}

	public class HyperlinkInline : TextChatInline
	{
		public HyperlinkInline(Uri navigateUrl, string body)
		{
			NavigateUri = navigateUrl;
			Body = body;
		}

		public Uri NavigateUri
		{
			get;
			private set;
		}
		public string Body
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return String.Format("{0} ({1})", Body, NavigateUri);
		}
	}

	public class IMRun : ChatInline
	{
		public IMRun(string body)
		{
			Body = body;
		}

		public string Body
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return Body;
		}
	}
}
