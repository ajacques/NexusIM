using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using System.Runtime.Serialization;

namespace NexusWeb
{
	internal static class WCFExceptions
	{
		public static Exception CrossSiteViolation
		{
			get {
				return new WebFaultException(HttpStatusCode.Unauthorized);
			}
		}
		public static Exception BadCredentials
		{
			get {
				return new WebFaultException<string>("test", HttpStatusCode.Unauthorized);
			}
		}
		public static Exception NotFriends
		{
			get {
				var test = new
				{
					UserId = "test"
				};
				return new WebFaultException<string>(test.ToString(), HttpStatusCode.Forbidden);
				return new WebFaultException<object>(test, HttpStatusCode.Forbidden);
			}
		}
	}
}