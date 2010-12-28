using InstantMessage;
using NexusIM.Managers;
using NexusIM.NexusCore;

namespace NexusIM.Protocol
{
	/// <summary>
	/// A wrapper protocol to allow this device to host the user's im network connections.
	/// </summary>
	/// <remarks>
	/// This class is similiar to the WebIMProtocolManager and WebIMSwarmConnector classes on the NexusCore
	/// </remarks>
	class SwarmHostedProtocol : IMProtocol
	{
		public SwarmHostedProtocol(AccountInfo accinfo)
		{
			mProtocolTypeShort = "swarm";
			mInternalProtocol = FromString(accinfo.ProtocolType);
			protocolType = accinfo.ProtocolType;
			mUsername = mInternalProtocol.Username = accinfo.Username;
			mInternalProtocol.Password = accinfo.Password;
			mGuid = accinfo.Guid;
			mAccountId = accinfo.AccountId;
			mInternalProtocol.Enabled = mEnabled = accinfo.Enabled;
			mInfo = accinfo;
		}

		// Wrapper Functions - Call the the appropriate function on the internal class then tell the swarm about it
		public override void BeginLogin()
		{
			NexusCoreManager.RegisterAsMaster(mAccountId);

			ProtocolReadyMessage message = new ProtocolReadyMessage();
			message.ProtocolId = mGuid;

			NexusCoreManager.SendSwarmMessage(message, MessageOptions.SendToActiveDevices);

			mLoginWaitHandle = mInternalProtocol.LoginWaitHandle;
			mInternalProtocol.BeginLogin();
		}

		public override void Disconnect()
		{
			mInternalProtocol.Disconnect();
		}

		// Variables
		private AccountInfo mInfo;
		private int mAccountId;
		private IMProtocol mInternalProtocol;
	}
}