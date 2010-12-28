using System;
using System.ComponentModel;
using System.Diagnostics;
using SilverlightContactMap.NexusCore;
using System.Collections.Generic;
using System.Linq;

namespace SilverlightContactMap
{
	delegate void OnNewLocation(int rowid, double latitude, double longitude, int accuracy, DateTime lastChanged);
	delegate void OnNewContact(int rowid, LocationServiceType servicetype, string username, bool messagable);
	delegate void OnError(Exception e);
	delegate void OnComplete();
	class NexusCoreLayer
	{
		public static void Connect(string devicetoken)
		{
			mDeviceToken = devicetoken;
			App.onApplicationExit += new EventHandler(App_onApplicationExit);

			mCoreClient = new CoreServiceClient();
			mCoreClient.OpenCompleted += new EventHandler<AsyncCompletedEventArgs>(OnConnect);
			mCoreClient.LoginWithTokenCompleted += new EventHandler<AsyncCompletedEventArgs>(OnLogin);
			mCoreClient.GetLocationDataCompleted += new EventHandler<GetLocationDataCompletedEventArgs>(OnLocationDataDownload);
			mCoreClient.GetLocationCompleted += new EventHandler<GetLocationCompletedEventArgs1>(OnLocationGetCompleted);
			mCoreClient.GetMultipleLocationsCompleted += new EventHandler<GetMultipleLocationsCompletedEventArgs>(OnMultipleLocationsCompleted);
			//mCoreClient.GetMyAccountInfoCompleted += new EventHandler<GetMyAccountInfoCompletedEventArgs>(CoreService_GetMyAccountInfo);
			mCoreClient.OpenAsync();
		}

		public static void RequestLocationUpdate(int rowid)
		{
			mCoreClient.GetLocationAsync(rowid);
		}

		// Callbacks
		private static void OnConnect(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Exception ex = e.Error.GetBaseException();
				Debug.WriteLine("NexusCoreLayer: Encountered error while connecting to service (" + ex.GetType().Name + "): " + ex.Message);
				return;
			}
			mCoreClient.LoginWithTokenAsync(mDeviceToken);
		}
		private static void OnLogin(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Exception ex = e.Error.GetBaseException();
				Debug.WriteLine("NexusCoreLayer: Encountered error while authenticating (" + ex.GetType().Name + "): " + ex.Message);
				return;
			}
			mCoreClient.GetLocationDataAsync(); // Get a list of all people we can view the locations of
		}
		private static void OnLocationDataDownload(object sender, GetLocationDataCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				if (onError != null)
					onError(e.Error.GetBaseException());

				return;
			}

			mRequestQueue = new List<int>();

			e.Result.ForEach(cli => onNewContact(cli.LocationId, cli.ServiceType, cli.Username, cli.Messagable));
			mRequestQueue.AddRange(e.Result.Select(cli => cli.LocationId)); // Request all locations

			mCoreClient.GetMultipleLocationsAsync(mRequestQueue);
		}
		private static void OnMultipleLocationsCompleted(object sender, GetMultipleLocationsCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				if (onError != null)
					onError(e.Error.GetBaseException());

				return;
			}

			foreach (UserLocationData data in e.Result)
			{
				onNewLocation(data.RowId, data.Latitude, data.Longitude, data.Accuracy, data.LastUpdated);
				mRequestQueue.Remove(data.RowId);
			}

			onComplete();
		}
		private static void OnLocationGetCompleted(object sender, GetLocationCompletedEventArgs1 e)
		{
			if (e.Error != null)
			{
				if (onError != null)
					onError(e.Error.GetBaseException());

				return;
			}

			onNewLocation(e.Result.RowId, e.Result.Latitude, e.Result.Longitude, e.Result.Accuracy, e.Result.LastUpdated);
		}
		private static void App_onApplicationExit(object sender, EventArgs e)
		{
			mCoreClient.LogoutAsync();
			mCoreClient.CloseAsync();
		}

		public static event OnNewContact onNewContact;
		public static event OnError onError;
		public static event OnNewLocation onNewLocation;
		public static event OnComplete onComplete;

		// Variables
		private static List<int> mRequestQueue;
		private static string mDeviceToken;
		private static CoreServiceClient mCoreClient;
	}
}