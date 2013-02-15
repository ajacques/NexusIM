using System;
using System.Collections.Generic;
using System.Text;

namespace InstantMessage.Protocols.Yahoo
{
	internal static class YahooServices2
	{
		public static readonly byte[] ymsg_pager_logoff = { 0x00, 0x02 };
		public static readonly byte[] ymsg_message = { 0x00, 0x06 };
		public static readonly byte[] ymsg_authentication_response = { 0x00, 0x54 };
	}

	internal enum YahooServices
	{		
		ymsg_pager_logoff = 0x02,
		ymsg_message = 0x06,
		ymsg_newmail = 0x0b,
		ymsg_skinname = 0x15,
		ymsg_ping = 0x12,
		ymsg_conference_invitation = 0x18,
		ymsg_conference_logon = 0x19,
		ymsg_conference_additional_invite = 0x1c,
		ymsg_conference_message = 0x1d,
		ymsg_conference_logoff = 0x1b,
		ymsg_notify = 0x4b,
		ymsg_verify = 0x4c,
		ymsg_p2p_file_transfer = 0x4d,
		ymsg_peer2peer = 0x4f,
		ymsg_webcam = 0x50,
		ymsg_authentication_response = 0x54,
		ymsg_list = 0x55,
		ymsg_authentication = 0x57,			
		ymsg_keepalive = 0x82,
		ymsg_add_buddy = 0x83,
		ymsg_remove_buddy = 0x84,
		ymsg_stealth_session = 0xba,
		ymsg_picture = 0xbe,
		ymsg_stealth_permanent = 0xb9,
		ymsg_visibility_toggle = 0xc5,
		ymsg_status_update = 0xc6,
		ymsg_picture_update = 0xc1,
		ymsg_file_transfer = 0xdc,
		ymsg_contact_details = 0xd3,
		ymsg_buddy_auth = 0xd6,
		ymsg_status_v15 = 0xf0,
		ymsg_list_v15 = 0xf1,
		ymsg_message_reply = 0xfb, // Reply saying you got this message
		ymsg_sms_message = 0x2ea
	}

	// YPacket 2 Engine - Now uses arrays... lots and lots of arrays... and BlockCopy...
	internal sealed class YPacket
	{
		public static YPacket FromPacket(byte[] packetdata, int startPosition, int count)
		{
			if (count < 20)
				throw new ArgumentOutOfRangeException("Length should be atleast 20 bytes");

			YPacket packet = new YPacket();

			// Extract all the header information from the packet
			Buffer.BlockCopy(packetdata, startPosition + 4, packet.version, 0, 2);
			Buffer.BlockCopy(packetdata, startPosition + 10, packet.ServiceByte, 0, 2);
			Buffer.BlockCopy(packetdata, startPosition + 12, packet.StatusByte, 0, 4);
			Buffer.BlockCopy(packetdata, startPosition + 16, packet.SessionByte, 0, 4);

			int key = -1;
			int startIndex = startPosition + 20;
			for (int i = startIndex; i < count - 1; i++)
			{
				if (packetdata[i] == 0xc0 && packetdata[i + 1] == 0x80)
				{
					if (key == -1)
					{
						key = Int32.Parse(dEncoding.GetString(packetdata, startIndex, i - startIndex));
						startIndex = i + 2;
						i++;
					} else {
						string value = dEncoding.GetString(packetdata, startIndex, i - startIndex);
						packet.parameters.Add(key, value);
						startIndex = i + 2;
						i++;
						key = -1;
					}
				}
			}
			packet.parameters.IsReadOnly = true; // Lock it

			return packet;
		}
		public static YPacket FromPacket(byte[] packetdata)
		{
			return FromPacket(packetdata, 0, packetdata.Length);
		}

		public byte[] ToBytes()
		{
			int totalBlockSize = 0;
			List<byte[]> blocks = new List<byte[]>(parameters.Count);
			foreach (var pair in parameters)
			{
				string key = pair.Key.ToString();
				string value = pair.Value;
				byte[] block = new byte[key.Length + value.Length + 4]; // Four bytes for the separators
				
				// Copy the Key and the Value into parameter array
				dEncoding.GetBytes(key, 0, key.Length, block, 0);
				dEncoding.GetBytes(value, 0, value.Length, block, key.Length + 2);

				// Now copy the separators in
				Buffer.BlockCopy(separator, 0, block, key.Length, 2);
				Buffer.BlockCopy(separator, 0, block, block.Length - 2, 2);

				blocks.Add(block);
				totalBlockSize += block.Length;
			}

			byte[] finalPacket = new byte[20 + totalBlockSize]; // 20 Bytes for the header

			// Convert the length to Big-Endian if needed
			if (BitConverter.IsLittleEndian)
				totalBlockSize = Endian.SwapInt32(totalBlockSize);

			byte[] lenBytes = BitConverter.GetBytes(totalBlockSize);

			Buffer.BlockCopy(lenBytes, 2, finalPacket, 8, 2);

			Buffer.BlockCopy(packetStartBytes, 0, finalPacket, 0, 4); // YMSG header
			Buffer.BlockCopy(version, 0, finalPacket, 4, 2); // YMSG17 - Two bytes
			// Next two bytes are the vendor id. Keep them empty

			Buffer.BlockCopy(service, 0, finalPacket, 10, 2); // Two bytes for the service
			Buffer.BlockCopy(status, 0, finalPacket, 12, 4); // Four bytes for the status
			if (session != null)
				Buffer.BlockCopy(session, 0, finalPacket, 16, 4); // Four bytes for the session id

			int blockPosition = 20;
			foreach (byte[] block in blocks)
			{
				Buffer.BlockCopy(block, 0, finalPacket, blockPosition, block.Length);
				blockPosition += block.Length;
			}

			return finalPacket;
		}

		public string Session
		{
			get	{
				return dEncoding.GetString(session, 0, session.Length);
			}
			set	{
				session = dEncoding.GetBytes(value);
			}
		}
		public byte[] SessionByte
		{
			get	{
				return session;
			}
			set	{
				session = value;
			}
		}
		public string Status
		{
			get	{
				return Encoding.UTF8.GetString(status, 0, status.Length);
			}
			set	{
				status = Encoding.UTF8.GetBytes(value);
			}
		}
		public byte[] StatusByte
		{
			get {
				return status;
			}
			set {
				status = value;
			}
		}
		public YahooServices Service
		{
			get	{
				return (YahooServices)service[1];
			}
			set	{
				service[1] = (byte)value;
			}
		}
		public byte[] ServiceByte
		{
			get	{
				return service;
			}
			set	{
				service = value;
			}
		}
		public byte[] Version
		{
			get	{
				return version;
			}
		}
		public IDictionary<int, string> Parameters
		{
			get	{
				return parameters;
			}
		}

		public void AddParameter(int key, string value)
		{
			parameters.Add(key, value);
		}

		private byte[] service = new byte[] { 0x00, 0x00 };
		private byte[] status = new byte[] { 0x00, 0x00, 0x00, 0x00 };
		private byte[] session = new byte[] { 0x00, 0x00, 0x00, 0x00 };
		private byte[] version = new byte[] { 0x00, 0x12 }; // YMSG18
		private static readonly byte[] separator = new byte[] { 0xC0, 0x80 };
		private static readonly byte[] packetStartBytes = new byte[] { 89, 77, 83, 71 }; // Means YMSG
		private static readonly Encoding dEncoding = Encoding.ASCII;
		private YPacketParamCollection parameters = new YPacketParamCollection();
	}
}