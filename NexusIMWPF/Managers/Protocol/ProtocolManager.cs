using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using InstantMessage;

namespace NexusIM.Managers
{
	static class InterfaceManager
	{
		/// <summary>
		/// Registers this class the platform handling class
		/// Must be run before any Protocols are Created
		/// </summary>
		public static void Setup()
		{
		}
		public static void Shutdown()
		{
		}
		public static IMProtocol CreateProtocol(string shortName)
		{
			IMProtocol protocol;

			switch (shortName)
			{
				case "yahoo":
					protocol = new IMYahooProtocol();
					break;
				default:
					throw new ArgumentException();
			}

			return protocol;
		}

		public static event EventHandler onWindowOpen;

		internal static void OpenBuddyWindow(IMBuddy iMBuddy, bool p)
		{
			throw new NotImplementedException();
		}
	}
}