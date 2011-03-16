using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using InstantMessage.Protocols.Irc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtocolTests
{
	internal delegate void OnClientConnect(EndPoint endpoint);
	internal delegate void OnClientMessage(string message);

	internal class IRCTestServer : IDisposable
	{
		public IRCTestServer()
		{
			mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
		}

		public void Dispose()
		{
			if (mSocket != null)
				mSocket.Dispose();

			mSocket = null;
		}

		public void Open(int portNumber)
		{
			mSocket.Bind(new IPEndPoint(IPAddress.Loopback, portNumber));
			mSocket.Listen(1);
			mSocket.BeginAccept(new AsyncCallback(OnClientConnectInternal), null);
		}
		public void SendIRCMessage(string message)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			mActiveClient.Send(bytes);
		}

		private void OnClientConnectInternal(IAsyncResult e)
		{
			mActiveClient = mSocket.EndAccept(e);
			byte[] dataqueue = new byte[2048];
			mActiveClient.BeginReceive(dataqueue, 0, dataqueue.Length, SocketFlags.None, new AsyncCallback(OnClientMessageInternal), dataqueue);

			if (OnClientConnect != null)
				OnClientConnect(mActiveClient.RemoteEndPoint);
		}
		private void OnClientMessageInternal(IAsyncResult e)
		{
			byte[] dataqueue = (byte[])e.AsyncState;
			int bytesRead = mActiveClient.EndReceive(e);

			string msg = Encoding.UTF8.GetString(dataqueue, 0, bytesRead);

			string[] messages = msg.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

			if (OnClientMessage != null)
			{
				foreach (string message in messages)
					OnClientMessage(message);
			}

			mActiveClient.BeginReceive(dataqueue, 0, dataqueue.Length, SocketFlags.None, new AsyncCallback(OnClientMessageInternal), dataqueue);
		}

		public bool IsListening
		{
			get {
				return mSocket.IsBound;
			}
		}
		public OnClientConnect OnClientConnect
		{
			get;
			set;
		}
		public OnClientMessage OnClientMessage
		{
			get;
			set;
		}

		private Socket mActiveClient;
		private Socket mSocket;
	}

	[TestClass]
	public class IRCProtocolTests
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
		public void ConnectTest()
		{
			Random rand = new Random();
			int port = rand.Next(1024, ushort.MaxValue);
			IRCProtocol protocol = new IRCProtocol("localhost", port);

			Assert.AreEqual(port, protocol.Port);

			ManualResetEvent rEvent = new ManualResetEvent(false);
			IRCTestServer testServ = new IRCTestServer();
			testServ.Open(port);

			testServ.OnClientConnect = new OnClientConnect((EndPoint ep) => {
				rEvent.Set();
			});

			Assert.IsTrue(testServ.IsListening);
			protocol.BeginLogin();
			protocol.EndLogin();

			Assert.IsTrue(rEvent.WaitOne(1000));

			protocol.Dispose();
		}

		[TestMethod]
		public void LoginTest()
		{
			Random rand = new Random();
			int port = rand.Next(1024, ushort.MaxValue);
			IRCProtocol protocol = new IRCProtocol("localhost", port);
			protocol.Username = "TestUsername";
			protocol.Nickname = "TestNickname";
			
			Assert.AreEqual(port, protocol.Port);

			CountdownEvent rEvent = new CountdownEvent(2);
			IRCTestServer testServ = new IRCTestServer();
			testServ.Open(port);

			testServ.OnClientMessage = new OnClientMessage((string message) => {
				if (Regex.IsMatch(message, "^USER [^ ]* [^ ]* [^ ]* :.*$"))
					rEvent.Signal();
				else if (Regex.IsMatch(message, "^NICK [^ ]*$"))
					rEvent.Signal();
			});
			protocol.BeginLogin();
			protocol.EndLogin();

			Assert.IsTrue(rEvent.Wait(-1), String.Format("Countdown failed -- {0} of {1} failed to signal", rEvent.InitialCount - rEvent.CurrentCount, rEvent.InitialCount));

			protocol.Dispose();
		}

		[TestMethod]
		public void QuitTest()
		{
			Random rand = new Random();
			int port = rand.Next(1024, ushort.MaxValue);
			IRCProtocol protocol = new IRCProtocol("localhost", port);
			protocol.Username = "TestUsername";
			protocol.Nickname = "TestNickname";

			Assert.AreEqual(port, protocol.Port);

			IRCTestServer testServ = new IRCTestServer();
			testServ.Open(port);

			protocol.BeginLogin();
			protocol.EndLogin();

			CountdownEvent rEvent = new CountdownEvent(1);
			testServ.OnClientMessage = new OnClientMessage((string message) =>
			{
				if (Regex.IsMatch(message, "^QUIT$"))
					rEvent.Signal();
			});

			rEvent.Reset(1);
			protocol.Disconnect();

			Assert.IsTrue(rEvent.Wait(1000), String.Format("Countdown failed - {0} of {1} failed to signal", rEvent.InitialCount - rEvent.CurrentCount, rEvent.InitialCount));
		}

		[TestMethod]
		public void QuitWithMessageTest()
		{
			Random rand = new Random();
			int port = rand.Next(1024, ushort.MaxValue);
			IRCProtocol protocol = new IRCProtocol("localhost", port);
			protocol.Username = "TestUsername";
			protocol.Nickname = "TestNickname";

			Assert.AreEqual(port, protocol.Port);

			IRCTestServer testServ = new IRCTestServer();
			testServ.Open(port);

			protocol.BeginLogin();
			protocol.EndLogin();
			string expectedQuitMessage = "Quit Message --- ";

			CountdownEvent rEvent = new CountdownEvent(1);
			testServ.OnClientMessage = new OnClientMessage((string message) =>
			{
				if (Regex.IsMatch(message, "^QUIT :" + expectedQuitMessage + "$"))
					rEvent.Signal();
				else
					Trace.WriteLine(message);
			});

			protocol.Disconnect(expectedQuitMessage);

			Assert.IsTrue(rEvent.Wait(1000), String.Format("Countdown failed - {0} of {1} failed to signal", rEvent.InitialCount - rEvent.CurrentCount, rEvent.InitialCount));
		}

		[TestMethod]
		public void WrapAroundTest()
		{
			IRCProtocol_Accessor protocol = new IRCProtocol_Accessor();

			protocol.mDataQueue = Encoding.UTF8.GetBytes("12345\r\n67890\r\n12345\r\n67890");
			IEnumerable<string> lines = protocol.SocketHandleLineWrapAround(protocol.mDataQueue.Length);

			Assert.AreEqual(3, lines.Count());
			Assert.IsNotNull(protocol.mBufferCutoffMessage);
			protocol.mBufferCutoffMessage = null;

			protocol.mDataQueue = Encoding.UTF8.GetBytes("12345\r\n67890\r\n12345\r\n67890 ");
			IEnumerable<string> lines2 = protocol.SocketHandleLineWrapAround(protocol.mDataQueue.Length - 1);

			Assert.AreEqual(4, lines2.Count());
		}
	}
}