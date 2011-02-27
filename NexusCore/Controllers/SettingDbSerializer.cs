using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using InstantMessage;
using NexusCore.Databases;

namespace NexusCore.Controllers
{
	static class Extensions
	{
		/// <remarks>
		/// The username and password must be set before calling this
		/// </remarks>
		public static void LoadSettings(this IMProtocol protocol)
		{
			NexusCoreDataContext db = new NexusCoreDataContext();

			var account = (from a in db.Accounts
						   where a.acctype == protocol.Protocol && a.username == protocol.Username
						   select a.id).FirstOrDefault();

			if (account == 0)
				throw new Exception("Account isn't in database");

			var settings = from a in db.AccountSettings
						   where a.accountid == account
						   select new KeyValuePair<string, string>(a.configkey, a.configvalue);

			foreach (var setting in settings)
				protocol.ConfigurationSettings.Add(setting);

			db.Dispose();
		}
		public static void SaveSettings(this IMProtocol protocol)
		{
			NexusCoreDataContext db = new NexusCoreDataContext();

			var account = (from a in db.Accounts
						   where a.acctype == protocol.Protocol && a.username == protocol.Username
						   select a).FirstOrDefault();

			if (account == null)
				throw new Exception("Account isn't in database");

			foreach (KeyValuePair<string, string> setting in protocol.ConfigurationSettings)
			{
				var settingrow = (from s in db.AccountSettings
							  where s.accountid == account.id && s.configkey == setting.Key
							  select s).FirstOrDefault();

				if (settingrow == null)
				{
					settingrow = new AccountSetting();
					settingrow.accountid = account.id;
					settingrow.configkey = setting.Key;
					db.AccountSettings.InsertOnSubmit(settingrow);
				}

				settingrow.configvalue = setting.Value;
			}

			db.SubmitChanges();
			db.Dispose();
		}
	}
	static class SettingDbSerializer
	{
		/// <summary>
		/// Subscribes to any Configuration Change events and saves new settings to the database
		/// </summary>
		/// <param name="protocol">Protocol to monitor for configuration changes</param>
		public static void Register(IMProtocol protocol)
		{
			
		}
		public static void Unregister(IMProtocol protocol)
		{
			
		}
		private static void ConfigSetting_Changed(object sender, DictionaryChangedEventArgs<string, string> args)
		{
			IMProtocol protocol = sender as IMProtocol;
			NexusCoreDataContext db = new NexusCoreDataContext();
			int accid;

			// Get an account id
			try	{
				accid = (from a in db.Accounts
							 where a.acctype == protocol.Protocol && a.username == protocol.Username
							 select a).First().id;
			} catch (Exception) {
				Unregister(protocol); // This protocol isn't in our registered protocol list - ignore it
				return;
			}
			
			// Get the previous row if this setting is already in the database
			var settingrow = (from s in db.AccountSettings
							  where s.accountid == accid && s.configkey == args.ChangedRow.Key
							  select s).FirstOrDefault();

			if (settingrow == null) // Create one if it isn't
			{
				settingrow = new AccountSetting();
				settingrow.accountid = accid;
				settingrow.configkey = args.ChangedRow.Key;
				db.AccountSettings.InsertOnSubmit(settingrow); // Insert it into the database
			}

			settingrow.configvalue = args.ChangedRow.Value;

			db.SubmitChanges(); // Save it to the database
		}
	}
}