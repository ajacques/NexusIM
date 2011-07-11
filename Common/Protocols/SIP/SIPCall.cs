using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols.SIP
{
	public enum SIPCallStatus
	{
		NotConnected,
		Trying = 100,
		Ringing = 180,
		OK = 200
	}

	public class SIPCall// : IVOIPCall
	{
		public SIPCall(SIPClient client, string callid)
		{
			mClient = client;
			mCallId = callid;
		}
		


		private string mCallId;
		private SIPClient mClient;
	}
}
