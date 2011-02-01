using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;

namespace CloudTests
{
	class UnitTestSession : IHttpSessionState
	{
		public UnitTestSession()
		{
			mObjects = new Dictionary<string, object>();
		}
		public void Abandon()
		{
			mObjects.Clear();
		}
		public void Add(string name, object value)
		{
			mObjects.Add(name, value);
		}
		public void Clear()
		{
			mObjects.Clear();
		}

		public int CodePage
		{
			get;
			set;
		}
		public HttpCookieMode CookieMode
		{
			get;
			internal set;
		}
		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}
		public int Count
		{
			get {
				return mObjects.Count;
			}
		}
		public System.Collections.IEnumerator GetEnumerator()
		{
			return mObjects.GetEnumerator();
		}
		public bool IsCookieless
		{
			get;
			internal set;
		}
		public bool IsNewSession
		{
			get;
			internal set;
		}
		public bool IsReadOnly
		{
			get;
			internal set;
		}
		public bool IsSynchronized
		{
			get;
			internal set;
		}
		public NameObjectCollectionBase.KeysCollection Keys
		{
			get;
			internal set;
		}
		public int LCID
		{
			get;
			set;
		}
		public SessionStateMode Mode
		{
			get;
			internal set;
		}
		public void Remove(string name)
		{
			mObjects.Remove(name);
		}
		public void RemoveAll()
		{
			throw new NotImplementedException();
		}
		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}
		public string SessionID
		{
			get;
			internal set;
		}
		public HttpStaticObjectsCollection StaticObjects
		{
			get;
			internal set;
		}
		public object SyncRoot
		{
			get;
			internal set;
		}
		public int Timeout
		{
			get;
			set;
		}
		public object this[int index]
		{
			get	{
				throw new NotImplementedException();
			}
			set	{
				throw new NotImplementedException();
			}
		}
		public object this[string name]
		{
			get	{
				try {
					return mObjects[name];
				} catch (KeyNotFoundException) {
					return null;
				}
			}
			set	{
				mObjects[name] = value;
			}
		}

		private Dictionary<string, object> mObjects;
	}
}