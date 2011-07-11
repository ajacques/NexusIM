using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Protocols.VOIP;

namespace InstantMessage.Protocols
{
	public interface IVOIPCapable : IProtocol
	{
		IVOIPCall StartCall(string username);
	}
}