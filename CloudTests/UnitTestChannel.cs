using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace CloudTests
{
	class UnitTestChannel : IContextChannel
	{
		#region IContextChannel Members

		public bool AllowOutputBatching
		{
			get;
			set;
		}

		public System.ServiceModel.Channels.IInputSession InputSession
		{
			get;
			internal set;
		}

		public EndpointAddress LocalAddress
		{
			get;
			internal set;
		}

		public TimeSpan OperationTimeout
		{
			get;
			set;
		}

		public System.ServiceModel.Channels.IOutputSession OutputSession
		{
			get;
			internal set;
		}

		public EndpointAddress RemoteAddress
		{
			get;
			internal set;
		}

		public string SessionId
		{
			get;
			internal set;
		}

		#endregion

		#region IChannel Members

		public T GetProperty<T>() where T : class
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public void Open()
		{
			throw new NotImplementedException();
		}

		public event EventHandler Opened;

		public event EventHandler Opening;

		public CommunicationState State
		{
			get;
			internal set;
		}

		#endregion

		#region IExtensibleObject<IContextChannel> Members

		public IExtensionCollection<IContextChannel> Extensions
		{
			get;
			internal set;
		}

		#endregion
	}
}
