using System;
using System.Windows.Controls;
using NexusIM.Managers;
using System.Windows;
using System.Windows.Media;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for SysTrayPeekWindow.xaml
	/// </summary>
	public partial class SysTrayPeekWindow : UserControl
	{
		public SysTrayPeekWindow()
		{
			this.InitializeComponent();

			this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(SysTrayPeekWindow_IsVisibleChanged);

			mGreenBrush = new SolidColorBrush(Colors.Green);
			mRedBrush = new SolidColorBrush(Colors.Red);

			mGreenBrush.Freeze();
			mRedBrush.Freeze();
		}

		private void SysTrayPeekWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
			{
				StatusString.Text = AccountManager.Status.ToString();

				if (AccountManager.Connected)
				{
					ConnStatus.Foreground = mGreenBrush;
					ConnStatus.Text = "Connected";
				} else {
					ConnStatus.Foreground = mRedBrush;
					ConnStatus.Text = "Not Connected";
				}
			}
		}

		private Brush mGreenBrush;
		private Brush mRedBrush;
	}
}