using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Protocols.VOIP;

namespace InstantMessage.Protocols.SIP
{
	public interface ISIPMediaProfile : IMediaProfile
	{
		IEnumerable<string> Attributes
		{
			get;
		}
	}
}