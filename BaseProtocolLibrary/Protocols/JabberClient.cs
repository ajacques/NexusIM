using System;
using System.Net;
using System.Xml;
using System.IO;
using System.Net.Sockets;

namespace InstantMessage.Xmpp
{
	public class XmppClient
	{
		public XmppClient()
		{
			mDoc = new XmlDocument();
			mDoc.PreserveWhitespace = false;
		}

		public void Login()
		{
			mClient = new TcpClient();
			mClient.Connect(mServer, mPort);
			mStream = mClient.GetStream();
			mWriter = new StreamWriter(mStream);
			mWriter.AutoFlush = true;
			mReader = new StreamReader(mStream);
			SendLoginString();
		}

		public void BeginLogin()
		{
			mClient = new TcpClient();
		}

		private void SendLoginString()
		{
			mDoc.AppendChild(mDoc.CreateXmlDeclaration("1.0", "utf-8", String.Empty));
			XmlElement stream = mDoc.CreateElement("stream", "stream", "http://etherx.jabber.org/streams");
			stream.SetAttribute("xmlns", "jabber:client");
			stream.SetAttribute("to", mServer);
			stream.SetAttribute("version", "1.0");
			mDoc.AppendChild(stream);

			mWriter.WriteLine(mDoc.OuterXml);

			mDoc.RemoveAll();
		}

		public string Username
		{
			get {
				return mUsername;
			}
			set {
				mUsername = value;
			}
		}
		public string Password
		{
			get {
				return mPassword;
			}
			set {
				mPassword = value;
			}
		}
		public string Server
		{
			get {
				return mServer;
			}
			set {
				mServer = value;
			}
		}
		public string Resource
		{
			get {
				return mResource;
			}
			set {
				mResource = value;
			}
		}

		// Variables
		private string mUsername = "";
		private string mPassword = "";
		private string mServer = "";
		private string mResource = "";
		private int mPort = 5222;
		private TcpClient mClient;
		private NetworkStream mStream;
		private StreamWriter mWriter;
		private StreamReader mReader;
		private XmlDocument mDoc;
	}
}