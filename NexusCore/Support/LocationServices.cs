using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Net;
using System.IO;
using System.Threading;
using NexusCore.DataContracts;
using NexusCore.Databases;

namespace NexusCore
{
	internal class LocationAsyncResult : IAsyncResult
	{
		public LocationAsyncResult(AsyncCallback callback, object userstate)
		{
			mCallback = callback;
			mState = userstate;
			mWaitHandle = new ManualResetEvent(false);
			mLocation = new Dictionary<string, UserLocationData>();
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
		public Dictionary<string, UserLocationData> Location
		{
			get {
				return mLocation;
			}
			set {
				mLocation = value;
			}
		}
		public Thread ExecutionThread
		{
			get {
				return mExecutionThread;
			}
			set {
				mExecutionThread = value;
			}
		}

		private object mState;
		private ManualResetEvent mWaitHandle;
		private AsyncCallback mCallback;
		private bool mCompleted;
		private HttpWebRequest mRequest;
		private Dictionary<string, UserLocationData> mLocation;
		private Thread mExecutionThread;
		public Exception mError;
	}

	class LocationUnavailableException : Exception
	{

	}

	static class LocationLookup
	{
		public static IAsyncResult BeginLookup(LocationServiceType type, string mIdentifier, AsyncCallback callback, object userstate)
		{
			ILocationService service = null;
			if (type == LocationServiceType.GoogleLatitude)
				service = new GoogleLatitude();
			else
				throw new Exception("");

			return service.BeginLookup(mIdentifier, callback, userstate);
		}
		public static IAsyncResult BeginLookupMultiple(LocationServiceType type, IEnumerable<string> identifiers, AsyncCallback callback, object userstate)
		{
			ILocationService service = null;
			if (type == LocationServiceType.GoogleLatitude)
				service = new GoogleLatitude();
			else
				throw new Exception("");

			return service.BeginLookupMultiple(identifiers, callback, userstate);			
		}
		public static UserLocationData EndLookup(IAsyncResult result)
		{
			var retval = ((LocationAsyncResult)result);

			retval.AsyncWaitHandle.WaitOne();

			if (retval.mError != null)
				throw retval.mError;

			return retval.Location.First().Value;
		}
		public static Dictionary<string, UserLocationData> EndLookupMultiple(IAsyncResult result)
		{
			var rval = ((LocationAsyncResult)result);

			rval.AsyncWaitHandle.WaitOne();

			if (rval.mError != null)
				throw rval.mError;

			return rval.Location;
		}
		public static UserLocationData Lookup(LocationServiceType type, string mIdentifier)
		{
			IAsyncResult result = BeginLookup(type, mIdentifier, null, null);
			result.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, 10));

			if (!result.IsCompleted)
				throw new TimeoutException("Location request timed-out");

