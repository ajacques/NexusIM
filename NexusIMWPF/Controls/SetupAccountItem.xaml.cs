using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using InstantMessage;
using InstantMessage.Protocols;
using InstantMessage.Protocols.Irc;
using NexusIM.Managers;
using NexusIM.Windows;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for SetupAccountItem.xaml
	/// </summary>
	public partial class SetupAccountItem : UserControl
	{
		public SetupAccountItem()
		{
			this.InitializeComponent();

			EnabledCheckBox.Checked += new RoutedEventHandler(EnabledCheckBox_Checked);
			EnabledCheckBox.Unchecked += new RoutedEventHandler(EnabledCheckBox_Checked);
			PasswordBox.PasswordChanged += new RoutedEventHandler(PasswordBox_PasswordChanged);
			ServerBox.TextChanged += new TextChangedEventHandler(ServerBox_TextChanged);
			ServerBox.LostFocus += new RoutedEventHandler(ServerBox_LostFocus);
			AutoConnectCheckbox.Checked += new RoutedEventHandler(AutoConnectCheckbox_CheckChanged);
			AutoConnectCheckbox.Unchecked += new RoutedEventHandler(AutoConnectCheckbox_CheckChanged);
		}

		public void Select()
		{
			Storyboard AnimFade = FindResource("EditFadeIn") as Storyboard;

			AnimFade.Begin();
			Selected = true;
		}
		public void Deselect()
		{
			Storyboard AnimFade = FindResource("EditFadeOut") as Storyboard;

			AnimFade.Begin();
			Selected = false;

			ApplyChanges();
			UpdateControl();

			if (!mProtocol.IsReady)
			{
				if (!String.IsNullOrEmpty(mProtocol.Protocol.Username))
				{
					AccountManager.Accounts.Add(mProtocol);
					IMSettings.Accounts.Add(mProtocol);
				}
			}
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			Placeholder.SetText(UsernameBox, "Username");
			
			//Placeholder.SetText(PasswordBox, "Password");
		}

		public void PopulateUIControls(IMProtocolWrapper extraData)
		{
			mProtocol = extraData;
			mProtocolType = mProtocol.Protocol.GetType();

			BuildControl();
			UpdateControl();
		}
		public void ApplyChanges()
		{
			if (!mProtocol.Enabled)
			{
				string password = PasswordBox.Password;
				if (!String.IsNullOrEmpty(password))
					mProtocol.Protocol.Password = password;

				if (mProtocolType == typeof(IRCProtocol))
				{
					IRCProtocol ircprot = (IRCProtocol)mProtocol.Protocol;
					ircprot.Server = ServerBox.Text;
					ircprot.Nickname = nicknameTextBox.Text;
					
					if (String.IsNullOrEmpty(ircprot.Username))
						ircprot.Username = UsernameBox.Text;
				} else
					mProtocol.Protocol.Username = UsernameBox.Text;
			}
		}

		private void AdjustControlHeight(int delta)
		{
			Storyboard fadeIn = (Storyboard)FindResource("EditFadeIn");
			DoubleAnimation height = (DoubleAnimation)fadeIn.Children[0];
			height.To += delta;

			Storyboard fadeOut = (Storyboard)FindResource("EditFadeOut");
			height = (DoubleAnimation)fadeIn.Children[0];
			height.From += delta;
		}
		private void UpdateControl()
		{
			if (mProtocolType == typeof(IRCProtocol))
			{
				IRCProtocol ircprot = (IRCProtocol)mProtocol.Protocol;
				UsernameBox.Text = ircprot.Username;
				nicknameTextBox.Text = ircprot.Nickname;
			} else
				UsernameBox.Text = mProtocol.Protocol.Username;

			// Setup stuff
			AutoConnectCheckbox.IsChecked = mProtocol.AutoConnect;
			MainAccountUsername.Text = mProtocol.Protocol.Username;
			MainAccountTypeLabel.Text = mProtocol.Protocol.Protocol;
			EnabledCheckBox.IsChecked = mProtocol.Enabled;
			if (mProtocol.Protocol.Password != null)
			{
				if (mProtocol.Enabled)
					PasswordBox.IsEnabled = false;
				PasswordBox.Password = String.Empty;
				SavedText.Visibility = Visibility.Visible;
				//Placeholder.SetText(ServerBox, "Server");
			}

			if (!String.IsNullOrEmpty(mProtocol.Protocol.Server))
				ServerBox.Text = mProtocol.Protocol.Server;

			if (mProtocol.Enabled)
			{
				UsernameBox.IsReadOnly = true;
				PasswordBox.IsEnabled = false;
			} else {
				UsernameBox.IsReadOnly = false;
				PasswordBox.IsEnabled = true;
			}
		}
		private void BuildControl()
		{
			if (mProtocolType == typeof(IRCProtocol))
			{
				IRCProtocol ircprot = (IRCProtocol)mProtocol.Protocol;
				ServerGrid.Visibility = Visibility.Visible;

				nicknameTextBox = InsertTextBox(0, "Nickname");
				nicknameTextBox.Text = ircprot.Nickname;
			} else {
				ServerGrid.Visibility = Visibility.Collapsed;
			}
		}
		private TextBox InsertTextBox(int rowPos, string placeholder)
		{
			TextBox textBox = new TextBox();
			textBox.Margin = new Thickness(0, 2, 4, 2);
			textBox.ToolTip = placeholder;

			RowDefinition row = new RowDefinition();
			row.Height = new GridLength(25);
			ControlBlock.RowDefinitions.Insert(rowPos, row);
			
			Placeholder.SetText(textBox, placeholder);

			foreach (UIElement element in ControlBlock.Children)
			{
				int controlRow = Grid.GetRow(element);

				if (controlRow < rowPos)
					continue;

				Grid.SetRow(element, controlRow + 1);
			}

			ControlBlock.Children.Add(textBox);
			Grid.SetRow(textBox, rowPos);

			AdjustControlHeight(30);

			return textBox;
		}

		private void DeleteAccount_Click(object sender, RoutedEventArgs e)
		{
			if (mProtocol.Protocol.Username == "")
			{
				Panel parent = (Panel)Parent;
				parent.Children.Remove(this);
				return;
			}

			AccountManager.Accounts.Remove(mProtocol);
			IMSettings.Accounts.Remove(mProtocol);
		}
		private void EnabledCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			ApplyChanges();

			IMRequiredDetail reason;
			mProtocol.Protocol.IsReady(out reason);

			if (reason == IMRequiredDetail.None)
			{
				MissingFieldMessage.Visibility = Visibility.Collapsed;
				mProtocol.Enabled = EnabledCheckBox.IsChecked.Value;

				UpdateControl();
			} else {
				EnabledCheckBox.IsChecked = false;
				MissingFieldMessage.Visibility = Visibility.Visible;
				Brush errorBrush =new SolidColorBrush(Color.FromRgb(255, 190, 190));

				if (reason.HasFlag(IMRequiredDetail.Username))
					UsernameBox.Background = errorBrush;
				else
					UsernameBox.Background = Brushes.White;
				if (reason.HasFlag(IMRequiredDetail.Nickname))
					nicknameTextBox.Background = errorBrush;
				if (reason.HasFlag(IMRequiredDetail.Server))
					ServerBox.Background = errorBrush;
			}
		}
		private void AutoConnectCheckbox_CheckChanged(object sender, RoutedEventArgs e)
		{
			mProtocol.AutoConnect = AutoConnectCheckbox.IsChecked.Value;
		}
		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (PasswordBox.SecurePassword.Length >= 1)
				SavedText.Visibility = Visibility.Collapsed;
		}
		private void ServerBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			//if (ServerBox.Text.Length >= 1)
			//	ServerBoxHint.Visibility = Visibility.Collapsed;
		}
		private void ServerBox_LostFocus(object sender, RoutedEventArgs e)
		{
			//if (ServerBox.Text.Length == 0)
				//ServerBoxHint.Visibility = Visibility.Visible;
		}
		private void SettingsLink_Click(object sender, RoutedEventArgs e)
		{
			if (mProtocolType == typeof(IRCProtocol))
			{
				ApplyChanges();
				IRCSettingWindow window = new IRCSettingWindow();
				window.PopulateUIControls(mProtocol);
				window.Owner = Window.GetWindow(this);
				window.ShowDialog();
			}
		}

		public bool Selected
		{
			get;
			private set;
		}

		// Variables
		private IMProtocolWrapper mProtocol;
		private Type mProtocolType;
		private TextBox nicknameTextBox;
	}
}