using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Phone.Notification;
using NexusPhone.NexusCore;
using NexusPhone.WebIM;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace NexusPhone
{
	internal class NewContactEventArgs : EventArgs
	{
		public IMBuddy Buddy
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Mediates all contact with the NexusCore
	/// </summary>
	internal static class AccountManager
	{
		/// <summary>
		/// Initiates all events and data needed for cloud communication. Must be called before any other methods
		/// </summary>
		public static void Setup()
		{
			mProtocols = new List<CloudHostedProtocol>();

			// Construct the binding
			CustomBinding binding = new CustomBinding();
			binding.Elements.Add(new HttpCookieContainerBindingElement());
			binding.Elements.Add(new TextMessageEncodingBindingElement());
			binding.Elements.Add(new FixedHttpTransportBindingElement());
			
			mCoreClient = new CoreServiceClient(); //new CoreServiceClient(binding, new EndpointAddress("http://core.nexus-im.com/CoreService/winphone"));

			mCoreClient.LoginCompleted += new EventHandler<AsyncCompletedEventArgs>(CoreService_LoginCompleted);
			mCoreClient.GetAccountsCompleted += new EventHandler<GetAccountsCompletedEventArgs>(CoreService_GetAccountsCompleted);
			mCoreClient.CookieContainer = new System.Net.CookieContainer();

			mWebIMClient = new WebIMWinPhoneClient();
			mWebIMClient.CookieContainer = mCoreClient.CookieContainer;
			mWebIMClient.StartPushStreamCompleted += new EventHandler<AsyncCompletedEventArgs>(mWebIMClient_StartPushStreamCompleted);
			mWebIMClient.PreconfiguredLoginCompleted += new EventHandler<AsyncCompletedEventArgs>(mWebIMClient_PreconfiguredLoginCompleted);
			mWebIMClient.SendMessageCompleted += new EventHandler<AsyncCompletedEventArgs>(mWebIMClient_SendMessageCompleted);
			mWebIMClient.StartSessionCompleted += new EventHandler<AsyncCompletedEventArgs>(mWebIMClient_StartSessionCompleted);

			SetupPushChannel();

			mCoreContext = new OperationContext(mCoreClient.InnerChannel);

			mSetup = true;
		}

		
		public static void Shutdown()
		{
			mNotifyChannel.UnbindToShellTile();
			mNotifyChannel.UnbindToShellToast();

			if (mNotifyChannel != null)
				mNotifyChannel.Close();
		}

		public static void Login(string username, string password)
		{
			if (!mSetup)
				throw new InvalidOperationException("AccountManager.Setup must be called first");

			mCoreClient.LoginAsync(username, password);
		}
		public static void Resume()
		{
			mWebIMClient.StartSessionAsync();
		}
		public static void LoginToWebIM()
		{
			mWebIMClient.StartSessionAsync();
		}
		private static void SetupPushChannel()
		{
			mNotifyChannel = HttpNotificationChannel.Find(mHttpNotifyChannelName);
			bool mNewChannel = false;

			if (mNotifyChannel == null) {
				mNotifyChannel = new HttpNotificationChannel(mHttpNotifyChannelName);
				mNewChannel = true;
			}
			mNotifyChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(NotifyChannel_ShellToastNotificationReceived);
			mNotifyChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(NotifyChannel_ChannelUriUpdated);
			mNotifyChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(NotifyChannel_ExceptionOccurred);
			mNotifyChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(NotifyChannel_NotificationReceived);

			if (mNewChannel)
			{
				mNotifyChannel.Open();
				mNotifyChannel.BindToShellToast();
				mNotifyChannel.BindToShellTile(new Collection<Uri>() { new Uri("http://192.168.2.35"), new Uri("http://localhost") });
			}
		}

		public static List<CloudHostedProtocol> Accounts
		{
			get {
				return mProtocols;
			}
		}
		public static WebIMWinPhoneClient WebIMService
		{
			get	{
				return mWebIMClient;
			}
		}
		public static string SessionCookieName
		{
			get	{
				return "NexusCore_SessionId";
			}
		}
		public static bool CanResume()
		{
			return DefinedSettings.CoreSessionId != null;
		}

		#region HttpNotificationChannel Events

		private static void NotifyChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
		{
			Debug.WriteLine("Newly created HttpNotificationChannel Uri: " + e.ChannelUri);
			mWebIMClient.StartPushStreamAsync(PushChannelType.MicrosoftPN, e.ChannelUri);
			mPushChannelUriSubmitted = true;
		}
		private static void NotifyChannel_ExceptionOccurred(object sender, NotificationChannelErrorEventArgs e)
		{
			mNotifyChannel.Close();
			mNotifyChannel.Open();
			if (Debugger.IsAttached)
				Debugger.Break();
		}
		private static void NotifyChannel_NotificationReceived(object sender, HttpNotificationEventArgs e)
		{
			DeserializeAsJson(e.Notification.Body);
		}
		private static void DeserializeAsJson(Stream body)
		{
			StreamReader reader = new StreamReader(body);
			string message = reader.ReadToEnd();

			JArray json = JArray.Parse(message);

			foreach (JObject jobject in json)
			{
				switch (((JValue)jobject["_type"]).Value.ToString())
				{
					case "nc":
						{
							string username = jobject["Username"].ValueToString();
							string status = jobject["Availability"].ValueToString();
							int protocolId = Convert.ToInt32(jobject["ProtocolId"].ValueToString());

							IMBuddy buddy = PushPipeline.HandleNewContactMessage(username, (IMBuddyStatus)Enum.Parse(typeof(IMBuddyStatus), status, true), protocolId);

							if (onNewContact != null)
								onNewContact(null, new NewContactEventArgs() { Buddy = buddy });

							break;
						}
					case "su":
						{
							string username = jobject["Username"].ValueToString();
							string status = jobject["Availability"].ValueToString();
							int protocolId = Convert.ToInt32(jobject["ProtocolId"].ValueToString());

							PushPipeline.HandleStatusUpdateMessage(username, protocolId, (IMBuddyStatus)Enum.Parse(typeof(IMBuddyStatus), status, true));

							break;
						}
					case "msg":
						{
							string sender = jobject["Sender"].ValueToString();
							string messagebody = jobject["MessageBody"].ValueToString();
							int protocolId = Convert.ToInt32(jobject["ProtocolId"].ValueToString());

							PushPipeline.HandleChatMessageMessage(sender, messagebody, protocolId);

							break;
						}
				}
			}
		}
		private static void DeserializeAsXml(Stream body)
		{
			XNamespace xmlns = "NXNotification";
			XDocument doc = XDocument.Load(body);

			foreach (var element in doc.Element(xmlns + "RawNotification").Elements())
			{
				switch (element.Name.LocalName)
				{
					case "ContactUpdate":
						{
							XElement user = element.Element(xmlns + "Username");
							XElement pId = element.Element(xmlns + "ProtocolId");
							XElement status = element.Element(xmlns + "Availability");
							int protocolId = Convert.ToInt32(pId.Value);

							IMBuddy buddy = PushPipeline.HandleNewContactMessage(user.Value, (IMBuddyStatus)Enum.Parse(typeof(IMBuddyStatus), status.Value, true), protocolId);

							if (onNewContact != null)
								onNewContact(null, new NewContactEventArgs() { Buddy = buddy });

							break;
						}
					case "StatusChange":
						{
							XElement user = element.Element(xmlns + "Username");
							XElement pId = element.Element(xmlns + "ProtocolId");
							XElement status = element.Element(xmlns + "Availability");
							int protocolId = Convert.ToInt32(pId.Value);

							PushPipeline.HandleStatusUpdateMessage(user.Value, protocolId, (IMBuddyStatus)Enum.Parse(typeof(IMBuddyStatus), status.Value, true));

							break;
						}
					case "ChatMessage":
						{
							XElement source = element.Element(xmlns + "Sender");
							XElement pId = element.Element(xmlns + "ProtocolId");
							XElement msg = element.Element(xmlns + "MessageBody");
							int protocolId = Convert.ToInt32(pId.Value);

							PushPipeline.HandleChatMessageMessage(source.Value, msg.Value, protocolId);

							break;
						}
				}
			}
		}
		private static void NotifyChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
		{

		}

		#endregion

		#region CoreServiceClient Events

		private static void CoreService_LoginCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (mNotifyChannel.ChannelUri != null && !mPushChannelUriSubmitted)
			{
				Debug.WriteLine("Pre-existing HttpNotificationChannel Uri: " + mNotifyChannel.ChannelUri);
				mWebIMClient.StartPushStreamAsync(PushChannelType.MicrosoftPN, mNotifyChannel.ChannelUri);
				mPushChannelUriSubmitted = true;
			}
			
			mCoreClient.GetAccountsAsync();
			foreach (Cookie cookie in mCoreClient.CookieContainer.GetCookies(new Uri("http://core.nexus-im.com")))
			{
				if (cookie.Name == SessionCookieName)
				{
					mSessionId = cookie.Value;
					DefinedSettings.CoreSessionId = mSessionId;
					break;
				}
			}
		}
		private static void CoreService_GetAccountsCompleted(object sender, GetAccountsCompletedEventArgs e)
		{
			mProtocols.Clear();
			mProtocols.AddRange(e.Result.Select(ai => new CloudHostedProtocol(ai)));

			if (mCompletedPushStreamSetup)
			{
				LoginToWebIM();
			}
			mCompletedGetAccounts = true;
		}
		#endregion

		#region WebIMService Events

		private static void mWebIMClient_StartPushStreamCompleted(object sender, AsyncCompletedEventArgs e)
		{
			mCompletedPushStreamSetup = true;

			if (mCompletedGetAccounts)
			{
				LoginToWebIM();
			}
		}
		private static void mWebIMClient_PreconfiguredLoginCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (LoginCompleted != null)
				LoginCompleted();
		}
		private static void mWebIMClient_SendMessageCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (e.UserState != null && e.UserState is IMMessage)
			{
				IMMessage message = e.UserState as IMMessage;
				
				if (message.OnDelivery != null)
					message.OnDelivery();
			}
		}
		private static void mWebIMClient_StartSessionCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (LoginCompleted != null)
				LoginCompleted();
		}

		#endregion

		public static event EventHandler<NewContactEventArgs> onNewContact;
		public static event GenericEvent LoginCompleted;

		private static string mSessionId;
		private static List<CloudHostedProtocol> mProtocols;
		private static HttpNotificationChannel mNotifyChannel;
		private static string mHttpNotifyChannelName = "nexusphone_push";
		private static CoreServiceClient mCoreClient;
		private static WebIMWinPhoneClient mWebIMClient;
		private static OperationContext mCoreContext;
		private static bool mSetup;
		private static bool mCompletedPushStreamSetup;
		private static bool mCompletedGetAccounts;
		private static bool mPushChannelUriSubmitted;
	}
}