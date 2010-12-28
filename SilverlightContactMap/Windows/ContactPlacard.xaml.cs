using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightContactMap.NexusCore;

namespace SilverlightContactMap.Windows
{
	public partial class ContactPlacard : UserControl
	{
		public ContactPlacard(ContactPoint point)
		{
			InitializeComponent();

			mPoint = point;
			Visibility = Visibility.Collapsed;
		}

		public string Username
		{
			set {
				mDisplayName.Text = value;
			}
		}
	
		private ContactPoint mPoint;

		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MainPage.Instance.CloseAllPopups();
			mPoint.OpenPopup();
		}
	}
}
