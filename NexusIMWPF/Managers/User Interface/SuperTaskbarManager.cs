using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using InstantMessage;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using NexusIM.Properties;

namespace NexusIM.Managers
{
	static class SuperTaskbarManager
	{
		/// <summary>
		/// Prepares the Super Taskbar for use by the manager
		/// </summary>
		public static void Setup()
		{
			if (!Win32.IsWin7AndUp() || mSetup || mAttemptedSetup)
				return;

			Trace.WriteLine("Loading SuperTaskbarManager");

			mAttemptedSetup = true;

			// Create the jump list and categories
			appJList = JumpList.CreateJumpList();
			frequentUsers = new JumpListCustomCategory("Frequent");

			// This is to find out whether or not we are going to have problems with jumplists being disabled later.
			try {
				appJList.Refresh();
			} catch (DirectoryNotFoundException) { // Reported when jump lists are disabled
				Trace.WriteLine("JumpLists are disabled. Unloading all data");
				Shutdown();
				return;
			} catch (Exception e) {
				Trace.WriteLine("Unknown JumpList Error of type (" + e.GetType().FullName + ") : " + e.Message + e.StackTrace);
				Shutdown();
				return;
			}

			string icondll = Path.Combine(Environment.CurrentDirectory, "NexusIM_Icons.dll"); // Path to dll that contains all icons

			statusItems = new Dictionary<IMStatus, JumpListLink>();
			pointIcon = new IconReference(icondll, -107);

			AccountManager.StatusChanged += new EventHandler<StatusUpdateEventArgs>(AccountManager_onStatusChange);
			
			//frequentUsers.JumpListItems.CollectionChanged += new NotifyCollectionChangedEventHandler(JumpList_removeItem);

			appJList.AddCustomCategories(frequentUsers);

			// Generate status points
			statusItems.Add(IMStatus.Available, createStatusItem("Available", "available"));
			statusItems.Add(IMStatus.Busy, createStatusItem("Busy", "busy"));
			statusItems.Add(IMStatus.Away, createStatusItem("Away", "away"));
			statusItems.Add(IMStatus.Invisible, createStatusItem("Appear offline", "invisible"));
			appJList.AddUserTasks(new JumpListSeparator());
			createStatusItem("Sign out", "offline");

			// TODO: Attempt a refresh before loading this stuff
			try	{
				appJList.Refresh();
				mSetup = true;
				Trace.WriteLine("SuperTaskbarManager successfully loaded");
			} catch (Exception e) {
				Trace.WriteLine("Unknown JumpList Error of type (" + e.GetType().FullName + ") : " + e.Message + e.StackTrace);
				Shutdown();
			}

			AccountManager_onStatusChange(null, null);
		}
		
		/// <summary>
		/// Removes all data used by the SuperTaskbar
		/// </summary>
		public static void Shutdown()
		{
			if (!mSetup)
				return;

			Trace.WriteLine("Unloading SuperTaskbarManager data.");
			AccountManager.StatusChanged -= new EventHandler<StatusUpdateEventArgs>(AccountManager_onStatusChange);

			statusItems.Clear();

			frequentUsers = null;
			appJList = null;
			mSetup = false;
			icoAvailable = null;
			icoAway = null;
			icoBusy = null;
			icoInvisible = null;
			icoOffline = null;
			statusItems = null;
			Trace.WriteLine("Unloaded all data used by the SuperTaskbarManager");
		}

		private static JumpListLink createStatusItem(string formalName, string internalName)
		{
			JumpListLink status = new JumpListLink(Assembly.GetCallingAssembly().Location, formalName);
			status.Arguments = "-status:" + internalName + " -jumplist";

			appJList.AddUserTasks(status);

			return status;
		}

		/// <summary>
		/// Gets whether or not this manager has succeeded in setup
		/// </summary>
		public static bool IsSetup
		{
			get {
				return mSetup;
			}
		}
		/// <summary>
		/// Gets whether or not this manager has attempted setup.
		/// </summary>
		public static bool AttemptedSetup
		{
			get {
				return mAttemptedSetup;
			}
		}

		private static void AccountManager_onStatusChange(object sender, StatusUpdateEventArgs e)
		{
			if (!mSetup)
				return;

			// Remove the dot icon from all icons first
			foreach (KeyValuePair<IMStatus, JumpListLink> link in statusItems)
			{
				link.Value.IconReference = new IconReference();
			}

			if (!AccountManager.Accounts.Any()) {
				if (icoOffline == null)
					icoOffline = Resources.offline_icon;
				TaskbarManager.Instance.SetOverlayIcon(icoOffline, "Offline");
			} else if (AccountManager.Status == IMStatus.Available) {
				if (icoAvailable == null)
					icoAvailable = Resources.available_icon;
				TaskbarManager.Instance.SetOverlayIcon(icoAvailable, "Available");
				statusItems[IMStatus.Available].IconReference = pointIcon;
			} else if (AccountManager.Status == IMStatus.Away) {
				if (icoAway == null)
					icoAway = Resources.away_icon;
				TaskbarManager.Instance.SetOverlayIcon(icoAway, "Away");
				statusItems[IMStatus.Away].IconReference = pointIcon;
			} else if (AccountManager.Status == IMStatus.Busy) {
				if (icoBusy == null)
					icoBusy = Resources.busy_icon;
				TaskbarManager.Instance.SetOverlayIcon(icoBusy, "Busy");
				statusItems[IMStatus.Busy].IconReference = pointIcon;
			} else if (AccountManager.Status == IMStatus.Invisible)	{
				if (icoInvisible == null)
					icoInvisible = Resources.invisible_icon;
				TaskbarManager.Instance.SetOverlayIcon(icoInvisible, "Invisible");
				statusItems[IMStatus.Invisible].IconReference = pointIcon;
			}

			appJList.Refresh();
		}
		
		// Variables
		private static bool mAttemptedSetup = false;
		private static bool mSetup = false;
		private static Icon icoAvailable;
		private static Icon icoAway;
		private static Icon icoBusy;
		private static Icon icoInvisible;
		private static Icon icoOffline;
		private static JumpList appJList;
		private static JumpListCustomCategory frequentUsers;
		private static IconReference pointIcon;
		private static Dictionary<IMStatus, JumpListLink> statusItems;
	}
}