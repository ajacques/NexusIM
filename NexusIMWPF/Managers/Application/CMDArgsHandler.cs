using System;
using System.Collections.Generic;
using System.Linq;
using InstantMessage;
using InstantMessage.Protocols.Irc;

namespace NexusIM.Managers
{
	/// <summary>
	/// 
	/// </summary>
	static class CMDArgsHandler
	{
		public static void HandleArg(string data)
		{
			if (String.IsNullOrWhiteSpace(data))
				return;

			string[] args = data.ToLowerInvariant().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			Dictionary<string, string> keyvalues = new Dictionary<string, string>();
			List<string> mutators = new List<string>();

			// We have all the args separated.. now what? format is -(property):(value)
			// (args) "(protocol string or file)"

			// -(property) = Mutators. These change the behavior of the properties and values

			// First we break down all the arguments and put them into a dictionary or list
			foreach (string argument in args)
			{
				if (argument.Contains(":")) // Key:Value pair
				{
					// Break down the pairs
					string[] kvpair = argument.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
					kvpair[0] = kvpair[0].Replace("-", "");

					keyvalues.Add(kvpair[0], kvpair[1]); // Store the values for later use
				} else if (argument.Substring(0, 1) == "-") {
					mutators.Add(argument.Substring(1));
				}
			}

			if (mutators.Contains("bringtofront"))
			{
				WindowSystem.OpenContactListWindow();
			}

			if (keyvalues.ContainsKey("status"))
			{
				if (keyvalues["status"] == "offline")
				{
					AccountManager.Connected = false;
					return;
				}
				try	{
					IMStatus newstatus = (IMStatus)Enum.Parse(typeof(IMStatus), keyvalues["status"], true);
					AccountManager.Status = newstatus;
				} catch (ArgumentException) {
					return;
				}
			}

			if (mutators.Contains("sendmessage") && keyvalues.ContainsKey("username"))
			{
				/*var buddies = from IMBuddy b in null
							  where b.Username == keyvalues["username"]
							  select new { b };


				if (buddies.Count() == 1)
				{
					InterfaceManager.OpenBuddyWindow(buddies.First().b, mutators.Contains("jumplist"));
				}*/
			}
		}

		private static void HandleProtocolString(string data)
		{
			string protocol = IMProtocol.FromProtocolString(data.Substring(0, data.IndexOf(":")));

			if (protocol == "irc") // Custom stuff for irc
			{
				string server = data.Substring(data.IndexOf("://") + 3, data.IndexOf("#") - (data.IndexOf("://") + 3));
				server = server.Replace("/", ""); // Chop off any slashes
				string room = data.Substring(data.IndexOf("#"));

				var paccount = from IMProtocol p in AccountManager.Accounts where p.Server == server select new { p };

				if (paccount.Count() > 0)
				{
					//paccount.FirstOrDefault().p.JoinChatRoom(room);
				} else {
					IMProtocol account = new IRCProtocol();
					account.Server = server;
					//account.onLogin += new EventHandler(account_onLogin);

					/* Do something here */

					AccountManager.AddNewAccount(account);
				}
			} else {
				/* Do something here */
			}
		}
	}
}