using System.Windows.Browser;
using InstantMessage;
using InstantMessage.Protocols.Irc;

namespace SilverIM
{
	[ScriptableType]
	public class JSControlTunnel
	{
		[ScriptableMember]
		public void AddAccount(string type, string username, string password = null, string hostname = null)
		{
			IMProtocol protocol = null;
			switch (type)
			{
				case "irc":
					protocol = new IRCProtocol(hostname, 4505);
					break;
				default:
					return;
			}
			protocol.Username = username;
			protocol.Password = password;
			protocol.BeginLogin();
			//((IRCProtocol)protocol).JoinChatRoom("#main");
		}
	}
}