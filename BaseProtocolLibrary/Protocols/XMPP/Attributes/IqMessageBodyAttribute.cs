using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage.Protocols.XMPP
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	internal sealed class IqMessageBodyAttribute : Attribute
	{
		public IqMessageBodyAttribute(string ns, string localname)
		{
			Namespace = ns;
			LocalName = localname;
		}

		public string Namespace
		{
			get;
			private set;
		}

		public string LocalName
		{
			get;
			private set;
		}
	}
}
