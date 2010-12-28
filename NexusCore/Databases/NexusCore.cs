using System;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NexusCore.DataContracts;

namespace NexusCore.Databases
{
	partial class NexusCoreDataContext
	{
		public User TryLogin(string username, string password)
		{
			password = hashPassword(password); // Prepare for this

			var users = from c in this.Users where c.username == username && c.password == password select c;

			return users.FirstOrDefault();
		}
		public User TryHashLogin(int userid, string pwdhash)
		{
			var users = from c in this.Users where c.id == userid && c.password == pwdhash select c;

			return users.FirstOrDefault();
		}
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
		private string hashPassword(string password)
		{
			string output = "";
			byte[] bytes = mEncoder.GetBytes(password);
			byte[] value = mHasher.ComputeHash(bytes);
			foreach (byte x in value)
				output += String.Format("{0:x2}", x);

			return output;
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

		private string serializeObject(object obj)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, obj);

			stream.Position = 0;
			StreamReader reader = new StreamReader(stream);

			return reader.ReadToEnd();
		}
		private object deserializeObject(string data)
		{
			MemoryStream stream = new MemoryStream(mEncoder.GetBytes(data));
			BinaryFormatter formatter = new BinaryFormatter();
			return formatter.Deserialize(stream);
		}

		private static HashAlgorithm mHasher = new SHA256Managed(); // Yeah!! SHA-256! We rule!
		private static Encoding mEncoder = Encoding.UTF8; // If they can type it, they can use it as a password
	}
}