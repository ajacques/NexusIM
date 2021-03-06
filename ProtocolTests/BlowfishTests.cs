﻿using System;
using System.Collections.Generic;
using System.Text;
using InstantMessage;
using InstantMessage.Events;
using InstantMessage.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Diagnostics;
using InstantMessage.Protocols;
using System.Reflection;

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
		public IMProtocol Protocol
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
		public event EventHandler<IMChatRoomGenericEventArgs> OnUserJoin;

		
		public void InviteUser(string username, string message)
		{
			throw new NotImplementedException();
		}

		IEnumerable<IContact> IChatRoom.Participants
		{
			get { throw new NotImplementedException(); }
		}
		
		public void SendMessage(string message, MessageFlags flags = MessageFlags.None)
		{
			OnMessageSendAttempt(this, new IMMessageEventArgs(null, message));
		}
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
			byte[] result = (byte[])InvokeMethod(typeof(BlowfishMessageHandler), "BlowCrypt_Decode", "BRurM1bWPZ1."); // Hi bob!
			

			//string correct = new string(new char[] { '\x03', '\xff', '_', '\r', '\xf2', 'v', '\r', '\xe7' });
			byte[] correct = new byte[] { 3, 255, 95, 13, 242, 118, 13, 231 };

			Assert.AreEqual(correct.Length, result.Length);
			for (int i = 0; i < result.Length; i++)
				Assert.AreEqual(correct[i], result[i], "Incorrect digit at position " + i);
		}

		[TestMethod]
		public void EncodeTest()
		{
			byte[] input = new byte[] { 0x03, 0xff, 95, 13, 0xf2, 118, 13, 0xe7};
			string result = (string)InvokeMethod(typeof(BlowfishMessageHandler), "BlowCrypt_Encode", input);

			string expected = "BRurM1bWPZ1.";

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void DecodeMultiBlockTest()
		{
			// Password for this is "password"
			byte[] result = (byte[])InvokeMethod(typeof(BlowfishMessageHandler), "BlowCrypt_Decode", "aWrek.SUQUO/.bxu0.5GWH6/51k0X.tfzLa.Ye2e5.px/5k1nnOHW.sjGwY097Jeg/xdeaF/"); // Long message testing here. please disregard

			byte[] correct = new byte[] { 116, 0xeb, 110, 0xb8, 0x16, 65, 0xdf, 0x0c, 72, 0xb7, 0xcb, 0x07, 0x02, 0x82, 51, 64, 0x0c, 0xc6, 84, 95, 61, 9, 96, 0xc7, 0xd6, 0x1c, 0x18, 0xdb, 0x07, 64, 68, 62, 0xbe, 0x8a, 0xc5, 94, 60, 0xb7, 70, 89, 107, 49, 0x03, 0xe3, 82, 66, 0xf2, 75 };

			Assert.AreEqual(correct.Length, result.Length);
			for (int i = 0; i < result.Length; i++)
				Assert.AreEqual(correct[i], result[i], "Incorrect digit at position " + i);
		}

		[TestMethod]
		public void DecryptTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler(room);
			handler.CryptoKey = mEncoder.GetBytes("password");
			byte[] input = new byte[] { 3, 255, 95, 13, 242, 118, 13, 231 }; // Taken from DecodeTest
			byte[] output = (byte[])InvokeMethod(handler, "Decrypt", input);
			byte[] expected = mEncoder.GetBytes("Hi bob!");

			Assert.AreEqual(expected.Length, output.Length);
			for (int i = 0; i < output.Length; i++)
				Assert.AreEqual(expected[i], output[i]);		
		}

		[TestMethod]
		public void ReceiveEventTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler(room);
			handler.CryptoKey = mEncoder.GetBytes("password");

			ManualResetEventSlim waitHandle = new ManualResetEventSlim();

			handler.OnMessageReceived += new EventHandler<IMMessageEventArgs>(delegate(object sender, IMMessageEventArgs args)
				{
					Assert.AreEqual("Hi bob!", args.Message);
					waitHandle.Set();
				});

			waitHandle.Wait(1000);
			room.ForgeMessage("+OK BRurM1bWPZ1.");

			if (!waitHandle.IsSet)
			{
				Assert.Fail("OnMessageReceived Event Handler was not tripped");
			}
			waitHandle.Dispose();
		}

		[TestMethod]
		public void SendEventTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler(room);
			handler.CryptoKey = mEncoder.GetBytes("password");

			ManualResetEventSlim waitHandle = new ManualResetEventSlim();

			room.OnMessageSendAttempt += new EventHandler<IMMessageEventArgs>(delegate(object sender, IMMessageEventArgs args)
			{
				Assert.AreEqual("+OK BRurM1bWPZ1.", args.Message);
				waitHandle.Set();
			});

			waitHandle.Wait(500);
			handler.SendMessage("Hi bob!");

			if (!waitHandle.IsSet)
			{
				Assert.Fail("OnMessageSendAttempt Event Handler was not triggered");
			}
		}

		[TestMethod]
		public void PaddingTest()
		{
			string tooshort = (string)InvokeMethod(typeof(BlowfishMessageHandler), "PadToMod", "12345", 8);
			string justright = (string)InvokeMethod(typeof(BlowfishMessageHandler), "PadToMod", "12345678", 8);
			string toolong = (string)InvokeMethod(typeof(BlowfishMessageHandler), "PadToMod", "1234567890", 8);
			string quitelong = (string)InvokeMethod(typeof(BlowfishMessageHandler), "PadToMod", "1234567890530305030356", 8);

			Assert.AreEqual(8, tooshort.Length, "Too Short");
			Assert.AreEqual(8, justright.Length, "Just Right");
			Assert.AreEqual(16, toolong.Length, "Too Long");
			Assert.IsTrue(quitelong.Length % 8 == 0, "Quite Long");
		}

		[TestMethod]
		public void EncryptTest()
		{
			DummyChatRoom room = new DummyChatRoom();

			var handler = new BlowfishMessageHandler(room);
			handler.CryptoKey = mEncoder.GetBytes("password");

			byte[] input = mEncoder.GetBytes("Hi bob!\0");
			byte[] expected = new byte[] { 3, 255, 95, 13, 242, 118, 13, 231 }; // Taken from DecryptTest

			byte[] output = (byte[])InvokeMethod(handler, "Encrypt", input);

			Assert.AreEqual(expected.Length, output.Length);
			for (int i = 0; i < output.Length; i++)
				Assert.AreEqual(expected[i], output[i], "Incorrect digit at position " + i);
		}

		[TestMethod]
		public void UTF8Test()
		{
			DummyChatRoom room = new DummyChatRoom();

			Encoding mUTF8 = Encoding.UTF8;
			string input = "ῺṻᵦƉƸeas";

			var handler = new BlowfishMessageHandler(room);
			handler.CryptoKey = mUTF8.GetBytes("password");
			byte[] crypt = (byte[])InvokeMethod(handler, "Encrypt", mUTF8.GetBytes(input));
			byte[] decrypt = (byte[])InvokeMethod(handler, "Decrypt", crypt);

			Assert.AreEqual(input, mUTF8.GetString(decrypt));
		}

		private static object InvokeMethod(object obj, string methodName, params object[] parameters)
		{
			MethodInfo minfo = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			Assert.IsNotNull(minfo, String.Format("Function {0} not found on type {1}", methodName, obj.GetType().FullName));
			return minfo.Invoke(obj, parameters);
		}
		private static object InvokeMethod(Type type, string methodName, params object[] parameters)
		{
			MethodInfo minfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
			return minfo.Invoke(null, parameters);
		}

		private static Encoding mEncoder = Encoding.ASCII;
	}
}