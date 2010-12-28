using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Security;

namespace NexusWeb.Databases
{
	partial class userdbDataContext
	{
		public User TryLogin(string username, string password)
		{
			password = HashString(password); // Prepare for this

			var user = (from c in this.Users
						where c.username == username && c.password == password
						select c).FirstOrDefault();

			user.lastseen = DateTime.UtcNow;
			SubmitChanges();

			return user;
		}

		/// <summary>
		/// Attempts to log out the current user with the specified security token.
		/// </summary>
		/// <exception cref="System.Exception">Thrown if the user id does not match any row in the database.</exception>
		/// <exception cref="System.Security.SecurityException">Thrown if the security token does not match the token stored in the database.</exception>
		/// <param name="userid">Specifies which user to try to logout.</param>
		/// <param name="token">Security token to prevent unintended log-outs caused by 3rd-party websites.</param>
		public void TryLogout(int userid)
		{
			var user = from u in Users
					   where u.id == userid
					   select u;

			User first = user.FirstOrDefault();

			if (first != null)
			{
				SubmitChanges();
			} else if (first == null)
				throw new Exception("Invalid userid") { };
			else
				throw new SecurityException("Invalid security Exception") ;
		}

		/// <summary>
		/// Returns the number of devices the specified user has
		/// </summary>
		public int GetDeviceCount(int userid)
		{
			return GetDevices(userid).Count();
		}
		public IQueryable<Account> GetAccounts(int userid)
		{
			var accounts = from a in Accounts
						   where a.userid == userid
						   select a;

			return accounts;
		}
		public IQueryable<UserLocation> GetPermittedLocationRows(int userid)
		{
			var locations = from t in UserLocations 
							join p in LocationPrivacies on t.id equals p.locationid 
							where p.userid == userid
							select t;

			return locations;
		}
		public IQueryable<Device> GetOnlineDevices(int userid)
		{
			var devices = from d in Devices
						  where d.userid == userid
						  && d.lastseen == null && d.lastsignin != null // Magic line to get the online devices
						  select d;

			return devices;
		}
		public IQueryable<Device> GetDevices(int userid)
		{
			var devices = from d in Devices
						  where d.userid == userid
						  select d;

			return devices;
		}
		/// <summary>
		/// Returns all of the friends of the given user.
		/// </summary>
		/// <param name="userid"></param>
		/// <returns></returns>
		public IQueryable<User> GetFriends(int userid)
		{
			var friends = Friends.Where(f => f.userid == userid || f.friendid == userid)
								 .Join(Users, f => f.friendid == userid ? f.userid : f.friendid, u => u.id, (f, u) => u);

						/*from u in Users
						  join f in Friends on u.Id equals f.friendid
						  where f.userid == userid || f.friendid == userid
						  select u;*/

			return friends;
		}
		/// <summary>
		/// Checks to see if the specified users are currently friends with each other.
		/// </summary>
		/// <param name="userid">First user id to check</param>
		/// <param name="friendid">Second user id to check</param>
		/// <returns>True if the two users are currently friends.</returns>
		public bool AreFriends(int userid, int friendid)
		{
			var friends = Friends.Where(f => f.userid == userid || f.friendid == userid);
			return friends.Any(f => (f.userid == userid && f.friendid == friendid) || (f.userid == userid && f.friendid == friendid));
		}
		public DeviceType GetDeviceType(string shortname)
		{
			return (from dt in DeviceTypes
					where dt.ShortName == shortname
					select dt).FirstOrDefault();
		}
		public DeviceType GetDeviceType(int typeid)
		{
			return (from dt in DeviceTypes
					where dt.Id == typeid
					select dt).FirstOrDefault();
		}
		public StatusUpdate GetUsersLastStatusUpdate(int userid)
		{
			var result = from s in StatusUpdates
				   join u in Users on s.Userid equals u.id
				   where u.id == userid
				   orderby s.Timestamp descending
				   select s;

			return result.First();
		}
		public string NewAuthToken(int userid)
		{
			AuthToken token = new AuthToken();
			token.userid = userid;
			token.token = PasswordGenerator.RandomString(30);
			token.expires = DateTime.UtcNow.AddDays(1);

			AuthTokens.InsertOnSubmit(token);
			SubmitChanges();

			return token.token;
		}
		public IQueryable<UserLocation> GetLocationRows(int userid)
		{
			return from u in Users
				   join ul in UserLocations on u.id equals ul.userid
				   where u.id == 3
				   select ul;
		}
		public bool HasLocationViewPermission(int localId, int remoteUserId)
		{
			if (Users.Where(u => u.id == remoteUserId).Select(u => u.locationsharestate).First() == false)
				return false;

			bool result = GetPermittedLocationRows(localId).Any(u => u.userid == remoteUserId);
			return result;
		}
		public User GetUser(int userid)
		{
			return Users.Where(u => u.id == userid).FirstOrDefault();
		}
		public IQueryable<User> GetUpcomingBirthdays(int userid)
		{
			DateTime now = DateTime.UtcNow;
			DateTime cutoff = now.AddDays(3);
			return GetFriends(userid).Where(u => now < u.DateOfBirth && u.DateOfBirth < cutoff);
		}
		public bool VerifyAclPermission(int aclid, int ownerid, int userid)
		{
			var acl = (from a in AccessControls
					  where a.Id == aclid
					  select new { IsBlacklist = a.IsBlacklist, PermitEveryone = a.PermitEveryone, PermitFriends = a.PermitFriends }).First();

			if (acl.PermitEveryone)
				return true;

			if (acl.PermitFriends)
				return AreFriends(ownerid, userid);

			// Now check for explicit permission
			if (UserAccessControls.Where(u => u.ControlId == aclid).Any(u => u.TargetUserId == userid && u.AccessPermitted))
				return true;

			return false;
		}

		public string HashString(string input)
		{
			string output = "";
			byte[] bytes = mEncoder.GetBytes(input);
			byte[] value = mHasher.ComputeHash(bytes);
			foreach (byte x in value)
				output += String.Format("{0:x2}", x);

			return output;
		}

		private static HashAlgorithm mHasher = new SHA256Managed(); // Yeah!! SHA-256! We rule!
		private static Encoding mEncoder = Encoding.UTF8; // If they can type it, they can use it as a password
	}
}