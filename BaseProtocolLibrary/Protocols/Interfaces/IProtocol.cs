using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Events;
using System.ComponentModel;

namespace InstantMessage
{
	public interface IProtocol
	{
		void BeginLogin();
		void EndLogin();
		void Disconnect();
		IMStatus Status
		{
			get;
		}
		string Username
		{
			get;
			set;
		}
		string Password
		{
			get;
			set;
		}

		event EventHandler LoginCompleted;
		event EventHandler<IMErrorEventArgs> ErrorOccurred;
		event EventHandler<IMDisconnectEventArgs> onDisconnect;
		event EventHandler<IMFriendEventArgs> ContactStatusChange;
		event EventHandler<IMMessageEventArgs> onMessageReceive;
		event PropertyChangedEventHandler PropertyChanged;
	}
}