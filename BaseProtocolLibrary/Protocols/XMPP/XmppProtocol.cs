using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace InstantMessage.Protocols.XMPP
{
	[IMNetwork("xmpp")]
	public class XmppProtocol : IMProtocol
	{
		// Constructors
		public XmppProtocol()
		{
			mBGMessageThread = new Thread(new ThreadStart(MessageLoop));
			mBGMessageThread.Name = "XmppProtocol - Message Loop";
			correlator = new MessageCorrelator();

			messageProcessors = new SortedDictionary<Type, ProcessMessage>(new TypeComparer());
			messageProcessors.Add(typeof(StreamInitMessage), ProcessStreamInitMessage);
			messageProcessors.Add(typeof(StartTlsMessage.ProceedMessage), ProcessTlsProceedMessage);
			messageProcessors.Add(typeof(SaslAuthMessage.SuccessMessage), ProcessSaslSuccessMessage);

			Resource = "Test";
		}

		// Public Methods
		public override void BeginLogin()
		{
			base.BeginLogin();

			networkSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			networkSocket.Connect(Server, 5222);
			networkSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
			networkSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 0);

			xmppStream = new XmppStream(new NetworkStream(networkSocket));

			mRunMsgThread = true;
			mBGMessageThread.Start();
			WriteMessage(new StreamInitMessage(Server));
		}

		// Private Methods
		private void MessageLoop()
		{
			xmppStream.InitReader();

			while (mRunMsgThread)
			{
				XmppMessage message = xmppStream.ReadMessage();

				ProcessMessage proc;
				if (messageProcessors.TryGetValue(message.GetType(), out proc))
				{
					proc(message);
				} else if (message is IqMessage) {
					correlator.TryHandleResponse(message as IqMessage);
				}
			}
		}
		private void WriteMessage(XmppMessage message)
		{
			xmppStream.WriteMessage(message);
		}
		private void WriteMessage(IqMessage message)
		{
			message.Id = correlator.GetNextId();
			message.Source = new Jid("adam", "adamjacques.com");
			WriteMessage(message as XmppMessage);
		}

		// Message Processors
		private void ProcessStreamInitMessage(XmppMessage message)
		{
			StreamInitMessage msg = message as StreamInitMessage;
			if (msg.Features.Any(f => f is StreamInitMessage.TlsCapableFeature) && !xmppStream.IsEncrypted)
			{
				WriteMessage(new StartTlsMessage());
			} else if (msg.Features.Any(f => f is StreamInitMessage.AuthMechanismFeature)) {
				WriteMessage(new PlainAuthMessage(Username, Password));
			}
		}
		private void ProcessTlsProceedMessage(XmppMessage message)
		{
			xmppStream.ActivateTLS(Server);
			WriteMessage(new StreamInitMessage(Server));
		}
		private void ProcessSaslSuccessMessage(XmppMessage message)
		{
			WriteMessage(new StreamInitMessage(Server));
		}

		// Nested Classes
		private sealed class TypeComparer : IComparer<Type>
		{
			public int Compare(Type x, Type y)
			{
				return x.GUID.CompareTo(y.GUID);
			}
		}
		
		// Delegates
		private delegate void ProcessMessage(XmppMessage message);

		// Properties
		public string Resource
		{
			get;
			set;
		}

		// Variables
		private IDictionary<Type, ProcessMessage> messageProcessors;
		private bool mRunMsgThread;
		private Thread mBGMessageThread;
		private Socket networkSocket;
		private XmppStream xmppStream;
		private MessageCorrelator correlator;
	}
}