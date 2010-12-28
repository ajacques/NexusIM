

namespace InstantMessage
{
	class IMTwitterProtocol : IMProtocol
	{
		public IMTwitterProtocol()
		{
			protocolType = "Twitter";
			mProtocolTypeShort = "twitter";
		}
		public override void BeginLogin()
		{
		}
		public override void Disconnect()
		{
		}
		public override string GetServerString(string username)
		{
			return "twitter.com";
		}
	}
}