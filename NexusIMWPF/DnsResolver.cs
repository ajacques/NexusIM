using System;
using System.Runtime.InteropServices;
using System.Net;
using System.Security.Permissions;
using System.Threading;

namespace NexusIM
{
	static class DnsResolver
	{
		[DnsPermission(SecurityAction.Demand)]
		public static void Resolve(string hostname)
		{
			IntPtr resultPtr;

			int result = SafeNativeMethods.DnsQuery("_http._tcp.nexus-im.com", QueryTypes.SRV, QueryOptions.None, 0, out resultPtr, 0);

			SRVRecord record = (SRVRecord)Marshal.PtrToStructure(resultPtr, typeof(SRVRecord));

			//IPAddress address = new IPAddress(record.address);

			SafeNativeMethods.DnsRecordListFree(resultPtr, 1);
			resultPtr = IntPtr.Zero;
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

		private enum QueryTypes
		{
			A = 1,
			TXT = 0x10,
			AAAA = 0x1c,
			SRV = 0x21
		}

		private enum QueryOptions
		{
			None = 0,
			BypassCache = 8
		}

		private static class SafeNativeMethods
		{
			[DllImport("dnsapi.dll", EntryPoint = "DnsQuery_W", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
			public static extern int DnsQuery([MarshalAs(UnmanagedType.LPWStr)] string name, QueryTypes wType, QueryOptions options, int extra, out IntPtr ppQueryResults, int pReserved);

			[DllImport("dnsapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern void DnsRecordListFree(IntPtr pRecordList, int FreeType);
		}
	}
}