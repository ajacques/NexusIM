using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Protocols.VOIP;
using System.IO;

namespace InstantMessage.Protocols.SIP
{
	public enum SIPCallStatus
	{
		NotConnected,
		Trying = 100,
		Ringing = 180,
		OK = 200
	}

	public class SIPCall : IVOIPCall
	{
		public SIPCall(SIPClient client, string callid)
		{
			mClient = client;
			mCallId = callid;
		}

		public void Dial()
		{
			mStatus = VOIPCallStatus.Connecting;
			StringWriter sbpBuilder = new StringWriter();

			foreach (IMediaProfile profile in Profiles)
			{
				if (profile.GetType() != typeof(ISIPMediaProfile))
					throw new ArgumentException("All Profiles must be of type ISIPMediaProfile");

				ISIPMediaProfile sip = (ISIPMediaProfile)profile;
				sbpBuilder.Write("m={0} {1} {2}", sip.MediaType, sip.Port, sip.MediaProtocol);
			}
		}

		public void Hangup()
		{
			throw new NotImplementedException();
		}

		public IList<IMediaProfile> Profiles
		{
			get {
				throw new NotImplementedException();
			}
		}

		public VOIPCallStatus Status
		{
			get {
				return mStatus;
			}
		}

		private string mCallId;
		private SIPClient mClient;
		private VOIPCallStatus mStatus;
	}
}
