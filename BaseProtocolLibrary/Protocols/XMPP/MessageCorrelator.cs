using System;
using System.Collections.Generic;
using InstantMessage.Protocols.XMPP.Messages;

namespace InstantMessage.Protocols.XMPP
{
	internal delegate void HandleResponse(IqMessage response, object userState);

	internal sealed class MessageCorrelator
	{
		public MessageCorrelator()
		{
			responseHandlers = new SortedDictionary<string, ResponseAttributes>();

			Random rand = new Random();

			byte[] bytes = new byte[8];
			rand.NextBytes(bytes);

			messageId = BitConverter.ToInt64(bytes, 0);
			syncroot = new object();
		}

		public void CreateRequest(IqMessage message, HandleResponse response, object userState)
		{
			ResponseAttributes attrib = new ResponseAttributes();
			attrib.Listener = response;
			attrib.userState = userState;

			responseHandlers.Add(message.Id, attrib);
		}
		public bool TryHandleResponse(IqMessage message)
		{
			ResponseAttributes source;

			if (!responseHandlers.TryGetValue(message.Id, out source))
				return false;

			source.Listener(message, source.userState);

			responseHandlers.Remove(message.Id);

			return true;
		}
		public string GetNextId()
		{
			// This is a Pseudo-random number generator based on the linear feedback shift register algorithm
			// Will generate up to 2^61 unique message identifiers
			lock (syncroot)
			{
				// Polynomial: x^62 + x^61 + x^6 + x^5
				messageId = (messageId >> 1) ^ (-(messageId & 1) & 0x1800000000000030);
			}

			byte[] id = BitConverter.GetBytes(messageId);

			string result = Convert.ToBase64String(id);
			result = result.TrimEnd('=');

			return result;
		}

		// Nested Classes
		private sealed class ResponseAttributes
		{
			public HandleResponse Listener;
			public object userState;
		}

		private long messageId;
		private object syncroot;
		private IDictionary<string, ResponseAttributes> responseHandlers;
	}
}