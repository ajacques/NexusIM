using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstantMessage
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	class SocialNetworkAttribute : Attribute
	{
		public SocialNetworkAttribute(string shortname)
		{
			
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	class IMNetworkAttribute : Attribute
	{
		public IMNetworkAttribute(string shortname)
		{
			ShortName = shortname;
		}

		public string ShortName
		{
			get;
			private set;
		}
	}
}