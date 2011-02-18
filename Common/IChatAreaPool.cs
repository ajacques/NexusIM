using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public interface IChatAreaPool
	{
		void PutInPool(int poolid, IContact contact);
		void RemoveFromPool(IContact contact);
		int? GetPool(IContact contact);
		int GetNextId();
	}
}