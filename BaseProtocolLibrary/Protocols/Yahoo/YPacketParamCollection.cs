using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace InstantMessage.Protocols.Yahoo
{
	internal class YPacketParamCollection : IDictionary<int, string>
	{
		private List<KeyValuePair<int, string>> mGroups = new List<KeyValuePair<int, string>>();

		#region IDictionary<int,string> Members

		public void Add(int key, string value)
		{
			if (IsReadOnly)
				throw new InvalidOperationException();

			mGroups.Add(new KeyValuePair<int, string>(key, value));
		}
		public bool ContainsKey(int key)
		{
			return mGroups.Any(s => s.Key == key);
		}
		public ICollection<int> Keys
		{
			get {
				throw new NotSupportedException();
			}
		}
		public bool Remove(int key)
		{
			if (IsReadOnly)
				throw new InvalidOperationException();

			throw new NotImplementedException();
		}
		public bool TryGetValue(int key, out string value)
		{
			foreach (var pair in mGroups)
			{
				if (pair.Key == key)
				{
					value = pair.Value;
					return true;
				}
			}
			value = null;
			return false;
		}
		public ICollection<string> Values
		{
			get { throw new NotImplementedException(); }
		}

		public string this[int key]
		{
			get	{
				throw new NotImplementedException();
			}
			set	{
				if (IsReadOnly)
					throw new InvalidOperationException();
				throw new NotImplementedException();
			}
		}

		#endregion

		#region ICollection<KeyValuePair<int,string>> Members

		public void Add(KeyValuePair<int, string> item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<int, string> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<int, string>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get;
			internal set;
		}

		public bool Remove(KeyValuePair<int, string> item)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<KeyValuePair<int,string>> Members

		public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
		{
			return mGroups.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}