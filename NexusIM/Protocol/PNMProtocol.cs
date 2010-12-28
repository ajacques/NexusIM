
namespace InstantMessage
{
	/// <summary>
	/// Implements the People Near Me API
	/// </summary>
	class IMPNMProtocol : IMProtocol
	{
		public IMPNMProtocol()
		{
			protocolType = "People Near Me";
			mProtocolTypeShort = "pnm";
		}
		public override void BeginLogin()
		{
			
		}
		public override void Disconnect()
		{
			CleanupBuddyList();
		}
		public override void ChangeStatus(IMStatus newstatus)
		{
			if (!mEnabled)
				return;

			if (IsOnlineStatus(newstatus) && !IsOnlineStatus(mStatus))
			{
				BeginLogin();
				mEnabled = true;
			} else if (!IsOnlineStatus(newstatus) && IsOnlineStatus(mStatus)) {
				Disconnect();
				mEnabled = false;
			}

			mStatus = newstatus;
		}
		public override void SendMessage(string friendName, string message)
		{
		}
		public override string GetServerString(string username)
		{
			return "(none)";
		}
	}
}