using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using System.Windows.Media;
#else
using System.Drawing;
#endif

namespace InstantMessage
{
	public class ChatInline 
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

	public class IMLineBreak : ChatInline
	{
		public override string ToString()
		{
			return Environment.NewLine;
		}
	}

	public class HyperlinkInline : ChatInline
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
		public IMRun(string p)
		{
			Body = p;
		}
		public IMRun()
		{
			
		}
		public string Body
		{
			get;
			set;
		}

		public override string ToString()
		{
			return Body;
		}
	}
}
