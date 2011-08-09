using System;
using System.Net;
using System.Net.Sockets;

namespace NexusIM
{
	static class IPAddressExtensions
	{
		public static bool IsPrivateNetwork(this IPAddress address)
		{
			byte[] addrbytes = address.GetAddressBytes();

			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				if (addrbytes[0] == 10) // Class A
					return true;
				if (addrbytes[0] == 172 && addrbytes[1] >= 16 && addrbytes[1] <= 31) // Class B
					return true;
				if (addrbytes[0] == 192 && addrbytes[1] == 168) // Class C
					return true;

				return false;
			} else if (address.AddressFamily == AddressFamily.InterNetworkV6) {
				if (addrbytes[0] == 252)
					return true;

				return false;
			} else
				throw new NotSupportedException("AddressFamily of type " + address.AddressFamily + " is not supported");
		}
	}
}