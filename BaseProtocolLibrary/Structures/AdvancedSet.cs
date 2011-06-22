using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;

namespace InstantMessage
{
	public class AdvancedSet<T> : SortedSet<T>, IEnumerable, INotifyCollectionChanged where T : class
	{
		public AdvancedSet() : base()
		{
			mLock = new ReaderWriterLockSlim();
		}
		public AdvancedSet(IComparer<T> comparer) : base(comparer)
		{
			mLock = new ReaderWriterLockSlim();
		}

		public new bool Add(T item)
		{
			mLock.EnterWriteLock();

			bool success;
			try	{
				success = base.Add(item);
			} finally {
				mLock.ExitWriteLock();
			}

			if (success)
				RaiseItemAdded(item);

			return success;
		}
		public new bool Remove(T item)
		{
			mLock.EnterWriteLock();

			bool success;
			try	{
				success = base.Remove(item);
			} finally {
				mLock.ExitWriteLock();
			}

			if (success)
				RaiseItemRemoved(item);

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
		}

		protected void RaiseItemAdded(T newItem)
		{
			if (CollectionChanged != null)
			{
				mLock.EnterReadLock();

				try {
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem));
				} finally {
					mLock.ExitReadLock();
				}
			}
		}
		protected void RaiseItemRemoved(T oldItem)
		{
			if (CollectionChanged != null)
			{
				mLock.EnterReadLock();

				try {
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem));
				} finally {
					mLock.ExitReadLock();
				}
			}
		}
		protected void RaiseItemRemoved(IList<T> oldItems)
		{
			if (CollectionChanged != null)
			{
				mLock.EnterReadLock();

				try {
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
				} finally {
					mLock.ExitReadLock();
				}
			}
		}
		protected void RaiseCollReset()
		{
			if (CollectionChanged != null)
			{
				mLock.EnterReadLock();

				try {
					CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				} finally {
					mLock.ExitReadLock();
				}
			}
		}

		public new IEnumerator<T> GetEnumerator()
		{
			return new ConcurrentEnumerator(base.GetEnumerator(), mLock);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Nested Class
		private class ConcurrentEnumerator : IEnumerator<T>
		{
			public ConcurrentEnumerator(IEnumerator<T> source, ReaderWriterLockSlim lockObject)
			{
				mSource = source;
				mLock = lockObject;
			}

			// Properties
			public T Current
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
			private IEnumerator<T> mSource;
		}

		// Events
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public ReaderWriterLockSlim SyncRoot
		{
			get {
				return mLock;
			}
		}

		// Variables
		private ReaderWriterLockSlim mLock;
	}
}