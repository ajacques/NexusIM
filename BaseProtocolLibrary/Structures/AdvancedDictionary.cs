using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;

namespace InstantMessage
{
	public class AdvancedDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>
	{
		public AdvancedDictionary() : base()
		{
			mLock = new ReaderWriterLockSlim();
		}
		public AdvancedDictionary(IComparer<TKey> comparer) : base(comparer)
		{
			mLock = new ReaderWriterLockSlim();
		}

		public new void Add(TKey key, TValue value)
		{
			mLock.EnterWriteLock();

			try	{
				base.Add(key, value);
			} finally {
				mLock.ExitWriteLock();
			}
			
			RaiseItemAdded(key, value);
		}
		public new bool Remove(TKey key)
		{
			mLock.EnterWriteLock();

			bool success;
			try	{
				success = base.Remove(key);
			} finally {
				mLock.ExitWriteLock();
			}

			if (success)
				RaiseItemRemoved(key);

			return success;
		}
		public new void Clear()
		{
			mLock.EnterWriteLock();

			try	{
				base.Clear();
			} finally {
				mLock.ExitWriteLock();
			}

			RaiseCollReset();
		}

		private void RaiseItemAdded(TKey newKey, TValue newValue)
		{
			if (DictionaryChanged != null)
			{
				mLock.EnterReadLock();

				try {
					DictionaryChanged(this, DictionaryChangedEventArgs<TKey, TValue>.NewDelete(newKey, newValue));
				} finally {
					mLock.ExitReadLock();
				}
			}
		}
		private void RaiseItemRemoved(TKey oldItem)
		{
			if (DictionaryChanged != null)
			{
				mLock.EnterReadLock();

				try {
					DictionaryChanged(this, DictionaryChangedEventArgs<TKey, TValue>.NewDelete(oldItem, default(TValue)));
				} finally {
					mLock.ExitReadLock();
				}
			}
		}
		private void RaiseCollReset()
		{
			if (DictionaryChanged != null)
			{
				mLock.EnterReadLock();

				try {
					DictionaryChanged(this, new DictionaryChangedEventArgs<TKey, TValue>());
				} finally {
					mLock.ExitReadLock();
				}
			}
		}

		public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new ConcurrentEnumerator(base.GetEnumerator(), mLock);
		}

		// Nested Class
		private class ConcurrentEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			public ConcurrentEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> source, ReaderWriterLockSlim lockObject)
			{
				mSource = source;
				mLock = lockObject;
			}

			// Properties
			public KeyValuePair<TKey, TValue> Current
			{
				get {
					return mSource.Current;
				}
			}
			public void Dispose()
			{
				mLock.ExitReadLock();
				mSource.Dispose();
				mLock = null;
				mSource = null;
			}
			object IEnumerator.Current
			{
				get {
					return Current;
				}
			}
			public bool MoveNext()
			{
				if (!haveLock)
				{
					mLock.EnterReadLock();
					haveLock = true;
				}

				return mSource.MoveNext();
			}
			public void Reset()
			{
				mSource.Reset();
			}

			// Variables
			private bool haveLock;
			private ReaderWriterLockSlim mLock;
			private IEnumerator<KeyValuePair<TKey, TValue>> mSource;
		}

		// Events
		public event EventHandler<DictionaryChangedEventArgs<TKey, TValue>> DictionaryChanged;

		protected ReaderWriterLockSlim SyncRoot
		{
			get {
				return mLock;
			}
		}
		protected object RootNode
		{
			get	{
				Type t = base.GetType().BaseType;

				if (t == typeof(AdvancedDictionary<TKey, TValue>))
					t = t.BaseType;

				FieldInfo info = t.GetField("root", BindingFlags.Instance | BindingFlags.NonPublic);
				return info.GetValue(this);
			}
		}

		// Variables
		private ReaderWriterLockSlim mLock;
	}
}