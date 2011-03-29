using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public interface IChatAreaPool
	{
		void PutInPool(int poolid, IMProtocol protocol, string objectId);
		void RemoveFromPool(IMProtocol protocol, string objectId);
		int? GetPool(IMProtocol protocol, string objectId);
		int GetNextId();
	}
}