using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage.Protocols.XMPP
{
	internal interface IAuthStrategy
	{
		void StartAuthentication(XmppProtocol protocol);
		void PerformStep(SaslChallengeMessage message);
		void Finalize(SaslAuthMessage.SuccessMessage message);
	}
}
