using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NexusCore.Databases;
using NexusCore.DataContracts;

namespace NexusCore.Support
{
	sealed class AccountEnumerable : IEnumerable<AccountInfo>
	{
		/// <summary>
		/// Creates a new AccountEnumerable to convert the database type to the custom AccountInfo type.
		/// Decrypts the password if keygenVector is not null
		/// </summary>
		public AccountEnumerable(IQueryable<Account> source, byte[] keygenVector = null)
		{
			mSource = source;
			mKeygenVector = keygenVector;
		}
		private sealed class AccountEnumerator : IEnumerator<AccountInfo>
		{
			public AccountEnumerator(IQueryable<Account> source, byte[] keygenVector = null)
			{
				mEnumerator = source.GetEnumerator();
				mKeygenVector = keygenVector;
			}

			public AccountInfo Current
			{
				get {
					Account source = mEnumerator.Current;

					AccountInfo info = new AccountInfo(source.acctype, source.username);
					info.mEnabled = source.enabled;
					info.mAccountId = source.id;
					info.mServer = source.server;
					if (mKeygenVector != null)
						info.mPassword = source.DecryptPassword(mKeygenVector);
					else
						info.mEncPassword = source.password;

					return info;
				}
			}
			public void Dispose()
			{
				mEnumerator.Dispose();
				mKeygenVector = null;
				mEnumerator = null;
			}

			object IEnumerator.Current
			{
				get {
					return Current;
				}
			}
			public bool MoveNext()
			{
				return mEnumerator.MoveNext();
			}
			public void Reset()
			{
				mEnumerator.Reset();
			}

			private IEnumerator<Account> mEnumerator;
			private byte[] mKeygenVector;
		}
		public IEnumerator<AccountInfo> GetEnumerator()
		{
			return new AccountEnumerator(mSource, mKeygenVector);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private IQueryable<Account> mSource;
		private byte[] mKeygenVector;
	}
}