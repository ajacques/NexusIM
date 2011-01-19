using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NexusIM.Windows
{
	class DummyWindow : Window
	{
		public DummyWindow()
		{
			this.Visibility = Visibility.Hidden;
			this.ShowInTaskbar = false;
			this.Width = 1;
			this.Height = 1;
		}
	}
}
