using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

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

			Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
			{
				var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
				var corner = transform.Transform(new Point(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight));

				this.Left = corner.X - this.ActualWidth;
				this.Top = corner.Y - this.ActualHeight - 40;
			}));
		}
	}
}