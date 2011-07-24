using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using InstantMessage;
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace NexusIM.Controls
{
	class ChatHistoryBox : RichTextBox, IAddChild
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
			LinkPreviewPopup = new Popup();
			LinkPreviewPopup.IsOpen = false;
			LinkPreviewPopup.VerticalOffset = 10;
			LinkPreviewPopup.AllowsTransparency = true;
		}

		public void AddChild(object value)
		{
			if (value.GetType() != typeof(Inline))
				throw new ArgumentException("Argument (value) must be of type Inline");

			mInlines.Inlines.Add((Inline)value);
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
		public void IMHyperlink_MouseEnter(object sender, RoutedEventArgs e)
		{
			Hyperlink href = (Hyperlink)sender;

			if (href.NavigateUri.Host == "www.youtube.com")
			{
				ToolTip = null;
				Match output = mYoutubeLinkMatch.Match(href.NavigateUri.PathAndQuery);

				if (output.Success)
				{
					string videoId = output.Groups[1].Value;
					YoutubePreviewPopup ytcontent = new YoutubePreviewPopup();
					ytcontent.LoadVideoMetadata(videoId);
					LinkPreviewPopup.Child = ytcontent;
					LinkPreviewPopup.Placement = PlacementMode.MousePoint;
					LinkPreviewPopup.IsOpen = true;
				}

				mLinkLinkPopup = true;
			} else {
				ToolTip tip = new ToolTip();
				StackPanel panel = new StackPanel();
				tip.Content = panel;
				//panel.Children.Add(FormatNiceUri(href.NavigateUri));

				ToolTip = tip;
			}
		}
		public void IMHyperlink_MouseLeave(object sender, MouseEventArgs e)
		{
			if (mLinkLinkPopup)
			{
				Storyboard fadeOut = new Storyboard();
				DoubleAnimation anim1 = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(1)));
				anim1.EasingFunction = new QuinticEase();
				anim1.SetValue(Storyboard.TargetProperty, LinkPreviewPopup.Child);
				anim1.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(FrameworkElement.OpacityProperty));
				fadeOut.Children.Add(anim1);

				EventHandler onComplete = null;
				onComplete = new EventHandler((send, args) => {
					fadeOut.Completed -= onComplete;
				});
				fadeOut.Completed += onComplete;
				fadeOut.Begin();
				mLinkLinkPopup = false;
			}			
		}

		private Paragraph mInlines;
		private bool mLinkLinkPopup;
		private Popup LinkPreviewPopup;
		private static Regex mYoutubeLinkMatch = new Regex(@"^/watch\?v=([a-zA-Z0-9_-]*)");
	}
}