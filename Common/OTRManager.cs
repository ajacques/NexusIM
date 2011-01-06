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
}