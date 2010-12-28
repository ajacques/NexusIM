using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Pipes;
using InstantMessage;
using System.IO;

namespace NexusCore
{
	public class NamedPipeProtocol : IMProtocol
	{
		public NamedPipeProtocol()
		{
			mProtocolTypeShort = "namedpipe";
		}
		public override void BeginLogin()
		{
			mStream = new NamedPipeClientStream(mServer);
			mStream.Connect(1000);
			mWriter = new StreamWriter(mStream);
			mWriter.AutoFlush = true;
			mWriter.WriteLine("LOGIN=" + Username + ":" + Password);
		}
		public override void Disconnect()
		{
			mWriter.WriteLine("DISCONNECT=");

			base.Disconnect();

			mWriter.Close();
			mStream.Dispose();
		}

		private NamedPipeClientStream mStream;
		private StreamWriter mWriter;
	}
}