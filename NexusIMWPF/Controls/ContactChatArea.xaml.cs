using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using InstantMessage;
using InstantMessage.Events;
using NexusIM.Managers;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for ContactChatArea.xaml
	/// </summary>
	public partial class ContactChatArea : UserControl, ITabbedArea
	{
		public ContactChatArea()
		{
			this.InitializeComponent();
			this.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
		}

		public void PopulateUIControls(object context)
		{
			DataContext = context;
		}
		public void ProcessChatMessage(IMMessageEventArgs e)
		{
			ProcessMessageImp(e.Sender.Nickname, Color.FromRgb(0, 0, 255), e.ComplexMessage.Inlines);
		}
		private void ProcessMessageImp(string author, Color authcolor, IEnumerable<ChatInline> complexmsg)
		{
			Dispatcher.BeginInvoke(new GenericEvent(() =>
			{
				ChatMessageInline inline = new ChatMessageInline();
				inline.Username = author;
				inline.UsernameColor = authcolor;
				foreach (ChatInline ninline in complexmsg)
				{
					if (ninline is IMLineBreak)
						inline.Inlines.Add(new LineBreak());
					else if (ninline is IMRun)
						inline.Inlines.Add(new Run(ninline.ToString()));
					else if (ninline is HyperlinkInline) {
						HyperlinkInline href = (HyperlinkInline)ninline;
						Hyperlink nhref = new Hyperlink();
						nhref.NavigateUri = href.NavigateUri;
						nhref.Inlines.Add(new Run(href.Body));
						nhref.Cursor = Cursors.Hand;
						nhref.Click += new RoutedEventHandler(IMHyperlink_Click);
						nhref.MouseEnter += new MouseEventHandler(IMHyperlink_MouseEnter);
						nhref.MouseLeave += new MouseEventHandler(IMHyperlink_MouseLeave);

						inline.Inlines.Add(nhref);
					}
				}
				//inline.MessageBody = e.Message;

				if (ChatHistoryBox.Inlines.Any())
					ChatHistoryBox.Inlines.Add(new LineBreak());

				ChatHistoryBox.Inlines.Add(inline);
				ChatHistoryContainer.ScrollToEnd();
			}));
		}
		private static TextBlock FormatNiceUri(Uri input)
		{
			TextBlock block = new TextBlock();
			Run schemeRun = new Run(input.Scheme);
			if (input.Scheme == "https")
				schemeRun.Foreground = new SolidColorBrush(Colors.DarkGreen);
			block.Inlines.Add(schemeRun);
			block.Inlines.Add(new Run("://"));

			Run hostRun = new Run();
			hostRun.FontWeight = FontWeight.FromOpenTypeWeight(700);
			if (!input.Host.Contains('.') || input.Host.Count(c => c == '.') == 1)
				hostRun.Text = input.Host;
			else {
				int sldpoint = input.Host.LastIndexOf('.', input.Host.Length - 5);
				string sld = input.Host.Substring(sldpoint + 1);

				if (sld.StartsWith("co."))
				{
					sldpoint = input.Host.LastIndexOf('.', sldpoint - 1);
					sld = input.Host.Substring(sldpoint + 1);
				}

				block.Inlines.Add(new Run(input.Host.Substring(0, sldpoint + 1)));

				hostRun.Text = sld;
			}
			
			block.Inlines.Add(hostRun);
			block.Inlines.Add(new Run(input.PathAndQuery));

			return block;
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			MessageBody.Focus();
		}

		// Event Handlers
		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null)
				return;

			Contact.onReceiveMessage += new EventHandler<IMMessageEventArgs>(OnReceiveMessage);
			Contact.PropertyChanged += new PropertyChangedEventHandler(Contact_PropertyChanged);
		}
		private void OnReceiveMessage(object sender, IMMessageEventArgs e)
		{
			ProcessChatMessage(e);		
		}
		private void MessageBody_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;

				string message = MessageBody.Text;
				MessageBody.Text = String.Empty;

				IEnumerable<ChatInline> inlines = new ChatInline[] { new IMRun(message) };
				inlines = IMMessageProcessor.ProcessComplexMessage(inlines);

				Contact.SendMessage(message);
				ProcessMessageImp("Me", Color.FromRgb(255, 100, 0), inlines);
				
				MessageLogger.LogMessageToRemote(Contact, message);
			}
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (Contact.Status == IMBuddyStatus.Offline)
				UserOfflineWarning.Visibility = Visibility.Visible;

			if (Contact.Protocol.Status == IMStatus.Invisible)
				SelfInvisibleWarning.Visibility = Visibility.Visible;
		}
		private void AppearOnline_Click(object sender, RoutedEventArgs e) {}
		private void AppearOnlineToAll_Click(object sender, RoutedEventArgs e)
		{
			AccountManager.Status = IMStatus.Available;
			SelfInvisibleWarning.Visibility = Visibility.Collapsed;
		}
		private void Contact_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Avatar")
			{

			}
		}
		private void IMHyperlink_Click(object sender, RoutedEventArgs e)
		{
			Hyperlink href = (Hyperlink)sender;
		}
		private void IMHyperlink_MouseEnter(object sender, RoutedEventArgs e)
		{
			Hyperlink href = (Hyperlink)sender;

			if (href.NavigateUri.Host == "www.youtube.com")
			{
				ChatHistoryContainer.ToolTip = null;
				Match output = mYoutubeLinkMatch.Match(href.NavigateUri.PathAndQuery);

				if (output.Success)
				{
					string videoId = output.Groups[1].Value;
					YoutubePreviewPopup ytcontent = new YoutubePreviewPopup();
					ytcontent.PopulateUIControls(videoId);
					LinkPreviewPopup.Child = ytcontent;
					LinkPreviewPopup.Placement = PlacementMode.MousePoint;
					LinkPreviewPopup.IsOpen = true;
				}
			} else {
				ToolTip tip = new ToolTip();
				StackPanel panel = new StackPanel();
				tip.Content = panel;
				panel.Children.Add(FormatNiceUri(href.NavigateUri));

				ChatHistoryContainer.ToolTip = tip;
			}
		}
		private void IMHyperlink_MouseLeave(object sender, MouseEventArgs e)
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
		}
		private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			MessageBody.Focus();
		}

		private IMBuddy Contact
		{
			get {
				return DataContext as IMBuddy;
			}
		}

		private Regex mYoutubeLinkMatch = new Regex(@"^/watch\?v=([a-zA-Z0-9_-]*)");
	}
}