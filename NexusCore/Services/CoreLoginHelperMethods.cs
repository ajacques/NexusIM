using System;
using System.Web;
using System.Web.SessionState;
using NexusCore.Databases;
using NexusCore.Support;

namespace NexusCore.Services
{
	public class CoreLoginHelperMethods : ICoreLogin
	{
		public void LoginToCore(string username, string password)
		{
			HttpSessionState session = HttpContext.Current.Session;

			NexusCoreDataContext db = new NexusCoreDataContext();
			User mUserData = db.TryLogin(username, password);

			db.Dispose();
			if (mUserData != null)
			{
				session["userid"] = mUserData.id;
			} else
				throw WCFExceptionTypes.BadLogin;
		}

		public void LoginToCore(string token)
		{
			throw new NotImplementedException();
		}
	}
}