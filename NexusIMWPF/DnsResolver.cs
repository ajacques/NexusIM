using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace NexusIM
{
	class ServiceRecordInfo
	{
		public ServiceRecordInfo(string name, int port, int priority, int weight)
		{
			ServerName = name;
			Port = port;
			Priority = priority;
			Weight = weight;
		}

		public string ServerName
		{
			get;
			private set;
		}
		public int Port
		{
			get;
			private set;
		}
		public int Priority
		{
			get;
			private set;
		}
		public int Weight
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return String.Format("{0}:{1} Priority: {2} Weight: {3}", ServerName, Port, Priority, Weight);
		}

		private class InstComparer : IComparer<ServiceRecordInfo>
		{
			public int Compare(ServiceRecordInfo x, ServiceRecordInfo y)
			{
				return x.Priority.CompareTo(y.Priority);
			}
		}

		public static IComparer<ServiceRecordInfo> Comparer
		{
			get {
				return new InstComparer();
			}
		}
	}

	static class DnsResolver
	{
		public static IEnumerable<ServiceRecordInfo> ResolveService(string hostname)
		{
			return ResolveMany<SRVRecord, ServiceRecordInfo>(hostname, DnsQueryType.SRV, (record) => new ServiceRecordInfo(record.pNameTarget, record.wPort, record.wPriority, record.wWeight));
		}

		public static IEnumerable<IPAddress> ResolveIP(string hostname)
		{
			IEnumerable<IPAddress> ipv4Addr = ResolveMany<ARecord, IPAddress>(hostname, DnsQueryType.A, (record) => new IPAddress(record.address));
			IEnumerable<IPAddress> ipv6Addr = ResolveMany<AAAARecord, IPAddress>(hostname, DnsQueryType.AAAA, (record) => new IPAddress(record.Address));

			return ipv4Addr.Concat(ipv6Addr);
		}

		[DnsPermission(SecurityAction.Demand)]
		private static IEnumerable<TResult> ResolveMany<TRecord, TResult>(string hostname, DnsQueryType type, Func<TRecord, TResult> translator)
		{
			IntPtr resultPtr;

			QueryResult result = SafeNativeMethods.DnsQuery(hostname, type, QueryOptions.None, 0, out resultPtr, 0);

			IList<TResult> records = new List<TResult>();

			if (result == QueryResult.NoRecords)
				return records;

			TRecord record;
			FieldInfo pNextField = typeof(TRecord).GetField("pNext");

			while (resultPtr != IntPtr.Zero)
			{
				record = (TRecord)Marshal.PtrToStructure(resultPtr, typeof(TRecord));
				records.Add(translator(record));

				resultPtr = (IntPtr)pNextField.GetValue(record);
			}

			SafeNativeMethods.DnsRecordListFree(resultPtr, 1);
			resultPtr = IntPtr.Zero;

			return records;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct ARecord
		{
			public IntPtr pNext;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pName;
			public short wType;
			public short wDataLength;
			public int flags;
			public int dwTtl;
			public int dwReserved;
			public long address;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct TXTRecord
		{
			public IntPtr pNext;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pName;
			public short wType;
			public short wDataLength;
			public int flags;
			public int dwTtl;
			public int dwReserved;
			public uint dwStringCount;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pString;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SRVRecord
		{
			public IntPtr pNext;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pName;
			public short wType;
			public short wDataLength;
			public int flags;
			public int dwTtl;
			public int dwReserved;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pNameTarget;
			public ushort wPriority;
			public ushort wWeight;
			public ushort wPort;
			public ushort Pad;

			public DnsQueryType QueryType
			{
				get {
					return (DnsQueryType)wType;
				}
			}
			public TimeSpan Ttl
			{
				get {
					return TimeSpan.FromSeconds(dwTtl);
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct AAAARecord
		{
			public IntPtr pNext;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pName;
			public short wType;
			public short wDataLength;
			public int flags;
			public int dwTtl;
			public int dwReserved;
			public long addressLow;
			public long addressHigh;

			public byte[] Address
			{
				get {
					byte[] array = new byte[16];

					Buffer.BlockCopy(BitConverter.GetBytes(addressLow), 0, array, 0, 8);
					Buffer.BlockCopy(BitConverter.GetBytes(addressHigh), 0, array, 8, 8);

					return array;
				}
			}
		}

		/// <summary>
		/// Specifies what DNS record should be resolved.
		/// </summary>
		private enum DnsQueryType
		{
			A = 1,
			PTR = 0x0c,
			TXT = 0x10,
			AAAA = 0x1c,
			SRV = 0x21
		}

		private enum QueryOptions
		{
			None = 0,
			BypassCache = 8
		}

		private enum QueryResult
		{
			None = 0,
			ServerFailure = 9002,
			NameDoesNotExist = 9003,
			QueryRefused = 9005,
			NoRecords = 9501
		}

		private static class SafeNativeMethods
		{
			[DllImport("dnsapi.dll", EntryPoint = "DnsQuery_W", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
			[return: MarshalAs(UnmanagedType.Error)]
			public static extern QueryResult DnsQuery([MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.U4)] DnsQueryType wType, [MarshalAs(UnmanagedType.U4)] QueryOptions options, int extra, out IntPtr ppQueryResults, int pReserved);

			[DllImport("dnsapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern void DnsRecordListFree(IntPtr pRecordList, int FreeType);
		}
	}
}