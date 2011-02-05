using System;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Configuration;
using System.Security.Authentication;

namespace NexusCore.Databases
{
	partial class NexusCoreDataContext
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns>Returns null if no results were found for that </returns>
		public User TryLogin(string username, string password)
		{
			var user = (from c in this.Users
					   where c.username == username && c.canlogin
					   select c).FirstOrDefault();

			if (user == null)
				return null;
			
			// Password System v2

			// Decrypt the salt retrieved from the user's row using the site key
			byte[] output = new byte[user.PasswordSalt.Length];
			ICryptoTransform decryptor = SaltDecryptor.CreateDecryptor();
			int count = decryptor.TransformBlock(user.PasswordSalt, 0, user.PasswordSalt.Length, output, 0);

			// Now build the input array from the decrypted hash and password
			byte[] pwdbytes = mEncoder.GetBytes(password);
			byte[] concatOutput = new byte[count + password.Length];
			Buffer.BlockCopy(output, 0, concatOutput, 0, count);
			Buffer.BlockCopy(pwdbytes, 0, concatOutput, count, pwdbytes.Length);

			byte[] pwdHash = HashString(concatOutput);

			for (int i = 0; i < pwdHash.Length; i++)
			{
				if (pwdHash[i] != user.Password[i])
					return null;
			}
			
			user.lastseen = DateTime.UtcNow;
			try	{
				SubmitChanges();
			} catch (ChangeConflictException) {} // Workaround for Expression Web 4 SuperPreview loading two pages at once

			return user;
		}
		public User TryHashLogin(int userid, string pwdhash)
		{
			throw new NotSupportedException();
			//var users = from c in this.Users where c.id == userid && c.password == pwdhash select c;

			//return users.FirstOrDefault();
		}

		#region Random
		public User GetUserById(int userid)
		{
			var users = from u in this.Users
						where u.id == userid
						select u;

			return users.FirstOrDefault();
		}
		public IEnumerable<Account> GetAccounts(int userid)
		{
			var accs = from a in Accounts
					   where a.userid == userid
					   select a;

			return accs;
		}
		public Device GetDeviceById(int deviceid)
		{
			var users = from d in this.Devices
						where d.id == deviceid
						select d;

			return users.FirstOrDefault();
		}
		public User TryTokenLogin(string token, out AuthToken tokenrow)
		{
			var users = from u in this.Users
						join t in this.AuthTokens on u.id equals t.userid
						where t.token == token
						select new { User = u, Token = t };

			if (users.FirstOrDefault() != null)
			{
				tokenrow = users.FirstOrDefault().Token;
				return users.FirstOrDefault().User;
			}
			else
			{
				tokenrow = null;
				return null;
			}
		}
		public Device TryDeviceTokenLogin(string token, out User userrow)
		{
			var users = from u in Users
						join d in Devices on u.id equals d.userid
						where d.logintoken == token
						select new { User = u, Device = d };

			var user = users.FirstOrDefault();
			if (user != null)
			{
				userrow = user.User;

				return user.Device;
			}
			else
			{
				userrow = null;
				return null;
			}
		}
		public Device TryDeviceTokenLogin(string token)
		{
			var users = from u in Users
						join d in Devices on u.id equals d.userid
						where d.logintoken == token
						select d;

			var user = users.FirstOrDefault();
			return user;
		}
		public Account GetSingleAccount(int accountid)
		{
			var acc = from a in Accounts
					  where a.id == accountid
					  select a;

			return acc.FirstOrDefault();
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
		public StatusUpdate GetUsersLastStatusUpdate(int userid)
		{
			var result = from s in StatusUpdates
						 join u in Users on s.Userid equals u.id
						 where u.id == userid
						 orderby s.Timestamp descending
						 select s;

			return result.First();
		}
		public IQueryable<UserLocation> GetPermittedLocationRows(int userid)
		{
			var locations = from t in UserLocations
							join p in LocationPrivacies on t.id equals p.locationid
							where p.userid == userid
							select t;

			return locations;
		}
		public bool HasLocationViewPermission(int localId, int remoteUserId)
		{
			if (Users.Where(u => u.id == remoteUserId).Select(u => u.locationsharestate).First() == false)
				return false;

			bool result = GetPermittedLocationRows(localId).Any(u => u.userid == remoteUserId);
			return result;
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
		#endregion

		public byte[] HashString(string input)
		{
			byte[] bytes = mEncoder.GetBytes(input);
			return HashString(bytes);
		}
		public byte[] HashString(byte[] input)
		{
			return PasswordHasher.ComputeHash(input);
		}

		private static SymmetricAlgorithm SaltDecryptor
		{
			get	{
				if (mSaltDecryptor == null)
				{
					mSaltDecryptor = new AesCryptoServiceProvider();
					mSaltDecryptor.Key = Encoding.Default.GetBytes(ConfigurationManager.AppSettings["SaltDecryptionKey"]);
					mSaltDecryptor.IV = new byte[16];
				}
				return mSaltDecryptor;
			}
		}
		private static HashAlgorithm PasswordHasher
		{
			get	{
				if (mHasher == null)
					mHasher = new SHA256Managed();
				return mHasher;
			}
		}

		private static SymmetricAlgorithm mSaltDecryptor;
		private static HashAlgorithm mHasher; // Yeah!! SHA-256! We rule!
		private static Encoding mEncoder = Encoding.UTF8; // If they can type it, they can use it as a password
	}
}