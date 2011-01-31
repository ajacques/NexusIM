using System;
using System.Text;

namespace InstantMessage
{
	public class BuddyAvatar
	{
		public BuddyAvatar()
		{

		}
		public static BuddyAvatar FromUrl(string url, string key)
		{
			BuddyAvatar retVal = new BuddyAvatar();

			retVal.mUrl = url;
			retVal.mKey = key;

			return retVal;
		}
		public string Url
		{
			get {
				return mUrl;
			}
		}
		public string Key
		{
			get {
				return mKey;
			}
		}
		private string mUrl = "";
		private string mKey = "";
	}
}