using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using InstantMessage;
using NexusIM.Managers;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for SelectRecipientWindow.xaml
	/// </summary>
	public partial class SelectRecipientWindow : Window
	{
		public SelectRecipientWindow()
		{
			this.InitializeComponent();
		}

		private void SearchThread(object searchText)
		{
			string name = searchText.ToString();
			if (String.IsNullOrWhiteSpace(name))
				return;

			string search = name.ToUpperInvariant();

			IEnumerable<IContact> contacts = AccountManager.Accounts.Where(p => p.Enabled && p.Protocol.ProtocolStatus == IMProtocolStatus.Online).SelectMany(s => s.Protocol.ContactList.Values);

			IEnumerable<IContact> results = contacts.Where(i =>
				(!String.IsNullOrEmpty(i.Nickname) &&
					(i.Nickname.ToUpperInvariant().StartsWith(search) || i.Nickname.ToUpperInvariant().Split(' ').Any(s => s.StartsWith(search)))
				) || i.Username.ToUpperInvariant().StartsWith(search));
			Brush subtleTextBrush = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			Brush heavyTextBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

			SuggestResults.Children.Clear();

			foreach (IMBuddy contact in results)
			{
				Grid container = new Grid();
				TextBlock textblock = new TextBlock();
				Run Nickname = new Run();
				Run Username = new Run();

				Nickname.Text = contact.Nickname;
				Username.Text = String.Format("({0})", contact.Username);

				int nickFind = contact.Nickname.ToUpperInvariant().IndexOf(search);
				int UserFind = contact.Username.ToUpperInvariant().IndexOf(search);

				if (nickFind != -1)
					Nickname.TextEffects.Add(new TextEffect() { Foreground = heavyTextBrush, PositionStart = nickFind, PositionCount = search.Length + 1 });
				if (UserFind != -1)
					Username.TextEffects.Add(new TextEffect() { Foreground = heavyTextBrush, PositionStart = UserFind, PositionCount = search.Length + 1 });

				if (!String.IsNullOrEmpty(contact.Nickname))
				{
					textblock.Inlines.Add(Nickname);
					textblock.Inlines.Add(new Run(" "));
					Username.Foreground = subtleTextBrush;
				}

				Line sep = new Line();
				sep.X2 = 260;
				sep.VerticalAlignment = VerticalAlignment.Bottom;
				sep.Stroke = (Brush)FindResource("SuggestionSeparator");

				textblock.Margin = new Thickness(5);

				textblock.Inlines.Add(Username);
				container.Children.Add(textblock);

				SuggestResults.Children.Add(container);
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Stopwatch searchThread = new Stopwatch();
			TextBox box = (TextBox)sender;
			searchThread = new Stopwatch();
			searchThread.Start();
			SearchThread(box.Text);

			Trace.WriteLine("Full ContactList search took " + searchThread.Elapsed.ToString());
			searchThread.Stop();
		}
	}
}