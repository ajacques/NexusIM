using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using InstantMessage;

namespace NexusIM.Misc
{
	internal class ProtocolCollection : SortedSet<IMProtocolWrapper>, INotifyCollectionChanged
	{
		public ProtocolCollection() : base(new WrapperComparer())
		{
			mLock = new ReaderWriterLock();
		}

		public new bool Add(IMProtocolWrapper item)
		{
			mLock.AcquireWriterLock(-1);

			bool success;
			try	{
				success = base.Add(item);
			} finally {
				mLock.ReleaseWriterLock();
			}

			if (success)
				RaiseItemAdded(item);

			return success;
		}
		public new bool Remove(IMProtocolWrapper item)
		{
			mLock.AcquireWriterLock(-1);

			bool success;
			try	{
				success = base.Remove(item);
			} finally {
				mLock.ReleaseWriterLock();
			}

			if (success)
				RaiseItemRemoved(item);

			return success;
		}

		private void RaiseItemAdded(IMProtocolWrapper newItem)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem ));
		}
		private void RaiseItemRemoved(IMProtocolWrapper oldItem)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem));
		}

		public new IEnumerator<IMProtocolWrapper> GetEnumerator()
		{
			return new ConcurrentEnumerator(base.GetEnumerator(), mLock);
		}

		// Nested Class
		private class ConcurrentEnumerator : IEnumerator<IMProtocolWrapper>
		{
			public ConcurrentEnumerator(IEnumerator<IMProtocolWrapper> source, ReaderWriterLock lockObject)
			{
				mSource = source;
				mLock = lockObject;
			}

			// Properties
			public IMProtocolWrapper Current
			{
				get {
					return mSource.Current;
				}
			}
			public void Dispose()
			{
				mLock.ReleaseReaderLock();
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
					mLock.AcquireReaderLock(-1);
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
			private ReaderWriterLock mLock;
			private IEnumerator<IMProtocolWrapper> mSource;
		}
		private class WrapperComparer : IComparer<IMProtocolWrapper>
		{
			public int Compare(IMProtocolWrapper left, IMProtocolWrapper right)
			{
				if (left.Protocol.Protocol != right.Protocol.Protocol)
					return left.Protocol.Protocol.CompareTo(right.Protocol.Protocol);

				return left.Protocol.Username.CompareTo(right.Protocol.Username);
			}
		}

		// Events
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		// Variables
		private ReaderWriterLock mLock;
	}
}