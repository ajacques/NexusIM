using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Data;
using InstantMessage;
using NexusIM.Managers;
using System.Collections.Generic;

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

			PasswordBox.Password = extraData.Protocol.Password;
		}
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			IMProtocolExtraData extraData = DataContext as IMProtocolExtraData;
			extraData.Protocol.Username = UsernameBox.Text;
			extraData.Protocol.Password = PasswordBox.Password;

			if (!extraData.IsReady)
				AccountManager.AddNewAccount(extraData);

			Deselect();
		}
	}
}