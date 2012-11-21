using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace InstantMessage.Events
{
	public class SocketErrorEventArgs : IMErrorEventArgs
	{
		public SocketErrorEventArgs(SocketException exception) : base(IMProtocolErrorReason.CONNERROR)
		{
			Exception = exception;
		}

		public SocketException Exception
		{
			get;
			private set;
		}
	}
}
