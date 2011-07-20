using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using InstantMessage;
using InstantMessage.Events;
using InstantMessage.Protocols.Irc;
using Microsoft.WindowsAPICodePack.Net;
using NexusIM.Misc;
using NexusIM.Windows;
using NexusIM.Controls;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace NexusIM.Managers
{
	class StatusUpdateEventArgs : EventArgs
	{
		public IMStatus NewStatus
		{
			get;
			internal set;
		}
		public IMStatus OldStatus
		{
			get;
			internal set;
		}
	}
	/// <summary>
	/// Controls the status and all registered protocols
	/// </summary>
	internal static class AccountManager
	{
		public static void Setup()
		{
			NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);

			Trace.WriteLine("AccountManager loaded and ready");
			Trace.WriteLine("Connectivity Status: " + NetworkListManager.Connectivity.ToString());
			mConnected = true;

			Accounts.CollectionChanged += new NotifyCollectionChangedEventHandler(Accounts_CollectionChanged);
			IMProtocol.AnyErrorOccurred += new EventHandler<IMErrorEventArgs>(IMProtocol_AnyErrorOccurred);
			IMProtocol.AnyLoginCompleted += new EventHandler(IMProtocol_AnyLoginCompleted);

			SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
			SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
		}

		[Obsolete("Use Accounts.Add instead", false)]
		public static void AddNewAccount(IMProtocol protocol)
		{
			AddNewAccount(new IMProtocolWrapper() { Protocol = protocol, Enabled = true, IsReady = false});
		}
		[Obsolete("Use Accounts.Add instead", false)]
		public static void AddNewAccount(IMProtocolWrapper extraData)
		{
			if (accounts.Contains(extraData))
				throw new ArgumentException("This protocol has already been added");

			extraData.IsReady = true;
			accounts.Add(extraData);

			if (Connected && extraData.Enabled && extraData.Protocol.ProtocolStatus == IMProtocolStatus.Offline)
			{
				extraData.Protocol.BeginLogin();
			}
		}

		// System Event Handlers
		private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			Trace.WriteLine("AccountManager: SessionSwitch - " + e.Reason);

			switch (e.Reason)
			{
				case SessionSwitchReason.SessionLock:
				case SessionSwitchReason.RemoteDisconnect:
					Status = IMStatus.Away;
					break;
				case SessionSwitchReason.SessionUnlock:
				case SessionSwitchReason.RemoteConnect:
				case SessionSwitchReason.ConsoleConnect:
					Status = IMStatus.Available;
					break;
			}
		}
		private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
		{
			Trace.WriteLine(String.Format("AccountManager: Network Availability Change (is available: {0})", e.IsAvailable));
		}
		private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
			switch (e.Mode)
			{
				case PowerModes.Suspend:
					AccountManager.Connected = false;
					break;
				case PowerModes.Resume:
					AccountManager.Connected = true;
					break;
			}
		}

		// Account Status Management Functions
		private static void Nic_AvailabilityChange(object sender, NetworkAvailabilityEventArgs e)
		{
			Trace.WriteLine("AccountManager: NIC Availability Change (is available: " + e.IsAvailable + ")");
		}
		private static void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback((object obj) => {
				if (e.NewItems != null)
				{
					foreach (IMProtocolWrapper extraData in e.NewItems)
					{
						extraData.IsReady = true;
						// Setup Event Handlers
						extraData.PropertyChanged += new PropertyChangedEventHandler(IndividualProtocol_PropertyChanged);

						if (extraData.Protocol is IRCProtocol)
						{
							extraData.Protocol.LoginCompleted += new EventHandler(IrcProtocol_LoginCompleted);

							IRCProtocol protocol = (IRCProtocol)extraData.Protocol;
							protocol.OnJoinChannel += new EventHandler<IMChatRoomEventArgs>(IrcProtocol_OnJoinChannel);
							protocol.NicknameCollisionHandler = new IRCProtocol.DuplicateNickname(IrcProtocol_OnNickCollision);
						}

						ConnectIfNeeded(extraData); // Now connect
					}
				}
			}));
		}
		private static void ConnectIfNeeded(IMProtocolWrapper extraData)
		{
			if (!Connected)
				return;
			
			if (extraData.Enabled != (extraData.Protocol.ProtocolStatus != IMProtocolStatus.Offline))
			{
				if (extraData.Enabled)
					extraData.Protocol.BeginLogin();
				else
					extraData.Protocol.Disconnect();
			}
		}
		private static void ConnectIfNeeded(object threadState)
		{
			ConnectIfNeeded((IMProtocolWrapper)threadState);
		}
		private static void IndividualProtocol_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Enabled":
					ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectIfNeeded), sender); // Don't Queue this entire method because something might change that we don't care about
					break;
			}
		}
		private static void IMProtocol_AnyLoginCompleted(object sender, EventArgs e)
		{
			IMProtocol protocol = (IMProtocol)sender;
			IMProtocolWrapper wrapper = Accounts.First(p => p.Protocol == protocol);

			wrapper.ErrorBackoff = null;
		}

		// IRC Protocol custom event handlers
		private static void IrcProtocol_LoginCompleted(object sender, EventArgs e)
		{
			IRCProtocol protocol = (IRCProtocol)sender;

			string autoexecute;
			if (protocol.ConfigurationSettings.TryGetValue("autoexecute", out autoexecute))
			{
				StringBuilder sb = new StringBuilder(32);

				StringReader reader = new StringReader(autoexecute);

				while (reader.Peek() != -1)
				{
					string command = reader.ReadLine();
					protocol.SendRawMessage(command);
				}
			}
		}
		private static void IrcProtocol_OnJoinChannel(object sender, IMChatRoomEventArgs e)
		{
			IRCChannel channel = (IRCChannel)e.ChatRoom;
			
			WindowSystem.OpenGroupChatWindow(channel);
		}
		private static string IrcProtocol_OnNickCollision(IRCProtocol protocol, string originalNick)
		{
			// Show a warning in the contact list
			WindowSystem.DispatcherInvoke(() => {
				CLErrorBox box = new CLErrorBox();
				box.PopulateProtocolControls(protocol);
				box.ErrorString.Text = "Somebody be stealing your nicknamez.";
				box.AddLink("Recover", (t, g) => {});
				box.AddLink("Change", (t, g) => {});
				WindowSystem.ContactListWindow.OpenErrorBox(box);
			});

			return originalNick + "_";
		}

		private static void IMProtocol_AnyErrorOccurred(object sender, IMErrorEventArgs e)
		{
			IMProtocol protocol = (IMProtocol)sender;
			IMProtocolWrapper wrapper = Accounts.First(p => p.Protocol == protocol);

			if (e is BadCredentialsEventArgs)
			{
				Trace.WriteLine(string.Format("Error: Protocol {0} [{1}] reported bad credentials for account.", protocol.Username, protocol.Protocol));

				WindowSystem.DispatcherInvoke(() => {
					UserCredentialsWindow window = new UserCredentialsWindow();
					window.Show();
				});
			} else if (e is SocketErrorEventArgs) {
				SocketErrorEventArgs socketErrors = (SocketErrorEventArgs)e;
				SocketException exception = socketErrors.Exception;
				
				StringBuilder traceString = new StringBuilder();
				traceString.AppendFormat("Error: Protocol {0} [{1}] reported a socket error. ", protocol.Username, protocol.Protocol);
				traceString.Append(exception.SocketErrorCode);
				traceString.Append(' ');

				WindowSystem.ContactListWindow.InsertErrorBox(wrapper, socketErrors);

				switch (exception.SocketErrorCode)
				{
					case SocketError.TimedOut:
					case SocketError.ConnectionReset:
						traceString.Append("Response: Internalize; Could be a connectivity problem.");

						if (wrapper.ErrorBackoff == null)
							wrapper.ErrorBackoff = new ProtocolErrorBackoff(wrapper);

						break;
					default:
						traceString.Append("Response: Alert User");

						WindowSystem.DispatcherInvoke(() => {
							SocketErrorTrayTip tip = new SocketErrorTrayTip();
							tip.PopulateControls(exception, protocol);
							WindowSystem.SysTrayIcon.ShowCustomBalloon(tip, System.Windows.Controls.Primitives.PopupAnimation.Slide, null);
						});

						break;
				}
				traceString.AppendLine();

				Trace.WriteLine(traceString);
			} else if (e is CertErrorEventArgs) { // Certificate Errors warn us about problems with the transport layer authentication
				CertErrorEventArgs certError = (CertErrorEventArgs)e;
				// Do some logging
				StringBuilder error = new StringBuilder();
				error.Append("AccountManager: X.509 Certificate Chain Error");
				error.AppendFormat(" ({0}) ", certError.PolicyErrors.ToString());
				error.AppendLine("Certificate Chain:");

				foreach (var cert in certError.Chain.ChainElements)
					error.AppendLine("\t" + cert.Certificate.SubjectName.Name);

				Trace.Write(error);

				// Now verify the thumbprint
				string thumbprint = certError.Certificate.GetCertHashString();
				string prevprint;
				if (protocol.ConfigurationSettings.TryGetValue("sslthumbprint", out prevprint)) // Check to see if we already have a thumbprint
				{
					if (thumbprint != prevprint)
					{
						// High priority problem - New SSL Certificate
						byte[] certData = certError.Certificate.Export(X509ContentType.Cert); // We have to export it because the X509Certificate class has some funky CLR Security settings on it prevent cross thread access
						WindowSystem.DispatcherInvoke(() =>
							{
								CLErrorBox box = new CLErrorBox();
								box.PopulateProtocolControls(protocol);
								box.SetErrorString("The server's certificate has changed.");
								box.AddLink("Accept", (t, g) => {} );
								box.AddLink("View Details", (t, g) => OpenCertDetailsWindow(certData));

								WindowSystem.ContactListWindow.OpenErrorBox(box);
							});
					} else
						certError.Continue = true; // The user previously accepted this certificate
				}
			} else
				Trace.WriteLine("AccountManager: An IMProtocol.Error event was thrown that the AccountManager can not handle (Type: " + e.GetType().FullName + ")");
		}
		private static void OpenCertDetailsWindow(byte[] certdata)
		{
			X509Certificate2 certificate = new X509Certificate2(certdata);

			TLSCertificateDetails window = new TLSCertificateDetails();
			window.PopulateControls(certificate);
			window.Owner = WindowSystem.ContactListWindow;
			window.Show();
		}

		public static event EventHandler<StatusUpdateEventArgs> StatusChanged;
		public static event PropertyChangedEventHandler PropertyChanged;

		private static void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(null, new PropertyChangedEventArgs(propertyName));
		}

		public static bool IsConnectedToInternet()
		{
			return true; // NetworkListManager.IsConnectedToInternet;
		}

		/// <summary>
		/// Returns a list of protocols currently setup by the user
		/// </summary>
		public static ProtocolCollection Accounts
		{
			get {
				if (accounts == null)
					accounts = new ProtocolCollection();
				return accounts;
			}
		}
		public static string StatusMessage
		{
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		/// <summary>
		/// Changes the status for all accounts to the specified status.
		/// </summary>
		public static IMStatus Status
		{
			get {
				return mGeneralStatus;
			}
			set	{
				if (mGeneralStatus != value)
				{
					IMStatus oldStatus = mGeneralStatus;
					mGeneralStatus = value;
#if DEBUG
					StackTrace trace = new StackTrace();
					Debug.WriteLine(String.Format("AccountManager: Status Change ({0} to {1}) requested by {2}.{3}", oldStatus, value, trace.GetFrame(1).GetMethod().DeclaringType.FullName, trace.GetFrame(1).GetMethod().Name));
#endif

					foreach (IMProtocolWrapper account in accounts)
					{
						if (!account.Enabled)
							continue;

						account.Protocol.Status = mGeneralStatus;
					}

					NotifyPropertyChanged("Status");
					if (StatusChanged != null)
						StatusChanged(null, new StatusUpdateEventArgs() { OldStatus = oldStatus, NewStatus = value });
				}
			}
		}
		public static bool Connected
		{
			get	{
				return mConnected;
			}
			set	{
				if (mConnected != value)
				{
					mConnected = value;

					foreach (var account in Accounts)
					{
						if (account.Enabled && account.Protocol.ProtocolStatus == IMProtocolStatus.Online)
						{
							if (value)
								account.Protocol.BeginLogin();
							else
								account.Protocol.Disconnect();
						}
					}
					NotifyPropertyChanged("Connected");
				}
			}
		}

		private static IMStatus mGeneralStatus = IMStatus.Available;
		private static ProtocolCollection accounts;
		private static bool mConnected;
	}
}