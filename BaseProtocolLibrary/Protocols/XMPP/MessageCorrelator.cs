using System;
using System.Collections.Generic;

namespace InstantMessage.Protocols.XMPP.Messages
{
	internal delegate void HandleResponse(IqMessage response, object userState);

	internal sealed class MessageCorrelator
	{
		public MessageCorrelator()
		{
			responseHandlers = new SortedDictionary<string, ResponseAttributes>();
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
			byte[] id = BitConverter.GetBytes(messageId);

			string result = Convert.ToBase64String(id);
			result = result.TrimEnd('=');
			messageId++;

			return result;
		}

		// Nested Classes
		private sealed class ResponseAttributes
		{
			public HandleResponse Listener;
			public object userState;
		}

		private long messageId;
		private IDictionary<string, ResponseAttributes> responseHandlers;
	}
}