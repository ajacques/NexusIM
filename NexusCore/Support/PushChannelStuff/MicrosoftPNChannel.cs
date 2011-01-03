using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace NexusCore.PushChannel
{
	internal class MicrosoftPNChannel : IPushChannel
	{
		public MicrosoftPNChannel(Uri uri)
		{
			mUri = uri;
		}

		public void PushMessage(IPushMessage message)
		{
			new MicrosoftPNRequest(this, message);
		}
		public void PushMessages(IEnumerable<IPushMessage> messages)
		{
			new MicrosoftPNRequest(this, messages);
		}
		
		private class MicrosoftPNRequest
		{
			public MicrosoftPNRequest(MicrosoftPNChannel channel, IPushMessage message)
			{
				mMessage = message;
				mChannel = channel;

				// Build Push Message Body
				XmlDocument mDoc = new XmlDocument();
				mDoc.AppendChild(mDoc.CreateXmlDeclaration("1.0", "utf-8", null));

				mRequest = WebRequest.Create(channel.mUri) as HttpWebRequest;
				mRequest.Method = "POST";
				mRequest.ContentType = "text/xml";
				if (message.MessageClass == PushMessageClass.Toast)
				{
					mRequest.Headers.Add("X-WindowsPhone-Target", "toast");
					mRequest.Headers.Add("X-NotificationClass", "2");
					XmlElement notif = mDoc.CreateElement("wp", "Notification", "WPNotification");
					XmlElement toast = mDoc.CreateElement("wp", "Toast", "WPNotification");
					XmlElement text1 = mDoc.CreateElement("wp", "Text1", "WPNotification");
					XmlElement text2 = mDoc.CreateElement("wp", "Text2", "WPNotification");
					text1.InnerText = (message as IToastPushMessage).Text1;
					text2.InnerText = (message as IToastPushMessage).Text2;
					toast.AppendChild(text1);
					toast.AppendChild(text2);
					notif.AppendChild(toast);
					mDoc.AppendChild(notif);
				} else if (message.MessageClass == PushMessageClass.Tile)
				{
					mRequest.Headers.Add("X-WindowsPhone-Target", "token");
					mRequest.Headers.Add("X-NotificationClass", "1");
				} else if (message.MessageClass == PushMessageClass.Internal)
				{
					mRequest.Headers.Add("X-NotificationClass", "3");
					XmlElement elem = mDoc.CreateElement("nx", "RawNotification", "NXNotification");
					elem.AppendChild(mDoc.ImportNode(message.XmlMessage, true));
					mDoc.AppendChild(elem);
				}

				messageBody = mDoc.OuterXml;

				mRequest.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream), null);
			}
			public MicrosoftPNRequest(MicrosoftPNChannel channel, IEnumerable<IPushMessage> messages)
			{
				mMessages = messages;
				mChannel = channel;

				// Build Push Message Body
				mRequest = WebRequest.Create(channel.mUri) as HttpWebRequest;
				mRequest.Method = "POST";
				mRequest.ContentType = "application/json";
				mRequest.Headers.Add("X-PayloadFormat", "Json");
				mRequest.Headers.Add("X-NotificationClass", "3");
				mRequest.Headers.Add("X-CallbackURI", "http://www.adrensoftware.com/intranet/wp7test.php");

				messageBody = SerializeMessagesAsJson(messages);
				//messageBody = SerializeMessagesAsJson(messages);

				mRequest.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream), null);
			}
			private string SerializeMessagesAsJsonDC(IEnumerable<IPushMessage> messages)
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(messages.ToArray().GetType(), new List<Type>() { typeof(NewContactMessage), typeof(ContactStatusChangeMessage) });//new DataContractJsonSerializer(typeof(IPushMessage), new List<Type>() { typeof(NewContactMessage) });
				MemoryStream memStream = new MemoryStream();

				serializer.WriteObject(memStream, messages.ToArray());
				return Encoding.UTF8.GetString(memStream.ToArray());
			}
			private string SerializeMessagesAsXmlDC(IEnumerable<IPushMessage> messages)
			{
				DataContractSerializer serializer = new DataContractSerializer(messages.ToArray().GetType(), new List<Type>() { typeof(NewContactMessage), typeof(ContactStatusChangeMessage) });//new DataContractJsonSerializer(typeof(IPushMessage), new List<Type>() { typeof(NewContactMessage) });
				MemoryStream memStream = new MemoryStream();

				serializer.WriteObject(memStream, messages.ToArray());

				return Encoding.UTF8.GetString(memStream.ToArray());
			}
			private string SerializeMessagesAsXml(IEnumerable<IPushMessage> messages)
			{
				XmlDocument mDoc = new XmlDocument();
				mDoc.AppendChild(mDoc.CreateXmlDeclaration("1.0", "utf-8", null));

				XmlElement elem = mDoc.CreateElement("nx", "RawNotification", "NXNotification");

				foreach (IPushMessage message in messages)
					elem.AppendChild(mDoc.ImportNode(message.XmlMessage, true));

				mDoc.AppendChild(elem);

				return mDoc.OuterXml;
			}
			private string SerializeMessagesAsJson(IEnumerable<IPushMessage> messages)
			{
				return JsonConvert.SerializeObject(messages);
			}
			private string SerializeMessagesAsBson(IEnumerable<IPushMessage> messages)
			{
				MemoryStream memStream = new MemoryStream();
				JsonSerializer serializer = new JsonSerializer();
				BsonWriter bsonWriter = new BsonWriter(memStream);

				serializer.Serialize(bsonWriter, messages);

				return Encoding.UTF8.GetString(memStream.ToArray());
			}

			// Callbacks
			private void OnGetRequestStream(IAsyncResult e)
			{
				StreamWriter writer = new StreamWriter(mRequest.EndGetRequestStream(e));
				writer.Write(messageBody);
				writer.Close();

				mRequest.BeginGetResponse(new AsyncCallback(OnGetResponse), null);
			}
			private void OnGetResponse(IAsyncResult e)
			{
				HttpWebResponse response;
				try
				{
					response = mRequest.EndGetResponse(e) as HttpWebResponse;
				} catch (WebException x)
				{
					response = x.Response as HttpWebResponse;
					if (response.Headers["x-SubscriptionStatus"] == "Expired")
						Debug.WriteLine("Push Notification send failed. Channel is now expired");

					if (mChannel.mErrorOccurred != null)
						mChannel.mErrorOccurred(PushChannelErrorState.Expired);
					return;
				}

				if (mMessage != null)
					Debug.WriteLine(String.Format("Push Notification of type ({0}) successfully pushed to client.", mMessage.GetType().Name));
				if (mMessages != null)
					Debug.WriteLine(String.Format("{0} Push Notifications successfully pushed to client.", mMessages.Count()));
				if (response.Headers["X-NotificationStatus"] != "Received")
					Debug.WriteLine("Notification Status: " + response.Headers["X-NotificationStatus"]);
				if (response.Headers["X-DeviceConnectionStatus"] != "Connected")
					Debug.WriteLine("Device Connection Status: " + response.Headers["X-DeviceConnectionStatus"]);
				if (response.Headers["X-SubscriptionStatus"] != "Active")
					Debug.WriteLine("Subscription Status: " + response.Headers["X-SubscriptionStatus"]);

				response.Close();
			}

			// Variables
			private HttpWebRequest mRequest;
			private MicrosoftPNChannel mChannel;
			private string messageBody;
			private IPushMessage mMessage;
			private IEnumerable<IPushMessage> mMessages;
		}
		
		// Variables
		private PushMessageErrorOccurred mErrorOccurred;
		private const int MaxMessageSize = 1024;
		private Uri mUri;		
	}
}