using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusIM;
using System.Net;

namespace NexusIMTests
{
	[TestClass]
	public class ExtensionTests
	{
		[TestMethod]
		public void IPAddressIsPrivateNetworkTest()
		{
			// IPv4
			Assert.IsTrue(IPAddress.Parse("192.168.2.1").IsPrivateNetwork());
			Assert.IsFalse(IPAddress.Parse("192.166.1.254").IsPrivateNetwork());
			Assert.IsTrue(IPAddress.Parse("10.1.1.1").IsPrivateNetwork());
			Assert.IsTrue(IPAddress.Parse("10.254.254.254").IsPrivateNetwork());
			Assert.IsTrue(IPAddress.Parse("172.16.2.1").IsPrivateNetwork());
			Assert.IsTrue(IPAddress.Parse("172.31.2.1").IsPrivateNetwork());

			// IPv6
			Assert.IsTrue(IPAddress.Parse("fc00::5").IsPrivateNetwork());
			Assert.IsFalse(IPAddress.Parse("::1").IsPrivateNetwork(), "IPv6 Link-Local address is not a private network");
		}
	}
}