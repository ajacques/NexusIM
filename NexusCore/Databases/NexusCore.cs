using System;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NexusCore.DataContracts;
using NexusCore.Properties;

namespace NexusCore.Databases
{
	partial class NexusCoreDataContext
	{
		public User TryLogin(string username, string password)
		{
			var user = (from c in this.Users
						where c.username == username
						select c).FirstOrDefault();

			if (user == null)
				return null;

			byte[] output = new byte[user.PasswordSalt.Length];
			ICryptoTransform decryptor = SaltDecryptor.CreateDecryptor();
			int count = decryptor.TransformBlock(user.PasswordSalt, 0, user.PasswordSalt.Length, output, 0);
			string salt = mEncoder.GetString(output, 0, count);

			password = HashString(salt + password);

			if (user.password != password)
				return null;
			
			user.lastseen = DateTime.UtcNow;
			try	{
				SubmitChanges();
			} catch (ChangeConflictException) {} // Workaround for Expression Web 4 SuperPreview loading two pages at once

			return user;
		}
		public User TryHashLogin(int userid, string pwdhash)
		{
			var users = from c in this.Users where c.id == userid && c.password == pwdhash select c;

			return users.FirstOrDefault();
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
		public IEnumerable<ISwarmMessage> GetDeviceMessageQueue(int deviceid)
		{
			throw new NotImplementedException();
		}
		public void AddToDeviceMessageQueue(int deviceid, ISwarmMessage message)
		{
			throw new NotImplementedException();
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
		#endregion

		public string HashString(string input)
		{
			byte[] bytes = mEncoder.GetBytes(input);
			return HashString(bytes);
		}
		public string HashString(byte[] input)
		{
			byte[] value = PasswordHasher.ComputeHash(input);
			StringBuilder sb = new StringBuilder();
			foreach (byte x in value)
				sb.AppendFormat("{0:x2}", x);

			return sb.ToString();
		}

		private static SymmetricAlgorithm SaltDecryptor
		{
			get	{
				if (mSaltDecryptor == null)
				{
					mSaltDecryptor = new AesCryptoServiceProvider();
					mSaltDecryptor.Key = Encoding.Default.GetBytes(Settings.Default.SaltDecryptionKey);
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