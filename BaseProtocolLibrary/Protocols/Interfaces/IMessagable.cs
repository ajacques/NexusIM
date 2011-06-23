using System;

namespace InstantMessage
{
	/// <summary>
	/// Represents an object that can be messages ie a contact or a chat room
	/// </summary>
	public interface IMessagable
	{
		/// <summary>
		/// Sends a message to the specified target.
		/// Some message content restrictions may apply depending on the protocol.
		/// </summary>
		/// <param name="message">The message to send</param>
		void SendMessage(string message);
	}
}