using System;
using System.Security.Cryptography;
using System.Text;
using InstantMessage.Events;
using System.Linq;
using System.Diagnostics;
using InstantMessage.Internal;
using InstantMessage.Protocols;

namespace InstantMessage.Security
{
	/// <summary>
	/// The Off-the-Record Manager handles peer-to-peer encryption using a pre-designed system.
	/// </summary>
	/// <remarks>
	/// Encryption starts with the Diffie-Hellman Key exchange to start a basic encryption channel.
	/// </remarks>
	public static class OTRManager
	{
		public static void Setup()
		{
			ECDiffieHellmanCng encryptor = new ECDiffieHellmanCng();
			encryptor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hmac;
			encryptor.HashAlgorithm = CngAlgorithm.Sha256;

			ECDiffieHellmanPublicKey publickey = encryptor.PublicKey;

			//IMProtocol.onMessageReceive += new EventHandler<IMMessageEventArgs>(handleGlobalMessages);
		}
		public static void SendRequest(IMBuddy buddy)
		{
			buddy.SendMessage(mKeyword + " if you can see this, you don't have the correct plugin.");
			buddy.onReceiveMessage += new EventHandler<IMMessageEventArgs>(handleBuddyMessageResponse);
		}

		private static void handleBuddyMessageResponse(object sender, IMMessageEventArgs args)
		{
			if (args.Message.Substring(4) == "?OTR:") // Is this an OTR data message?
			{
				ECDiffieHellmanCng decryptor = new ECDiffieHellmanCng();
				decryptor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
				decryptor.HashAlgorithm = CngAlgorithm.Sha1;
			}
		}
		private static void handleGlobalMessages(object sender, IMMessageEventArgs args)
		{
			if (args.Message.Length > 8 && args.Message.Substring(0, 8) == "?OTR?v2?")
			{
				ECDiffieHellmanCng encryptor = new ECDiffieHellmanCng();
				encryptor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hmac;
				encryptor.HashAlgorithm = CngAlgorithm.Sha256;

				ECDiffieHellmanPublicKey publickey = encryptor.PublicKey;

				string preencode = Encoding.Default.GetString(new byte[] { 0x0002, 0x02 }) + Encoding.Default.GetString(publickey.ToByteArray());

				string output = "?OTR:";
				output += fastBase64Encode(preencode) + ".";

				args.Sender.SendMessage(output);
			}
		}
		private static string fastBase64Encode(string input)
		{
			return Convert.ToBase64String(Encoding.Default.GetBytes(input));
		}
		private static string fastBase64Decode(string input)
		{
			return Encoding.Default.GetString(Convert.FromBase64String(input));
		}

		private static string mKeyword = "?OTR?v2";
	}
	public class BlowfishMessageHandler
	{
		public BlowfishMessageHandler(IChatRoom room)
		{
			mRoom = room;
			mRoom.OnMessageReceived += new EventHandler<IMMessageEventArgs>(OnMessage);
		}

		public void SendMessage(string message)
		{
			SendMessage(mRoom, message);
		}
		public void SendMessage(IMessagable recipient, string message)
		{
			if (recipient == null)
				throw new ArgumentNullException("recipient");

			byte[] msgBytes = mEncoder.GetBytes(PadToMod(message, 8));
			byte[] cryptBytes = Encrypt(msgBytes);

			recipient.SendMessage("+OK " + BlowCrypt_Encode(cryptBytes));
		}

		private void OnMessage(object sender, IMMessageEventArgs args)
		{
			if (args.Message.StartsWith("+OK "))
			{
				string cryptData = args.Message.Substring(4);
				byte[] decoded = BlowCrypt_Decode(cryptData);

				byte[] decrypted = Decrypt(decoded);

				if (OnMessageReceived != null)
					OnMessageReceived(sender, new IMMessageEventArgs(args.Sender, mEncoder.GetString(decrypted), args.Flags | MessageFlags.Decrypted));
			}
		}
		private string PadToMod(string input, int mod)
		{
			int len = input.Length;

			if (len % mod == 0)
				return input;

			//int padding = (len < mod ? mod : len + (len % mod));
			
			while (input.Length % mod != 0)
				input += '\0';

			return input;
		}
		private byte[] BlowCrypt_Decode(string input)
		{
			input = input.PadRight(12);
			string[] blocks = new string[(input.Length / 12)];
			for (int i = 0; i < blocks.Length; i++)
			{
				if (i < blocks.Length - 1)
					blocks[i] = input.Substring(i * 12, 12);
				else
					blocks[i] = input.Substring(i * 12);
			}

			byte[] blockResult = new byte[blocks.Length * 8];

			for (int b = 0; b < blocks.Length; b++)
			{
				string block = blocks[b];
				int left = 0;
				int right = 0;				

				for (int i = 0; i < 6; i++)
				{
					var p = block[i];
					right +=  mBase64Table.IndexOf(p) << (i * 6);
				}
				for (int i = 0; i < 6; i++)
				{
					var p = block[i + 6];
					left += mBase64Table.IndexOf(p) << (i * 6);
				}

				byte[] leftBytes = BitConverter.GetBytes(left);
				byte[] rightBytes = BitConverter.GetBytes(right);

				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(leftBytes);
					Array.Reverse(rightBytes);
				}

				Array.Copy(leftBytes, 0, blockResult, b * 8, 4);
				Array.Copy(rightBytes, 0, blockResult, b * 8 + 4, 4);
			}

			return blockResult;
		}
		private string BlowCrypt_Encode(byte[] input)
		{
			int numBlocks = input.Length / 8;
			byte[][] blocks = new byte[numBlocks][];
			
			for (int i = 0; i < numBlocks; i++)
			{
				blocks[i] = input.Skip(i * 8).Take(8).ToArray();
			}

			string result = "";

			foreach (byte[] block in blocks)
			{
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(block, 0, 4);
					Array.Reverse(block, 4, 4);
				}
				uint left = BitConverter.ToUInt32(block, 0);
				uint right = BitConverter.ToUInt32(block, 4);

				for (int i = 0; i < 6; i++)
				{
					result += mBase64Table[(int)(right & 0x3f)];
					right >>= 6;
				}
				for (int i = 0; i < 6; i++)
				{
					result += mBase64Table[(int)(left & 0x3f)];
					left >>= 6;
				}
			}

			return result;
		}
		private byte[] Decrypt(byte[] input)
		{
			byte[] copy = input.Clone() as byte[];

			mBlowfish.Decipher(copy, copy.Length);

			byte[] result = copy.TakeWhile(b => b != 0).ToArray(); // Strip trailing \0

			return result;
		}
		private byte[] Encrypt(byte[] input)
		{
			byte[] copy = input.Clone() as byte[];

			mBlowfish.Encipher(copy, copy.Length);

			return copy;
		}

		// Properties
		public byte[] CryptoKey
		{
			get	{
				return mCryptoKey;
			}
			set {
				mCryptoKey = value;
				mBlowfish = new Blowfish(mCryptoKey);
			}
		}

		// Events
		public event EventHandler<IMMessageEventArgs> OnMessageReceived;

		// Variables
		private Blowfish mBlowfish;
		private byte[] mCryptoKey;
		private IChatRoom mRoom;
		private static Encoding mEncoder = Encoding.ASCII;
		private const string mBase64Table = "./0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	}
}