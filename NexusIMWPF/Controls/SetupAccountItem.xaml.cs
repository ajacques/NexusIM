using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Data;
using InstantMessage;
using NexusIM.Managers;
using System.Collections.Generic;
using System.Diagnostics;
using System;

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
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			BindingExpression expression = BindingOperations.GetBindingExpression(UsernameBox, TextBox.TextProperty);
			expression.UpdateTarget();
		}
		
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			IMProtocolExtraData extraData = DataContext as IMProtocolExtraData;
			IEnumerable<BindingExpression> expressions = new BindingExpression[] { BindingOperations.GetBindingExpression(UsernameBox, TextBox.TextProperty) };
			foreach (BindingExpression exp in expressions)
				exp.UpdateTarget();

			PasswordBox.Password = String.Empty;
			Debug.WriteLine("User canceled saving new account");
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			IMProtocolExtraData extraData = DataContext as IMProtocolExtraData;
			extraData.Protocol.Username = UsernameBox.Text;
			extraData.Protocol.Password = PasswordBox.Password;

			if (!extraData.IsReady)
			{
				AccountManager.Accounts.Add(extraData);
				IMSettings.Accounts.Add(extraData);
				Debug.WriteLine("User is saving new account of type" + extraData.Protocol.Protocol + ". Registering with IMSetttings and AccountManager.");
			} else
				Debug.WriteLine("User is saving account of type " + extraData.Protocol.Protocol);

			Deselect();
		}
		private void DeleteAccount_Click(object sender, RoutedEventArgs e)
		{
			IMProtocolExtraData protocol = DataContext as IMProtocolExtraData;

			AccountManager.Accounts.Remove(protocol);
			IMSettings.Accounts.Remove(protocol);
		}
	}
}