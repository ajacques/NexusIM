using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Security.Authentication;
using System.Security;

namespace NexusCore.Support
{
	/// <summary>
	/// Contains commonly used exceptions for publicly accessible WCF methods. Allows for easier client-side exception handling due to all errors returning the same exception.
	/// </summary>
	internal static class WCFExceptionTypes
	{
		public static Exception BadLogin
		{
			get	{
				return new FaultException<AuthenticationException>(new AuthenticationException("The specified user credentials do not match any currently registered user account."), new FaultReason("User Credentials do not match any registered user."), new FaultCode("Sender", new FaultCode("Auth")));
			}
		}

		public static Exception NotAuthorized
		{
			get {
				return new FaultException<SecurityException>(new SecurityException("This session has not been authorized to do this operation"), new FaultReason("Session has not logged-in"), new FaultCode("Sender"));
			}
		}

		public static Exception AlreadyAuthenticated
		{
			get	{
				return new FaultException(new FaultReason("Session already authorized"));
			}
		}
	}
}