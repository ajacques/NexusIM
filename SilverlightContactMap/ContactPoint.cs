using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Maps.MapControl;
using SilverlightContactMap.NexusCore;
using SilverlightContactMap.Windows;

namespace SilverlightContactMap
{
	public class ContactPoint
	{
		public ContactPoint(int rowid)
		{
			mLocation = new Location();
			mPushpin = new ContactPlacard(this);
			mPopup = new ContactPopup(this);
			mRadiusPolygon = new MapPolygon();
			mRadiusPolygon.Stroke = new SolidColorBrush(Colors.Blue);
			mRadiusPolygon.StrokeThickness = 2;
			mRowId = rowid;
		}

		public void OpenPlacard()
		{
			mPopup.Visibility = Visibility.Collapsed;
			mPushpin.Visibility = Visibility.Visible;

			mPushpin.Username = mUsername;
			mPopup.Username = mUsername;
		}
		public void OpenPopup()
		{
			mPushpin.Visibility = Visibility.Collapsed;
			mPopup.Visibility = Visibility.Visible;
		}
		public int UpdatePoints()
		{
			int points = 0;

			Point outpoint;
			DateTime now = DateTime.UtcNow; // Everything is in Utc use it
			TimeSpan updatespan = now.Subtract(mLastUpdated);

			if (MainPage.Instance.contacts.TryLocationToViewportPoint(mLocation, out outpoint)) // Is the person in the current viewport
				points++;
			
			return points;
		}
		public void UpdateDates()
		{
			mPopup.LastChanged = mLastUpdated;
		}
		private LocationCollection GetRadiusPoints(Location center, double radius)
		{
			LocationCollection points = new LocationCollection();
			return points;

			int earthRadius = 3959 * 5280; // Earth Mean Radius in Miles
			double lat = (center.Latitude * Math.PI) / 180;
			double lon = (center.Longitude * Math.PI) / 180;
			double angular = radius / earthRadius;

			for (int i = 0; i <= 360; i += 10)
			{
				Location p2 = new Location(0, 0);
				double brng = i * Math.PI / 180;
				p2.Latitude = Math.Asin(Math.Sin(lat) * Math.Cos(angular) + Math.Cos(lat) * Math.Sin(angular) * Math.Cos(brng));
				p2.Longitude = ((lon + Math.Atan2(Math.Sin(brng) * Math.Sin(angular) * Math.Cos(lat), Math.Cos(angular) - Math.Sin(lat) * Math.Sin(p2.Latitude))) * 180) / Math.PI;
				p2.Latitude = (p2.Latitude * 180) / Math.PI;

				points.Add(p2);
			}

			return points;
		}

		// Properties
		public ContactPlacard Placard
		{
			get {
				return mPushpin;
			}
			set {
				mPushpin = value;
			}
		}
		public ContactPopup Popup
		{
			get {
				return mPopup;
			}
			set {
				mPopup = value;
			}
		}
		public MapPolygon RadiusPolygon
		{
			get {
				return mRadiusPolygon;
			}
			set {
				mRadiusPolygon = value;
			}
		}
		public int RowId
		{
			get {
				return mRowId;
			}
		}
		public Location Location
		{
			get {
				return mLocation;
			}
			set {
				mLocation = value;
			}
		}
		public bool Ready
		{
			get {
				return mReady;
			}
			set {
				mReady = value;
			}
		}
		public DateTime LastUpdated
		{
			get {
				return mLastUpdated;
			}
			set {
				mLastUpdated = value;

				if (mPopup != null)
					mPopup.LastChanged = value;
			}
		}
		public DateTime LastChecked
		{
			get {
				return mLastChecked;
			}
			set {
				mLastChecked = value;
			}
		}
		public LocationServiceType ServiceType
		{
			get {
				return mServiceType;
			}
			set {
				mServiceType = value;

				if (mPopup != null)
					mPopup.ServiceType = mServiceType;
			}
		}
		public string Username
		{
			get {
				return mUsername;
			}
			set {
				mUsername = value;

				mPushpin.Username = value;
				mPopup.Username = value;
			}
		}
		public int Accuracy
		{
			set {
				mPopup.Accuracy = value;

				mRadiusPolygon.Locations = GetRadiusPoints(mLocation, value);
			}
		}
		public bool Messagable
		{
			get {
				return mMessagable;
			}
			set {
				mMessagable = value;

				if (value)
					mPopup.SendMessage.Visibility = Visibility.Visible;
				else
					mPopup.SendMessage.Visibility = Visibility.Collapsed;
			}
		}

		// Variables
		private Location mLocation;
		private ContactPlacard mPushpin;
		private ContactPopup mPopup;
		private MapPolygon mRadiusPolygon;
		private bool mReady;
		private int mRowId;
		private DateTime mLastUpdated;
		private DateTime mLastChecked;
		private LocationServiceType mServiceType;
		private string mUsername;
		private bool mMessagable;
	}
}
