using System;
using System.Collections.Generic;
using System.Text;
using InstantMessage;
using InstantMessage.Events;
using InstantMessage.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Diagnostics;

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
		public void SendMessage(string message)
		{
			if (OnMessageSendAttempt != null)
				OnMessageSendAttempt(this, new IMMessageEventArgs(null, message));
		}
		public void Leave(string reason)
		{
			throw new NotImplementedException();
		}

		internal void ForgeMessage(string input)
		{
			if (OnMessageReceived != null)
				OnMessageReceived(this, new IMMessageEventArgs(null, input));
		}
		internal bool IsMessageHandlerAttached()
		{
			return OnMessageReceived != null;
		}

		internal event EventHandler<IMMessageEventArgs> OnMessageSendAttempt;
		public event EventHandler<IMMessageEventArgs> OnMessageReceived;
		public event EventHandler OnUserListReceived;
		public event EventHandler OnJoin;
	}

	[TestClass]
	public class BlowfishTests
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get	{
				return testContextInstance;
			}
			set	{
				testContextInstance = value;
			}
		}

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

			var handler = new BlowfishMessageHandler_Accessor(room); // password is "password"
			byte[] result = handler.BlowCrypt_Decode("BRurM1bWPZ1."); // Hi bob!

			//string correct = new string(new char[] { '\x03', '\xff', '_', '\r', '\xf2', 'v', '\r', '\xe7' });
			byte[] correct = new byte[] { 3, 255, 95, 13, 242, 118, 13, 231 };

			Assert.AreEqual(correct.Length, result.Length);
			for (int i = 0; i < result.Length; i++)
				Assert.AreEqual(correct[i], result[i], "Incorrect digit at position " + i);
		}

		[TestMethod]
		public void EncodeTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler_Accessor(room); // password is "password"
			byte[] input = new byte[] { 0x03, 0xff, 95, 13, 0xf2, 118, 13, 0xe7};
			string result = handler.BlowCrypt_Encode(input);

			string expected = "BRurM1bWPZ1.";

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void DecodeMultiBlockTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler_Accessor(room); // Password for this is "password"
			byte[] result = handler.BlowCrypt_Decode("aWrek.SUQUO/.bxu0.5GWH6/51k0X.tfzLa.Ye2e5.px/5k1nnOHW.sjGwY097Jeg/xdeaF/"); // Long message testing here. please disregard
			
			byte[] correct = new byte[] { 116, 0xeb, 110, 0xb8, 0x16, 65, 0xdf, 0x0c, 72, 0xb7, 0xcb, 0x07, 0x02, 0x82, 51, 64, 0x0c, 0xc6, 84, 95, 61, 9, 96, 0xc7, 0xd6, 0x1c, 0x18, 0xdb, 0x07, 64, 68, 62, 0xbe, 0x8a, 0xc5, 94, 60, 0xb7, 70, 89, 107, 49, 0x03, 0xe3, 82, 66, 0xf2, 75 };

			Assert.AreEqual(correct.Length, result.Length);
			for (int i = 0; i < result.Length; i++)
				Assert.AreEqual(correct[i], result[i], "Incorrect digit at position " + i);
		}

		[TestMethod]
		public void DecryptTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler_Accessor(room);
			handler.CryptoKey = mEncoder.GetBytes("password");
			byte[] input = new byte[] { 3, 255, 95, 13, 242, 118, 13, 231 }; // Taken from DecodeTest
			byte[] output = handler.Decrypt(input);
			byte[] expected = mEncoder.GetBytes("Hi bob!");

			Assert.AreEqual(expected.Length, output.Length);
			for (int i = 0; i < output.Length; i++)
				Assert.AreEqual(expected[i], output[i]);		
		}

		[TestMethod]
		public void ReceiveEventTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler_Accessor(room);
			handler.CryptoKey = mEncoder.GetBytes("password");

			ManualResetEventSlim waitHandle = new ManualResetEventSlim();

			handler.add_OnMessageReceived(new EventHandler<IMMessageEventArgs>(delegate(object sender, IMMessageEventArgs args)
				{
					Assert.AreEqual("Hi bob!", args.Message);
					waitHandle.Set();
				}));

			waitHandle.Wait(1000);
			room.ForgeMessage("+OK BRurM1bWPZ1.");

			if (!waitHandle.IsSet)
			{
				Assert.Fail("OnMessageReceived Event Handler was not tripped");
			}
		}

		[TestMethod]
		public void SendEventTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler_Accessor(room);
			handler.CryptoKey = mEncoder.GetBytes("password");

			ManualResetEventSlim waitHandle = new ManualResetEventSlim();

			room.OnMessageSendAttempt += new EventHandler<IMMessageEventArgs>(delegate(object sender, IMMessageEventArgs args)
			{
				Assert.AreEqual("+OK BRurM1bWPZ1.", args.Message);
				waitHandle.Set();
			});

			waitHandle.Wait(1000);
			handler.SendMessage("Hi bob!");

			if (!waitHandle.IsSet)
			{
				Assert.Fail("OnMessageSendAttempt Event Handler was not triggered");
			}
		}

		[TestMethod]
		public void PaddingTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler_Accessor(room);
			string tooshort = handler.PadToMod("12345", 8);
			string justright = handler.PadToMod("12345678", 8);
			string toolong = handler.PadToMod("1234567890", 8);
			string quitelong = handler.PadToMod("1234567890530305030356", 8);

			Assert.AreEqual(8, tooshort.Length, "Too Short");
			Assert.AreEqual(8, justright.Length, "Just Right");
			Assert.AreEqual(16, toolong.Length, "Too Long");
			Assert.IsTrue(quitelong.Length % 8 == 0, "Quite Long");
		}

		[TestMethod]
		public void EncryptTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler_Accessor(room);
			handler.CryptoKey = mEncoder.GetBytes("password");

			byte[] input = mEncoder.GetBytes("Hi bob!\0");
			byte[] expected = new byte[] { 3, 255, 95, 13, 242, 118, 13, 231 }; // Taken from DecryptTest

			byte[] output = handler.Encrypt(input);

			Assert.AreEqual(expected.Length, output.Length);
			for (int i = 0; i < output.Length; i++)
				Assert.AreEqual(expected[i], output[i], "Incorrect digit at position " + i);
		}

		private static Encoding mEncoder = Encoding.ASCII;
	}
}