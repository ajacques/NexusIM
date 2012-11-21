using System;
using System.Net;
using System.Threading.Tasks;

namespace InstantMessage
{
	public abstract class HostnameResolverBase : IHostnameResolver
	{
		public abstract IPEndPoint Resolve(IMProtocol protocol);

		public async Task<IPEndPoint> ResolveAsync(IMProtocol protocol)
		{
			Task<IPEndPoint> task = new Task<IPEndPoint>(() => Resolve(protocol));

			return await task;
		}
	}
}
