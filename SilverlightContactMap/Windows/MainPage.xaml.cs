using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Maps.MapControl;
using SilverlightContactMap.NexusCore;

namespace SilverlightContactMap.Windows
{
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();

			SetupEventHandlers();

			mInstance = this;
			mPoints = new List<ContactPoint>();

			BeginLogin();
		}

		public static MainPage Instance
		{
			get {
				return mInstance;
			}
		}

		private DispatcherTimer mTimeUpdateTimer;
		private DispatcherTimer mUpdateTimer;
		private List<ContactPoint> mPoints;
		private static MainPage mInstance;

		private void SetupEventHandlers()
		{
			NexusCoreLayer.onNewContact += new OnNewContact(NexusCore_onNewContact);
			NexusCoreLayer.onNewLocation += new OnNewLocation(NexusCore_OnLocation);
			NexusCoreLayer.onComplete += new OnComplete(NexusCore_onComplete);
		}

		private void BeginLogin()
		{
			string mToken = GetCookie("AUTHTOKEN");
			NexusCoreLayer.Connect(mToken);
		}

		// Api
		public void CloseAllPopups()
		{
			foreach (ContactPoint point in mPoints.Where(cp => cp.Ready))
			{
				point.Popup.Visibility = Visibility.Collapsed;
				point.Placard.Visibility = Visibility.Visible;
			}
		}
		public void ZoomToLocation(Location newLoc)
		{
			contacts.SetView(newLoc, 6);
		}
		public void ZoomToPoint(ContactPoint point)
		{
			LocationCollection collection = point.RadiusPolygon.Locations;
			double t1 = collection.Min(l => l.Latitude);
			double t2 = collection.Max(l => l.Latitude);
			double l1 = collection.Min(l => l.Longitude);
			double l2 = collection.Max(l => l.Longitude);
			LocationRect rect = new LocationRect(t1, l1, t2, l2);

			contacts.SetView(rect);
		}

		private void FixZIndexes()
		{
			mPoints.Sort(delegate(ContactPoint a, ContactPoint b)
			{
				return 0 - a.Location.Latitude.CompareTo(b.Location.Latitude); // 90 ... -90
			});

			// Remove all children
			UIElement[] elems = pLayer.Children.Select(ui => ui).ToArray();
			foreach (UIElement elem in elems)
				pLayer.Children.Remove(elem);

			// Do it old school
			foreach (ContactPoint point in mPoints) // Precision Circle -> Placard -> Popup
			{
				if (point.RadiusPolygon != null)
					pLayer.Children.Add(point.RadiusPolygon);
				if (point.Placard != null)
					pLayer.AddChild(point.Placard, point.Location);
			}

			foreach (ContactPoint point in mPoints)
			{
				if (point.Popup != null)
					pLayer.AddChild(point.Popup, point.Location);
			}

			/*foreach (ContactPoint points in mPoints) // Precision Circle -> Placard -> Popup
			{
				Canvas.SetZIndex(points.RadiusPolygon, 20);
				Canvas.SetZIndex(points.Placard, (int)(90 + ((points.Location.Latitude + 90) / 180)));
				Canvas.SetZIndex(points.Popup, 100);
			}*/
		}
		private string GetCookie(string key)
		{
			string[] cookies = HtmlPage.Document.Cookies.Split(';');

			foreach (string cookie in cookies)
			{
				string[] keyValue = cookie.Split('=');
				if (keyValue.Length == 2)
				{
					if (keyValue[0].Trim(' ').ToString() == key)
						return keyValue[1];
				}
			}
			return null;
		}

		// Timer Functions
		private void ChangedTextUpdater_Tick(object sender, EventArgs e)
		{
			if (mPoints.Count() == 0)
				return;

			// Update the Last Updated times
			foreach (ContactPoint point in mPoints)
				point.UpdateDates();

			DateTime now = DateTime.UtcNow; // All Date/Times are handled internally as Utc

			IEnumerable<TimeSpan> times = mPoints.Select(cp => now.Subtract(cp.LastUpdated));
			TimeSpan smallest = TimeSpan.FromSeconds(times.Min(cp => cp.TotalSeconds)); // Gets the most recently updated (as in the location changed) point

			if (smallest.TotalMinutes < 1)
				mTimeUpdateTimer.Interval = new TimeSpan(0, 0, 1);
			else if (smallest.TotalHours < 1)
				mTimeUpdateTimer.Interval = new TimeSpan(0, 1, 0);
			else
				mTimeUpdateTimer.Interval = new TimeSpan(1, 0, 0);
		}
		/// <summary>
		/// Handles the update of contact locations using a timer
		/// </summary>
		private void UpdateQueue_NextUpdate(object sender, EventArgs e)
		{
			List<ContactPoint> dupe = mPoints.ToList();
			dupe.Sort(delegate(ContactPoint a, ContactPoint b)
			{
				return a.UpdatePoints().CompareTo(b.UpdatePoints());
			});

			IEnumerable<ContactPoint> points = dupe.Take(5);
			Random rand = new Random();
			ContactPoint point = dupe.ElementAt(rand.Next(dupe.Count()));
			
		}

		// User Interface Callbacks
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			mUpdateTimer = new DispatcherTimer();
			mUpdateTimer.Interval = new TimeSpan(0, 0, 30);
			mUpdateTimer.Tick += new EventHandler(UpdateQueue_NextUpdate);

			mTimeUpdateTimer = new DispatcherTimer();
			mUpdateTimer.Interval = new TimeSpan(0, 0, 1);
			mTimeUpdateTimer.Tick += new EventHandler(ChangedTextUpdater_Tick);

		}
		private void Placard_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ContactPlacard placard = sender as ContactPlacard;
			ContactPoint point = mPoints.Where(p => p.Placard == placard).First();

			foreach (ContactPoint mpoint in mPoints)
			{
				mpoint.Placard.Visibility = Visibility.Visible;
				mpoint.Popup.Visibility = Visibility.Collapsed;
			}

			point.Placard.Visibility = Visibility.Collapsed;
			point.Popup.Visibility = Visibility.Visible;
		}
		private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
		{
			signout.TextDecorations = TextDecorations.Underline;
		}
		private void signout_MouseLeave(object sender, MouseEventArgs e)
		{
			signout.TextDecorations = null;
		}
		private void signout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			FixZIndexes();
		}
		private void LoadingPage_onLogin(string username, string password)
		{
			
			
		}

		private void NexusCore_onNewContact(int rowid, LocationServiceType servicetype, string username, bool messagable)
		{
			ContactPoint point;
			if (!mPoints.Any(cp => cp.RowId == rowid))
			{
				point = new ContactPoint(rowid);
				mPoints.Add(point);
			} else
				point = mPoints.Where(cp => cp.RowId == rowid).First();

			point.ServiceType = servicetype;
			point.Username = username;
			point.Messagable = messagable;
		}
		private void NexusCore_OnLocation(int rowid, double latitude, double longitude, int accuracy, DateTime lastChanged)
		{
			ContactPoint point;
			point = mPoints.Where(cp => cp.RowId == rowid).First();

			//JSLogger.WriteLine("location: " + point.Username);

			point.LastUpdated = lastChanged;
			point.LastChecked = DateTime.UtcNow;
			point.Location.Latitude = latitude;
			point.Location.Longitude = longitude;
			point.Accuracy = accuracy;

			point.OpenPlacard();
			point.Ready = true;

			pLayer.Children.Add(point.RadiusPolygon);
			pLayer.AddChild(point.Placard, point.Location, PositionOrigin.BottomCenter);
			pLayer.AddChild(point.Popup, point.Location, PositionOrigin.BottomCenter);
		}
		private void NexusCore_onComplete()
		{
			FixZIndexes();
			JSLogger.WriteLine("Completed");
			mTimeUpdateTimer.Start();
		}
	}
}