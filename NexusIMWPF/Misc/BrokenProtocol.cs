using System.Diagnostics;
using InstantMessage;
using InstantMessage.Events;

namespace NexusIM
{
	[IMNetwork("broken")]
	class BrokenProtocol : IMProtocol
	{
		public override void BeginLogin()
		{
			base.BeginLogin();
			
			triggerOnError(new IMErrorEventArgs(IMProtocolErrorReason.CONNERROR));
		}
	}
}