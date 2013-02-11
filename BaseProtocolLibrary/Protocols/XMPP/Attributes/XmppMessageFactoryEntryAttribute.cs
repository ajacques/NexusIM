using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage.Protocols.XMPP
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	internal sealed class XmppMessageFactoryEntryAttribute : Attribute
	{
	}
}
