using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols
{
	[Flags]
	public enum MessageFlags
	{
		None,
		Decrypted,
	}
}
