using System.Linq;

namespace CloudTests
{
	partial class NexusCoreDataContext
	{
		public string GetTestToken()
		{
			var token = from t in this.AuthTokens where t.userid == 1 select t;
			return token.FirstOrDefault().token;
		}

		public string GetDeviceTestToken()
		{
			return (from t in this.Devices
					select t.logintoken).FirstOrDefault();
		}

		public User GetTestUserRow()
		{
			User t = new User();
			t.username = "test";
			t.password = "test";
			t.id = 1;
			return t;
		}
	}
}