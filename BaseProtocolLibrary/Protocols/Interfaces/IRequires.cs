using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public interface IRequiresUsername : IProtocol
	{
		string Username
		{
			get;
			set;
		}
	}
	public interface IRequiresPassword : IProtocol
	{
		string Password
		{
			get;
			set;
		}
	}
}
