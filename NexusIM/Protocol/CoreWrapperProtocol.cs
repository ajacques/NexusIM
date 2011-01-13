using System;
using InstantMessage;
using NexusIM.Managers;
using NexusIM.NexusCore;

namespace NexusIM.Protocol
{
	class CoreWrapperProtocol : IMProtocol
	{
		public CoreWrapperProtocol(AccountInfo accinfo)
		{
			base.protocolType = accinfo.ProtocolType;
			base.mUsername = accinfo.Username;
			base.mPassword = accinfo.Password;
			base.mServer = accinfo.Server;
			base.status = IMProtocolStatus.ONLINE;
			base.mGuid = accinfo.Guid;
			base.enableSaving = false;
			base.mEnabled = accinfo.Enabled;
		}

		public override void SendMessage(IMBuddy buddy, string message)
		{
			SwarmChatMessage msg = new SwarmChatMessage();
			msg.Recipient = buddy.Username;
			msg.ViaAccount = Guid;
			msg.MessageContents = message;
			//NexusCoreManager.NexusCore.BeginSendSwarmMessage(msg, MessageOptions.SendToAccountMaster, new AsyncCallback(delegate(IAsyncResult e) { NexusCoreManager.NexusCore.EndSendSwarmMessage(e); }), null);
		}
	}
}