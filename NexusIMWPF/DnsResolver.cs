using System;
using System.Collections.Generic;
using System.Net;
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

	[DnsPermission(SecurityAction.Demand)]
	static class DnsResolver
	{
		public static IEnumerable<ServiceRecordInfo> ResolveService(string hostname)
		{
			IntPtr resultPtr;

			QueryResult result = SafeNativeMethods.DnsQuery(hostname, DnsQueryType.SRV, QueryOptions.None, 0, out resultPtr, 0);

			ICollection<ServiceRecordInfo> records = new SortedSet<ServiceRecordInfo>(ServiceRecordInfo.Comparer);

			SRVRecord record;
			do {
				record = (SRVRecord)Marshal.PtrToStructure(resultPtr, typeof(SRVRecord));
				records.Add(new ServiceRecordInfo(record.pNameTarget, record.wPort, record.wPriority, record.wWeight));
			} while (record.pNext.ToInt32() != 0);

			SafeNativeMethods.DnsRecordListFree(resultPtr, 1);
			resultPtr = IntPtr.Zero;

			return records;
		}

		public static IEnumerable<IPAddress> ResolveIP(string hostname)
		{
			IntPtr resultPtr;

			QueryResult result = SafeNativeMethods.DnsQuery(hostname, DnsQueryType.A, QueryOptions.none, 0, out resultPtr, 0);

			IList<IPAddress> records = new List<IPAddress>();

			ARecord record;
			do {
				record = (ARecord)Marshal.PtrToStructure(resultPtr, typeof(ARecord));
				records.Add(new IPAddress(record.address));
			} while (record.pNext.ToInt32() != 0);

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
			public long address;
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
			NameDoesNotExist = 9003
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