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
			} else if (address.AddressFamily == AddressFamily.InterNetworkV6) {
				if (addrbytes[0] == 252)
					return true;
			} else
				throw new NotSupportedException("AddressFamily of type " + address.AddressFamily + " is not supported");

			return false;
		}

		public static bool IsLoopback(this IPAddress address)
		{
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				byte[] addrbytes = address.GetAddressBytes();

				if (addrbytes[0] == 127)
					return true;
			} else if (address.AddressFamily == AddressFamily.InterNetworkV6) {
				byte[] addrbytes = address.GetAddressBytes();

				for (int i = 0; i < addrbytes.Length - 1; i++)
				{
					if (addrbytes[i] != 0)
						return false;
				}

				if (addrbytes[addrbytes.Length - 1] == 1)
					return true;
			} else
				throw new NotSupportedException("AddressFamily of type " + address.AddressFamily + " is not supported");

			return false;
		}

		public static bool IsLinkLocal(this IPAddress address)
		{
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				byte[] addrbytes = address.GetAddressBytes();

				if (addrbytes[0] == 169 && addrbytes[1] == 254)
					return true;
			} else if (address.AddressFamily == AddressFamily.InterNetworkV6) {
				return address.IsIPv6LinkLocal;
			} else
				throw new NotSupportedException("AddressFamily of type " + address.AddressFamily + " is not supported");

			return false;
		}
	}
}