using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusWeb.Infrastructure.Redis;
using System.Threading;

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

		[TestMethod]
		public void ExpireTest()
		{
			Random r = new Random();
			RedisClient client = new RedisClient("192.168.56.102");

			byte[] b = new byte[16];
			r.NextBytes(b);
			client.Connect();

			try	{
				client.Set("testexpires", b, TimeSpan.FromSeconds(1));
			} catch (RedisException e) {
				Trace.WriteLine("ActiveServer: " + e.ActiveServer);
				Trace.WriteLine("ErrorDetail: " + e.ErrorDetail);
				throw;
			}

			Assert.IsNotNull(client.Get("testexpires"));
			Thread.Sleep(2000);

			Assert.IsNull(client.Get("testexpires"));
		}

		[TestMethod]
		public void IncrementTest()
		{
			RedisClient client = new RedisClient("192.168.56.102");
			
			client.Connect();
			try	{
				int resp = client.Increment("inctest", 2);
				Trace.WriteLine(resp);
			} catch (RedisException e) {
				Trace.WriteLine("ActiveServer: " + e.ActiveServer);
				Trace.WriteLine("ErrorDetail: " + e.ErrorDetail);
				throw;
			}
		}
	}
}