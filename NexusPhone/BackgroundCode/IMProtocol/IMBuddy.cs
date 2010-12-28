using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NexusPhone
{
	public delegate void GenericEvent();
	public class IMBuddy : INotifyPropertyChanged
	{
		/// <summary>
		/// Used in DesignMode Only
		/// </summary>
		public IMBuddy()
		{
#if DEBUG
			mUsername = "[Username]";
			mStatusMessage = "[Test Long Status Message Here]";
			mMessages.Add(new IMMessage(this, "[Test Message]"));
#endif
		}
		internal IMBuddy(CloudHostedProtocol protocol)
		{
			mProtocol = protocol;
			mStatus = IMBuddyStatus.Offline;
		}

		public void ShowWindow()
		{
			WindowSystem.OpenChatWindow(this);
		}
		public void SendMessage(IMMessage message)
		{
			mProtocol.SendMessage(message);

			mMessages.Add(message);

			NotifyPropertyChanged("Messages");
			NotifyPropertyChanged("UnreadMessages");
		}
		public void ReceiveMessage(IMMessage message)
		{
			mMessages.Add(message);

			NotifyPropertyChanged("Messages");
			NotifyPropertyChanged("UnreadMessages");
		}

		protected void NotifyPropertyChanged(String propertyName)
		{
			if (PropertyChanged != null)
			{
				if (Dispatcher != null)
					Dispatcher.BeginInvoke(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
				else
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		// Properties
		public string Username
		{
			get {
				return mUsername;
			}
			set {
				if (value != mUsername)
				{
					mUsername = value;

					NotifyPropertyChanged("Username");
				}
			}
		}
		public string StatusMessage
		{
			get	{
				return mStatusMessage;
			}
			set	{
				if (value != mStatusMessage)
				{
					mStatusMessage = value;

					NotifyPropertyChanged("StatusMessage");
				}
			}
		}
		public IMBuddyStatus Status
		{
			get {
				return mStatus;
			}
			set {
				if (value != mStatus)
				{
					mStatus = value;

					NotifyPropertyChanged("Status");
				}
			}
		}
		public Guid Guid
		{
			get	{
				return mGuid;
			}
		}
		public ImageSource Avatar
		{
			get {
				BitmapImage image = new BitmapImage();
				image.UriSource = new Uri("/ApplicationIcon.png", UriKind.Relative);

				return image;
			}
		}
		public IEnumerable<IMMessage> Messages
		{
			get {
				return mMessages;
			}
		}
		public IEnumerable<IMMessage> UnreadMessages
		{
			get {
				return mMessages.Where(im => !im.Read);
			}
		}
		public Dispatcher Dispatcher
		{
			get;
			set;
		}

		// Events
		public event PropertyChangedEventHandler PropertyChanged;

		// Variables
		private IMBuddyStatus mStatus;
		private CloudHostedProtocol mProtocol;
		private string mUsername;
		private string mStatusMessage;
		private Guid mGuid;
		private List<IMMessage> mMessages = new List<IMMessage>();
	}

	public enum IMBuddyStatus
	{
		Available,
		Away,
		Busy,
		Offline
	}
}