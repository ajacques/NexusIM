using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;

namespace NexusIM.Misc
{
	internal class AdvancedSet<T> : SortedSet<T>, INotifyCollectionChanged where T : class
	{
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
		
		/// <summary>
		/// Searches the tree for an item matching the predicate without using recursion.
		/// </summary>
		protected T SearchNoStack(object root, Func<T, int> predicate)
		{
			if (root == null)
				return null;

			Type type = root.GetType();
			FieldInfo left = type.GetField("Left");
			FieldInfo right = type.GetField("Right");

			int num;
			mLock.EnterReadLock();
			for (object node = root; node != null; node = (num < 0) ? left.GetValue(node) : right.GetValue(node))
			{
				T value = (T)type.GetField("Item").GetValue(node);

				num = predicate(value);
				if (num == 0)
				{
					mLock.ExitReadLock();
					return value;
				}
			}
			mLock.ExitReadLock();
			return null;
		}

		protected T Search(object node, Func<T, int> predicate)
		{
			if (node == null)
				return null;

			Type type = node.GetType();
			T value = (T)type.GetField("Item").GetValue(node);

			int comparison = predicate(value);

			if (comparison == 0)
				return value;

			object newNode;

			if (comparison < 0)
				newNode = type.GetField("Left").GetValue(node);
			else
				newNode = type.GetField("Right").GetValue(node);
			
			return Search(newNode, predicate);
		}

		private void RaiseItemAdded(T newItem)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem ));
		}
		private void RaiseItemRemoved(T oldItem)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem));
		}
		private void RaiseCollReset()
		{
			if (CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public new IEnumerator<T> GetEnumerator()
		{
			return new ConcurrentEnumerator(base.GetEnumerator(), mLock);
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

				if (t == typeof(AdvancedSet<T>))
					t = t.BaseType;

				FieldInfo info = t.GetField("root", BindingFlags.Instance | BindingFlags.NonPublic);
				return info.GetValue(this);
			}
		}

		// Variables
		private ReaderWriterLockSlim mLock;
	}
}