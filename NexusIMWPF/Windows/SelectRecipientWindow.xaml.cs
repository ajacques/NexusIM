using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InstantMessage;
using NexusIM.Managers;
using System.Windows.Documents;
using System.Windows.Media;

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

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox name = sender as TextBox;

			SuggestResults.Children.Clear();
			if (String.IsNullOrWhiteSpace(name.Text))
				return;

			string search = name.Text.ToUpperInvariant();

			IEnumerable<IMBuddy> contacts = AccountManager.Accounts.Where(p => p.Enabled && p.Protocol.ProtocolStatus == IMProtocolStatus.Online).SelectMany(s => s.Protocol.ContactList);

			IEnumerable<IMBuddy> results = contacts.Where(i => 
				(!String.IsNullOrEmpty(i.Nickname) &&
					(i.Nickname.ToUpperInvariant().StartsWith(search) || i.Nickname.ToUpperInvariant().Split(' ').Any(s => s.StartsWith(search)))
				) || i.Username.ToUpperInvariant().StartsWith(search));
			Brush subtleTextBrush = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			Brush heavyTextBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

			foreach (IMBuddy contact in results)
			{
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

				textblock.Inlines.Add(Username);

				SuggestResults.Children.Add(textblock);
			}
		}
	}
}