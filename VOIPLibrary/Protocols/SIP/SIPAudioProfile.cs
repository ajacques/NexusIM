using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Protocols.VOIP;

namespace InstantMessage.Protocols.SIP
{
	public class SIPAudioProfile : IMediaProfile
	{
		public SIPAudioProfile()
		{
			mAttributes = new List<string>();

			mAttributes.Add("rtpmap:9 G722/8000");
			mAttributes.Add("rtpmap:101 telephone-event/8000");
			mAttributes.Add("rmtp:101 0-16");
			mAttributes.Add("ptime:20");
			mAttributes.Add("sendrecv");
		}

		public string MediaType
		{
			get {
				return "audio";
			}
		}

		public int Port
		{
			get {
				return 50000;
			}
		}

		public string MediaProtocol
		{
			get	{
				return "RTP/AVP";
			}
		}

		public IEnumerable<string> Attributes
		{
			get	{
				return mAttributes;
			}
		}

		private IList<string> mAttributes;
	}
}