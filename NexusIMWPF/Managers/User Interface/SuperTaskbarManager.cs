using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using NexusIM.Managers;
using System.Drawing;
using System.Windows;
using InstantMessage;

namespace NexusIM.Managers
{
	static class SuperTaskbarManager
	{
		internal class userJListDataComparer : IComparer<userJListData>
		{
			public int Compare(userJListData a, userJListData b)
			{
				return b.mFrequency.CompareTo(a.mFrequency);
			}
		}
		internal class userJListData
		{
			public userJListData(JumpListLink link, Dictionary<string, string> specs)
			{
				mLink = link;
				mSpecs = specs;
			}
			public userJListData(IMBuddy buddy)
			{
				mBuddy = buddy;
			}
			public userJListData(IMBuddy buddy, JumpListLink link)
			{
				mLink = link;
				mBuddy = buddy;
				isVisible = true;
			}
			public bool isVisible = false;
			public int mFrequency = 1;
			public JumpListLink mLink = null;
			public IMBuddy mBuddy = null;
			public Dictionary<string, string> mSpecs = new Dictionary<string, string>();
		}
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

			usersSort = new List<userJListData>();
			statusItems = new Dictionary<IMStatus, JumpListLink>();
			statusIcons = new Dictionary<IMBuddyStatus, IconReference>();
			pointIcon = new IconReference(icondll, -107);

			AccountManager.StatusChanged += new EventHandler<StatusUpdateEventArgs>(AccountManager_onStatusChange);
			thisExe = Assembly.GetExecutingAssembly();
			
			//frequentUsers.JumpListItems.CollectionChanged += new NotifyCollectionChangedEventHandler(JumpList_removeItem);

			appJList.AddCustomCategories(frequentUsers);

			// Generate status points
			statusItems.Add(IMStatus.AVAILABLE, createStatusItem("Available", "available"));
			statusItems.Add(IMStatus.BUSY, createStatusItem("Busy", "busy"));
			statusItems.Add(IMStatus.AWAY, createStatusItem("Away", "away"));
			statusItems.Add(IMStatus.INVISIBLE, createStatusItem("Appear offline", "invisible"));
			appJList.AddUserTasks(new JumpListSeparator());
			statusItems.Add(IMStatus.OFFLINE, createStatusItem("Sign off...", "offline"));

			statusIcons.Clear();
			statusIcons.Add(IMBuddyStatus.Available, new IconReference(icondll, 102));
			statusIcons.Add(IMBuddyStatus.Busy, new IconReference(icondll, 104));
			statusIcons.Add(IMBuddyStatus.Idle, new IconReference(icondll, 103));
			statusIcons.Add(IMBuddyStatus.Away, new IconReference(icondll, 103));
			statusIcons.Add(IMBuddyStatus.Offline, new IconReference(icondll, 105));

			// TODO: Attempt a refresh before loading this stuff
			try	{
				appJList.Refresh();
				mSetup = true;
				Trace.WriteLine("SuperTaskbarManager successfully loaded");
			} catch (Exception e) {
				Trace.WriteLine("Unknown JumpList Error of type (" + e.GetType().FullName + ") : " + e.Message + e.StackTrace);
				Shutdown();
			}
		}

		public static void IncrementBuddyUsageStat(IMBuddy buddy) {}

		/// <summary>
		/// Removes all data used by the SuperTaskbar
		/// </summary>
		public static void Shutdown()
		{
			if (!mSetup)
				return;

			Trace.WriteLine("Unloading SuperTaskbarManager data.");
			AccountManager.StatusChanged -= new EventHandler<StatusUpdateEventArgs>(AccountManager_onStatusChange);
			InterfaceManager.onWindowOpen -= new EventHandler(IMBuddy_windowOpen);

			// Change all the frequent user icons to say they are offline. Change this to an unknown icon?
			foreach (userJListData item in usersSort)
			{
				item.mLink.IconReference = statusIcons[IMBuddyStatus.Offline];
			}

			statusIcons.Clear();
			statusItems.Clear();

			frequentUsers = null;
			appJList = null;
			mSetup = false;
			icoAvailable = null;
			icoAway = null;
			icoBusy = null;
			icoInvisible = null;
			icoOffline = null;
			statusIcons = null;
			statusItems = null;
			Trace.WriteLine("Unloaded all data used by the SuperTaskbarManager");
		}

		/// <summary>
		/// Causes the SuperTaskbar to be updated.
		/// </summary>
		public static void TriggerUpdate()
		{
			AccountManager_onStatusChange(null, null);
		}

