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
using System.Windows.Navigation;
using System.Windows.Shapes;
using InstantMessage;

namespace NexusIM.Controls
{
	/// <summary>
	/// Interaction logic for ChatMessageNotification.xaml
	/// </summary>
	public partial class ChatMessageNotification : UserControl
	{
		public ChatMessageNotification()
		{
			this.InitializeComponent();
		}

		public IContact ContactSource
		{
			set	{
				ContactGrid.DataContext = value;
			}
		}
	}
}