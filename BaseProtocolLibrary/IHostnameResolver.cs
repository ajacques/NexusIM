using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessage
{
	public interface IHostnameResolver
	{
		IPEndPoint Resolve(IMProtocol protocol);
		Task<IPEndPoint> ResolveAsync(IMProtocol protocol);
	}
}
