using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InstantMessage.Security;
using InstantMessage;
using InstantMessage.Events;

namespace ProtocolTests
{
	internal class DummyChatRoom : IChatRoom
	{
		public string Name
		{
			get { throw new NotImplementedException(); }
		}
		public bool Joined
		{
			get { throw new NotImplementedException(); }
		}
		public IEnumerable<string> Participants
		{
			get { throw new NotImplementedException(); }
		}
		public void SayMessage(string message)
		{
			throw new NotImplementedException();
		}
		public void Leave(string reason)
		{
			throw new NotImplementedException();
		}

		internal bool IsMessageHandlerAttached()
		{
			return OnMessageReceived != null;
		}

		public event EventHandler<IMMessageEventArgs> OnMessageReceived;
		public event EventHandler OnUserListReceived;
		public event EventHandler OnJoin;
	}

	[TestClass]
	public class BlowfishTests
	{
		[TestMethod]
		public void EventSubscribeTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler(room);

			Assert.IsTrue(room.IsMessageHandlerAttached(), "Did not attach to event");
		}

		[TestMethod]
		public void DecodeTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler_Accessor(room);
			string result = handler.BlowCrypt_Decode("BRurM1bWPZ1.");

			//string correct = new string(new char[] { '\x03', '\xff', '_', '\r', '\xf2', 'v', '\r', '\xe7' });
			string correct = Encoding.Default.GetString(new byte[] { 3, 255, 95, 13, 242, 118, 13, 231 });

			Assert.AreEqual(correct, result);
		}
	}
}
