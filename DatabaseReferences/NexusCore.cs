using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NexusCore.Databases
{
	partial class NexusCoreDataContext
	{
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
			byte[] output = new byte[48];
			Buffer.BlockCopy(user.PasswordSalt, 0, output, 0, user.PasswordSalt.Length);
			ICryptoTransform decryptor = SaltDecryptor.CreateDecryptor();
			int count = decryptor.TransformBlock(output, 0, output.Length, output, 0);

			// Now build the input array from the decrypted hash and password
			byte[] pwdbytes = Encoder.GetBytes(password);
			byte[] concatOutput = new byte[count + password.Length];
			Buffer.BlockCopy(output, 0, concatOutput, 0, count); // BlockCopy is acceptable here because we are copying bytes which means we don't have to worry about the actual size of objects or other bounds checking
			Buffer.BlockCopy(pwdbytes, 0, concatOutput, count, pwdbytes.Length);

			byte[] pwdHash = HashString(concatOutput);

			// Check each byte to see if they match
			for (int i = 0; i < pwdHash.Length; i++)
			{
				if (pwdHash[i] != user.Password[i])
					return null;
			}
			
			// Update the database to reflect that the user has just logged in
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
			byte[] bytes = Encoder.GetBytes(input);
			return HashString(bytes);
		}
		public byte[] HashString(byte[] input)
		{
			return PasswordHasher.ComputeHash(input);
		}

		/// <summary>
		/// Returns the Symmetric Algorithm that is used to decrypt the password salt.
		/// Also loads the decryption key and IV
		/// </summary>
		public static SymmetricAlgorithm SaltDecryptor
		{
			get	{
				if (mSaltDecryptor == null)
				{
					mSaltDecryptor = new AesManaged();
					mSaltDecryptor.Key = Encoding.Default.GetBytes(ConfigurationManager.AppSettings["SaltDecryptionKey"]);
					mSaltDecryptor.IV = new byte[16];
				}
				return mSaltDecryptor;
			}
		}
		public static HashAlgorithm PasswordHasher
		{
			get	{
				if (mHasher == null)
					mHasher = new SHA256Managed();
				return mHasher;
			}
		}
		public static Encoding Encoder
		{
			get	{
				return Encoding.UTF8;
			}
		}

		private static SymmetricAlgorithm mSaltDecryptor;
		private static HashAlgorithm mHasher;
	}

	public partial class Account
	{
		public string DecryptPassword(byte[] keygenVector)
		{
			SymmetricAlgorithm symkey = new AesManaged();			
			symkey.Key = ComputeEncryptionKey(keygenVector);
			symkey.IV = new byte[16];

			ICryptoTransform transform = symkey.CreateDecryptor();
			byte[] oPassword = new byte[password.Length];
			int count = transform.TransformBlock(password, 0, password.Length, oPassword, 0);
			
			string result = NexusCoreDataContext.Encoder.GetString(oPassword, 0, count);
			result = result.TrimEnd('\0');

			return result;
		}
		/// <summary>
		/// Computes the cipher text for the new password
		/// </summary>
		/// <remarks>
		/// Does not save the changes to the database. You must called SubmitChanges() after calling this method
		/// </remarks>
		/// <param name="keygenVector">16-byte array used as a vector to create the encryption key</param>
		/// <param name="newPassword">String to change the password</param>
		public void ChangePassword(byte[] keygenVector, string newPassword)
		{
			SymmetricAlgorithm symkey = new AesManaged();
			symkey.Key = ComputeEncryptionKey(keygenVector);
			symkey.IV = new byte[16];

			byte[] nPwdBytes = NexusCoreDataContext.Encoder.GetBytes(newPassword);
			byte[] paddedPassword = new byte[nPwdBytes.Length + (48 - (nPwdBytes.Length % 48))];
			Buffer.BlockCopy(nPwdBytes, 0, paddedPassword, 0, nPwdBytes.Length);

			byte[] output = new byte[paddedPassword.Length];
			ICryptoTransform transform = symkey.CreateEncryptor();
			int count = transform.TransformBlock(paddedPassword, 0, paddedPassword.Length, output, 0);

			password = output;
		}

		private byte[] ComputeEncryptionKey(byte[] keygenVector)
		{
			byte[] idBytes = BitConverter.GetBytes(id);
			if (!BitConverter.IsLittleEndian)
				Array.Reverse(idBytes);

			byte[] stage1 = new byte[idBytes.Length + keygenVector.Length];
			Buffer.BlockCopy(idBytes, 0, stage1, 0, idBytes.Length);
			Buffer.BlockCopy(keygenVector, 0, stage1, idBytes.Length, keygenVector.Length);

			HashAlgorithm hasher = NexusCoreDataContext.PasswordHasher;
			return hasher.ComputeHash(stage1);
		}
	}
}