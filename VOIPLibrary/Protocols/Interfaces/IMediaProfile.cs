using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols.VOIP
{
	public interface IMediaProfile
	{
		string MediaType
		{
			get;
		}
		int Port
		{
			get;
		}
		string MediaProtocol
		{
			get;
		}
		IEnumerable<string> Attributes
		{
			get;
		}
	}
}
