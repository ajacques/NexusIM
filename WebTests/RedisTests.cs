using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NexusWeb.Infrastructure.Redis;
using System.Threading;
using System.Collections.Generic;

namespace WebTests
{
	[TestClass]
	public class RedisTests
	{
		[TestMethod]
		public void PerformanceTest()
		{
			Random r = new Random();
			RedisClient client = new RedisClient(mServer);
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
			RedisClient client = new RedisClient(mServer);

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
			Assert.AreEqual(TimeSpan.FromSeconds(1), client.GetTimeToLive("testexpires"));
			Thread.Sleep(2000);

			Assert.IsNull(client.Get("testexpires"));
		}

		[TestMethod]
		public void IncrementTest()
		{
			RedisClient client = new RedisClient(mServer);
			
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

		[TestMethod]
		public void DeleteTest()
		{
			RedisClient client = new RedisClient(mServer);
			client.Connect();

			try	{
				client.Set("testkey", new byte[] { 0x0, 0x05, 0xff });
				client.Set("testkey1", new byte[] { 0x0, 0x05, 0xff });
				client.Set("testkey2", new byte[] { 0x0, 0x05, 0xff });

				Assert.AreEqual(3, client.Delete("testkey", "testkey1", "testkey2"));
			} catch (RedisException e) {
				Trace.WriteLine("ActiveServer: " + e.ActiveServer);
				Trace.WriteLine("ErrorDetail: " + e.ErrorDetail);
				throw;
			}
		}

		[TestMethod]
		public void EmptyListTest()
		{
			RedisClient client = new RedisClient(mServer);
			client.Connect();
			client.ChangeDatabase(1);
			client.FlushDatabase();

			RedisList list = client.GetList("UnitList");
			Assert.IsFalse(list.IsReadOnly);
			Assert.AreEqual(0, list.Count);
		}

		[TestMethod]
		public void ListAddtest()
		{
			RedisClient client = new RedisClient(mServer);
			client.Connect();
			client.ChangeDatabase(1);
			client.FlushDatabase();

			RedisList list = client.GetList("UnitList");

			for (byte i = 65; i < 75; i++)
				list.Add(new byte[] { i });

			Assert.AreEqual(10, list.Count);
			Assert.AreEqual(65, list[0][0]);

			IEnumerator<byte[]> cursor = list.GetEnumerator();
			for (byte i = 65; i < 75; i++)
			{
				Assert.AreEqual(i, cursor.Current[0]);

				cursor.MoveNext();
			}
		}

		[TestMethod]
		public void BatchListAddtest()
		{
			RedisClient client = new RedisClient(mServer);
			client.Connect();
			client.ChangeDatabase(1);
			client.FlushDatabase();

			RedisList list = client.GetList("UnitList");

			list.BeginBatchOperation();
			for (byte i = 65; i < 75; i++)
				list.Add(new byte[] { i });
			list.CommitBatchOperation();

			Assert.AreEqual(10, list.Count);
			Assert.AreEqual(65, list[0][0]);

			IEnumerator<byte[]> cursor = list.GetEnumerator();
			for (byte i = 65; i < 75; i++)
			{
				Assert.AreEqual(i, cursor.Current[0]);

				cursor.MoveNext();
			}
		}

		private static readonly string mServer = "192.168.56.102";
	}
}