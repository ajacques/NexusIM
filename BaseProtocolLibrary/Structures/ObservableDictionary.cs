using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InstantMessage;

namespace System.Collections.Specialized
{
	public enum DictionaryChangedEventType
	{
		Added,
		Removed
	}
	public class DictionaryChangedEventArgs<TKey, TValue> : EventArgs
	{
		internal static DictionaryChangedEventArgs<TKey, TValue> NewCreate(TKey key, TValue value) 
		{
			DictionaryChangedEventArgs<TKey, TValue> ret = new DictionaryChangedEventArgs<TKey, TValue>();
			ret.mType = DictionaryChangedEventType.Added;
			ret.mChanged = new KeyValuePair<TKey, TValue>(key, value);

			return ret;
		}
		internal static DictionaryChangedEventArgs<TKey, TValue> NewDelete(TKey key, TValue value)
		{
			DictionaryChangedEventArgs<TKey, TValue> ret = new DictionaryChangedEventArgs<TKey, TValue>();
			ret.mType = DictionaryChangedEventType.Removed;
			ret.mChanged = new KeyValuePair<TKey, TValue>(key, value);

			return ret;
		}

		public DictionaryChangedEventType ChangeType
		{
			get {
				return mType;
			}
		}
		public KeyValuePair<TKey, TValue> ChangedRow
		{
			get {
				return mChanged;
			}
		}

		private DictionaryChangedEventType mType;
		private KeyValuePair<TKey, TValue> mChanged;
	}
	public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		public ObservableDictionary() {}

		public void TriggeredAdd(IMProtocol protocol, TKey key, TValue value)
		{
			try	{
				base.Add(key, value);
			} catch (ArgumentException) {
				base[key] = value;
			}

			if (DictionaryChanged != null)
				DictionaryChanged(protocol, DictionaryChangedEventArgs<TKey, TValue>.NewDelete(key, value));
		}

		public void Add(KeyValuePair<TKey, TValue> pair)
		{
			base.Add(pair.Key, pair.Value);
		}

		public void TrggeredRemove(IMProtocol protocol, TKey key)
		{
			if (DictionaryChanged != null)
				DictionaryChanged(protocol, DictionaryChangedEventArgs<TKey, TValue>.NewDelete(key, base[key]));

			base.Remove(key);
		}

		public event EventHandler<DictionaryChangedEventArgs<TKey, TValue>> DictionaryChanged;
	}
}
