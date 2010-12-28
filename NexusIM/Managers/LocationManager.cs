using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using InstantMessage;

namespace NexusIM.Managers
{
	internal class LocationAsyncResult : IAsyncResult
	{
		public LocationAsyncResult(AsyncCallback callback, object userstate)
		{
			mState = userstate;
			mWaitHandle = new ManualResetEvent(false);
			mLocation = new UserLocation();
		}

		public void Trigger()
		{
			mCompleted = true;
			mWaitHandle.Set();

			if (mCallback != null)
				mCallback(this);
		}
		public object AsyncState
		{
			get {
				return mState;
			}
		}
		public WaitHandle AsyncWaitHandle
		{
			get {
				return mWaitHandle;
			}
		}
		public bool CompletedSynchronously
		{
			get {
				throw new NotImplementedException();
			}
		}
		public bool IsCompleted
		{
			get {
				return mCompleted;
			}
		}
		public HttpWebRequest HttpRequest
		{
			get {
				return mRequest;
			}
			set {
				mRequest = value;
			}
		}
		public UserLocation Location
		{
			get {
				return mLocation;
			}
			set {
				mLocation = value;
			}
		}

		private object mState;
		private ManualResetEvent mWaitHandle;
		private AsyncCallback mCallback;
		private bool mCompleted;
		private HttpWebRequest mRequest;
		private UserLocation mLocation;
	}

	struct LocationStruct
	{
		public IMBuddy mBuddy;
		public UserLocation mLocation;
	}

	class LocationManager
	{
		public static void Setup()
		{
			IMProtocol.onLogin += new EventHandler(IMProtocol_onLogin);

			mContacts = new List<LocationStruct>();

			if (IMSettings.SettingInterface.IsLoaded)
				IMSettings_Loaded(null, null);
			else
				IMSettings.SettingInterface.onFileLoad += new EventHandler(IMSettings_Loaded);
		}

		// Properties
		public static List<LocationStruct> LocationContacts
		{
			get {
				return mContacts;
			}
		}

		// Public Methods
		public static IAsyncResult BeginLookup(IMBuddy buddy, AsyncCallback callback, object userstate)
		{
			ILocationService service = null;
			// Figure out which service to use
			string sName = IMSettings.GetContactSetting(buddy, "locationservice", "default");

			if (sName == "glatitude")
				service = new GoogleLatitude();
			else
				throw new InvalidOperationException("This contact doesn't have a 'locationservice' setting set");

			LocationAsyncResult result = new LocationAsyncResult(callback, userstate);

			return service.BeginLookup(buddy, callback, userstate);
		}
		public static UserLocation EndLookup(IAsyncResult result)
		{
			return ((LocationAsyncResult)result).Location;
		}
		internal static string GetLocationToken(IMBuddy buddy)
		{
			return IMSettings.GetContactSetting(buddy, "locationuid", "-0");
		}

		// Callbacks
		private static void IMSettings_Loaded(object sender, EventArgs e)
		{
			Dictionary<IMBuddy, Dictionary<string, string>> cSettings = IMSettings.SettingInterface.ContactSettings;

			IEnumerable<IMBuddy> equipped = cSettings.Where(p => p.Value.Keys.Contains("locationservice")).Select(p => p.Key);

			foreach (IMBuddy buddy in equipped)
			{
				LocationAsyncResult result = BeginLookup(buddy, null, null) as LocationAsyncResult;
				LocationStruct lStruct = new LocationStruct();
				lStruct.mBuddy = buddy;
				lStruct.mLocation = result.Location;

				mContacts.Add(lStruct);
			}
		}
		private static void IMProtocol_onLogin(object sender, EventArgs e)
		{
			if (IMSettings.SettingInterface is BasicXmlSettingsBinding)
			{
				BasicXmlSettingsBinding xtc = IMSettings.SettingInterface as BasicXmlSettingsBinding;
				xtc.FixContactSettings();
				IMSettings_Loaded(null, null);
			}
		}

		// Events
		public static event EventHandler onLocationChange;

		// Variables
		private static List<LocationStruct> mContacts;
	}

	class UserLocation
	{	
		public UserLocation() {}
		public UserLocation(double lat, double lon)
		{
			mLatitude = lat;
			mLongitude = lon;
		}
		public double Latitude
		{
			get {
				return mLatitude;
			}
			set {
				mLatitude = value;
			}
		}
		public double Longitude
		{
			get {
				return mLongitude;
			}
			set {
				mLongitude = value;
			}
		}
		public int Accuracy
		{
			get {
				return mAccuracy;
			}
			set {
				mAccuracy = value;
			}
		}

		private double mLatitude;
		private double mLongitude;
		private int mAccuracy;
	}

	interface ILocationService
	{
		IAsyncResult BeginLookup(IMBuddy buddy, AsyncCallback callback, object userstate);
	}

	class GoogleLatitude : ILocationService
	{
		public IAsyncResult BeginLookup(IMBuddy buddy, AsyncCallback callback, object userstate)
		{
			string uid = LocationManager.GetLocationToken(buddy); // String because this number is very big.. past int MaxVal and I don't need to do any math on it

			Uri lookUri = new Uri("https://www.google.com/latitude/apps/badge/api?user=" + uid + "&type=json");
			HttpWebRequest request = WebRequest.Create(lookUri) as HttpWebRequest;
			LocationAsyncResult result = new LocationAsyncResult(callback, userstate);
			result.HttpRequest = request;

			request.BeginGetResponse(new AsyncCallback(Request_GetResponse), result);
			
			return result;
		}

		private void Request_GetResponse(IAsyncResult e)
		{
			LocationAsyncResult result = e.AsyncState as LocationAsyncResult;
			HttpWebRequest request = result.HttpRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream);
			string jsondata = reader.ReadToEnd();
			reader.Close();

			Hashtable table = JSON.JsonDecode(jsondata) as Hashtable;
			ArrayList features = table["features"] as ArrayList;
			Hashtable feature = features[0] as Hashtable;

			Hashtable geometry = feature["geometry"] as Hashtable;
			ArrayList coordinates = geometry["coordinates"] as ArrayList;
			double latitude = (double)coordinates[1];
			double longitude = (double)coordinates[0];
			result.Location.Latitude = latitude;
			result.Location.Longitude = longitude;

			Hashtable properties = feature["properties"] as Hashtable;
			result.Location.Accuracy = Convert.ToInt32(properties["accuracyInMeters"]);

			result.Trigger();
		}
	}
}