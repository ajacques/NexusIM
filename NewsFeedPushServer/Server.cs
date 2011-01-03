using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nugget;
using NewsFeedPushServer;
using JONSParser;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace ConsoleApp
{
	// The server side socket
	class NewsFeedAppSocket : WebSocket
	{
		public override void Incoming(string data)
		{
			if (string.IsNullOrWhiteSpace(data))
				return;

			// All incoming packets should be in JSON, but we should always verify to prevent exploits
			dynamic input = JSON.Parse(data);

			if (input.__targetinvoke.ToString() == "PostStatusMessage")
			{
				string d = input._d.messageBody.ToString();
				Debug.WriteLine("User posted message" + d);
			}
		}
		public override void Disconnected()
		{
			Console.WriteLine("--- disconnected ---");
		}
		public override void Connected(ClientHandshake handshake)
		{
			// Not much to do here. Anybody can connect, but can they authenticate themselves
			Console.WriteLine("User Connected: " + handshake.Host);
		}

		private void Login(string authtoken)
		{
			NexusCoreDataContext db = new NexusCoreDataContext();
			var token = db.AuthTokens.Where(at => at.token == authtoken).FirstOrDefault();
			if (token == null)
				throw new Exception();

			mDbUser = token.User;

			db.AuthTokens.DeleteOnSubmit(token);
			//db.SubmitChanges();

			Trace.WriteLine("User now Authenticated as " + token.User.firstname);
		}
		private string SerializeToJson(object objectGraph, Type type)
		{
			MemoryStream stream = new MemoryStream();

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(StreamStateChange));
			serializer.WriteObject(stream, objectGraph);

			return stream.ContentToString();
		}

		private User mDbUser;
		private StreamState mState = StreamState.NotLoggedIn;
	}
	
	class Server
	{
		static void Main(string[] args)
		{
			Trace.AutoFlush = true;
			Trace.Listeners.Add(new ConsoleTraceListener());
			nugget = new WebSocketServer(8181, "http://dev.nexus-im.com", "ws://dev.nexus-im.com:8181");

			nugget.RegisterHandler<NewsFeedAppSocket>("/newsfeed");
			Log.Level = LogLevel.Info;
			nugget.Start();

			Console.ReadLine();
		}

		public static WebSocketServer nugget;
	}
}