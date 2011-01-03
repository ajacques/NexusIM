using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using InstantMessage;
using NexusCore.DataContracts;
using InstantMessage.Events;

namespace NexusCore.PushChannel
{
	internal enum PushChannelErrorState
	{
		None,
		Expired,
		Inactive
	}

	internal class PushChannelContext
	{
		public PushChannelContext(PushChannelType channelType, Uri channelUri)
		{
			mChannelType = channelType;
			mChannelUri = channelUri;
			mProtocols = new List<Tuple<IMProtocol, int>>();

			if (channelType == PushChannelType.MicrosoftPN)
				mChannel = new MicrosoftPNChannel(mChannelUri);
		}
		internal void ChangeState(PushChannelErrorState state)
		{

		}

		public void PushNewContacts(IEnumerable<IMBuddy> contacts, int protocolId)
		{
			foreach (var buddy in contacts)
				PushChannel.PushMessage(new NewContactMessage(buddy, protocolId), mChannel);
		}

		public void AddAccount(IMProtocol protocol, int protocolId)
		{
			protocol.ContactList.CollectionChanged += new NotifyCollectionChangedEventHandler(ContactList_CollectionChanged);
			protocol.ContactStatusChange += new EventHandler<IMFriendEventArgs>(ContactStatusChange);
			protocol.onMessageReceive += new EventHandler<IMMessageEventArgs>(OnMessageReceive);

			mProtocols.Add(Tuple.Create<IMProtocol, int>(protocol, protocolId));
		}
		private void OnMessageReceive(object sender, IMMessageEventArgs e)
		{
			if (mChannelUri != null)
				PushChannel.PushMessage(new NewIMMessage(DatabaseIdFor(sender as IMProtocol), e.Sender.Username, e.Message), mChannel);
		}
		private void ContactStatusChange(object sender, IMFriendEventArgs e)
		{
			if (mChannelUri != null)
				PushChannel.PushMessage(new ContactStatusChangeMessage(DatabaseIdFor(e.Buddy.Protocol as IMProtocol), e.Buddy), mChannel);
		}
		private void ContactList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (mChannelUri != null)
			{
				var newItems = e.NewItems.Cast<IMBuddy>();

				PushNewContacts(newItems, DatabaseIdFor(newItems.First().Protocol as IMProtocol));
			}
		}
		private int DatabaseIdFor(IMProtocol protocol)
		{
			return mProtocols.Where(t => t.Item1 == protocol).Select(t => t.Item2).FirstOrDefault();
		}

		private List<Tuple<IMProtocol, int>> mProtocols;
		private IPushChannel mChannel;
		private PushChannelType mChannelType;
		private Uri mChannelUri;
	}
}