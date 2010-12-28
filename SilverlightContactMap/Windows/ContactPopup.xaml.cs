using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Globalization;
using System.Windows.Input;
using SilverlightContactMap;
using System.Windows.Controls;
using SilverlightContactMap.NexusCore;

namespace SilverlightContactMap.Windows
{
	public partial class ContactPopup : UserControl
	{
		public ContactPopup(ContactPoint point)
		{
			InitializeComponent();

			mPoint = point;
			this.Visibility = Visibility.Collapsed;
		}

		public ContactPoint ContactPoint
		{
			get {
				return mPoint;
			}
			set {
				mPoint = value;
			}
		}
		public DateTime LastChanged
		{
			set {
				StatusChange.Text = value.ToHumanReadableString();
			}
		}
		public int Accuracy
		{
			set {
				mAccuracy.Text = String.Format(Strings.AccuracyString, value.ToString("N0", CultureInfo.CurrentCulture), value == 1 ? Strings.FeetSingularString : Strings.FeetPluralString);
			}
		}
		public LocationServiceType ServiceType
		{
			set {
				mServiceType.Text = String.Format(Strings.ViaServiceType, Strings.GoogleLatitudeServiceType);
			}
		}
		public string Username
		{
			set {
				mDisplayName.Text = value;
			}
		}

		private ContactPoint mPoint;
		private long mLastClick;

		// User Interface Callbacks
		private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			mPoint.Placard.Visibility = Visibility.Visible;
			this.Visibility = Visibility.Collapsed;
		}
		private void RefreshButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			NexusCoreLayer.RequestLocationUpdate(mPoint.RowId);
		}
		private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if ((DateTime.Now.Ticks - mLastClick) < 2310000) //What? How many seconds?
			{
				MainPage.Instance.ZoomToPoint(mPoint);
			}

			mLastClick = DateTime.Now.Ticks;
		}
		private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
	}
}