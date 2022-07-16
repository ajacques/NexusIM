using System;
using System.Linq;
using System.Net;
using InstantMessage;

namespace NexusIM.Misc
{
	internal class ServiceRecordHostnameResolver : HostnameResolverBase
	{
		public override IPEndPoint Resolve(IMProtocol protocol)
		{
			if (protocol == null)
			{
				throw new ArgumentNullException("protocol");
			}
			if (protocol.Server == null)
			{
				throw new ArgumentNullException("protocol.Server");
			}

			string hostname = String.Format("_xmpp-client._tcp.{0}", protocol.Server);

			var targets = DnsResolver.ResolveService(hostname).GroupBy(s => s.Priority);

			if (!targets.Any())
            {
				throw new Exception("Did not find any endpoints for " + hostname);
            }

			var ips = targets.First();
			int weightsum = ips.Sum(s => s.Weight);

			// Weighted Random
			Random rand = new Random();
			int r = rand.Next(0, weightsum);

			int running = 0;
			foreach (var record in ips)
			{
				running += record.Weight;
				if (running > r)
				{
					return new IPEndPoint(DnsResolver.ResolveIP(record.ServerName).First(), record.Port);
				}
			}

			return null;
		}
	}
}