		private static JumpListLink createBuddyQLink(IMBuddy buddy)
		{
			JumpListLink link = new JumpListLink(Environment.CurrentDirectory, buddy.DisplayName);
			link.Arguments = "-protocol:" + buddy.Protocol.Protocol + " -account:" + buddy.Protocol.Username + " -username:" + buddy.Username + " -sendmessage -jumplist";

			return link;
		}
		/// <summary>
		/// Generates a buddy quick link from data retrieved from the configuration file.
		/// </summary>
		/// <param name="username">The username of the buddy to use when clicked</param>
		/// <param name="protocol">The protocol type that this buddy is on</param>
		/// <param name="account"></param>
		/// <param name="tempname"></param>
		/// <returns></returns>
		private static JumpListLink createBuddyQLink(string username, string protocol, string account, string tempname)
		{
			JumpListLink link = new JumpListLink(Environment.CurrentDirectory, tempname);
			link.IconReference = statusIcons[IMBuddyStatus.Offline];
			link.Arguments = "-protocol:" + protocol + " -account:" + account + " -username:" + username + " -sendmessage - jumplist";

			return link;
		}
		private static JumpListLink createStatusItem(string formalName, string internalName)
		{
			JumpListLink status = new JumpListLink(Environment.CurrentDirectory, formalName);
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

		/// <summary>
		/// This is used to respond to users removing items.
		/// </summary>
		/// <remarks>
		/// This uses modifications to the WindowsApiCodePack to work correctly.
		/// </remarks>
		private static void JumpList_removeItem(object sender, NotifyCollectionChangedEventArgs e)
		{
			try {
				if (e.Action == NotifyCollectionChangedAction.Remove)
				{
					foreach (int index in e.OldItems)
					{
						
						List<string> users = null; //IMSettings.SettingInterface.GetSettingList("taskbarusers");
						if (users == null)
						{
							users = new List<string>();
							//IMSettings.SetSettingList("taskbarusers", users);
						}
						try	{
							//IJumpListItem item = frequentUsers.JumpListItems.ElementAt(index);
							//users.Remove(users.Single(pt => pt.Contains("username:" + usersSort[index].mBuddy.Username)));
						} catch (Exception) {}
						IMSettings.Save();
						usersSort.RemoveAt(index);
					}
				}
			} catch (Exception) {} // don't let this fall back to the JumpList
		}
		private static void IMProtocol_onLogin(object sender, EventArgs e)
		{
			TriggerUpdate();

			IMProtocol protocol = (IMProtocol)sender;

			usersSort.ForEach(delegate(userJListData j)
			{
				if (protocol.ShortProtocol == j.mSpecs["protocol"] && protocol.Username == j.mSpecs["account"])
				{
					IMBuddy buddy = null;
					try	{
						buddy = protocol.ContactList.First(h => h.Username == j.mSpecs["username"]);
					} catch (Exception) {}
					if (buddy != null)
					{
						j.mBuddy = buddy;
						j.mLink.IconReference = statusIcons[buddy.Status];
					}
				}
			});
			try	{
				appJList.Refresh();
			} catch (Exception f) {
				Trace.TraceError("JumpList error in unexpected location (" + f.GetType().Name + "): " + f.Message);
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

			statusItems[IMStatus.OFFLINE].Title = "Sign off...";

			if (AccountManager.Status == IMStatus.OFFLINE || !AccountManager.Accounts.Any()) {
				statusItems[IMStatus.OFFLINE].Title = "Sign in...";

				if (icoOffline == null)
					icoOffline = new Icon(thisExe.GetManifestResourceStream("NexusIM.Resources.offline_icon.ico"));
				TaskbarManager.Instance.SetOverlayIcon(icoOffline, "Offline");
			} else if (AccountManager.Status == IMStatus.AVAILABLE) {
				if (icoAvailable == null)
					icoAvailable = new Icon(thisExe.GetManifestResourceStream("NexusIM.Resources.available_icon.ico"));
				TaskbarManager.Instance.SetOverlayIcon(icoAvailable, "Available");
				statusItems[IMStatus.AVAILABLE].IconReference = pointIcon;
			} else if (AccountManager.Status == IMStatus.AWAY) {
				if (icoAway == null)
					icoAway = new Icon(thisExe.GetManifestResourceStream("NexusIM.Resources.away_icon.ico"));
				TaskbarManager.Instance.SetOverlayIcon(icoAway, "Away");
				statusItems[IMStatus.AWAY].IconReference = pointIcon;
			} else if (AccountManager.Status == IMStatus.BUSY) {
				if (icoBusy == null)
					icoBusy = new Icon(thisExe.GetManifestResourceStream("NexusIM.Resources.busy_icon.ico"));
				TaskbarManager.Instance.SetOverlayIcon(icoBusy, "Busy");
				statusItems[IMStatus.BUSY].IconReference = pointIcon;
			} else if (AccountManager.Status == IMStatus.INVISIBLE)	{
				if (icoInvisible == null)
					icoInvisible = new Icon(thisExe.GetManifestResourceStream("NexusIM.Resources.invisible_icon.ico"));
				TaskbarManager.Instance.SetOverlayIcon(icoInvisible, "Invisible");
				statusItems[IMStatus.INVISIBLE].IconReference = pointIcon;
			}

			appJList.Refresh();
		}
		private static void IMBuddy_windowOpen(object sender, EventArgs e)
		{
			IMBuddy buddy = (IMBuddy)sender;

			var matches = from u in usersSort where u.mBuddy == buddy select new { Container = u };

			if (matches.Count() >= 1)
			{
				var match = matches.First();
				userJListData data = match.Container;
				data.mFrequency++;

				usersSort.Sort(new userJListDataComparer());
			} else {
				if (usersSort.Count <= 5)
				{
					JumpListLink item = createBuddyQLink(buddy);
					item.IconReference = statusIcons[buddy.Status];

					usersSort.Add(new userJListData(buddy, item));

					frequentUsers.AddJumpListItems(item);
					appJList.Refresh();

					List<string> users = null; //IMSettings.SettingInterface.GetSettingList("taskbarusers");
					if (users == null)
					{
						users = new List<string>();
						//IMSettings.SettingInterface.SetSettingList("taskbarusers", users);
					}
					users.Add("username:" + buddy.Username + ";protocol:" + buddy.Protocol.ShortProtocol + ";account:" + buddy.Protocol.Username + ";temp:" + buddy.DisplayName + ";count:1");
					//if (IMSettings.SettingInterface.AutoSave)
						//IMSettings.SettingInterface.Save();
				} else {
					usersSort.Add(new userJListData(buddy));
				}
			}
		}
		private static void IMBuddy_statusChange(object sender, EventArgs e)
		{
			IMBuddy buddy = (IMBuddy)sender;

			usersSort.ForEach(delegate(userJListData j)
			{
				userJListData item = null;
				try	{
					item = usersSort.First(h => h.mBuddy == buddy);
				} catch (Exception) {}
				if (item != null)
				{
					item.mLink.IconReference = statusIcons[buddy.Status];
				}
			});
			appJList.Refresh();
		}

		private static void loadFrequentUsers()
		{
			List<string> users = null; //IMSettings.SettingInterface.GetSettingList("taskbarusers");

			if (users == null)
				return;

			users.ForEach(delegate(string input)
			{
				string[] parts = input.Split(new string[] { ";" }, StringSplitOptions.None);

				Dictionary<string, string> dict = parts.ToDictionary(part => part.Split(new string[] { ":" }, StringSplitOptions.None)[0], part => part.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
				JumpListLink link = createBuddyQLink(dict["username"], dict["protocol"], dict["account"], dict["temp"]);
				userJListData data = new userJListData(link, dict);
				data.mFrequency = Convert.ToInt32(dict["count"]);

				usersSort.Add(data);
			});

			usersSort.Sort(new userJListDataComparer());

			usersSort.ForEach(delegate(userJListData data)
			{
				frequentUsers.AddJumpListItems(data.mLink);
			});
		}
		private static void saveFrequentUsers()
		{
			List<string> templist = new List<string>();

			usersSort.ForEach(delegate(userJListData item)
			{
				string genstring = "protocol:" + item.mBuddy.Protocol.Username + ";username:" + item.mBuddy.Protocol.ShortProtocol + ";username:" + item.mBuddy.Username + ";temp:" + item.mBuddy.DisplayName + ";count:" + item.mFrequency;
				templist.Add(genstring);
			});

			//IMSettings.SettingInterface.SetSettingList("taskbarusers", templist);
		}

		private static bool mAttemptedSetup = false;
		private static bool mSetup = false;
		private static Assembly thisExe;
		private static Icon icoAvailable;
		private static Icon icoAway;
		private static Icon icoBusy;
		private static Icon icoInvisible;
		private static Icon icoOffline;
		private static JumpList appJList;
		private static JumpListCustomCategory frequentUsers;
		private static IconReference pointIcon;
		private static List<userJListData> usersSort;
		private static Dictionary<IMStatus, JumpListLink> statusItems;
		private static Dictionary<IMBuddyStatus, IconReference> statusIcons;
	}
}