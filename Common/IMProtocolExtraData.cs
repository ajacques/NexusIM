using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;

namespace InstantMessage
{
	public class IMProtocolExtraData
	{
		public IMProtocol Protocol
		{
			get;
			set;
		}
		public bool Enabled
		{
			get;
			set;
		}
		public int DatabaseId
		{
			get;
			set;
		}
		public bool IsReady
		{
			get;
			set;
		}
	}
}