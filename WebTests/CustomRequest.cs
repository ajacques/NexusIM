using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;

namespace WebTests
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
			get	{
				throw new NotImplementedException();
			}
			set	{
				throw new NotImplementedException();
			}
		}

		public HttpCookieMode CookieMode
		{
			get { throw new NotImplementedException(); }
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
			get { throw new NotImplementedException(); }
		}

		public bool IsNewSession
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		public NameObjectCollectionBase.KeysCollection Keys
		{
			get {
				throw new NotImplementedException();
			}
		}

		public int LCID
		{
			get	{
				throw new NotImplementedException();
			}
			set	{
				throw new NotImplementedException();
			}
		}

		public SessionStateMode Mode
		{
			get { throw new NotImplementedException(); }
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
			get { throw new NotImplementedException(); }
		}

		public HttpStaticObjectsCollection StaticObjects
		{
			get { throw new NotImplementedException(); }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public int Timeout
		{
			get	{
				throw new NotImplementedException();
			}
			set	{
				throw new NotImplementedException();
			}
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