using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage.Protocols.XMPP.Messages;

namespace InstantMessage.Protocols.XMPP
{
	internal interface IAuthStrategy
	{
		void StartAuthentication(XmppProtocol protocol);
		void PerformStep(SaslChallengeMessage message);
		void FinalizeStep(SaslAuthMessage.SuccessMessage message);
	}
}
