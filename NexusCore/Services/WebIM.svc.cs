using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.SessionState;
using InstantMessage;
using NexusCore.Controllers;
using NexusCore.Databases;
using NexusCore.DataContracts;
using NexusCore.PushChannel;
using NexusCore.Support;

namespace NexusCore.Services
{
	/// <summary>
	/// Service used to connect to im networks by clients that aren't capable of connecting directly themselves (ex. Http, Silverlight, Mobile clients)
	/// </summary>
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[ServiceKnownType(typeof(IMBuddyStatus))]
	public class WebIMService : CoreLoginHelperMethods, IWebIMService, IWebIMWinPhone
	{
		#region IWebIMWinPhone
		public void PreconfiguredLogin(int protocolId, IMStatus status)
		{
			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				throw WCFExceptionTypes.NotAuthorized;

			int userid = (int)session["userid"];

			NexusCoreDataContext db = new NexusCoreDataContext();
			
			var account = (from acc in db.Accounts
						  where acc.id == protocolId && acc.userid == userid
						  select acc).FirstOrDefault();

			PushChannelContext context = new PushChannelContext((PushChannelType)session["pushChannelType"], (Uri)session["pushChannelUri"]);

			if (account != null)
			{
				account.LoginState = true;
				account.HostComputer = Environment.MachineName;
				db.SubmitChanges();
				
				AccountInfo info = account;
				Trace.WriteLine("WebIM: Remote login requested of type " + info.ProtocolType + " for " + info.Username);
				
				StorageItem item = WebIMProtocolManager.SetupAccount(info);
				item.ProtocolId = account.id;
				IMProtocol protocol = item.Protocol;
				item.PushContext = context;
				context.AddAccount(protocol, account.id);
				
				protocol.Status = status;
				protocol.BeginLogin();
				protocol.EndLogin();
			}

			db.Dispose();
		}
		public void StartSession()
		{
			HttpSessionState session = HttpContext.Current.Session;

			if (session["userid"] == null)
				throw WCFExceptionTypes.NotAuthorized;

			int userid = (int)session["userid"];

			Trace.WriteLine("WebIM: Starting Session");

			NexusCoreDataContext db = new NexusCoreDataContext();

			var userAccounts = from a in db.Accounts
						   where a.userid == userid && a.enabled
						   select new {id = a.id};

			var accounts = from a in userAccounts.AsEnumerable()
						   join mem in WebIMProtocolManager.StorageBin on a.id equals mem.ProtocolId into g
						   from mem in g.DefaultIfEmpty()
						   select new { Db = a.id, Memory = mem, IsPrepared = mem != null };

			Trace.WriteLine(String.Format("WebIM: StartSession Statistics: {0} new, {1} take-overs", accounts.Count(a => !a.IsPrepared), accounts.Count(a => a.IsPrepared)));

			PushChannelContext context = new PushChannelContext((PushChannelType)session["pushChannelType"], (Uri)session["pushChannelUri"]);

			foreach (var account in accounts)
			{
				if (!account.IsPrepared)
				{
					PreconfiguredLogin(account.Db, IMStatus.Available);
				} else {
					account.Memory.PushContext = context;
					//account.Memory.PushContext.PushNewContacts(account.Memory.Protocol.ContactList, account.Db);
				}
			}
		}
		public void SendMessage(Guid protocol, string receiver, string message)
		{
			WebIMProtocolManager.FindItem(protocol).Protocol.SendMessage(receiver, message);
		}
		public void StartPushStream(PushChannelType type, Uri urichannel)
		{
			HttpSessionState session = HttpContext.Current.Session;

			Trace.WriteLine(String.Format("WebIM: PushStream Setup (Type: {0}, Uri: {1})", type, urichannel));
			session["pushChannelType"] = type;
			session["pushChannelUri"] = urichannel;
			session["pushChannelStarted"] = true;
		}
		public void StopPushStream()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}