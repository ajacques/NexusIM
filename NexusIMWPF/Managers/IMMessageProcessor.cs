using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using InstantMessage;
using InstantMessage.Events;

namespace NexusIM.Managers
{
	class IMMessageProcessor
	{
		public static void Setup()
		{
			AccountManager.Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);
		}

		private static void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (IMProtocolWrapper extraData in e.NewItems)
				{
					extraData.Protocol.onMessageReceive += new EventHandler<IMMessageEventArgs>(IMProtocol_OnMessageReceived);
				}
			}
		}
		private static void IMProtocol_OnMessageReceived(object sender, IMMessageEventArgs e)
		{

		}

		public static Inline ProcessMessage(string message, MouseButtonEventHandler hyperlinkMouseClick = null, MouseEventHandler hyperlinkMouseEnter = null, MouseEventHandler hyperlinkMouseLeave = null)
		{
			message = message.TrimEnd(' ', '\t');

			Span result = new Span();
			int index = message.IndexOf("http://");

			index = index == -1 ? message.IndexOf("https://") : index;
			index = index == -1 ? message.IndexOf("ftp://") : index;

			if (index != -1)
			{
				int endIndex = message.IndexOf(' ', index);
				string trailing = endIndex != -1 ? message.Substring(endIndex) : null;

				endIndex = endIndex != -1 ? endIndex : message.Length;

				string hyperlink = message.Substring(index, endIndex - index);

				Uri href = null;
				try	{
					href = new Uri(hyperlink);
				} catch (UriFormatException) {
					Debug.WriteLine("Uri " + hyperlink + " failed to parse due to invalid format");
					result.Inlines.Add(new Run(hyperlink));
				}

				message = message.Substring(0, index);

				Hyperlink hinline = new Hyperlink();
				hinline.NavigateUri = href;
				hinline.Inlines.Add(new Run(hyperlink));
				if (hyperlinkMouseEnter != null)
					hinline.MouseEnter += hyperlinkMouseEnter;
				if (hyperlinkMouseLeave != null)
					hinline.MouseLeave += hyperlinkMouseLeave;
				if (hyperlinkMouseClick != null)
					hinline.MouseUp += hyperlinkMouseClick;

				result.Inlines.Add(hinline);
			} else
				result.Inlines.Add(message);

			return result;
		}
		public static IEnumerable<ChatInline> ProcessComplexMessage(IEnumerable<ChatInline> source)
		{
			List<ChatInline> processed = new List<ChatInline>();
			foreach (ChatInline inline in source)
			{
				if (!(inline is IMRun))
				{
					processed.Add(inline);
					continue;
				}

				IMRun run = (IMRun)inline;
				int index = run.Body.IndexOf("http://");

				index = index == -1 ? run.Body.IndexOf("https://") : index;
				index = index == -1 ? run.Body.IndexOf("ftp://") : index;

				if (index != -1)
				{
					int endIndex = run.Body.IndexOf(' ', index);
					string trailing = endIndex != -1 ? run.Body.Substring(endIndex) : null;

					endIndex = endIndex != -1 ? endIndex : run.Body.Length;

					string hyperlink = run.Body.Substring(index, endIndex - index);

					Uri href;
					try	{
						href = new Uri(hyperlink);
					} catch (UriFormatException) {
						Debug.WriteLine("Uri " + hyperlink + " failed to parse due to invalid format");
						processed.Add(run);
						continue;
					}

					run.Body = run.Body.Substring(0, index);

					HyperlinkInline hinline = new HyperlinkInline(new Uri(hyperlink), hyperlink);
					processed.Add(hinline);
				} else
					processed.Add(run);
			}

			return processed;
		}
	}
}