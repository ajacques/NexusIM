using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage.Protocols.AudioVideo
{
	internal class StunBindingResponse : StunPacket
	{
		protected override ushort MessageMethod
		{
			get {
				return 1;
			}
		}

		protected override byte MessageClass
		{
			get {
				return 2;
			}
		}
	}
}
