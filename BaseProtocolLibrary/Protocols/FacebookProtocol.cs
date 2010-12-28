using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace InstantMessage
{
	/// <summary>
	/// Implements the People Near Me API
	/// </summary>
	class IMFacebookProtocol : IMProtocol
	{
		public IMFacebookProtocol()
		{
			protocolType = "Facebook Chat";
			mProtocolTypeShort = "facebook";
			
		}
		public override void BeginLogin()
		{
		}
		public override void Disconnect()
		{
			
		}
		public override void AddFriend(string name, string nickname, string group)
		{
			return;
		}
		public override void ChangeStatus(IMStatus newstatus)
		{
			if (IsOnlineStatus(newstatus) && !IsOnlineStatus(mStatus) && mEnabled)
				BeginLogin();
			else if (!IsOnlineStatus(newstatus) && IsOnlineStatus(mStatus))
				Disconnect();
		}
		public override void SendMessage(string friendName, string message)
		{
			
		}
		public override void IsTyping(string uname, bool isTyping)
		{

		}
		protected override void ChangeAvatar()
		{

		}
		public override string GetServerString(string username)
		{
			return "facebook.com";
		}
	}
}