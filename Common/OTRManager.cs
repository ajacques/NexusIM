using System;
using System.Security.Cryptography;
using System.Text;
using InstantMessage.Events;
using System.Linq;
using System.Diagnostics;
using InstantMessage.Internal;

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
			buddy.sendMessage(mKeyword + " if you can see this, you don't have the correct plugin.");
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

				args.Sender.sendMessage(output);
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

		private void OnMessage(object sender, IMMessageEventArgs args)
		{
			if (args.Message.StartsWith("+OK "))
			{
				string cryptData = args.Message.Substring(4).PadRight(12);
				string decoded = BlowCrypt_Decode(cryptData);
				byte[] decodedBytes = mEncoder.GetBytes(decoded);

				mBlowfish.Decipher(decodedBytes, decodedBytes.Length);

				string decrypted = mEncoder.GetString(decodedBytes);

				if (OnMessageReceived != null)
					OnMessageReceived(sender, new IMMessageEventArgs<object>(args.Sender, decrypted));
			}
		}
		private string BlowCrypt_Decode(string input)
		{
			string[] blocks = new string[(input.Length / 12)];
			for (int i = 0; i < blocks.Length; i++)
			{
				if (i < blocks.Length - 1)
					blocks[i] = input.Substring(i * 12, 12);
				else
					blocks[i] = input.Substring(i * 12);
			}

			string result = "";

			foreach (string block in blocks)
			{
				Debug.WriteLine("new s");
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
				byte[] bytes = new byte[8];

				Array.Copy(leftBytes, bytes, 4);
				Array.Copy(rightBytes, 0, bytes, 4, 4);

				result += mEncoder.GetString(bytes);
			}

			return result;
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
		public event EventHandler<IMMessageEventArgs<object>> OnMessageReceived;

		// Variables
		private Blowfish mBlowfish;
		private byte[] mCryptoKey;
		private IChatRoom mRoom;
		private static Encoding mEncoder = Encoding.ASCII;
		private const string mBase64Table = "./0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	}
}