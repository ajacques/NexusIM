using System.ComponentModel;
using NexusIM.Misc;

namespace InstantMessage
{
	public enum ProtocolSource
	{
		LocalSettingFile,
		NexusCore
	}
	public sealed class IMProtocolWrapper : INotifyPropertyChanged
	{
		public IMProtocol Protocol
		{
			get;
			set;
		}
		public bool Enabled
		{
			get {
				return mEnabled;
			}
			set	{
				if (mEnabled != value)
				{
					mEnabled = value;

					NotifyPropertyChanged("Enabled");
				}
			}
		}
		public int DatabaseId
		{
			get;
			set;
		}
		public bool IsReady
		{
			get;
			set;
		}
		public bool AutoConnect
		{
			get	{
				return mAutoConnect;
			}
			set	{
				if (mAutoConnect != value)
				{
					mAutoConnect = value;

					NotifyPropertyChanged("AutoConnect");
				}
			}
		}
		public ProtocolSource Source
		{
			get;
			set;
		}
		internal ProtocolErrorBackoff ErrorBackoff
		{
			get;
			set;
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		// Variables
		private bool mEnabled;
		private bool mAutoConnect;
	}
}