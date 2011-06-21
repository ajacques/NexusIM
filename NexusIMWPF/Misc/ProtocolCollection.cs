using System.Collections.Generic;
using InstantMessage;
using System.Linq;

namespace NexusIM.Misc
{
	internal class ProtocolCollection : AdvancedSet<IMProtocolWrapper>
	{
		public ProtocolCollection() : base(new WrapperComparer())
		{
			
		}

		public IMProtocolWrapper Find(IMProtocol protocol)
		{
			return Find(protocol.Protocol, protocol.Username);
		}
		public IMProtocolWrapper Find(string type, string username)
		{
			return Enumerable.FirstOrDefault(this, w => w.Protocol.Protocol == type && w.Protocol.Username == username);
			/*object val = base.RootNode;

			return SearchNoStack(val, (w) => {
				if (type != w.Protocol.Protocol)
					return type.CompareTo(w.Protocol.Protocol);
				return username.CompareTo(w.Protocol.Username);
			});*/
		}

		private class WrapperComparer : IComparer<IMProtocolWrapper>
		{
			public int Compare(IMProtocolWrapper left, IMProtocolWrapper right)
			{
				if (left.Protocol.Protocol != right.Protocol.Protocol)
					return left.Protocol.Protocol.CompareTo(right.Protocol.Protocol);

				return left.Protocol.Username.CompareTo(right.Protocol.Username);
			}
		}
	}
}