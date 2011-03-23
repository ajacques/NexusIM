using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusWeb.Infrastructure.Redis;
using System.Diagnostics;

namespace WebTests
{
	[TestClass]
	public class RedisTests
	{
		[TestMethod]
		public void PerformanceTest()
		{
			Random r = new Random();
			RedisClient client = new RedisClient("192.168.56.102");
			byte[] b = new byte[16];
			r.NextBytes(b);
			client.Connect();

			Stopwatch sw = new Stopwatch();
			sw.Start();
			try	{
				client.Set("test", b);
			} catch (RedisException e) {
				Trace.WriteLine("ActiveServer: " + e.ActiveServer);
				Trace.WriteLine("ErrorDetail: " + e.ErrorDetail);
				throw;
			}
			sw.Stop();

			Trace.WriteLine(sw.Elapsed);
		}
	}
}