using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace InstantMessage.Protocols.Yahoo
{
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

	internal class YPacket
	{
		/// <summary>
		/// Disassembles a Y! MSG packet from the string and returns a YPacket built from the input
		/// </summary>
		/// <param name="packetdata">String containing the packet data (Ex. "YMSG...")</param>
		/// <returns>YPacket class based on the input</returns>
		public static YPacket FromPacket(string packetdata)
		{
			return FromPacket(dEncoding.GetBytes(packetdata));
		}
		public static YPacket FromPacket(byte[] packetdata)
		{
			if (packetdata.Length < 20)
				throw new ArgumentOutOfRangeException("Length should be atleast 20 bytes");

			YPacket packet = new YPacket();

			// Extract all the header information from the packet
			Buffer.BlockCopy(packetdata, 4, packet.version, 0, 2);
			Buffer.BlockCopy(packetdata, 10, packet.ServiceByte, 0, 2);
			Buffer.BlockCopy(packetdata, 12, packet.StatusByte, 0, 4);
			Buffer.BlockCopy(packetdata, 16, packet.SessionByte, 0, 4);

			int key = -1;
			int startIndex = 20;
			for (int i = 20; i < packetdata.Length - 1; i++)
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

		public byte[] ToBytes()
		{
			int totalBlockSize = 0;
			List<byte[]> blocks = new List<byte[]>();
			foreach (var pair in parameters)
			{
				byte[] keyBytes = dEncoding.GetBytes(pair.Key.ToString());
				byte[] valBytes = dEncoding.GetBytes(pair.Value);
				byte[] block = new byte[keyBytes.Length + valBytes.Length + 4]; // Four bytes for the separators
				
				Buffer.BlockCopy(keyBytes, 0, block, 0, keyBytes.Length);
				Buffer.BlockCopy(separator, 0, block, keyBytes.Length, 2);
				Buffer.BlockCopy(valBytes, 0, block, keyBytes.Length + 2, valBytes.Length);
				Buffer.BlockCopy(separator, 0, block, keyBytes.Length + valBytes.Length + 2, 2);

				blocks.Add(block);
				totalBlockSize += block.Length;
			}

			byte[] finalPacket = new byte[20 + totalBlockSize];

			byte[] lenBytes = BitConverter.GetBytes(totalBlockSize);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(lenBytes);
			Buffer.BlockCopy(lenBytes, 2, finalPacket, 8, 2);

			Buffer.BlockCopy(packetStartBytes, 0, finalPacket, 0, 4); // YMSG header
			Buffer.BlockCopy(version, 0, finalPacket, 4, 2); // YMSG17 - Two bytes
			// Next two bytes are the vendor id. Keep them empty

			Buffer.BlockCopy(service, 0, finalPacket, 10, 2); // Two bytes for the service
			Buffer.BlockCopy(status, 0, finalPacket, 12, 4); // Four bytes for the status
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
		private byte[] version = new byte[] { 0x00, 0x11 }; // YMSG17
		private static readonly byte[] separator = new byte[] { 0xC0, 0x80 };
		private static readonly byte[] packetStartBytes = new byte[] { 89, 77, 83, 71 }; // Means YMSG
		private static readonly Encoding dEncoding = Encoding.ASCII;
		private YPacketParamCollection parameters = new YPacketParamCollection();
	}
}
