using System;
using System.Collections.Generic;
using InstantMessage;
using NexusIM.Managers;

namespace NexusIM
{
	/// <summary>
	/// Automatically responses to other users with a canned message if the current user is idle
	/// </summary>
	static class AutoResponder
	{
		/// <summary>
		/// Sets up all data and events need to work
		/// </summary>
		public static void Setup()
		{
			UserIdle.onUserReturn += new EventHandler(UserIdle_onUserReturn);
			IMProtocol.onMessageReceive += new EventHandler<IMMessageEventArgs>(MessageReceive);
		}
		/// <summary>
		/// Cleans up all data and pointers used by the Auto Responder
		/// </summary>
		public static void Shutdown()
		{
			UserIdle.onUserReturn -= new EventHandler(UserIdle_onUserReturn);
			IMProtocol.onMessageReceive -= new EventHandler<IMMessageEventArgs>(MessageReceive);
			usershandled.Clear();
			usershandled = null;
		}
		
		// Event Callbacks
		private static void MessageReceive(object sender, IMMessageEventArgs e)
		{
			if (UserIdle.IsIdle)
			{
				if (!usershandled.Contains(e.Sender))
				{
					if (AccountManager.StatusMessage == "")
						e.Sender.sendMessage("Auto Response: I'm currently Away from my Keyboard");
					else
						e.Sender.sendMessage("Auto Response: " + AccountManager.StatusMessage);

					usershandled.Add(e.Sender);
				}
			}
		}
		private static void UserIdle_onUserReturn(object sender, EventArgs e)
		{
			usershandled.Clear();
		}

		private static List<IMBuddy> usershandled = new List<IMBuddy>(); // Contains all the users that the auto responder has 'handled'. Cleared when the user returns
	}
}