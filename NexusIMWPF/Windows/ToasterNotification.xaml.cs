using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace NexusIM.Windows
{
	/// <summary>
	/// Interaction logic for ToasterNotification.xaml
	/// </summary>
	public partial class ToasterNotification : Window
	{
		public ToasterNotification()
		{
			this.InitializeComponent();
		}

		public UserControl NotificationContent
		{
			set {
				ContentGrid.Children.Add(value);
			}
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
			var corner = transform.Transform(new Point(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight));

			this.Left = corner.X - this.ActualWidth;
			this.Top = corner.Y - this.ActualHeight - 30;
		}
	}
}