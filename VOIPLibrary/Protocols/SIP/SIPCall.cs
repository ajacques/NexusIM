using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Protocols.VOIP;
using System.IO;
using System.Diagnostics;

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
			mProfiles = new List<IMediaProfile>();
		}

		public void Invite(string username)
		{
			mStatus = VOIPCallStatus.Connecting;
			StringWriter sbpBuilder = new StringWriter();

			foreach (IMediaProfile profile in Profiles)
			{
				sbpBuilder.WriteLine("m={0} {1} {2}", profile.MediaType, profile.Port, profile.MediaProtocol);

				foreach (string attribute in profile.Attributes)
				{
					sbpBuilder.Write("a=");
					sbpBuilder.WriteLine(attribute);
				}
			}

			Trace.WriteLine(String.Format("SIP: Inviting user {0} to call {1}", username, mCallId));

			mClient.InviteUser(this, username, sbpBuilder.ToString());
		}

		public void Hangup()
		{
			throw new NotImplementedException();
		}

		public IList<IMediaProfile> Profiles
		{
			get {
				return mProfiles;
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
		private List<IMediaProfile> mProfiles;
	}
}
