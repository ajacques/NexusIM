using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using InstantMessage;
using NexusIM.Managers;

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
			PasswordBox.PasswordChanged += new RoutedEventHandler(PasswordBox_PasswordChanged);
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

			if (!mProtocol.Enabled)
			{
				mProtocol.Protocol.Username = UsernameBox.Text;

				string password = PasswordBox.Password;
				if (!String.IsNullOrEmpty(password))
				{
					mProtocol.Protocol.Password = password;
					PasswordBox.Password = String.Empty;
					SavedText.Visibility = Visibility.Visible;
				}
				PopulateUIControls(mProtocol);
			}

			if (!mProtocol.IsReady)
			{
				AccountManager.Accounts.Add(mProtocol);
				IMSettings.Accounts.Add(mProtocol);
			}
		}

		public bool Selected
		{
			get;
			private set;
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}

		public void PopulateUIControls(IMProtocolExtraData extraData)
		{
			mProtocol = extraData;

			// Setup stuff
			UsernameBox.Text = extraData.Protocol.Username;
			AutoConnectCheckbox.IsChecked = extraData.AutoConnect;
			MainAccountUsername.Text = extraData.Protocol.Username;
			MainAccountTypeLabel.Text = extraData.Protocol.Protocol;
			EnabledCheckBox.IsChecked = extraData.Enabled;

			if 

			if (extraData.Enabled)
			{
				UsernameBox.IsReadOnly = true;
				PasswordBox.IsEnabled = false;
			} else {
				UsernameBox.IsReadOnly = false;
				PasswordBox.IsEnabled = true;
			}
		}

		private void DeleteAccount_Click(object sender, RoutedEventArgs e)
		{
			AccountManager.Accounts.Remove(mProtocol);
			IMSettings.Accounts.Remove(mProtocol);
		}
		private void EnabledCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			mProtocol.Enabled = EnabledCheckBox.IsChecked.Value;

			PopulateUIControls(mProtocol);
		}
		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (PasswordBox.SecurePassword.Length >= 1)
				SavedText.Visibility = Visibility.Collapsed;
		}

		private IMProtocolExtraData mProtocol;
	}
}