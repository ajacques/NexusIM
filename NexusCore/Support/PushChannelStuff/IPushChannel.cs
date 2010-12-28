using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusCore.PushChannel
{
	interface IPushChannel
	{
		void PushMessage(IPushMessage message);
		void PushMessages(IEnumerable<IPushMessage> messages);
	}
}