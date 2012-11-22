using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	public interface IRequiresUsername : IProtocol
	{
		new string Username
		{
			get;
			set;
		}
	}
	public interface IRequiresPassword : IProtocol
	{
		new string Password
		{
			get;
			set;
		}
	}
}