			return EndLookup(result);
		}
		public static Dictionary<string, UserLocationData> LookupMultiple(LocationServiceType type, IEnumerable<string> mIdentifier)
		{
			IAsyncResult result = BeginLookupMultiple(type, mIdentifier, null, null);
			result.AsyncWaitHandle.WaitOne();

			if (!result.IsCompleted)
				throw new TimeoutException("Location request timed-out");

			return EndLookupMultiple(result);
		}
	}

	interface ILocationService
	{
		IAsyncResult BeginLookup(string mIdentifier, AsyncCallback callback, object userstate);
		IAsyncResult BeginLookupMultiple(IEnumerable<string> mIdentifier, AsyncCallback callback, object userstate);
		UserLocationData EndLookup(IAsyncResult result);
		Dictionary<string, UserLocationData> EndLookupMultiple(IAsyncResult result);
		UserLocationData Lookup(string mIdentifier);
		Dictionary<string, UserLocationData> LookupMultiple(IEnumerable<string> mIdentifier);
	}

	class GoogleLatitude : ILocationService
	{
		public IAsyncResult BeginLookup(string mIdentifier, AsyncCallback callback, object userstate)
		{
			Uri lookUri = new Uri(String.Format("https://www.google.com/latitude/apps/badge/api?user={0}&type=json", mIdentifier));
			HttpWebRequest request = WebRequest.Create(lookUri) as HttpWebRequest;
			LocationAsyncResult result = new LocationAsyncResult(callback, userstate);
			result.HttpRequest = request;

			request.AutomaticDecompression = DecompressionMethods.GZip;
			request.UserAgent = "NexusCore Web Service (http://www.adrensoftware.com)";
			request.Timeout = 9500; // 9.5 Seconds
			request.BeginGetResponse(new AsyncCallback(Request_GetResponse), result);
			
			return result;
		}
		public IAsyncResult BeginLookupMultiple(IEnumerable<string> mIdentifier, AsyncCallback callback, object userstate)
		{
			Uri lookUri = new Uri(String.Format("https://www.google.com/latitude/apps/badge/api?user={0}&type=json", mIdentifier.Aggregate((s, n) => s + "," + n)));
			HttpWebRequest request = WebRequest.Create(lookUri) as HttpWebRequest;
			LocationAsyncResult result = new LocationAsyncResult(callback, userstate);
			result.HttpRequest = request;

			request.AutomaticDecompression = DecompressionMethods.GZip;
			request.UserAgent = "NexusCore Web Service (http://www.adrensoftware.com)";
			request.Timeout = 9500; // 9.5 Seconds
			request.BeginGetResponse(new AsyncCallback(Request_GetResponse), result);

			return result;
		}
		public UserLocationData EndLookup(IAsyncResult result)
		{
			var rval = ((LocationAsyncResult)result);

			rval.AsyncWaitHandle.WaitOne();

			if (rval.mError != null)
				throw rval.mError;

			return rval.Location.First().Value;
		}
		public Dictionary<string, UserLocationData> EndLookupMultiple(IAsyncResult result)
		{
			var rval = ((LocationAsyncResult)result);

			rval.AsyncWaitHandle.WaitOne();

			if (rval.mError != null)
				throw rval.mError;

			return rval.Location;
		}
		public UserLocationData Lookup(string mIdentifier)
		{
			IAsyncResult result = BeginLookup(mIdentifier, null, null);

			return EndLookup(result);
		}
		public Dictionary<string, UserLocationData> LookupMultiple(IEnumerable<string> mIdentifier)
		{
			IAsyncResult result = BeginLookupMultiple(mIdentifier, null, null);

			return EndLookupMultiple(result);
		}

		private void Request_GetResponse(IAsyncResult e)
		{
			LocationAsyncResult result = e.AsyncState as LocationAsyncResult;
			result.ExecutionThread = Thread.CurrentThread;
			HttpWebRequest request = result.HttpRequest;
			HttpWebResponse response = request.EndGetResponse(e) as HttpWebResponse;
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream);
			string jsondata = reader.ReadToEnd();
			reader.Close();

			Hashtable table = JSON.JsonDecode(jsondata) as Hashtable;
			ArrayList features = table["features"] as ArrayList;

			if (features.Count == 0)
			{
				result.mError = new LocationUnavailableException();
				result.Trigger();
				return;
			}

			foreach (Hashtable feature in features)
			{
				Hashtable geometry = feature["geometry"] as Hashtable;
				ArrayList coordinates = geometry["coordinates"] as ArrayList;
				double latitude = (double)coordinates[1];
				double longitude = (double)coordinates[0];
				UserLocationData location = new UserLocationData();
				location.Latitude = latitude;
				location.Longitude = longitude;

				Hashtable properties = feature["properties"] as Hashtable;
				location.Accuracy = (int)(Convert.ToInt32(properties["accuracyInMeters"]) * 3.281);
				location.mGeocode = (string)properties["reverseGeocode"];

				int timestamp = Convert.ToInt32(properties["timeStamp"]);
				DateTime epoch = new DateTime(1970, 1, 1);
				location.TimeChanged = epoch.AddSeconds(timestamp);

				result.Location.Add(properties["id"] as string, location);
			}

			result.Trigger();
		}
	}
}