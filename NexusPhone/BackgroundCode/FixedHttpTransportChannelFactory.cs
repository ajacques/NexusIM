using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using System.Xml;

namespace NexusPhone
{
	public class FixedHttpRequestChannel : IRequestChannel, IHttpCookieContainerManager
	{
		public FixedHttpRequestChannel(BindingContext bindingContext, EndpointAddress endpointAddress, Uri via)
		{
			mEndpoint = endpointAddress;
			mVia = via;
			context = bindingContext;
			messageEncoder = bindingContext.BindingParameters.Find<MessageEncodingBindingElement>().CreateMessageEncoderFactory();
			transportElement = bindingContext.Binding.Elements.Find<FixedHttpTransportBindingElement>();

			mBufferMgr = BufferManager.CreateBufferManager(transportElement.MaxBufferSize, (int)transportElement.MaxReceivedMessageSize);
		}

		#region IRequestChannel Members

		public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
		{
			return new FixedHttpRequestResult(mVia, message, mContainer, messageEncoder, mBufferMgr, callback, state) { MaxBufferSize = transportElement.MaxBufferSize };
		}
		public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
		{
			return BeginRequest(message, new TimeSpan(0, 1, 0), callback, state);
		}
		public Message EndRequest(IAsyncResult result)
		{
			if (!(result is FixedHttpRequestResult))
				throw new ArgumentException();

			FixedHttpRequestResult nResult = result as FixedHttpRequestResult;
			return nResult.ResponseMessage;
		}
		public EndpointAddress RemoteAddress
		{
			get {
				return mEndpoint;
			}
		}
		public Message Request(Message message, TimeSpan timeout)
		{
			throw new NotImplementedException();
		}
		public Message Request(Message message)
		{
			throw new NotImplementedException();
		}
		public Uri Via
		{
			get {
				return mVia;
			}
		}

		#endregion
		#region IChannel Members

		public T GetProperty<T>() where T : class
		{
			if (typeof(T) == typeof(IHttpCookieContainerManager))
			{
				return (T)(object)this;
			} else 
				throw new NotSupportedException();
		}

		#endregion
		#region ICommunicationObject Members

		public void Abort()
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginClose(AsyncCallback callback, object state)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginOpen(AsyncCallback callback, object state)
		{
			throw new NotImplementedException();
		}

		public void Close(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			throw new NotImplementedException();
		}

		public event EventHandler Closed;

		public event EventHandler Closing;

		public void EndClose(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void EndOpen(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public event EventHandler Faulted;

		public void Open(TimeSpan timeout)
		{
			mState = CommunicationState.Opened;
		}

		public void Open()
		{
			mState = CommunicationState.Opened;
		}

		public event EventHandler Opened;

		public event EventHandler Opening;

		public CommunicationState State
		{
			get {
				return mState;
			}
		}

		#endregion

		private class FixedHttpRequestResult : IAsyncResult
		{
			public FixedHttpRequestResult(Uri via, Message message, CookieContainer cookieContainer, MessageEncoderFactory messageFactory, BufferManager bufMgr, AsyncCallback callback, object userState)
			{
				mMessageFactory = messageFactory;
				mMessage = message;
				mBufferMgr = bufMgr;
				mCallback = callback;
				mUserState = userState;
				mRequest = WebRequest.CreateHttp(via);
				mRequest.CookieContainer = cookieContainer;
				mRequest.Method = "POST";
				mRequest.UserAgent = "NexusPhone";
				mRequest.ContentType = messageFactory.Encoder.ContentType;
				message.Headers.Add(MessageHeader.CreateHeader("To", "http://www.w3.org/2005/08/addressing", via, true));

				//mRequest.BeginGetResponse(new AsyncCallback(OnGetResponse), null);
				mRequest.BeginGetRequestStream(new AsyncCallback(OnGetRequestStream), null);
			}

			private void OnGetRequestStream(IAsyncResult e)
			{
				Stream stream = mRequest.EndGetRequestStream(e);
				StreamWriter httpWriter = new StreamWriter(stream);
				StringWriter tempString = new StringWriter();

				ArraySegment<byte> segment = mMessageFactory.Encoder.WriteMessage(mMessage, Int32.MaxValue, mBufferMgr);
				
				stream.Write(segment.Array, 0, segment.Count);
				stream.Flush();
				stream.Close();

				mRequest.BeginGetResponse(new AsyncCallback(OnGetResponse), null);
			}
			private void OnGetResponse(IAsyncResult e)
			{
				HttpWebResponse response;
				Stream outputStream;
				try	{
					response = mRequest.EndGetResponse(e) as HttpWebResponse;
					outputStream = response.GetResponseStream();
				} catch (WebException x) {
					response = x.Response as HttpWebResponse;
					outputStream = response.GetResponseStream();

					StreamReader reader = new StreamReader(outputStream);
					string html = reader.ReadToEnd();

					mCallback(this);

					return;
				}
				
				byte[] arr = new byte[MaxBufferSize];
				int read = outputStream.Read(arr, 0, (int)MaxBufferSize);
				ArraySegment<byte> segment = new ArraySegment<byte>(arr, 0, read);

				mRespMessage = mMessageFactory.Encoder.ReadMessage(segment, mBufferMgr);
				
				if (mRespMessage.Headers.FindHeader("Session", "com.nexusim.core") != -1)
				{
					string session = mRespMessage.Headers.GetHeader<string>("Session", "com.nexusim.core");

					mRequest.CookieContainer.Add(new Uri("http://core.nexus-im.com"), new Cookie(AccountManager.SessionCookieName, session));
				}

				Complete();
			}
			private void Complete()
			{
				completed = true;

				if (mCallback != null)
					mCallback(this);
			}
			public object AsyncState
			{
				get {
					return mUserState;
				}
			}
			public System.Threading.WaitHandle AsyncWaitHandle
			{
				get { throw new NotImplementedException(); }
			}
			public bool CompletedSynchronously
			{
				get {
					return false;
				}
			}
			public bool IsCompleted
			{
				get {
					return completed;
				}
			}
			public Message ResponseMessage
			{
				get	{
					return mRespMessage;
				}
			}

			private Message mMessage;
			private bool completed;
			private Message mRespMessage;
			public long MaxBufferSize;
			private BufferManager mBufferMgr;
			private MessageEncoderFactory mMessageFactory;
			private HttpWebRequest mRequest;
			private AsyncCallback mCallback;
			private object mUserState;
		}

		public CookieContainer CookieContainer
		{
			get	{
				return mContainer;
			}
			set	{
				mContainer = value;
			}
		}

		private BufferManager mBufferMgr;
		private MessageEncoderFactory messageEncoder;
		private BindingContext context;
		private CookieContainer mContainer;
		private EndpointAddress mEndpoint;
		private FixedHttpTransportBindingElement transportElement;
		private Uri mVia;
		private CommunicationState mState = CommunicationState.Closed;
	}
	public class FixedHttpTransportChannelFactory : ChannelFactoryBase<IRequestChannel>
	{
		public FixedHttpTransportChannelFactory(FixedHttpTransportBindingElement bindingElement, BindingContext bindingContext)
		{
			element = bindingElement;
			context = bindingContext;
		}
	
		protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
		{
			FixedHttpRequestChannel channel = new FixedHttpRequestChannel(context, address, via);

			return channel;
		}

		protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
		{
			throw new NotImplementedException();
		}

		protected override void OnEndOpen(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		protected override void OnOpen(TimeSpan timeout)
		{
			
		}

		private FixedHttpTransportBindingElement element;
		private BindingContext context;
	}
}