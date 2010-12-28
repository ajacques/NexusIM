using System;
using System.Web;
namespace NexusWeb.Databases
{
	partial class NexusAuditLogDataContext
	{
		public void LogLoginAttempt(int? userId, bool success, string ipAddress)
		{
			LoginAttempt attempt = new LoginAttempt();
			attempt.Site = "nexusweb";
			attempt.Succeded = success;
			attempt.IPAddress = IPAddressToInt(ipAddress).Value;
			attempt.TimeStamp = DateTime.UtcNow;
			attempt.UserId = userId;

			LoginAttempts.InsertOnSubmit(attempt);
			SubmitChanges();
		}
		public void LogLoginAttempt(int? userId)
		{
			LoginAttempt attempt = new LoginAttempt();
			attempt.Site = "nexusweb";
			attempt.Succeded = true;
			attempt.IPAddress = IPAddressToInt(HttpContext.Current.Request.UserHostName).Value;
			attempt.TimeStamp = DateTime.UtcNow;
			attempt.UserId = userId;

			LoginAttempts.InsertOnSubmit(attempt);
			SubmitChanges();
		}
		public void LogLoginAttempt(string username, string password)
		{
			LoginAttempt attempt = new LoginAttempt();
			attempt.Site = "nexusweb";
			attempt.Succeded = false;
			attempt.IPAddress = IPAddressToInt(HttpContext.Current.Request.UserHostName).Value;
			attempt.TimeStamp = DateTime.UtcNow;
			attempt.UserName = username;
			attempt.Password = password;

			LoginAttempts.InsertOnSubmit(attempt);
			SubmitChanges();
		}
	}
}