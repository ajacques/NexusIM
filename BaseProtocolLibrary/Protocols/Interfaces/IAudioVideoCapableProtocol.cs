using System;
using InstantMessage.Events;

namespace InstantMessage.Protocols
{
	public interface IAudioVideoCapableProtocol : IProtocol
	{
		event EventHandler<IncomingCallEventArgs> IncomingCall;
	}
}
